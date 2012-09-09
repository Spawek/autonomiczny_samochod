using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace autonomiczny_samochod
{
    public class FakeCarCommunicator : ICarComunicator
    {
        public event SpeedInfoReceivedEventHander evSpeedInfoReceived;

        public event SteeringWheelAngleInfoReceivedEventHandler evSteeringWheelAngleInfoReceived;

        private System.Threading.Thread mFakeThread;

        public FakeCarCommunicator()
        {
            evSpeedInfoReceived += new SpeedInfoReceivedEventHander(FakeCarCommunicator_evSpeedInfoReceived);
            evSteeringWheelAngleInfoReceived += new SteeringWheelAngleInfoReceivedEventHandler(FakeCarCommunicator_evSteeringWheelAngleInfoReceived);

            mFakeThread = new System.Threading.Thread(new ThreadStart(mFakeThreadTasks));
            mFakeThread.Start();
        }

        void FakeCarCommunicator_evSteeringWheelAngleInfoReceived(object sender, SteeringWheelAngleInfoReceivedEventArgs args)
        {
            //do nothing
        }

        void FakeCarCommunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            //do nothing
        }

        void mFakeThreadTasks()
        {
            System.Threading.Thread.Sleep(1000); //wait 1s

            evSpeedInfoReceived(this, new SpeedInfoReceivedEventArgs(25.0));
            evSteeringWheelAngleInfoReceived(this, new SteeringWheelAngleInfoReceivedEventArgs(10.0));
        }

        public void SendNewSpeedSettingMessage(double speedSetting)
        {
            Console.WriteLine(String.Format("[FakeCarCommunicator] new speed setting has been send: {0}", speedSetting));
        }

        public void SendNewSteeringWheelAngleSettingMessage(double angleSetting)
        {
            Console.WriteLine(String.Format("[FakeCarCommunicator] new steering angle setting has been send: {0}", angleSetting));
        }
    }
}

