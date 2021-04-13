using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorProject.ViewModels
{
    public class GraphManager
    {
        public GraphViewModel FeatureGraph { get; set; }
        public GraphViewModel CorrelativeGraph { get; set; }
        public GraphViewModel LinearRegressionGraph { get; set; }

        public GraphManager()
        {
            this.FeatureGraph = new GraphViewModel();
            this.CorrelativeGraph = new GraphViewModel();
            this.LinearRegressionGraph = new GraphViewModel();
        }

    }
}
