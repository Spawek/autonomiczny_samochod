using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Helpers;

namespace autonomiczny_samochod
{
    public class FakeCarCommunicator : ICarCommunicator
    {
        public event SpeedInfoReceivedEventHander evSpeedInfoReceived;
        public event SteeringWheelAngleInfoReceivedEventHandler evSteeringWheelAngleInfoReceived;

        private FakeCarModel model;

        public ICar ICar { get; private set; }
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

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private const int TIMER_INTERVAL_IN_MS = 10;

        public FakeCarCommunicator(ICar car)
        {
            ICar = car;
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
        public void InitRegulatorsEventsHandling()
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



        public bool IsInitiated()
        {
            throw new NotImplementedException();
        }

    }
}

