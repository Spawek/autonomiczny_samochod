﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;

namespace autonomiczny_samochod.Model.Regulators
{
    class PIDRegulator
    {
        //variables needed in CalculateSteering method
        private double I_Factor_sum = 0.0;
        private double D_Factor_sum = 0.0;
        private double LastDiffBetwTargetAndCurrentValue = 0.0;

        //declared here to make logging possible
        private double P_Factor;
        private double I_Factor;
        private double D_Factor;

        private double lastObjectValueReceived = 0.0;
        private DateTime lastObjectValueReceivedTime = DateTime.Now;
        private double lastDeviation = 0.0;

        public double targetValue;

        private PIDSettings settigs;

        private string reulatorgName;

        public double CalculatedSteering { get; private set; }
        
        public PIDRegulator(PIDSettings stgs, string regName)
        {
            settigs = stgs;
            reulatorgName = regName;
        }

        /// <summary>
        /// sends current object output value to regulator
        /// lets regulator calculating steering setting value
        /// </summary>
        /// <param name="currValue"></param>
        /// <returns>steering setting</returns>
        public double ProvideObjectCurrentValueToRegulator(double currValue)
        {
            return CalculateSteering(currValue);
        }

        /// <summary>
        /// sets target value and calculates new steering
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public double SetTargetValue(double target)
        {
            targetValue = target;
            return CalculateSteering(lastObjectValueReceived); //calculates steering with new target value and old current value
        }

        private double CalculateSteering(double currValue)
        {
            TimeSpan timeFromLastValueReceived = lastObjectValueReceivedTime - DateTime.Now;
            double deviation = targetValue - currValue;

            //P
            P_Factor = deviation * settigs.P_FACTOR_MULTIPLER;

            //I
            I_Factor_sum *= Math.Pow(
                settigs.I_FACTOR_SUM_SUPPRESSION_PER_SEC,
                (double)timeFromLastValueReceived.Milliseconds / 1000.0
            ); //suppressing old value
            I_Factor_sum += deviation * (double)timeFromLastValueReceived.Milliseconds / 1000.0;
            Limiter.Limit(ref I_Factor_sum, settigs.I_FACTOR_SUM_MIN_VALUE, settigs.I_FACTOR_SUM_MAX_VALUE);
            I_Factor = I_Factor_sum * settigs.I_FACTOR_MULTIPLER;

            //D
            D_Factor_sum *= Math.Pow(
                settigs.D_FACTOR_SUPPRESSION_PER_SEC,
                (double)timeFromLastValueReceived.Milliseconds / 1000.0
            ); //suppresing olf value
            D_Factor_sum += deviation - lastDeviation;
            Limiter.Limit(ref D_Factor_sum, settigs.D_FACTOR_SUM_MIN_VALUE, settigs.D_FACTOR_SUM_MAX_VALUE);
            D_Factor = D_Factor_sum * settigs.D_FACTOR_MULTIPLER;

            //calculating steering = limitted (P + I + D)
            CalculatedSteering = P_Factor + I_Factor + D_Factor;
            CalculatedSteering = Limiter.ReturnLimmitedVar(CalculatedSteering, settigs.MIN_FACTOR_CONST, settigs.MAX_FACTOR_CONST);

            lastObjectValueReceived = currValue;
            lastObjectValueReceivedTime = DateTime.Now;
            lastDeviation = deviation; //nice option to check is letting lastDeviation always be 0

            return CalculatedSteering;
        }
    }
}
