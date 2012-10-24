using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pololu.UsbWrapper;
using Pololu.Usc;

//Created by Mateusz Nowakowski
//Refactored and merged by Maciej (Spawek) Oziebly 
namespace car_communicator
{
    public class ServoDriver
    {
        Usc Driver = null;
        public void Initialize()
        {
            List<DeviceListItem> list = Usc.getConnectedDevices();
            if (list.Count > 0)
            { //TODO: przerobić to, bo jest logicznie bez sensu (bierze pierwsze urządzenie), a w 3 liniach 2 są niepotrzebne [Spawek]
                DeviceListItem item = null;
                item = list[0]; //TODO: dorobić tutaj sprawdzanie ID urządzenia [Spawek]
                Driver = new Usc(item);
            }
            else
            {
                Console.WriteLine("Pololu Servo Driver is disconnected");
            }
        }

        public void setTarget(byte channel, ushort target)
        {
            Driver.setTarget(channel, target);
        }

    }
}
