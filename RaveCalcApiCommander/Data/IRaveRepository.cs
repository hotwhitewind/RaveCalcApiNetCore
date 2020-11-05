using RaveCalcApiCommander.Models;
using Stratogos.Jovian.Rave.Charts;
using Stratogos.Jovian.Rave.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Data
{
    public interface IRaveRepository
    {
        public RaveChart GetRaveChartInJson(DateTime dateTimeBirth);
        public CycleChart GetCycleChartInJson(DateTime dateTimeBirth, DateTime dateTimeCalc, HdStructures.Cycle type);
        public TransitChart GetTransitChartInJson(DateTime dateTimeBirth, DateTime dateTimeTransit);
        public ConnectionChart GetConnectionChartInJson(DateTime dateTimeBirth1, DateTime dateTimeBirth2);
        public PentaModel GetPentaModelInJson(ArrayList birthdates);
    }
}
