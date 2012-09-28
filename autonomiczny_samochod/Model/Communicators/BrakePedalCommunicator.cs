﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod.Model.Communicators
{
    class BrakePedalCommunicator : ComponentCommunicator
    {
        private ICarCommunicator CarCommunicator;

        public bool sensorsInitiated { get; private set; }
        public bool steeringMechanismInitiated { get; private set; }

        public BrakePedalCommunicator(ICarCommunicator realCarCommunicator)
        {
            CarCommunicator = realCarCommunicator;

            sensorsInitiated = false;
            steeringMechanismInitiated = false;

            InitSensors();
            InitSteeringMechanism();
        }

        /// <summary>
        /// initializes steering mechanism in a new thread
        /// </summary>
        private void InitSteeringMechanism()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// initializes steering mechanism in a new thread
        /// </summary>
        private void InitSensors()
        {
            throw new NotImplementedException();
        }

        public bool IsInitiated()
        {
            return (sensorsInitiated && steeringMechanismInitiated);
        }

        private void SensorsFatalFailure()
        {
            CarCommunicator.ICar.ActivateAlertBrake();

            Logger.Log(this, "######################################################");
            Logger.Log(this, "sensors fatal failure occured - alert brake activated!");
            Logger.Log(this, "######################################################");
        }

    }
}
