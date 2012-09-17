using System;
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

        private SpeedRegulatorPIDParameters regulatorConsts { get; set; }

        public double SpeedSteering
        {
            get { return lastSteeringSeetingSend; }
        }

        //copies of speed informations
        private double targetSpeedLocalCopy = -66.6;
        private double currentSpeedLocalCopy = -66.6;
        private double lastSteeringSeetingSend = -66.6;

        //alert brake fields
        private const double ALERT_BRAKE_SPEED = -100.0;
        private bool alertBrakeActive = false;

        //timer initialization
        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        private const int TIMER_INTERVAL_IN_MS = 10;

        public PIDSpeedRegulator(ICar parent)
        {
            Car = parent;
            CarComunicator = parent.CarComunicator;

            Car.evAlertBrake += new EventHandler(Car_evAlertBrake);
            Car.evTargetSpeedChanged += new TargetSpeedChangedEventHandler(Car_evTargetSpeedChanged);
            evNewSpeedSettingCalculated += new NewSpeedSettingCalculatedEventHandler(SimpleSpeedRegulator_evNewSpeedSettingCalculated);
            CarComunicator.evSpeedInfoReceived += new SpeedInfoReceivedEventHander(CarComunicator_evSpeedInfoReceived);
            
            //timer init
            mTimer.Interval = TIMER_INTERVAL_IN_MS;
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Start();

            regulatorConsts = new SpeedRegulatorPIDParameters();
        }

        //handling external events
        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            currentSpeedLocalCopy = args.GetSpeedInfo();
            Logger.Log(this, String.Format("new current speed value acquired: {0}", args.GetSpeedInfo()));
        }
        void SimpleSpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
 	        Logger.Log(this, String.Format("new speed setting calculated: {0}", args.getSpeedSetting()));
        }

        /// <summary>
        /// if ALERT_BRAKE is not active 
        ///     calculate speed steering
        ///     if (speed steering changed)
        ///         send it everywhere (by invoking event)
        ///     end
        /// end
        /// </summary>
        void mTimer_Tick(object sender, EventArgs e)
        {
            if (alertBrakeActive)
            {
                NewSpeedSettingCalculatedEventHandler alertBrakeSpeedSendingEvent = evNewSpeedSettingCalculated;
                if (alertBrakeSpeedSendingEvent != null)
                {
                    alertBrakeSpeedSendingEvent(this, new NewSpeedSettingCalculatedEventArgs(ALERT_BRAKE_SPEED));
                }

                lastSteeringSeetingSend = ALERT_BRAKE_SPEED;
            }
            else
            {
                double calculatedSteeringSetting = CalculateSteeringSetting();

                if (lastSteeringSeetingSend != calculatedSteeringSetting)
                {
                    NewSpeedSettingCalculatedEventHandler newSpeedCalculatedEvent = evNewSpeedSettingCalculated;
                    if (newSpeedCalculatedEvent != null)
                    {
                        newSpeedCalculatedEvent(this, new NewSpeedSettingCalculatedEventArgs(calculatedSteeringSetting));
                    }

                    lastSteeringSeetingSend = calculatedSteeringSetting;
                }
            }
        }

        //variables needed in CalculateSteeringSetting() foo
        private double I_Factor_sum = 0.0;
        private double D_Factor_sum = 0.0;
        private double LastDiffBetwTargetAndCurrentValue = 0.0;

        //declared here to make logging possible
        private double P_factor;
        private double I_factor;
        private double D_factor;

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
                //common side calculations
                double CurrentDiffBetwTargetAndCurrentValue = targetSpeedLocalCopy - currentSpeedLocalCopy;
                double DiffBetwTargetAndCurrentValueChange = CurrentDiffBetwTargetAndCurrentValue - LastDiffBetwTargetAndCurrentValue;
                LastDiffBetwTargetAndCurrentValue = CurrentDiffBetwTargetAndCurrentValue;

                //I side calculations
                I_Factor_sum *= regulatorConsts.I_FACTOR_SUM_SUPPRESING_CONST; //integral suppresing 
                I_Factor_sum += CurrentDiffBetwTargetAndCurrentValue; //adding new value to intergral
                Limiter.Limit(ref I_Factor_sum, regulatorConsts.I_FACTOR_SUM_MIN_VAlUE_CONST, regulatorConsts.I_FACTOR_SUM_MAX_VAlUE_CONST); //limiting intergal

                //D side calculatios
                D_Factor_sum *= regulatorConsts.D_FACTOR_SUPPRESING_CONST; //"integral" suppresing
                D_Factor_sum += DiffBetwTargetAndCurrentValueChange; //adding new value to "integral"

                //calculating P, I, D factors
                P_factor = CurrentDiffBetwTargetAndCurrentValue * regulatorConsts.P_FACTOR_CONST;
                I_factor = I_Factor_sum * regulatorConsts.I_FACTOR_CONST;
                D_factor = D_Factor_sum * regulatorConsts.D_FACTOR_CONST;

                //calculating and limiting regulator output
                double PID_factor = P_factor + I_factor + D_factor;
                double Limitted_PID_Factor = Limiter.ReturnLimmitedVar(PID_factor, regulatorConsts.MIN_FACTOR_CONST, regulatorConsts.MAX_FACTOR_CONST);

                return Limitted_PID_Factor;
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

        public IDictionary<string, double> GetRegulatorParameters()
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();
            dict["P_FACTOR"] = P_factor;
            dict["I_FACTOR"] = I_factor;
            dict["D_FACTOR"] = D_factor;

            return dict;
        }
    }
}
