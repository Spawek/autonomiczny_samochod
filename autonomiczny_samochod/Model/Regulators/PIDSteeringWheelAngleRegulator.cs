using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class PIDSteeringWheelAngleRegulator : ISteeringWheelAngleRegulator
    {
        public event EventHandler evTargetSteeringWheelAngleChanged;


        public void GetCurrentSteeringWheelAngle()
        {
            throw new NotImplementedException();
        }

        public event EventHandler evNewSteeringWheelSettingCalculated;

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

        event NewSteeringWheelSettingCalculatedEventHandler ISteeringWheelAngleRegulator.evNewSteeringWheelSettingCalculated
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        
        public int WheelAngleSteering
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


        double ISteeringWheelAngleRegulator.WheelAngleSteering
        {
            get { throw new NotImplementedException(); }
        }


        public IDictionary<string, double> GetRegulatorParameters()
        {
            throw new NotImplementedException();
        }
    }
}
