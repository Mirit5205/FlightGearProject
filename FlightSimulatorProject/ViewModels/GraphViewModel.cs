using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Series;
using FlightSimulatorProject.Models;
using OxyPlot.Axes;

namespace FlightSimulatorProject.ViewModels
{

    public class GraphViewModel : INotifyPropertyChanged
    {
        private GraphModel m;

        public string Title
        {
            get
            {
                return m.Title;
            }
            set
            {
                if (m.Title != value)
                {
                    m.Title = value;
                    OnPropertyChange("Title");
                }
            }
        }

        public string LeftAxeTitle
        {
            get
            {
                return m.LeftAxeTitle;
            }
            set
            {
                if (m.LeftAxeTitle != value)
                {
                    m.LeftAxeTitle = value;
                    OnPropertyChange("LeftAxeTitle");
                }
            }
        }

        public string BottomAxeTitle
        {
            get
            {
                return m.BottomAxeTitle;
            }
            set
            {
                if (m.BottomAxeTitle != value)
                {
                    m.BottomAxeTitle = value;
                    OnPropertyChange("BottomAxeTitle");
                }
            }
        }
        public PlotModel DataPlot
        {
            get
            {
                return m.DataPlot;
            }
            set
            {
            }
        }

        public LineSeries Line
        {
            get
            {
                return m.FirstLine;
            }
            set
            {
            }
        }

        public LinearAxis LeftAxis
        {
            get
            {
                return m.LeftAxis;
            }
            set
            {
            }
        }

        public LinearAxis BottomAxis
        {
            get
            {
                return m.BottomAxis;
            }
            set
            {
            }
        }

        public void addCurve(FunctionSeries f)
        {
            m.addCurve(f);
        }

        public GraphViewModel()
        {
            m = new GraphModel();
            m.PropertyChanged += OnModelChanged;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnModelChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChange(e.PropertyName);
        }
        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

