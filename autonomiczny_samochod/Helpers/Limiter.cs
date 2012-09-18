using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    static class Limiter
    {
        /// <summary>
        /// makes sure that variable is in range of [lowerLimit, upperLimit]
        /// </summary>
        public static double Limit(ref double var, double lowerLimit, double upperLimit)
        {
            if (var < lowerLimit)
            {
                var = lowerLimit;
            }
            else if (var > upperLimit)
            {
                var = upperLimit;
            }

            return var;
        }

        /// <summary>
        /// return var limmited in range [lowerLimit, upperLimit]
        /// </summary>
        public static double ReturnLimmitedVar(double var, double lowerLimit, double upperLimit)
        {
            if (var < lowerLimit)
            {
                var = lowerLimit;
            }
            else if (var > upperLimit)
            {
                var = upperLimit;
            }

            return var;
        }
    }
}
