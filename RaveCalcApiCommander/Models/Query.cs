using Stratogos.Jovian.Rave.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Models
{
    public class Query
    {
        public string auth { get; set; }
        public string birthdate { get; set; }
    }

    public class CycleQuery
    {
        public string auth { get; set; }
        public string birthdate { get; set; }
        public string cycledate { get; set; }
        public string cycletype { get; set; }
    }

    public class TransitQuery
    {
        public string auth { get; set; }
        public string birthdate { get; set; }
        public string transitdate { get; set; }
    }

    public class ConnectionQuery
    {
        public string auth { get; set; }
        public string birthdate1 { get; set; }
        public string birthdate2 { get; set; }
    }

    public class PentaQuery
    {
        public string auth { get; set; }
        public string birthdate1 { get; set; }
        public string birthdate2 { get; set; }
        public string birthdate3 { get; set; }
        public string birthdate4 { get; set; }
        public string birthdate5 { get; set; }

    }
}
