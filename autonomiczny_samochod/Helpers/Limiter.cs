using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    class Limiter
    {
        /// <summary>
        /// makes sure that variable is in range of [lowerLimit, upperLimit]
        /// </summary>
        /// <param name="var"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns>returns value</returns>
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
    }
}
