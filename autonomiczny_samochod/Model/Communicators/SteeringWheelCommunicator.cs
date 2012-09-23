using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod.Model.Communicators
{
    //it could implement some interface, but for now i can't see any need to do that
    class SteeringWheelCommunicator
    {
        private ICarCommunicator CarCommunicator;

        public SteeringWheelCommunicator(RealCarCommunicator communicator)
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
