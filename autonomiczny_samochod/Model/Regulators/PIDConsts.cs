using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod
{
    public class SpeedRegulatorPIDParameters
    {
        //P part settings
        public double P_FACTOR_CONST = 80.0;

        //I part settings
        public double I_FACTOR_CONST = 10.0; //2.0; //0.4; //hypys radzi, żeby to wyłączyć bo może być niestabilny (a tego baardzo nie chcemy)
        public double I_FACTOR_SUM_MAX_VAlUE_CONST = 250.0;
        public double I_FACTOR_SUM_MIN_VAlUE_CONST = -250.0;
        public double I_FACTOR_SUM_SUPPRESING_CONST = 0.88; //1.0 = suppresing disabled

        //D part settings
        public double D_FACTOR_CONST = 120.0;
        public double D_FACTOR_SUPPRESING_CONST = 0.78;

        //steering limits
        public double MAX_FACTOR_CONST = 1000.0;
        public double MIN_FACTOR_CONST = -1000.0;
    }
}