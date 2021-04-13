using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using OxyPlot.Axes;
using System.ComponentModel;
using FlightSimulatorProject.Views;

namespace FlightSimulatorProject.Models
{
    public class GraphModel : INotifyPropertyChanged
    {
        FunctionSeries curve;
        LinearAxis leftAxe = new LinearAxis();
        LinearAxis bottomAxe = new LinearAxis();

        string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    //ConsoleAllocator.ShowConsoleWindow();
                    //Console.WriteLine(_title);
                    DataPlot.Title = Title;
                    OnPropertyChange("Title");
                }
            }
        }

        public string LeftAxeTitle
        {
            get
            {
                return leftAxe.Title;
            }
            set
            {
                if (leftAxe.Title != value)
                {
                    leftAxe.Title = value;
                    //ConsoleAllocator.ShowConsoleWindow();
                    //Console.WriteLine(leftAxe.Title);
                    OnPropertyChange("LeftAxeTitle");
                }
            }
        }

        public string BottomAxeTitle
        {
            get
            {
                return bottomAxe.Title;
            }
            set
            {
                if (bottomAxe.Title != value)
                {
                    bottomAxe.Title = value;
                    ConsoleAllocator.ShowConsoleWindow();
                    Console.WriteLine(bottomAxe.Title);
                    OnPropertyChange("BottomAxeTitle");
                }
            }
        }
        public LinearAxis LeftAxis { get; set; }
        public LinearAxis BottomAxis { get; set; }

        public LineSeries FirstLine { get; set; }
        public LineSeries SecondLine { get; set; }

        public DataPoint NewPoint { get; set; }

        public int SleepTime { get; set; }
        public PlotModel DataPlot { get; set; }

        public void addCurve(FunctionSeries f)
        {
            if (curve != null)
            {
                DataPlot.Series.RemoveAt(2);
            }
            curve = f;
            DataPlot.Series.Add(curve);
        }

        /*
        public void addCurve(LineSeries l)
        {
            if (LinearCurve != null)
            {
                DataPlot.Series.RemoveAt(1);
            }
            LinearCurve = l ;
            DataPlot.Series.Add(LinearCurve);
        }
        */
        public GraphModel()
        {
            FirstLine = new LineSeries();
            SecondLine = new LineSeries();
            leftAxe.Position = AxisPosition.Left;
            bottomAxe.Position = AxisPosition.Bottom;
            
            DataPlot = new PlotModel();
            DataPlot.Axes.Add(leftAxe);
            bottomAxe.Title = "time";
            DataPlot.Axes.Add(bottomAxe);

            DataPlot.Series.Add(FirstLine);
            DataPlot.Series.Add(SecondLine);
            DataPlot.Title = Title;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}

