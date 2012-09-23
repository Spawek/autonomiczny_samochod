using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod.Model.Communicators
{
    class AccelerationPedalCommunivator
    {
        private ICarCommunicator CarCommunicator;

        public AccelerationPedalCommunivator(ICarCommunicator communicator)
        {
            CarCommunicator = communicator;

            InitSensors();
            InitSteeringMechanism();
        }

        private void InitSteeringMechanism()
        {
            throw new NotImplementedException();
        }

        private void InitSensors()
        {
            throw new NotImplementedException();
        }

    }
}
