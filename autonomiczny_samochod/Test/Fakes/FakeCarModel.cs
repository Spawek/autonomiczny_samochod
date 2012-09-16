using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class FakeCarModel
    {
        private ICarCommunicator communicator;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private StatsCollector statsCollector = new StatsCollector();

        //steering parameters
        private double __speedSteering__ = 0;
        private double __wheelAngleSteering__ = 0;

        //target parameters (needed for stats only)
        private double __targetSpeed__ = 0;
        private double __targetWheelAngle__ = 0;

        //steering parameters
        public double SpeedSteering
        {
            get
            {
                return __speedSteering__;
            }
            set
            {
                __speedSteering__ = value;
            }
        }
        public double WheelAngleSteering
        {
            get 
            { 
                return __wheelAngleSteering__; 
            }
            set 
            { 
                __wheelAngleSteering__ = value; 
            }
        }

        //actual car parameters
        private double __speed__ = 0.0;
        private double __wheelAngle__ = 0.0;
        private double __carAngle__ = 0.0;
        private double __carX__ = 0.0;
        private double __carY__ = 0.0;

        public double Speed
        {
            get { return __speed__; }
            private set
            {
                __speed__ = value;
            }
        }

        public double SteeringWheelAngle
        {
            get; set;
        }
        public double WheelAngle
        {
            get { return __wheelAngle__; }
            set
            {
                __wheelAngle__ = value;
            }
        }
        //TODO: implement accessors/mutators for car parameters

        //model constants
        private const double SLOWING_DOWN_FACTOR = 0.991;
        private const double ACCELERATING_FACTOR = 0.002;
        private const double STEERING_WHEEL_TO_WHEELS_TRANSMISSION = 0.2;
        private const double STEERING_WHEEL_STEERING_FACTOR = 0.08;

        public FakeCarModel(ICarCommunicator carComunicator)
        {
            communicator = carComunicator;
            
            timer.Interval = TIMER_INTERVAL_IN_MS;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        public void SubscribeForTargetParams()
        {
            communicator.ISpeedRegulator.Car.evTargetSpeedChanged += new TargetSpeedChangedEventHandler(Car_evTargetSpeedChanged);
            communicator.ISpeedRegulator.Car.evTargetSteeringWheelAngleChanged += new TargetSteeringWheelAngleChangedEventHandler(Car_evTargetSteeringWheelAngleChanged);
        }

        void Car_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            __targetWheelAngle__ = args.GetTargetWheelAngle();
        }

        void Car_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            __targetSpeed__ = args.GetTargetSpeed();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //speed
            Speed *= SLOWING_DOWN_FACTOR;
            Speed += SpeedSteering * ACCELERATING_FACTOR;
            Logger.Log(this, String.Format("new wheel angle has been modeled: {0}   (current angle steering: {1})", Speed, SpeedSteering));

            //wheels angle
            SteeringWheelAngle += WheelAngleSteering * STEERING_WHEEL_STEERING_FACTOR;
            WheelAngle = SteeringWheelAngle * STEERING_WHEEL_TO_WHEELS_TRANSMISSION;
            Logger.Log(this, String.Format("new speed has been modeled: {0}   (current speed steering: {1})", WheelAngle, WheelAngleSteering));
        }

        private DateTime StartingDateTime = DateTime.Now;
        private TimeSpan GetMsFromStart()
        {
            return DateTime.Now - StartingDateTime;
        }
    }
}
