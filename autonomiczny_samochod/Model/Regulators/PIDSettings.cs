using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autonomiczny_samochod.Model.Regulators
{
    abstract class PIDSettings
    {
        //P part settings
        public double P_FACTOR_MULTIPLER; // = 80.0;

        //I part settings
        public double I_FACTOR_MULTIPLER; // = 10.0; //2.0; //0.4; //hypys radzi, żeby to wyłączyć bo może być niestabilny (a tego baardzo nie chcemy)
        public double I_FACTOR_SUM_MAX_VALUE; // = 250.0;
        public double I_FACTOR_SUM_MIN_VALUE; // = -250.0;
        public double I_FACTOR_SUM_SUPPRESSION_PER_SEC; // = 0.88; //1.0 = suppresing disabled

        //D part settings
        public double D_FACTOR_MULTIPLER; // = 120.0;
        public double D_FACTOR_SUPPRESSION_PER_SEC; // = 0.78;
        public double D_FACTOR_SUM_MIN_VALUE;
        public double D_FACTOR_SUM_MAX_VALUE;

        //steering limits
        public double MAX_FACTOR_CONST; // = 100.0;
        public double MIN_FACTOR_CONST; // = -100.0;
    }
}
