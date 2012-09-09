using System;
using System.Threading;

namespace autonomiczny_samochod
{
    public class ExampleFakeCar : ICar
    {
        public event EventHandler evAlertBrake;

        public event TargetSpeedChangedEventHandler evTargetSpeedChanged;

        public event TargetSteeringWheelAngleChangedEventHandler evTargetSteeringWheelAngleChanged;

        public ICarComunicator CarComunicator { get; private set; }
        public CarController Controller { get; private set; }
        public bool IsAlertBrakeActive { get; private set; }
        public ISpeedRegulator SpeedRegulator { get; private set; }
        public ISteeringWheelAngleRegulator SteeringWheelAngleRegulator { get; private set; }
        public CarInformations carInfo { get; private set; }

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
                evTargetSpeedChanged(this, new TargetSpeedChangedEventArgs(value));
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
                evTargetSteeringWheelAngleChanged(this, new TargetSteeringWheelAngleChangedEventArgs(value));
            }
        }

        public ExampleFakeCar(CarController parent)
        {
            Controller = parent;
            CarComunicator = new FakeCarCommunicator();
            SpeedRegulator = new SimpleSpeedRegulator(this);
            SteeringWheelAngleRegulator = new SimpleSteeringWheelRegulator(this);
            IsAlertBrakeActive = false;
            carInfo = new CarInformations(-66.6, -66.6); 

            evAlertBrake += new EventHandler(ExampleFakeCar_evAlertBrake);
            evTargetSpeedChanged += new TargetSpeedChangedEventHandler(ExampleFakeCar_evTargetSpeedChanged);
            evTargetSteeringWheelAngleChanged += new TargetSteeringWheelAngleChangedEventHandler(ExampleFakeCar_evTargetSteeringWheelAngleChanged);
        }

        void ExampleFakeCar_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            Console.WriteLine("[Example Car] target wheel angle changed to: {0}", args.GetTargetWheelAngle());
        }

        void ExampleFakeCar_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            Console.WriteLine("[Example Car] target speed changed to: {0}", args.GetTargetSpeed());
        }

        void ExampleFakeCar_evAlertBrake(object sender, EventArgs e)
        {
            Console.WriteLine("[Example Car] ALERT BRAKE!");
        }

        public void TurnOnAlertBrake()
        {
            evAlertBrake(this, EventArgs.Empty);
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
            return carInfo.wheelAngle;
        }

        public double GetCurrentSpeed()
        {
            return carInfo.speed;
        }

        public CarInformations GetCarInfo()
        {
            return carInfo;
        }
    }
}