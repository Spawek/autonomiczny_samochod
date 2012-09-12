using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class RealCarCommunicator : ICarCommunicator
    {

        public void SendNewSpeedSettingMessage()
        {
            throw new NotImplementedException();
        }

        public void SendNewSteeringWheelAngleSettingMessage()
        {
            throw new NotImplementedException();
        }

        public event SpeedInfoReceivedEventHander evSpeedInfoReceived;

        public event SteeringWheelAngleInfoReceivedEventHandler evSteeringWheelAngleInfoReceived;


        public void SendNewSpeedSettingMessage(double speedSetting)
        {
            throw new NotImplementedException();
        }

        public void SendNewSteeringWheelAngleSettingMessage(double angleSetting)
        {
            throw new NotImplementedException();
        }

        public void InitEventsHandling()
        {
            throw new NotImplementedException();
        }


        public ISpeedRegulator ISpeedRegulator
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

        public ISteeringWheelAngleRegulator ISteeringWheelAngleRegulator
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
