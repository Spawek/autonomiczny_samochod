using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class PIDSpeedRegulator : ISpeedRegulator
    {
        public event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        public ICarCommunicator communicator
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


        public void GetCurrentSpeed()
        {
            throw new NotImplementedException();
        }


        public ICar Car { get; private set; }


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
    }
}
