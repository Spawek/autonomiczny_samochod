﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class PIDSpeedRegulator : ISpeedRegulator
    {
        public event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        public ICar Car { get; private set; }
        public ICarCommunicator CarComunicator{ get; private set; }

        double ISpeedRegulator.SpeedSteering
        {
            get { return lastSteeringSeetingSend; }
        }

        private double targetSpeedLocalCopy = -66.6;
        private double currentSpeedLocalCopy = -66.6;
        private double lastSteeringSeetingSend = -66.6;

        private const double ALERT_BRAKE_SPEED = -100.0;

        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        private const int timerIntervalInMs = 10;

        private bool alertBrakeActive = false;

        public PIDSpeedRegulator(ICar parent)
        {
            Car = parent;
            CarComunicator = parent.CarComunicator;

            Car.evAlertBrake += new EventHandler(Car_evAlertBrake);
            Car.evTargetSpeedChanged += new TargetSpeedChangedEventHandler(Car_evTargetSpeedChanged);
            evNewSpeedSettingCalculated += new NewSpeedSettingCalculatedEventHandler(SimpleSpeedRegulator_evNewSpeedSettingCalculated);
            CarComunicator.evSpeedInfoReceived += new SpeedInfoReceivedEventHander(CarComunicator_evSpeedInfoReceived);
            
            //timer init
            mTimer.Interval = timerIntervalInMs;
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Start();
        }

        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            currentSpeedLocalCopy = args.GetSpeedInfo();
            Logger.Log(this, String.Format("new current speed value acquired: {0}", args.GetSpeedInfo()));
        }

        void  SimpleSpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
 	        Logger.Log(this, String.Format("new speed setting calculated: {0}", args.getSpeedSetting()));
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            if (alertBrakeActive)
            {
                if (lastSteeringSeetingSend != ALERT_BRAKE_SPEED)
                {
                    NewSpeedSettingCalculatedEventHandler temp = evNewSpeedSettingCalculated;
                    if (temp != null)
                    {
                        temp(this, new NewSpeedSettingCalculatedEventArgs(ALERT_BRAKE_SPEED));
                    }

                    lastSteeringSeetingSend = ALERT_BRAKE_SPEED;
                }
            }
            else
            {
                double calculatedSteeringSetting = CalculateSteeringSetting();

                if (lastSteeringSeetingSend != calculatedSteeringSetting)
                {
                    NewSpeedSettingCalculatedEventHandler temp = evNewSpeedSettingCalculated;
                    if (temp != null)
                    {
                        temp(this, new NewSpeedSettingCalculatedEventArgs(calculatedSteeringSetting));
                    }

                    lastSteeringSeetingSend = calculatedSteeringSetting;
                }
            }
        }

        private const double P_FACTOR_CONST = 25.0;
        private const double I_FACTOR_CONST = 0.4;
        private const double D_FACTOR_CONST = 25.0; //disabled
        private const double D_FACTOR_DECREASING_CONST = 0.6;

        private const double MAX_FACTOR_CONST = 1000.0;
        private const double MIN_FACTOR_CONST = -1000.0;

        private double I_Factor_sum = 0.0;
        private double D_Factor_sum = 0.0;
        private double LastDiffBetwTargetAndCurrentValue = 0.0;

        private double CalculateSteeringSetting()
        {
            if(currentSpeedLocalCopy == -66.6)
            {
                Logger.Log(this, "currentSpeedLocalCopy is not initialized! Calculations will not be done");
                return 0.0;
            }
            else if(targetSpeedLocalCopy == -66.6)
            {
                Logger.Log(this, "targetSpeedLocalCopy is not initialized! Calculations will not be done");
                return 0.0;
            }
            else
            {
                double CurrentDiffBetwTargetAndCurrentValue = targetSpeedLocalCopy - currentSpeedLocalCopy;
                double DiffBetwTargetAndCurrentValueChange = CurrentDiffBetwTargetAndCurrentValue - LastDiffBetwTargetAndCurrentValue;

                //I
                I_Factor_sum += CurrentDiffBetwTargetAndCurrentValue;

                //D
                D_Factor_sum *= D_FACTOR_DECREASING_CONST;
                D_Factor_sum += DiffBetwTargetAndCurrentValueChange;


                double P_FACTOR = CurrentDiffBetwTargetAndCurrentValue * P_FACTOR_CONST;
                double I_FACTOR = I_Factor_sum * I_FACTOR_CONST;
                double D_FACTOR = D_Factor_sum * D_FACTOR_CONST;

                double PID_FACTOR = P_FACTOR + I_FACTOR + D_FACTOR;

                if (PID_FACTOR > MAX_FACTOR_CONST)
                    PID_FACTOR = MAX_FACTOR_CONST;
                else if (PID_FACTOR < MIN_FACTOR_CONST)
                    PID_FACTOR = MIN_FACTOR_CONST;

                return PID_FACTOR;
            }
        }

        void Car_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            targetSpeedLocalCopy = args.GetTargetSpeed();
            Logger.Log(this, String.Format("target speed changed to: {0}", args.GetTargetSpeed()));
        }

        void Car_evAlertBrake(object sender, EventArgs e)
        {
            alertBrakeActive = true;
            Logger.Log(this, "ALERT BRAKE!");
        }


        public int SpeedSteering
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
