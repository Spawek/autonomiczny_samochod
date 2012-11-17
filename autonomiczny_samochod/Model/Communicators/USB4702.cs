using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automation.BDaq;
using Helpers;

namespace car_communicator
{
    /// <summary>
    /// extension card connector
    /// </summary>
    public class USB4702
    {
        static int buffer;
        static InstantAoCtrl instantAoCtrl = new InstantAoCtrl(); //for initialize analog outputs
        static InstantDoCtrl instantDoCtrl = new InstantDoCtrl(); //for initialize digital outputs
        static EventCounterCtrl eventSpeedCounterCtrl = new EventCounterCtrl(); // for initialize counter

        const int STEERING_WHEEL_SET_PORT = 0;
        const double STEERING_WHEEL_MIN_SET_VALUE_IN_VOLTS = 1.2; //1.0 can cause error but its teoretical min
        const double STEERING_WHEEL_MID_SET_VALUE_IN_VOLTS = 2.5; // środek
        const double STEERING_WHEEL_MAX_SET_VALUE_IN_VOLTS = 3.8; //4.0 is teoretical max

        const int BRAKE_STRENGTH_SET_PORT = 1;
        const double BRAKE_MIN_SET_VALUE_IN_VOLTS = 0;
        const double BRAKE_MAX_SET_VALUE_IN_VOLST = 5;

        const int BRAKE_ENABLE_PORT_NO = 666; //TODO: check it!!!!
        const byte BRAKE_ENABLE_ON_PORT_LEVEL = 66; //TODO: check it!!!!
        const byte BRAKE_ENABLE_OFF_PORT_LEVEL = 66; //TODO: check it!!!!

        const int BRAKE_STOP_PORT_NO = 666; //TODO: check it!!!!
        const byte BRAKE_STOP_ON_PORT_LEVEL = 66; //TODO: check it!!!!
        const byte BRAKE_STOP_OFF_PORT_LEVEL = 66; //TODO: check it!!!!

        const int BRAKE_DIRECTION_PORT_NO = 666; //TODO: check it!!!!
        const int BRAKE_BACKWARD_PORT_LEVEL = 66; //TODO: check it!!!!
        const int BRAKE_FORWARD_PORT_LEVEL = 66; //TODO: check it!!!!

        public void Initialize()
        {
            string deviceDescription = "USB-4702,BID#0"; // '0' -> 1st extension card

            try
            {
                //Analog outputs
                instantAoCtrl.SelectedDevice = new DeviceInformation(deviceDescription); // AO0

                //Digital output
                instantDoCtrl.SelectedDevice = new DeviceInformation(deviceDescription);

                //Counter
                eventSpeedCounterCtrl.SelectedDevice = new DeviceInformation(deviceDescription);

                eventSpeedCounterCtrl.Channel = 0;
                eventSpeedCounterCtrl.Enabled = false; // block counter

                setPortDO(BRAKE_ENABLE_PORT_NO, BRAKE_ENABLE_ON_PORT_LEVEL); //enabling brake engine
                setPortDO(BRAKE_STOP_PORT_NO, BRAKE_STOP_ON_PORT_LEVEL);
            }
            catch (Exception e)
            {
                Logger.Log(this, "cannot initialize connection for USB4702", 2);
                Logger.Log(this, String.Format("Exception received: {0}", e.Message), 2);

                //throw; //TODO: IMPORTANT: TEMPORARY!!
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value">0-5V (will be checked anyway - throws if bad)</param>
        public void setPortAO(int channel, double value)
        {
            if (value > 4 || value < 1)
                throw new ArgumentException("value is not in range", "value");

            instantAoCtrl.Write(channel, value);
        }

        public void setPortDO(int port, byte level)
        {
            if (level == 1)
            {
                buffer |= (1 << port);
                Console.WriteLine(buffer);
                instantDoCtrl.Write(0, (byte)buffer);
            }
            else
            {
                buffer &= ~(1 << port);
                Console.WriteLine(buffer);
                instantDoCtrl.Write(0, (byte)buffer);
            }
        }

        //working
        public void RestartSpeedCounter()
        {
            eventSpeedCounterCtrl.Enabled = false;
            eventSpeedCounterCtrl.Enabled = true;
        }

        public int getSpeedCounterStatus()
        {
            return eventSpeedCounterCtrl.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strength">
        /// -100 max left [in percents]
        /// 100 max right [in percents]
        /// </param>
        public void setSteeringWheel(double strength)
        {
            if (strength < -100 || strength > 100)
            {
                Logger.Log(this, "strength is not in range", 2);
                throw new ArgumentException("strenght is not in range");
            }

            Helpers.ReScaller.ReScale(ref strength, -100, 100, STEERING_WHEEL_MIN_SET_VALUE_IN_VOLTS, STEERING_WHEEL_MAX_SET_VALUE_IN_VOLTS);

            setPortAO(STEERING_WHEEL_SET_PORT, strength);
        }

        public void SetBrake(double strength)
        {
            Logger.Log(this, String.Format("Brake steering value is being set to: {0}%", strength));

            if (strength < -100 || strength > 100)
            {
                Logger.Log(this, "strength is not in range", 2);
                throw new ArgumentException("strenght is not in range");
            }

            if (strength < 0) //backward
            {
                strength *= -1;
                setPortDO(BRAKE_DIRECTION_PORT_NO, BRAKE_BACKWARD_PORT_LEVEL);
            }
            else
            {
                setPortDO(BRAKE_DIRECTION_PORT_NO, BRAKE_FORWARD_PORT_LEVEL);
            }

            Helpers.ReScaller.ReScale(ref strength, 0, 100, BRAKE_MIN_SET_VALUE_IN_VOLTS, BRAKE_MAX_SET_VALUE_IN_VOLST);

            setPortAO(BRAKE_STRENGTH_SET_PORT, strength);
        }
    }

}
