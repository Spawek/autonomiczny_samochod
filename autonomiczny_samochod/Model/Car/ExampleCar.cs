using System;

namespace autonomiczny_samochod
{
    public class ExampleFakeCar : ICar
    {
        public event EventHandler evAlertBrake;

        public event TargetSpeedChangedEventHandler evTargetSpeedChanged;

        public event TargetSteeringWheelAngleChangedEventHandler evTargetSteeringWheelAngleChanged;

        public ICarCommunicator CarComunicator { get; private set; }

        public CarController Controller { get; private set; }

        public bool IsAlertBrakeActive { get; private set; }

        public ISpeedRegulator SpeedRegulator { get; private set; }

        public ISteeringWheelAngleRegulator SteeringWheelAngleRegulator { get; private set; }

        public CarInformations carInfo { get; private set; }

        public double CurrentSpeed
        {
            get;
            private set;
        }

        public double CurrentWheelAngle
        {
            get;
            private set;
        }

        private double __targetSpeed__;
        private double TargetSpeed
        {
            get
            {
                return __targetSpeed__;
            }
            set
            {
                __targetSpeed__ = value;

                TargetSpeedChangedEventHandler temp = evTargetSpeedChanged;
                if (temp != null)
                {
                    temp(this, new TargetSpeedChangedEventArgs(value));
                }
            }
        }

        private double __targetWheelAngle__;
        private double TargetWheelAngle
        {
            get
            {
                return __targetWheelAngle__;
            }
            set
            {
                __targetWheelAngle__ = value;

                TargetSteeringWheelAngleChangedEventHandler temp = evTargetSteeringWheelAngleChanged;
                if (temp != null)
                {
                    temp(this, new TargetSteeringWheelAngleChangedEventArgs(value));
                }
            }
        }

        public ExampleFakeCar(CarController parent)
        {
            Controller = parent;

            CarComunicator = new FakeCarCommunicator();
            //SpeedRegulator = new SimpleSpeedRegulator(this);
            SpeedRegulator = new PIDSpeedRegulator(this);
            SteeringWheelAngleRegulator = new SimpleSteeringWheelRegulator(this);
            CarComunicator.ISpeedRegulator = SpeedRegulator; //TODO: REFACTOR THIS SHIT!!!
            CarComunicator.ISteeringWheelAngleRegulator = SteeringWheelAngleRegulator; //TODO: AND THIS!!!
            CarComunicator.InitEventsHandling(); //AND THIS!

            IsAlertBrakeActive = false;
            carInfo = new CarInformations(-66.6, -66.6);

            evAlertBrake += new EventHandler(ExampleFakeCar_evAlertBrake);
            evTargetSpeedChanged += new TargetSpeedChangedEventHandler(ExampleFakeCar_evTargetSpeedChanged);
            evTargetSteeringWheelAngleChanged += new TargetSteeringWheelAngleChangedEventHandler(ExampleFakeCar_evTargetSteeringWheelAngleChanged);
            CarComunicator.evSpeedInfoReceived += new SpeedInfoReceivedEventHander(CarComunicator_evSpeedInfoReceived);
            CarComunicator.evSteeringWheelAngleInfoReceived += new SteeringWheelAngleInfoReceivedEventHandler(CarComunicator_evSteeringWheelAngleInfoReceived);
        }

        void CarComunicator_evSteeringWheelAngleInfoReceived(object sender, SteeringWheelAngleInfoReceivedEventArgs args)
        {
            CurrentWheelAngle = args.GetAngle();
        }

        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            CurrentSpeed = args.GetSpeedInfo();
        }

        private void ExampleFakeCar_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            Logger.Log(this, String.Format("target wheel angle changed to: {0}", args.GetTargetWheelAngle()));
        }

        private void ExampleFakeCar_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            Logger.Log(this, String.Format("target speed changed to: {0}", args.GetTargetSpeed()));
        }

        private void ExampleFakeCar_evAlertBrake(object sender, EventArgs e)
        {
            Logger.Log(this, "ALERT BRAKE!");
        }

        public void TurnOnAlertBrake()
        {
            EventHandler temp = evAlertBrake;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        public void SetTargetSpeed(double speed)
        {
            TargetSpeed = speed;
        }

        public void SetTargetWheelAngle(double targetAngle)
        {
            TargetWheelAngle = targetAngle;
        }

        public double GetWheelAngle()
        {
            return CurrentWheelAngle;
        }

        public double GetCurrentSpeed()
        {
            return CurrentSpeed;
        }

        public CarInformations GetCarInfo()
        {
            return carInfo;
        }

        public double GetTargetSpeed()
        {
            return TargetSpeed;
        }

        public double GetTargetWheelAngle()
        {
            return TargetWheelAngle;
        }

        public double GetSpeedSteering()
        {
            return SpeedRegulator.SpeedSteering;
        }

        public double GetWheelAngleSteering()
        {
            return SteeringWheelAngleRegulator.WheelAngleSteering;
        }
    }
}