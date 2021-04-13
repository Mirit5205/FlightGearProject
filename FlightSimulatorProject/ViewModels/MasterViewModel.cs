using FlightSimulatorProject.Helpers;
using FlightSimulatorProject.Interfaces;
using FlightSimulatorProject.Models;
using FlightSimulatorProject.Views;
using MathNet.Numerics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FlightSimulatorProject.ViewModels
{
    public class MasterViewModel : INotifyPropertyChanged
    {
        IServer infoServer;

        Stopwatch stopwatch;

        const int TimeInterval = 10000;

        const string EXE_FILE = "fgfs.exe";

        const string FG_SOCKET = "--generic=socket,out,10,127.0.0.1,6400,tcp,playback_small";

        const string FG_SIMULATON = "--fdm=null";

        const string FG_CLIENT = "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small";

        public string APIPath { get; set; }

        public double Aileron { 
            get
            {
                return Convert.ToDouble(infoServer.Aileron);
            } 
            set
            {
            }
        }

        public double Throttle
        {
            get
            {
                return Convert.ToDouble(infoServer.Thorottle);
            }
            set
            {
            }
        }

        public double AirSpeed
        {
            get
            {
                return Convert.ToDouble(infoServer.AirSpeed);
            }
            set
            {
            }
        }

        public double AltMeter
        {
            get
            {
                return Convert.ToDouble(infoServer.AltMeter);
            }
            set
            {
            }
        }

        public double Yaw
        {
            get
            {
                return Convert.ToDouble(infoServer.Yaw);
            }
            set
            {
            }
        }

        public string Direction
        {
            get
            {
                return infoServer.Direction;
            }
            set
            {
            }
        }

        public double Roll
        {
            get
            {
                return Convert.ToDouble(infoServer.Roll);
            }
            set
            {
            }
        }

        public double Pitch
        {
            get
            {
                return Convert.ToDouble(infoServer.Pitch);
            }
            set
            {
            }
        }


        public double JoyStickAileron
        {
            get
            {
                return infoServer.JoyStickAileron;
            }
        }

        public string Elevator
        {
            get
            {
                return infoServer.Elevator;
            }
            set
            {

            }

        }
        public double JoyStickElevator
        {
            get
            {
                return infoServer.JoyStickAileron;
            }
        }

        public double JoyStickThrottle
        {
            get
            {
                return infoServer.JoyStickThrottle;
            }
        }


        public PanelViewModel VM_Panel { get; set; }
        public GraphManager GraphM { get; set; }

        //feature property
        public string Feature
        {
            get
            {
                return infoServer.Feature;
            }
            set
            {
                if (infoServer.Feature != value)
                {
                    //notify that feature and correalted feature has changed
                    infoServer.Feature = value;
                    OnPropertyChange("Feature");
                    OnPropertyChange("CorrelatedFeature");
                }
            }
        }

        //graph's title property
        public string FeatureTitle
        {
            get
            {
                return GraphM.FeatureGraph.Title;
            }
            set
            {
                if (GraphM.FeatureGraph.Title != value)
                {
                    //update titles for all the graphs
                    GraphM.FeatureGraph.Title = value;
                    GraphM.CorrelativeGraph.Title = infoServer.CorrelatedFeature;
                    GraphM.LinearRegressionGraph.Title = Feature + " " + "&" + " " + infoServer.CorrelatedFeature;
                    GraphM.LinearRegressionGraph.addCurve(new FunctionSeries(infoServer.getCurve(Feature), infoServer.getMinGraphBoundery(Feature),
                                                          infoServer.getMaxGraphBoundery(Feature), 0.1));
                   
                    OnPropertyChange("Title");
                }
            }
        }

        //graph's left axe property
        public string FeatureLeftAxeTitle
        {
            get
            {
                return GraphM.FeatureGraph.LeftAxeTitle;
            }
            set
            {
                if (GraphM.FeatureGraph.LeftAxeTitle != value)
                {
                    //update left axes for all the graphs
                    GraphM.FeatureGraph.LeftAxeTitle = value;
                    GraphM.CorrelativeGraph.LeftAxeTitle = infoServer.CorrelatedFeature;
                    GraphM.LinearRegressionGraph.LeftAxeTitle = infoServer.CorrelatedFeature;


                    OnPropertyChange("LeftAxeTitle");
                }
            }
        }

        //graph's bottom axe property
        public string FeatureBottomAxeTitle
        {
            get
            {
                return GraphM.FeatureGraph.BottomAxeTitle;
            }
            set
            {
                if (GraphM.LinearRegressionGraph.BottomAxeTitle != Feature)
                {
                    //update bottom axe for the linear curve graph
                    GraphM.LinearRegressionGraph.BottomAxeTitle = Feature;

                }
                if (GraphM.FeatureGraph.BottomAxeTitle != value)
                {
                    //update bottom axes for the feature and correalted feature graphs
                    GraphM.FeatureGraph.BottomAxeTitle = value;
                    GraphM.CorrelativeGraph.BottomAxeTitle = value;
                    OnPropertyChange("BottomAxeTitle");
                }
            }
        }

        //return the size of the data in the csv (rows number)
        //in order to binding to the slider.
        public int DataSize
        {
            get
            {
                return infoServer.DataSize;
            }
        }

        public MasterViewModel()
        {
            
            stopwatch = new Stopwatch();
            infoServer = new InfoServer();
            infoServer.PropertyChanged += OnViewModelChanged;
            infoServer.Open();
            //infoServer.ReadThread();
            //C:\Program Files\FlightGear 2020.3.6\bin
            VM_Panel = new PanelViewModel(new PanelModel(new Client(), infoServer));
            GraphM = new GraphManager();
            initialLinearRegressionProperties();

            GraphM.FeatureGraph.PropertyChanged += OnViewModelChanged;
            GraphM.CorrelativeGraph.PropertyChanged += OnViewModelChanged;
            GraphM.LinearRegressionGraph.PropertyChanged += OnViewModelChanged;
        }

        /*
         * tread for diplay and update graphs
         */
        public void GraphRender()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += dispatcherTimer_Tick;
            timer.Start();
        }

        /*
         * inital the propery for the linear regression curve and poitns
         */
        private void initialLinearRegressionProperties()
        {
            (GraphM.LinearRegressionGraph.DataPlot.Series[0] as LineSeries).LineStyle = LineStyle.None;
            (GraphM.LinearRegressionGraph.DataPlot.Series[0] as LineSeries).MarkerType = MarkerType.Circle;
            (GraphM.LinearRegressionGraph.DataPlot.Series[0] as LineSeries).Color = OxyColor.FromRgb(255, 0, 0);
            (GraphM.LinearRegressionGraph.DataPlot.Series[0] as LineSeries).MarkerSize = 3;
        }
        public DataPoint getNewFeaturePoint()
        {
            //get new feature point
            return infoServer.LastFeaturePoint;
        }
        public DataPoint getNewCorrelatedFeaturePoint()
        {
            //get new correlated feature point
            return infoServer.LastCorrelatedFeaturePoint;
        }

        public DataPoint getNewRegressionPoint()
        {
            //get new linear regression point
            return infoServer.LastLinearRegressionPoint;
        }

        /*
         * add point to the linear regression chrat and display the points
         * that added in the last 30 seconds in red color.
         */
        private void addPointsToLinearCurve(DataPoint LinearRegressionPoint)
        {

            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            (GraphM.LinearRegressionGraph.DataPlot.Series[0] as LineSeries).Points.Add(LinearRegressionPoint);

        }

        /*
         * add new point for every graph.
         */
        private void addPointsToLineSeries(DataPoint FeaturePoint, DataPoint CorrelativeFeaturePoint
                                                , DataPoint LinearRegressionPoint )
        {
            (GraphM.FeatureGraph.DataPlot.Series[0] as LineSeries).Points.Add(FeaturePoint);
            (GraphM.CorrelativeGraph.DataPlot.Series[0] as LineSeries).Points.Add(CorrelativeFeaturePoint);
            addPointsToLinearCurve(LinearRegressionPoint);
        }

        /*
         * update all the charts in order to display the changes.
         */
        private void updatePlots()
        {
            GraphM.FeatureGraph.DataPlot.InvalidatePlot(true);
            GraphM.CorrelativeGraph.DataPlot.InvalidatePlot(true);
            GraphM.LinearRegressionGraph.DataPlot.InvalidatePlot(true);
        }

        /*
         * every tick getting new points for the charts in order
         * to crate dynamic graphs. 
         */
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (GraphM.FeatureGraph.DataPlot != null)
                {
                    FeatureBottomAxeTitle = "time";
                    FeatureLeftAxeTitle = Feature;
                    FeatureTitle = Feature;

                    DataPoint newFeaturePoint = getNewFeaturePoint();
                    DataPoint newCorelatedFeaturePoint = getNewCorrelatedFeaturePoint();
                    DataPoint newRegressionPoint = getNewRegressionPoint();


                    if (!GraphM.FeatureGraph.Line.Points.Contains(newFeaturePoint))
                    {
                        addPointsToLineSeries(newFeaturePoint, newCorelatedFeaturePoint, newRegressionPoint);
                    }
                    updatePlots();
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChange(e.PropertyName);
        }

        /*
         * notify that feature changed.
         * clean all the titles, points and lines from the charts
         */
        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                if (propertyName.Equals("Feature"))
                {
                    List<DataPoint> featureGraphPoints = GraphM.FeatureGraph.Line.Points;
                    List<DataPoint> correlatedFeatureGraphPoints = GraphM.CorrelativeGraph.Line.Points;
                    List<DataPoint> linearRegressionGraphPoints = GraphM.LinearRegressionGraph.Line.Points;

                    featureGraphPoints.Clear();
                    correlatedFeatureGraphPoints.Clear();
                    linearRegressionGraphPoints.Clear();
                }

                stopwatch.Restart();

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
