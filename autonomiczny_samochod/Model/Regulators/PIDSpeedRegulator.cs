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

        private SpeedRegulatorPIDParameters regulatorConsts { get; set; }

        public double SpeedSteering
        {
            get { return lastSteeringSeetingSend; }
        }

        //copies of speed informations
        private double targetSpeedLocalCopy = 0.0;
        private double currentSpeedLocalCopy = 0.0;
        private double lastSteeringSeetingSend = 0.0;

        //alert brake fields
        private const double ALERT_BRAKE_SPEED = 0.0;
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
            
            //timer init
            mTimer.Interval = TIMER_INTERVAL_IN_MS;
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Start();

            regulatorConsts = new SpeedRegulatorPIDParameters();
        }

        //handling external events
        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            currentSpeedLocalCopy = args.GetSpeedInfo();
            Logger.Log(this, String.Format("new current speed value acquired: {0}", args.GetSpeedInfo()));
        }
        void SimpleSpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
 	        Logger.Log(this, String.Format("new speed setting calculated: {0}", args.getSpeedSetting()));
        }

        /// <summary>
        /// if ALERT_BRAKE is not active 
        ///     calculate speed steering
        ///     if (speed steering changed)
        ///         send it everywhere (by invoking event)
        ///     end
        /// end
        /// </summary>
        void mTimer_Tick(object sender, EventArgs e)
        {
            if (alertBrakeActive)
            {
                NewSpeedSettingCalculatedEventHandler alertBrakeSpeedSendingEvent = evNewSpeedSettingCalculated;
                if (alertBrakeSpeedSendingEvent != null)
                {
                    alertBrakeSpeedSendingEvent(this, new NewSpeedSettingCalculatedEventArgs(ALERT_BRAKE_SPEED));
                }

                lastSteeringSeetingSend = ALERT_BRAKE_SPEED;
            }
            else
            {
                double calculatedSteeringSetting = regulator.ProvideObjectCurrentValueToRegulator(currentSpeedLocalCopy);

                if (lastSteeringSeetingSend != calculatedSteeringSetting)
                {
                    NewSpeedSettingCalculatedEventHandler newSpeedCalculatedEvent = evNewSpeedSettingCalculated;
                    if (newSpeedCalculatedEvent != null)
                    {
                        newSpeedCalculatedEvent(this, new NewSpeedSettingCalculatedEventArgs(calculatedSteeringSetting));
                    }

                    lastSteeringSeetingSend = calculatedSteeringSetting;
                }
            }
        }

        void Car_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            targetSpeedLocalCopy = args.GetTargetSpeed();
            Logger.Log(this, String.Format("target speed changed to: {0}", args.GetTargetSpeed()));
        }

        void Car_evAlertBrake(object sender, EventArgs e)
        {
            alertBrakeActive = true;
            Logger.Log(this, "ALERT BRAKE!");
        }

        public IDictionary<string, double> GetRegulatorParameters()
        {
            return regulator.GetRegulatorParameters();
        }
    }
}
