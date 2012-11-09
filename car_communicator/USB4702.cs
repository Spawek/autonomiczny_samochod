using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automation.BDaq;

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

        public void Initialize()
        {
            string deviceDescription = "USB-4702,BID#0"; // '0' -> 1st extension card

            //Analog outputs
            instantAoCtrl.SelectedDevice = new DeviceInformation(deviceDescription); // AO0

            //Digital output
            instantDoCtrl.SelectedDevice = new DeviceInformation(deviceDescription);

            //Counter
            eventSpeedCounterCtrl.SelectedDevice = new DeviceInformation(deviceDescription);
            eventSpeedCounterCtrl.Channel = 0;
            eventSpeedCounterCtrl.Enabled = false; // block counter
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value">0-5V (will be checked anyway - throws if bad)</param>
        public void setPortAO(int channel, double value)
        {
            if (value > 5 || value < 0)
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


    }

}
