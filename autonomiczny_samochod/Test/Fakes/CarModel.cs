﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;

namespace autonomiczny_samochod
{
    public class CarModel
    {
        private ICarCommunicator communicator;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private StatsCollector statsCollector = new StatsCollector();

        private static int TIMER_INTERVAL_IN_MS = 15;

        //steering parameters
        public double SpeedSteering { get; set; }
        public double WheelAngleSteering { get; set; }
        public double BrakeSteering { get; set; }

        public Gear Gear { get; set; }

        //actual car parameters
        private double __wheelAngle__ = 0.0;
        private double __carAngle__ = 0.0;
        private double __carX__ = 0.0;
        private double __carY__ = 0.0;

        public double Speed { get; private set; }

        public double SteeringWheelAngle{ get; private set; }
        public double WheelAngle { get; private set; }
        public double BrakePosition { get; private set; }
        //TODO: implement accessors/mutators for car parameters

        //model constants
        private const double SLOWING_DOWN_FACTOR = 0.991;
        private const double ACCELERATING_FACTOR = 0.002;

        private const double BRAKE_PUSHING_OR_PULLING_SPEED_FACTOR = 0.01;
        private const double BRAKING_DOWN_WITH_BRAKES_FACTOR = 0.04;

        private const double STEERING_WHEEL_TO_WHEELS_TRANSMISSION = 0.2;
        private const double STEERING_WHEEL_STEERING_FACTOR = 0.08;

        public CarModel(ICarCommunicator carComunicator)
        {
            communicator = carComunicator;

            Gear = Gear.drive; //initial gear

            BrakePosition = 0;
            WheelAngle = 0;
            SteeringWheelAngle = 0;
            Speed = 0;
            
            timer.Interval = TIMER_INTERVAL_IN_MS;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //brake posiotion
            BrakePosition += BrakeSteering * BRAKE_PUSHING_OR_PULLING_SPEED_FACTOR;

            Speed *= SLOWING_DOWN_FACTOR;

            if (Gear == Gear.drive)
            {
                Speed += SpeedSteering * ACCELERATING_FACTOR;
            }
            else if (Gear == Gear.reverse)
            {
                Speed -= SpeedSteering * ACCELERATING_FACTOR;
            }

            if (Speed > 0)
            {
                Speed -= BrakePosition * BRAKING_DOWN_WITH_BRAKES_FACTOR;
            }
            else
            {
                Speed += BrakePosition * BRAKING_DOWN_WITH_BRAKES_FACTOR;
            }
            Logger.Log(this, String.Format("new wheel angle has been modeled: {0}   (current angle steering: {1})", Speed, SpeedSteering));

            //wheels angle
            SteeringWheelAngle += WheelAngleSteering * STEERING_WHEEL_STEERING_FACTOR;
            WheelAngle = SteeringWheelAngle * STEERING_WHEEL_TO_WHEELS_TRANSMISSION;
            Logger.Log(this, String.Format("new speed has been modeled: {0}   (current speed steering: {1})", WheelAngle, WheelAngleSteering));
        }
    }
}
