using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using autonomiczny_samochod.Model.Regulators;

namespace autonomiczny_samochod
{
    public class PIDSpeedRegulator : ISpeedRegulator
    {
        public event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        public ICar Car { get; private set; }
        public ICarCommunicator CarComunicator{ get; private set; }

        /// <summary>
        /// setting this value will also send event "evNewSpeedSettingCalculated"
        /// </summary>
        public double SpeedSteering
        {
            private set
            {
                if (alertBrakeActive)
                {
                    __speedSteering__ = ALERT_BRAKE_SPEED_STEERING;
                }
                else
                {
                    __speedSteering__ = value;
                }

                NewSpeedSettingCalculatedEventHandler newSpeedCalculatedEvent = evNewSpeedSettingCalculated;
                if (newSpeedCalculatedEvent != null)
                {
                    newSpeedCalculatedEvent(this, new NewSpeedSettingCalculatedEventArgs(__speedSteering__));
                }
            }
            get { return __speedSteering__; }
        }
        private double __speedSteering__;

        //copies of speed informations
        private double targetSpeedLocalCopy = 0.0;
        private double currentSpeedLocalCopy = 0.0;
        private double lastSteeringSeetingSend = 0.0;

        //alert brake fields
        private const double ALERT_BRAKE_SPEED_STEERING = 0.0;
        private bool alertBrakeActive = false;

        //timer initialization
        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        private const int TIMER_INTERVAL_IN_MS = 10;

        private class Settings : PIDSettings
        {
            public Settings()
            {
                //P part settings
                P_FACTOR_MULTIPLER = 1;

                //I part settings
                I_FACTOR_MULTIPLER = 0; //hypys radzi, żeby to wyłączyć bo może być niestabilny (a tego baardzo nie chcemy)
                I_FACTOR_SUM_MAX_VALUE = 0;
                I_FACTOR_SUM_MIN_VALUE = 0;
                I_FACTOR_SUM_SUPPRESSION_PER_SEC = 0; // = 0.88; //1.0 = suppresing disabled

                //D part settings
                D_FACTOR_MULTIPLER = 0;
                D_FACTOR_SUPPRESSION_PER_SEC = 0;
                D_FACTOR_SUM_MIN_VALUE = 0;
                D_FACTOR_SUM_MAX_VALUE = 0;

                //steering limits
                MAX_FACTOR_CONST = 100; // = 100.0; //MAX throtle
                MIN_FACTOR_CONST = -100; // = -100.0; //MAX brake
            }
        }

        private PIDRegulator regulator;

        public PIDSpeedRegulator(ICar parent)
        {
            Car = parent;
            CarComunicator = parent.CarComunicator;

            regulator = new PIDRegulator(new Settings(), "speed PID regulator");

            Car.evAlertBrake += new EventHandler(Car_evAlertBrake);
            Car.evTargetSpeedChanged += new TargetSpeedChangedEventHandler(Car_evTargetSpeedChanged);
            evNewSpeedSettingCalculated += new NewSpeedSettingCalculatedEventHandler(SimpleSpeedRegulator_evNewSpeedSettingCalculated);
            CarComunicator.evSpeedInfoReceived += new SpeedInfoReceivedEventHander(CarComunicator_evSpeedInfoReceived);
        }

        //handling external events
        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            currentSpeedLocalCopy = args.GetSpeedInfo();
            Logger.Log(this, String.Format("new current speed value acquired: {0}", args.GetSpeedInfo()));

            //this setter also sends event "evNewSpeedSettingCalculated"
            SpeedSteering = regulator.ProvideObjectCurrentValueToRegulator(currentSpeedLocalCopy);
        }

        void Car_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            targetSpeedLocalCopy = args.GetTargetSpeed();
            Logger.Log(this, String.Format("target speed changed to: {0}", args.GetTargetSpeed()));

            //this setter also sends event "evNewSpeedSettingCalculated"
            SpeedSteering = regulator.SetTargetValue(targetSpeedLocalCopy);
        }

        void SimpleSpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
 	        Logger.Log(this, String.Format("new speed setting calculated: {0}", args.getSpeedSetting()));
        }

        void Car_evAlertBrake(object sender, EventArgs e)
        {
            alertBrakeActive = true;
            SpeedSteering = ALERT_BRAKE_SPEED_STEERING;
            Logger.Log(this, "ALERT BRAKE ACRIVATED!", 2);
        }

        public IDictionary<string, double> GetRegulatorParameters()
        {
            return regulator.GetRegulatorParameters();
        }
    }
}
