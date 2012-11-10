using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using autonomiczny_samochod.Model.Communicators;
using car_communicator;

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

        //CONST
        const int SPEED_MEASURING_TIMER_INTERVAL_IN_MS = 10; //in ms
        const int SPEED_TABLE_SIZE = 100;
        const double WHEEL_CIRCUIT_IN_M = 1.3822996; //44cm * pi //TODO: check it - its probably wrong
        const int NO_OF_HAAL_METERS = 5;
        const int TICKS_TO_RESTART = 10000; 

        //sub-communicators
       // private BrakePedalCommunicator brakePedalCommunicator { get; set; } //obsolete
        private AccelerationPedalCommunivator accelerationPedalCommunivator { get; set; }
        private SteeringWheelCommunicator steeringWheelCommunicator { get; set; }
        private USB4702 extentionCardCommunicator { get; set; }
        private ServoDriver servoDriver { get; set; }
        private RS232Controller angleAndSpeedMeter { get; set; }


        //car speed receiving
        System.Windows.Forms.Timer SpeedMeasuringTimer = new System.Windows.Forms.Timer();
        int [] lastTicksMeasurements = new int[SPEED_TABLE_SIZE];
        int tickTableIterator = 0;
        int lastTicks = 0;


        public RealCarCommunicator(ICar parent)
        {
            ICar = parent;
            //brakePedalCommunicator = new BrakePedalCommunicator(this); //obsolete
            //accelerationPedalCommunivator = new AccelerationPedalCommunivator(this);
            //steeringWheelCommunicator = new SteeringWheelCommunicator(this);
            extentionCardCommunicator = new USB4702();
            servoDriver = new ServoDriver();
            angleAndSpeedMeter = new RS232Controller();

            //TODO: make thread for every initialization
            extentionCardCommunicator.Initialize();
            servoDriver.Initialize();
            angleAndSpeedMeter.Initialize();
            
            SpeedMeasuringTimer.Interval = SPEED_MEASURING_TIMER_INTERVAL_IN_MS;
            SpeedMeasuringTimer.Tick += new EventHandler(SpeedMeasuringTimer_Tick);
        }

        void SpeedMeasuringTimer_Tick(object sender, EventArgs e)
        {
            //read
 	        int ticks = extentionCardCommunicator.getSpeedCounterStatus();

            //calculations
            lastTicksMeasurements[tickTableIterator] = ticks - lastTicks;
            lastTicks = ticks;

            tickTableIterator = (tickTableIterator++) % SPEED_TABLE_SIZE;

            double speed = WHEEL_CIRCUIT_IN_M / NO_OF_HAAL_METERS * lastTicksMeasurements.Sum() / (SPEED_TABLE_SIZE * SPEED_MEASURING_TIMER_INTERVAL_IN_MS);

            if(ticks > TICKS_TO_RESTART)
            {
                extentionCardCommunicator.RestartSpeedCounter();
                lastTicks = 0;
            }

            //sending event
            SpeedInfoReceivedEventHander SpeedEvent = evSpeedInfoReceived;
            if (SpeedEvent != null)
            {
                SpeedEvent(this, new SpeedInfoReceivedEventArgs(speed));
            }
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
            //model.WheelAngleSteering = args.getSteeringWheelAngleSetting(); //TODO: remake it
        }

        void ISpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
            //model.SpeedSteering = args.getSpeedSetting(); //TODO: remake it
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
            return true; //TODO: fix it
           // return (brakePedalCommunicator.IsInitiated() && accelerationPedalCommunivator.IsInitiated() && steeringWheelCommunicator.IsInitiated());
        }

    }
}
