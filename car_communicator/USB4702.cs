using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automation.BDaq;

namespace car_communicator
{
    public class USB4702
    {
        static int buffer;
        static InstantAoCtrl instantAoCtrl = new InstantAoCtrl(); //for initialize analog outputs
        static InstantDoCtrl instantDoCtrl = new InstantDoCtrl(); //for initialize digital outputs
        static EventCounterCtrl eventCounterCtrl = new EventCounterCtrl(); // for initialize counter

        public void Initialize()
        {
            string deviceDescription = "USB-4702,BID#0";

            //Analog outputs
            instantAoCtrl.SelectedDevice = new DeviceInformation(deviceDescription); // AO0

            //Digital output
            instantDoCtrl.SelectedDevice = new DeviceInformation(deviceDescription);
            //buffer = 0;   //Initialize all digital outputs with low level
            //instantDoCtrl.Write(0,(byte) buffer);

            //Counter
            eventCounterCtrl.SelectedDevice = new DeviceInformation(deviceDescription);
            eventCounterCtrl.Channel = 0;
            eventCounterCtrl.Enabled = false; // block counter
        }

        public void setPortAO(int channel, double value)
        {
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

        public void RestartCounter()
        {
            eventCounterCtrl.Enabled = false;
            eventCounterCtrl.Enabled = true;
        }

        public int getCounterStatus()
        {
            return eventCounterCtrl.Value;
        }
    }

}
