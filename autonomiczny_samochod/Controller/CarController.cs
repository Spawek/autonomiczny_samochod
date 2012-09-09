using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using autonomiczny_samochod;

namespace autonomiczny_samochod
{
    //TODO: provide some logs
    public class CarController
    {
        private ICar Model { get; set; }
        public MainWindow MainWindow { get; private set; }

        public CarController(MainWindow window)
        {
            MainWindow = window;

            Model = new ExampleFakeCar(this);
            Model.TurnOnAlertBrake();
            Model.SetTargetSpeed(25.0);
            Model.SetTargetWheelAngle(60.0);
        }

        public CarInformations GetCarInformation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// sets target wheel angle in degrees
        ///     right -> angle > 0
        ///     left  -> angle < 0
        /// </summary>
        /// <param name="targetAngle"></param>
        public void SetTargetWheelAngle(double targetAngle)
        {
            Model.SetTargetWheelAngle(targetAngle);
        }

        /// <summary>
        /// sets target speed in km/h
        /// </summary>
        /// <param name="setTargetSpeed"></param>
        public void SetTargetSpeed(double targetSpeed)
        {
            Model.SetTargetSpeed(targetSpeed);
        }
    }
}
