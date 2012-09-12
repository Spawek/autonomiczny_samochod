using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace autonomiczny_samochod
{
    public class FakeCarCommunicator : ICarCommunicator
    {
        public event SpeedInfoReceivedEventHander evSpeedInfoReceived;
        public event SteeringWheelAngleInfoReceivedEventHandler evSteeringWheelAngleInfoReceived;

        private FakeCarModel model;

        public ISpeedRegulator ISpeedRegulator
        {
            get;
            set;
        }

        public ISteeringWheelAngleRegulator ISteeringWheelAngleRegulator
        {
            get;
            set;
        }

        private System.Threading.Thread mFakeThread;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private const int TIMER_INTERVAL_IN_MS = 10;

        public FakeCarCommunicator()
        {
            evSpeedInfoReceived += new SpeedInfoReceivedEventHander(FakeCarCommunicator_evSpeedInfoReceived);
            evSteeringWheelAngleInfoReceived += new SteeringWheelAngleInfoReceivedEventHandler(FakeCarCommunicator_evSteeringWheelAngleInfoReceived);

            

            //mFakeThread = new System.Threading.Thread(new ThreadStart(mFakeThreadTasks));
            //mFakeThread.Start();

            model = new FakeCarModel(this);

            timer.Interval = TIMER_INTERVAL_IN_MS;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// this has to be invoked before 1st use
        /// </summary>
        public void InitEventsHandling()
        {
            ISpeedRegulator.evNewSpeedSettingCalculated += new NewSpeedSettingCalculatedEventHandler(ISpeedRegulator_evNewSpeedSettingCalculated);
            ISteeringWheelAngleRegulator.evNewSteeringWheelSettingCalculated += new NewSteeringWheelSettingCalculatedEventHandler(ISteeringWheelAngleRegulator_evNewSteeringWheelSettingCalculated);
        }

        void ISteeringWheelAngleRegulator_evNewSteeringWheelSettingCalculated(object sender, NewSteeringWheelSettingCalculateddEventArgs args)
        {
            model.WheelAngleSteering = args.getSteeringWheelAngleSetting();
        }

        void ISpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
            model.SpeedSteering = args.getSpeedSetting();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            SteeringWheelAngleInfoReceivedEventHandler tempAngleEvent = evSteeringWheelAngleInfoReceived;
            if (tempAngleEvent != null)
            {
                tempAngleEvent(this, new SteeringWheelAngleInfoReceivedEventArgs(model.WheelAngle));
            }

            SpeedInfoReceivedEventHander tempSpeedEvent = evSpeedInfoReceived;
            if (tempSpeedEvent != null)
            {
                tempSpeedEvent(this, new SpeedInfoReceivedEventArgs(model.Speed));
            }
        }

        //some internal event handler is needed
        void FakeCarCommunicator_evSteeringWheelAngleInfoReceived(object sender, SteeringWheelAngleInfoReceivedEventArgs args)
        {
            //do nothing
        }

        //some internal event handler is needed
        void FakeCarCommunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            //do nothing
        }

        void mFakeThreadTasks()
        {
            System.Threading.Thread.Sleep(1000); //wait 1s
            evSpeedInfoReceived(this, new SpeedInfoReceivedEventArgs(25.0));
            evSteeringWheelAngleInfoReceived.Invoke(this, new SteeringWheelAngleInfoReceivedEventArgs(10.0));

            System.Threading.Thread.Sleep(1000); //wait 1s
            evSpeedInfoReceived(this, new SpeedInfoReceivedEventArgs(30.0));
            evSteeringWheelAngleInfoReceived.Invoke(this, new SteeringWheelAngleInfoReceivedEventArgs(5.0));

            System.Threading.Thread.Sleep(1000); //wait 1s
            evSpeedInfoReceived(this, new SpeedInfoReceivedEventArgs(35.0));
            evSteeringWheelAngleInfoReceived.Invoke(this, new SteeringWheelAngleInfoReceivedEventArgs(0.0));
        }

        public void SendNewSpeedSettingMessage(double speedSetting)
        {
            Logger.Log(this, String.Format("new speed setting has been send: {0}", speedSetting));

            model.SpeedSteering = speedSetting;
        }

        public void SendNewSteeringWheelAngleSettingMessage(double angleSetting)
        {
            Logger.Log(this, String.Format("new steering angle setting has been send: {0}", angleSetting));
            model.WheelAngleSteering = angleSetting;
        }

    }
}

