using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace autonomiczny_samochod
{
    //needed for evTargetSpeedChanged
    public delegate void TargetSpeedChangedEventHandler(object sender, TargetSpeedChangedEventArgs args);
    public class TargetSpeedChangedEventArgs : EventArgs
    {
        private double targetSpeed;
        public TargetSpeedChangedEventArgs(double speed)
        {
            targetSpeed = speed;
        }

        public double GetTargetSpeed()
        {
            return targetSpeed;
        }
    }

    //needed for evTargetSteeringWheelAngleChanged
    public delegate void TargetSteeringWheelAngleChangedEventHandler(object sender, TargetSteeringWheelAngleChangedEventArgs args);
    public class TargetSteeringWheelAngleChangedEventArgs
    {
        private double targetAngle;
        public TargetSteeringWheelAngleChangedEventArgs(double angle)
        {
            targetAngle = angle;
        }

        public double GetTargetWheelAngle()
        {
            return targetAngle;
        }
    }

    public interface ICar
    {
        event EventHandler evAlertBrake;
        event TargetSpeedChangedEventHandler evTargetSpeedChanged;
        event TargetSteeringWheelAngleChangedEventHandler evTargetSteeringWheelAngleChanged;

        ISteeringWheelAngleRegulator SteeringWheelAngleRegulator
        {
            get;
        }

        CarController Controller
        {
            get;
        }

        ISpeedRegulator SpeedRegulator
        {
            get;
        }

        ICarCommunicator CarComunicator
        {
            get;
        }
    
        double GetWheelAngle();

        void SetTargetWheelAngle(double targetAngle);

        void SetTargetSpeed(double targetSpeed);

        void TurnOnAlertBrake();

        double GetCurrentSpeed();

        CarInformations GetCarInfo();

        double GetTargetSpeed();

        double GetTargetWheelAngle();

        double GetSpeedSteering();

        double GetWheelAngleSteering();
    }
}
