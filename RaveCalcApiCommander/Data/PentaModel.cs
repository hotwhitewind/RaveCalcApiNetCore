using Stratogos.Jovian.Rave.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Data
{
    public class PentaModel
    {
        public string pentaTheme { get; set; }
        public Dictionary<string, List<RaveChartInfo>> activators { get; set; }
        public PentaChart pentaChart { get; set; }
    }

    public class RaveChartInfo
    {
        public int ChartID { get; set; }
        public string BG5PentaKeynote { get; set; }
        public List<string> planetInfo { get; set; }
    }
}
