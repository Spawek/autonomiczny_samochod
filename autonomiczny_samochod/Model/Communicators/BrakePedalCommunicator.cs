using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod.Model.Communicators
{
    class BrakePedalCommunicator
    {
        private ICarCommunicator CarCommunicator;

        public BrakePedalCommunicator(ICarCommunicator realCarCommunicator)
        {
            CarCommunicator = realCarCommunicator;

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
