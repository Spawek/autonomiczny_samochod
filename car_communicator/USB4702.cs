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

        const double STEERING_WHEEL_MIN_SET_VALUE_IN_VOLTS = 1; 
        const double STEERING_WHEEL_MAX_SET_VALUE_IN_VOLTS = 4; 

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
        /// -1 max left
        /// 1 max right
        /// </param>
        public void setSteeringWheel(double strength)
        {
            if (strength < -1 || strength > 1)
            {
                Logger.Log(this, "strength is not in range", 2);
                throw new ArgumentException("strenght is not in range");
            }

            Helpers.ReScaller.ReScale(ref strength, -1, 1, STEERING_WHEEL_MIN_SET_VALUE_IN_VOLTS, STEERING_WHEEL_MAX_SET_VALUE_IN_VOLTS);

            setPortAO(0, strength);
        }
    }

}
