using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public struct CarInformations
    {
        public double speed;
        public double wheelAngle;

        public CarInformations(double _speed, double _wheelAngle)
        {
            speed = _speed;
            wheelAngle = _wheelAngle;
        }
    }
}
