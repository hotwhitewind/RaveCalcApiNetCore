using Stratogos.Jovian.Rave;
using Stratogos.Jovian.Rave.Charts;
using Stratogos.Jovian.Rave.Structures;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RaveCalcApiCommander.Data
{
    public class MocRaveRepository : IRaveRepository
    {
        private readonly IEmbededResourceService _embededResourceService;
        public MocRaveRepository(IEmbededResourceService embededResourceService)
        {
            string text = System.AppDomain.CurrentDomain.BaseDirectory + "EphemerisDataFiles";
            Generator.IsAdvancedImaging = true;
            Generator.Init(text);
            _embededResourceService = embededResourceService;
        }

        public ConnectionChart GetConnectionChartInJson(DateTime dateTimeBirth1, DateTime dateTimeBirth2)
        {
            RaveChart chart1 = Generator.CalculateRaveChart(dateTimeBirth1) as RaveChart;
            chart1.ChartName = "birthDay1";
            RaveChart chart2 = Generator.CalculateRaveChart(dateTimeBirth2) as RaveChart;
            chart2.ChartName = "birthDay2";
            var connection = Generator.CalculateConnectionChart(new System.Collections.ArrayList() { chart1, chart2 }) as ConnectionChart;
            connection.ChartName = "connectionChart";
            return connection;
        }

        public CycleChart GetCycleChartInJson(DateTime dateTimeBirth, DateTime dateTimeCalc, HdStructures.Cycle type)
        {
            RaveChart chart = Generator.CalculateRaveChart(dateTimeBirth) as RaveChart;
            chart.ChartName = "birthDay";
            if (type == HdStructures.Cycle.None)
            {
                return null;
            }
            CycleChart cycleChart;

            if (type == HdStructures.Cycle.Solar_Return || type == HdStructures.Cycle.Lunar_Return)
            {
                cycleChart = (Generator.CalculateCycleChart(chart as IChart, type, dateTimeCalc) as CycleChart);
            }
            else if (type == HdStructures.Cycle.Second_Saturn_Return)
            {
                DateTime dateTime2 = dateTimeBirth.AddYears(50);
                cycleChart = (Generator.CalculateCycleChart(chart, type, dateTime2) as CycleChart);
            }
            else
            {
                cycleChart = (Generator.CalculateCycleChart(chart as IChart, type) as CycleChart);
            }

            cycleChart.Chart2DisplayName = "cycleChart";
            cycleChart.ChartName = "cycleChart";
            return cycleChart;
        }

        public PentaModel GetPentaModelInJson(List<DateTime> birthdates)
        {
            ArrayList raveList = new ArrayList();
            foreach (var date in birthdates)
            {
                var rave = Generator.CalculateRaveChart((DateTime)date) as RaveChart;
                raveList.Add(rave);
            }

            raveList.Sort(new RaveChartComparer());
            PentaChart pentaChart = Generator.CalculatePenta(raveList);
            PentaModel pentaModel = new PentaModel();
            pentaModel.pentaChart = pentaChart;
            string key = pentaChart.ActiveCenterNum.ToString() + pentaChart.NumChildRaves.ToString();
            pentaModel.pentaTheme = _embededResourceService.GetBG5PentaThemes(Convert.ToInt32(key));
            pentaModel.activators = new Dictionary<string, List<RaveChartInfo>>();

            foreach (var gate in HdStructures.Gates)
            {
                if (gate == null) continue;
                var FamilyPentaKeynote = _embededResourceService.GetFamilyPentaKeynotes(gate.ID);
                if (FamilyPentaKeynote == String.Empty) continue;
                var RaveChartInfo = new List<RaveChartInfo>();
                pentaModel.activators.Add(FamilyPentaKeynote, RaveChartInfo);
                foreach (RaveChart chart in pentaChart.ChildRaves)
                {
                    var gateActivation = pentaChart.GetGateActivations(gate.ID, chart.ID);
                    if (gateActivation.Count != 0)
                    {
                        RaveChartInfo newRaveChartInfo = new RaveChartInfo();
                        newRaveChartInfo.ChartID = chart.ID;
                        newRaveChartInfo.planetInfo = new List<string>();
                        newRaveChartInfo.BG5PentaKeynote = _embededResourceService.GetBG5PentaKeynotes(Convert.ToInt32(gate.ID));
                        foreach (var planet in gateActivation)
                        {
                            var info = PlanetInfo(planet);
                            newRaveChartInfo.planetInfo.Add(info);
                        }
                        RaveChartInfo.Add(newRaveChartInfo);
                    }
                }
            }
            return pentaModel;
        }

        public RaveChart GetRaveChartInJson(DateTime dateTimeBirth)
        {
            RaveChart chart = Generator.CalculateRaveChart(dateTimeBirth) as RaveChart;
            return chart;
        }

        public TransitChart GetTransitChartInJson(DateTime dateTimeBirth, DateTime dateTimeTransit)
        {
            RaveChart chart = Generator.CalculateRaveChart(dateTimeBirth) as RaveChart;
            chart.ChartName = "birthDay";
            TransitChart transitChart = Generator.CalculateTransitChart(chart, dateTimeTransit) as TransitChart;
            transitChart.ChartName = "transitChart";
            return transitChart;
        }

        private string PlanetInfo(PlanetData planetData)
        {
            string text = string.Empty;
            if (planetData.ActivationType == HdStructures.ActivationType.Personality)
            {
                text += "Personal";
            }
            else
            {
                text += "Design";
            }
            var planetName = Enum.Parse(typeof(HdStructures.Planets), planetData.ID.ToString());
            text = text + " " + planetName.ToString();
            text = text + " " + "at Line " + planetData.Line.ToString();
            if (planetData.FixingState != HdStructures.FixingState.None)
            {
                var state = Enum.Parse(typeof(HdStructures.FixingState), planetData.FixingState.ToString());

                text = text + " (" + state + ")";
            }
            string text2 = text;
            return string.Concat(new string[]
            {
                text2,
                "  [ ",
                _embededResourceService.GetBG5LineKeynotes(planetData.Line),
                " ]",
            });
        }
    }
}
