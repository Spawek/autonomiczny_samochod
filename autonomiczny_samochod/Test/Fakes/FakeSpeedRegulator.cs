using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class FakeSpeedRegulator : ISpeedRegulator
    {
        public event NewSpeedSettingCalculatedEventHandler evNewSpeedSettingCalculated;

        public ICar Car { get; set; }

        public double SpeedSteering { get; set; }

        public IDictionary<string, double> GetRegulatorParameters()
        {
            return new Dictionary<string, double>();    
        }
    }
}
