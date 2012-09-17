﻿using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public delegate void NewSpeedSettingCalculatedEventHandler(object sender, NewSpeedSettingCalculatedEventArgs args);
    public class NewSpeedSettingCalculatedEventArgs : EventArgs
    {
        private double speedSetting;
        public NewSpeedSettingCalculatedEventArgs(double setting)
        {
            speedSetting = setting;
        }

        public double getSpeedSetting()
        {
            return speedSetting;
        }
    }

    public interface ISpeedRegulator
    {

        event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        ICar Car
        {
            get;
        }

        ICarCommunicator CarComunicator
        {
            get;
        }

        double SpeedSteering
        {
            get;
        }

        System.Collections.Generic.IDictionary<string, double> GetRegulatorParameters();
    }
}
