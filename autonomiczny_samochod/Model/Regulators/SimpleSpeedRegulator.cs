using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    /// <summary>
    /// use PIDSpeedRegulator
    /// </summary>
    [Obsolete]
    public class SimpleSpeedRegulator : ISpeedRegulator
    {
        public event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        public ICar Car { get; private set; }
        public ICarCommunicator CarComunicator{ get; private set; }

        //it's P regulator -> only 1 factor
        private const double PFactor = 25.0;

        private const double ALERT_BRAKE_SPEED = -100.0;

        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        private const int timerIntervalInMs = 10;

        private bool alertBrakeActive = false;
        
        public SimpleSpeedRegulator(ICar parent)
        {
            Car = parent;
            CarComunicator = parent.CarComunicator;

            Car.evAlertBrake += new EventHandler(Car_evAlertBrake);
            Car.evTargetSpeedChanged += new TargetSpeedChangedEventHandler(Car_evTargetSpeedChanged);
            evNewSpeedSettingCalculated += new NewSpeedSettingCalculatedEventHandler(SimpleSpeedRegulator_evNewSpeedSettingCalculated);
            CarComunicator.evSpeedInfoReceived += new SpeedInfoReceivedEventHander(CarComunicator_evSpeedInfoReceived);
            
            //timer init
            mTimer.Interval = timerIntervalInMs;
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Start();
        }

        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            currentSpeedLocalCopy = args.GetSpeedInfo();
            Logger.Log(this, String.Format("new current speed value acquired: {0}", args.GetSpeedInfo()));
        }

        void  SimpleSpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
 	        Logger.Log(this, String.Format("new speed setting calculated: {0}", args.getSpeedSetting()));
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            if (alertBrakeActive)
            {
                if (lastSteeringSeetingSend != ALERT_BRAKE_SPEED)
                {
                    NewSpeedSettingCalculatedEventHandler temp = evNewSpeedSettingCalculated;
                    if (temp != null)
                    {
                        temp(this, new NewSpeedSettingCalculatedEventArgs(ALERT_BRAKE_SPEED));
                    }

                    lastSteeringSeetingSend = ALERT_BRAKE_SPEED;
                }
            }
            else
            {
                double calculatedSteeringSetting = CalculateSteeringSetting();

                if (lastSteeringSeetingSend != calculatedSteeringSetting)
                {
                    NewSpeedSettingCalculatedEventHandler temp = evNewSpeedSettingCalculated;
                    if (temp != null)
                    {
                        temp(this, new NewSpeedSettingCalculatedEventArgs(calculatedSteeringSetting));
                    }

                    lastSteeringSeetingSend = calculatedSteeringSetting;
                }
            }
        }

        private double targetSpeedLocalCopy = -66.6;
        private double currentSpeedLocalCopy = -66.6;
        private double lastSteeringSeetingSend = -66.6;

        private double CalculateSteeringSetting()
        {
            if(currentSpeedLocalCopy == -66.6)
            {
                Logger.Log(this, "currentSpeedLocalCopy is not initialized! Calculations will not be done");
                return 0.0;
            }
            else if(targetSpeedLocalCopy == -66.6)
            {
                Logger.Log(this, "targetSpeedLocalCopy is not initialized! Calculations will not be done");
                return 0.0;
            }
            else
            {
 	            return (targetSpeedLocalCopy - currentSpeedLocalCopy) * PFactor;
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


        public double SpeedSteering
        {
            get { throw new NotImplementedException(); }
        }


        public IDictionary<string, double> GetRegulatorParameters()
        {
            throw new NotImplementedException();
        }
    }
}
