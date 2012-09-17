using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class PIDSteeringWheelAngleRegulator : ISteeringWheelAngleRegulator
    {
        public event EventHandler evTargetSteeringWheelAngleChanged;
        public event NewSteeringWheelSettingCalculatedEventHandler evNewSteeringWheelSettingCalculated;

        public ICar Car
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ICarCommunicator CarComunicator
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public double WheelAngleSteering
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, double> GetRegulatorParameters()
        {
            throw new NotImplementedException();
        }

        public void GetCurrentSteeringWheelAngle()
        {
            throw new NotImplementedException();
        }
    }
}
