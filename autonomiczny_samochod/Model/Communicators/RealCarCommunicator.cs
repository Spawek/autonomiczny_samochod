using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using autonomiczny_samochod.Model.Communicators;

namespace autonomiczny_samochod
{
    public class RealCarCommunicator : ICarCommunicator
    {
        public event SpeedInfoReceivedEventHander evSpeedInfoReceived;
        public event SteeringWheelAngleInfoReceivedEventHandler evSteeringWheelAngleInfoReceived;

        public ISpeedRegulator ISpeedRegulator 
        { 
            get
            {
                return ICar.SpeedRegulator;
            } 
        }
        public ISteeringWheelAngleRegulator ISteeringWheelAngleRegulator
        { 
            get
            {
                return ICar.SteeringWheelAngleRegulator;
            }
        }

        public ICar ICar { get; private set; }

        //sub-communicators
        private BrakePedalCommunicator brakePedalCommunicator { get; set; }
        private AccelerationPedalCommunivator accelerationPedalCommunivator { get; set; }
        private SteeringWheelCommunicator steeringWheelCommunicator { get; set; }

        public RealCarCommunicator(ICar parent)
        {
            ICar = parent;
            brakePedalCommunicator = new BrakePedalCommunicator(this);
            accelerationPedalCommunivator = new AccelerationPedalCommunivator(this);
            steeringWheelCommunicator = new SteeringWheelCommunicator(this);
        }
        
        public void InitRegulatorsEventsHandling()
        {
            throw new NotImplementedException();
        }

        public void SendNewSpeedSettingMessage(double speedSetting)
        {
            throw new NotImplementedException();
        }

        public void SendNewSteeringWheelAngleSettingMessage(double angleSetting)
        {
            throw new NotImplementedException();
        }

        public bool IsInitiated()
        {
            return (brakePedalCommunicator.IsInitiated() && accelerationPedalCommunivator.IsInitiated() && steeringWheelCommunicator.IsInitiated());
        }

    }
}
