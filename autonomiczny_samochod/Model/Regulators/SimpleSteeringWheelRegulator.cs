using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;

namespace autonomiczny_samochod
{
    public class SimpleSteeringWheelRegulator : ISteeringWheelAngleRegulator
    {
        public event NewSteeringWheelSettingCalculatedEventHandler evNewSteeringWheelSettingCalculated;

        public ICar Car { get; private set; }
        public ICarCommunicator CarComunicator { get; private set; }

        public double WheelAngleSteering
        {
            get { return lastCalculatedSteeringWheelSetting; }
        }

        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();

        //it's P regulator -> only 1 factor
        private const double PFactor = 5.0;
        private const int TIMER_INTERVAL_IN_MS = 10;

        private double targetWheelAngleLocalCopy = -66.6;
        private double currentWheelAngle = -66.6;
        private double lastCalculatedSteeringWheelSetting = -66.6;

        public SimpleSteeringWheelRegulator(ICar parent)
        {
            //attributes init
            Car = parent;
            CarComunicator = parent.CarComunicator;

            //event handling
            Car.evTargetSteeringWheelAngleChanged += new TargetSteeringWheelAngleChangedEventHandler(Car_evTargetSteeringWheelAngleChanged);
            evNewSteeringWheelSettingCalculated += new NewSteeringWheelSettingCalculatedEventHandler(SimpleSteeringWheelRegulator_evNewSteeringWheelSettingCalculated);
            CarComunicator.evSteeringWheelAngleInfoReceived += new SteeringWheelAngleInfoReceivedEventHandler(CarComunicator_evSteeringWheelAngleInfoReceived);

            //timer init
            mTimer.Interval = TIMER_INTERVAL_IN_MS;
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Start();
        }

        //internal events handling
        void mTimer_Tick(object sender, EventArgs e)
        {
            double calculatedSteeringSetting = CalculatSteeringSetting();

            if (lastCalculatedSteeringWheelSetting != calculatedSteeringSetting)
            {
                NewSteeringWheelSettingCalculatedEventHandler temp = evNewSteeringWheelSettingCalculated;
                if (temp != null)
                {
                    temp(this, new NewSteeringWheelSettingCalculateddEventArgs(calculatedSteeringSetting));
                }

                lastCalculatedSteeringWheelSetting = calculatedSteeringSetting;
            }
        }
        void SimpleSteeringWheelRegulator_evNewSteeringWheelSettingCalculated(object sender, NewSteeringWheelSettingCalculateddEventArgs args)
        {
            Logger.Log(this, String.Format("New steering wheel setting calculated: {0}", args.getSteeringWheelAngleSetting()));
        }

        //external events handling
        void CarComunicator_evSteeringWheelAngleInfoReceived(object sender, SteeringWheelAngleInfoReceivedEventArgs args)
        {
            currentWheelAngle = args.GetAngle();
            Logger.Log(this, String.Format("steering wheel angle info received: {0}", args.GetAngle()));
        }
        void Car_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            targetWheelAngleLocalCopy = args.GetTargetWheelAngle();
            Logger.Log(this, String.Format("target wheel angle changed to: {0}", targetWheelAngleLocalCopy));
        }

        private double CalculatSteeringSetting()
        {
            if (targetWheelAngleLocalCopy == -66.6)
            {
                Logger.Log(this, "target wheel angle is not initialized. Calculations will not be done");
                return 0;
            }
            else if (currentWheelAngle == -66.6)
            {
                Logger.Log(this, "current wheel angle is not initialized. Calculations will not be done");
                return 0;
            }
            else
            {
                return (targetWheelAngleLocalCopy - currentWheelAngle) * PFactor;
            }
        }

        public IDictionary<string, double> GetRegulatorParameters()
        {
            throw new NotImplementedException();
        }
    }
}
