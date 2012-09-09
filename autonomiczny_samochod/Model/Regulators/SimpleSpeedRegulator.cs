using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class SimpleSpeedRegulator : ISpeedRegulator
    {
        public event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        public ICar Car { get; private set; }
        public ICarComunicator CarComunicator{ get; private set; }

        private double targetSpeedLocalCopy = -66.6;
        private double currentSpeedLocalCopy = -66.6;
        private double lastSteeringSeetingSend = -66.6;

        //it's P regulator -> only 1 factor
        private const double PFactor = 5.0;

        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        private const int timerIntervalInMs = 10;
        
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
            Console.WriteLine(String.Format("[SimpleSpeedRegulator] new current speed value acquired: {0}", args.GetSpeedInfo()));
        }

        void  SimpleSpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
 	        Console.WriteLine(String.Format("[SimpleSpeedRegulator] new speed setting calculated: {0}", args.getSpeedSetting()));
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            double calculatedSteeringSetting = CalculateSteeringSetting();

            if(lastSteeringSeetingSend != calculatedSteeringSetting)
            {
                evNewSpeedSettingCalculated(this, new NewSpeedSettingCalculatedEventArgs(calculatedSteeringSetting));
                lastSteeringSeetingSend = calculatedSteeringSetting;
            }
        }

        private double CalculateSteeringSetting()
        {
            if(currentSpeedLocalCopy == -66.6)
            {
                Console.WriteLine("[SimpleSpeedRegulator] currentSpeedLocalCopy is not initialized! Calculations will not be done");
                return 0.0;
            }
            else if(targetSpeedLocalCopy == -66.6)
            {
                Console.WriteLine("[SimpleSpeedRegulator] targetSpeedLocalCopy is not initialized! Calculations will not be done");
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
            Console.WriteLine(String.Format("[SimpleSpeedRegulator] target speed changed to: {0}", args.GetTargetSpeed()));
        }

        void Car_evAlertBrake(object sender, EventArgs e)
        {
            Console.WriteLine("[SimpleSpeedRegulator] ALERT BRAKE!");
        }
    }
}
