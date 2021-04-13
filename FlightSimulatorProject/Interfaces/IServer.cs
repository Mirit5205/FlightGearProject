using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorProject.Interfaces
{
    public interface IServer : INotifyPropertyChanged
    {
        void Open();

        void Close();
        string[] Read();
        void ReadThread();

        Func<double, double> getCurve(string feature);
        double getMinGraphBoundery(string feature);
        double getMaxGraphBoundery(string feature);
        bool Stop { get; set; }
        public DataPoint LastFeaturePoint { get; set; }
        public DataPoint LastCorrelatedFeaturePoint { get; set; }
        public DataPoint LastLinearRegressionPoint { get; set; }

        public string Aileron { get; set; }
        public string Elevator { get; set; }
        public string Pitch { get; set; }
        public string Yaw { get; set; }
        public string Roll { get; set; }
        public string AirSpeed { get; set; }
        public string AltMeter { get; set; }
        public string Direction { get; set; }



        public string Thorottle { get; set; }
        public double JoyStickAileron { get; set; }
        public double JoyStickThrottle { get; set; }


        public int SleepTime { get; set; }

        public int DataSize { get; }
        string Feature { get; set; }
        string CorrelatedFeature { get; set; }


    }
}
