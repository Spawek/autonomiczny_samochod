using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;

namespace autonomiczny_samochod.Model.Regulators
{
    /// <summary>
    /// calculates new steering whenever new target is set or new brake possition is received
    /// </summary>
    public class PIDBrakeRegulator : IBrakeRegulator
    {
        public event NewBrakeSettingCalculatedEventHandler evNewBrakeSettingCalculated;

        public double BrakeSteering
        {
            get
            {
                return regulator.CalculatedSteering;
            }
        }

        public ICar ICar { get; set; }

        public IDictionary<string, double> GetRegulatorParameters()
        {
            return regulator.GetRegulatorParameters();
        }

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
                MAX_FACTOR_CONST = 100; // = 100.0;
                MIN_FACTOR_CONST = -100; // = -100.0;
            }
        }

        private PIDRegulator regulator;

        public PIDBrakeRegulator(ICar car)
        {
            ICar = car;
            regulator = new PIDRegulator(new Settings(), "brake PID regulator");

            ICar.evTargetSteeringWheelAngleChanged += new TargetSteeringWheelAngleChangedEventHandler(ICar_evTargetSteeringWheelAngleChanged);
            ICar.CarComunicator.evBrakePositionReceived += new BrakePositionReceivedEventHandler(CarComunicator_evBrakePositionReceived);
            evNewBrakeSettingCalculated += new NewBrakeSettingCalculatedEventHandler(PIDBrakeRegulator_evNewBrakeSettingCalculated);
        }

        void PIDBrakeRegulator_evNewBrakeSettingCalculated(object sender, NewBrakeSettingCalculatedEventArgs args)
        {
            Logger.Log(this, String.Format("New brake steering has been calculated: {0}", args.GetBrakeSetting()));
        }

        public void SetTarget(double target)
        {
            double calculatedSteering = regulator.SetTargetValue(target);
            NewBrakeSettingCalculatedEventHandler temp = evNewBrakeSettingCalculated;
            if (temp != null)
            {
                temp(this, new NewBrakeSettingCalculatedEventArgs(calculatedSteering));
            }
        }

        void CarComunicator_evBrakePositionReceived(object sender, BrakePositionReceivedEventArgs args)
        {
            double calculatedSteering = regulator.ProvideObjectCurrentValueToRegulator(args.GetPosition());
            NewBrakeSettingCalculatedEventHandler temp = evNewBrakeSettingCalculated;
            if (temp != null)
            {
                temp(this, new NewBrakeSettingCalculatedEventArgs(calculatedSteering));
            }
        }

        void ICar_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            regulator.targetValue = args.GetTargetWheelAngle();
        }

    }
}
