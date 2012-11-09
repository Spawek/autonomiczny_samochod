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
            
            if(list.Count != 1)
            {
                throw new ApplicationException("there are 0 or more than 1 connected devices - probably servos are not connected");
            }

            var Driver = new Usc(list[0]);
        }

        public void setTarget(byte channel, ushort target)
        {
            if (channel == 0)
            {
                if (!(target == Const.GEAR_P || target == Const.GEAR_R || target == Const.GEAR_N || target == Const.GEAR_D))
                {
                    throw new ApplicationException("wrong target");
                }
                else if (channel == 1)
                {
                    if (target < Const.MIN_THROTTLE || target > Const.MAX_THROTTLE)
                    {
                        throw new ApplicationException("wrong target");
                    }
                }
            }
            else
            {
                throw new ApplicationException("unknown target");
            }

            Driver.setTarget(channel, target);
        }

    }
}
