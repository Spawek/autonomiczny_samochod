using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    //evSpeedInfoReceived
    public delegate void SpeedInfoReceivedEventHander(object sender, SpeedInfoReceivedEventArgs args);
    public class SpeedInfoReceivedEventArgs
    {
        private double speedInfo;

        public SpeedInfoReceivedEventArgs(double speed)
        {
            speedInfo = speed;
        }

        public double GetSpeedInfo()
        {
            return speedInfo;
        }
    }

    //evSteeringWheelAngleInfoReceived
    public delegate void SteeringWheelAngleInfoReceivedEventHandler(object sender, SteeringWheelAngleInfoReceivedEventArgs args);
    public class SteeringWheelAngleInfoReceivedEventArgs
    {
        private double wheelAngleInfo;

        public SteeringWheelAngleInfoReceivedEventArgs(double angle)
        {
            wheelAngleInfo = angle;
        }

        public double GetAngle()
        {
            return wheelAngleInfo;
        }

    }

    public interface ICarComunicator
    {
        event SpeedInfoReceivedEventHander evSpeedInfoReceived;
        event SteeringWheelAngleInfoReceivedEventHandler evSteeringWheelAngleInfoReceived;

        void SendNewSpeedSettingMessage(double speedSetting);

        void SendNewSteeringWheelAngleSettingMessage(double angleSetting);
    }
}
