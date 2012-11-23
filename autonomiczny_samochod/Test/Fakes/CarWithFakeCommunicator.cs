﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using autonomiczny_samochod.Model.Regulators;

namespace autonomiczny_samochod
{
    public class CarWithFakeCommunicator : ICar
    {
        public event EventHandler evAlertBrake;
        public event TargetSpeedChangedEventHandler evTargetSpeedChanged;
        public event TargetSteeringWheelAngleChangedEventHandler evTargetSteeringWheelAngleChanged;

        public ISteeringWheelAngleRegulator SteeringWheelAngleRegulator { get; private set; }
        public ISpeedRegulator SpeedRegulator { get; private set; }
        public IBrakeRegulator BrakeRegulator { get; private set; }

        public CarController Controller { get; private set; }
        public ICarCommunicator CarComunicator { get; private set; }
        public CarInformations CarInfo { get; private set; }

        public CarWithFakeCommunicator(CarController parent)
        {
            Controller = parent;
            CarInfo = new CarInformations();

            CarComunicator = new FakeCarCommunicator(this);

            SteeringWheelAngleRegulator = new PIDSteeringWheelAngleRegulator(this);
            SpeedRegulator = new PIDSpeedRegulator(this);
            BrakeRegulator = new PIDBrakeRegulator(this);

            CarComunicator.InitRegulatorsEventsHandling();
        }

        public void SetTargetWheelAngle(double targetAngle)
        {
            CarInfo.TargetWheelAngle = targetAngle;

            TargetSteeringWheelAngleChangedEventHandler temp = evTargetSteeringWheelAngleChanged;
            if (temp != null)
            {
                temp(this, new TargetSteeringWheelAngleChangedEventArgs(targetAngle));
            }
        }

        public void SetTargetSpeed(double targetSpeed)
        {
            CarInfo.TargetSpeed = targetSpeed;

            TargetSpeedChangedEventHandler temp = evTargetSpeedChanged;
            if (temp != null)
            {
                temp(this, new TargetSpeedChangedEventArgs(targetSpeed));
            }
        }

        public void ActivateAlertBrake()
        {
            EventHandler temp = evAlertBrake;
            if (temp != null)
            {
                temp(this, new EventArgs());
            }
        }
    }
}
