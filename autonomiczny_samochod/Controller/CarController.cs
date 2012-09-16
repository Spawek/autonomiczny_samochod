﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using autonomiczny_samochod;

namespace autonomiczny_samochod
{
    //TODO: provide some logs
    public class CarController
    {
        public ICar Model { get; private set; }
        public MainWindow MainWindow { get; private set; }
        private System.Threading.Thread mFakeSignalsSenderThread;

        //stats collecting
        private StatsCollector statsCollector = new StatsCollector();
        private int TICKS_TO_SAVE_STATS = 250;
        private System.Windows.Forms.Timer mStatsCollectorTimer = new System.Windows.Forms.Timer();
        private const int TIMER_INTERVAL_IN_MS = 10;

        public CarController(MainWindow window)
        {
            MainWindow = window;

            Model = new ExampleFakeCar(this);

            mStatsCollectorTimer.Interval = TIMER_INTERVAL_IN_MS;
            mStatsCollectorTimer.Tick += new EventHandler(mStatsCollectorTimer_Tick);
            mStatsCollectorTimer.Start();

            mFakeSignalsSenderThread = new System.Threading.Thread(new System.Threading.ThreadStart(mFakeSignalsSenderFoo));
            mFakeSignalsSenderThread.Start();
        }

        void mStatsCollectorTimer_Tick(object sender, EventArgs e)
        {
            //stats collecting
            statsCollector.PutNewStat("time", Time.GetTimeFromProgramBeginnig().TotalMilliseconds);
            statsCollector.PutNewStat("current speed", Model.GetCurrentSpeed());
            statsCollector.PutNewStat("target speed", Model.GetTargetSpeed());
            statsCollector.PutNewStat("speed steering", Model.GetSpeedSteering());
            statsCollector.PutNewStat("current angle", Model.GetWheelAngle());
            statsCollector.PutNewStat("target angle", Model.GetTargetWheelAngle());
            statsCollector.PutNewStat("angle steering", Model.GetWheelAngleSteering());

            if (TICKS_TO_SAVE_STATS-- == 0)
            {
                statsCollector.WriteStatsToFile("stats.txt");
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, String.Format("STATS HAS BEEN WRITTEN TO FILE: stats.txt"));
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, "----------------------------------------------------------------");
                Logger.Log(this, "----------------------------------------------------------------");
            }

        }

        void mFakeSignalsSenderFoo()
        {
            System.Threading.Thread.Sleep(1000);

            Model.SetTargetSpeed(50.0);
            Model.SetTargetWheelAngle(60.0);

            System.Threading.Thread.Sleep(1000);

            Model.SetTargetSpeed(25.0);
            Model.SetTargetWheelAngle(30.0);
        }

        public CarInformations GetCarInformation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// sets target wheel angle in degrees
        ///     right -> angle > 0
        ///     left  -> angle < 0
        /// </summary>
        /// <param name="targetAngle"></param>
        public void SetTargetWheelAngle(double targetAngle)
        {
            Model.SetTargetWheelAngle(targetAngle);
        }

        /// <summary>
        /// sets target speed in km/h
        /// </summary>
        /// <param name="setTargetSpeed"></param>
        public void SetTargetSpeed(double targetSpeed)
        {
            Model.SetTargetSpeed(targetSpeed);
        }

        public void ChangeTargetSpeed(double change)
        {
            Model.SetTargetSpeed(Model.GetTargetSpeed() + change);
        }

        public void ChangeTargetWheelAngle(double change)
        {
            Model.SetTargetWheelAngle(Model.GetTargetWheelAngle() + change);
        }

        public void AlertBrake()
        {
            Model.TurnOnAlertBrake();
        }
    }
}
