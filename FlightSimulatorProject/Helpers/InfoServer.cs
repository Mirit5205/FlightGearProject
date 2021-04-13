using FlightSimulatorProject.Interfaces;
using FlightSimulatorProject.Views;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;

namespace FlightSimulatorProject.Helpers
{
    class InfoServer : IServer
    {
        FileParser csvParser;
        private TcpListener listener;
        private TcpClient client;
        private BinaryReader reader;

        //tread for reading data from fg
        Thread getInfoThread;

        //feature with his correlated feature
        Dictionary<string, string> correlationDict = new Dictionary<string, string>();

        //feature with his correlated feature and their linear regression
        Dictionary<string, Func<double, double>> linearRegressionCurves = new Dictionary<string, Func<double, double>>();

        Dictionary<string, Tuple<double, double>> graphBondaries = new Dictionary<string, Tuple<double, double>>();

        //declere port and host  
        const string LOCAL_HOST = "127.0.0.1";
        const int PORT = 6400;

        public int SleepTime { get; set; }

        double time = 0;
        public bool Connected { get; set; } = false;
        public bool Stop { get; set; } = false;
        public DataPoint LastFeaturePoint { get; set; }
        public DataPoint LastCorrelatedFeaturePoint { get; set; }
        public DataPoint LastLinearRegressionPoint { get; set; }

        //return the number of data rows in the csv file
        public int DataSize
        {
            get
            {
                return csvParser.DataDict["aileron"].Count();
            }
        }

        //return the regression curve of the specified feature
        public Func<double,double> getCurve(string feature)
        {

            return linearRegressionCurves[feature];
        }

        public double getMinGraphBoundery(string feature)
        {
            return graphBondaries[feature].Item1;
        }
        public double getMaxGraphBoundery(string feature)
        {
            return graphBondaries[feature].Item2;
        }
        private void initalGraphBounderis()
        {
            foreach (var feature in csvParser.DataDict)
            {
                Tuple<double, double> data = new Tuple<double, double>(feature.Value.Min(), feature.Value.Max());
                graphBondaries.Add(feature.Key, data);

            }
        }

        string _aileron;
        public string Aileron
        {
            get
            {
                return _aileron;
            }
            set
            {
                if (_aileron != value)
                {
                    _aileron = value;
                    JoyStickAileron = Convert.ToDouble(Aileron);
                    OnPropertyChange("Aileron");
                    OnPropertyChange("JoyStickAileron");

                }
            }
        }

        string _elevator;
        public string Elevator
        {
            get
            {
                return _elevator;
            }
            set
            {
                if (_elevator != value)
                {
                    _elevator = value;
                    JoyStickElevator = Convert.ToDouble(Elevator);
                    OnPropertyChange("Elevator");
                    OnPropertyChange("JoyStickElevator");

                }
            }
        }

        string _airSpeed;
        public string AirSpeed
        {
            get
            {
                return _airSpeed;
            }
            set
            {
                if (_airSpeed != value)
                {
                    _airSpeed = value;
                    JoyStickElevator = Convert.ToDouble(AirSpeed);
                    OnPropertyChange("AirSpeed");
                }
            }
        }

        string _altMeter;
        public string AltMeter
        {
            get
            {
                return _altMeter;
            }
            set
            {
                if (_altMeter != value)
                {
                    _altMeter = value;
                    JoyStickElevator = Convert.ToDouble(AltMeter);
                    OnPropertyChange("AltMeter");
                }
            }
        }
        private List<string> DirectionList = new List<string> { "0", "N" };
        /*public string Direction
        {

            get { return DirectionList[1]; }

            set { DirectionList[0] = value.ToString(); DirectionList[1] = calcDirection(DirectionList[0]); OnPropertyChange("Direction"); }
        }*/
        string calcDirection(string val)
        {
            var DirectionDict = new Dictionary<string, Tuple<int, int>>()
            {
                { "N",  Tuple.Create(0, 45)},
                {"NE", Tuple.Create(45,90) },
                { "E",Tuple.Create(90, 135)},
                { "SE",Tuple.Create(135, 180)},
                { "S",Tuple.Create(180, 225)},
                { "SW",Tuple.Create(225, 270)},
                { "W",Tuple.Create(270, 315)},
                { "NW",Tuple.Create(315, 360)}};

            float flt1 = float.Parse(val);

            foreach (KeyValuePair<string, Tuple<int, int>> element in DirectionDict)
            {
                if (flt1 >= element.Value.Item1 && element.Value.Item2 >= flt1)
                {
                    return element.Key;
                }

            }

            return "NN";
        }
        public string Direction
        {
            get
            {
                return DirectionList[1];
            }
            set
            {

                DirectionList[0] = value.ToString();
                DirectionList[1] = calcDirection(DirectionList[0]);
                OnPropertyChange("Direction");
            }
            /*
                if (_direction != value)
                {
                    _direction = value;
                    JoyStickElevator = Convert.ToDouble(Direction);
                    OnPropertyChange("Direction");
                }
            */
            
        }
        string _yaw;
        public string Yaw
        {
            get
            {
                return _yaw;
            }
            set
            {
                if (_yaw != value)
                {
                    _yaw = value;
                    JoyStickElevator = Convert.ToDouble(Yaw);
                    OnPropertyChange("Yaw");
                }
            }
        }

        string _roll;
        public string Roll
        {
            get
            {
                return _roll;
            }
            set
            {
                if (_roll != value)
                {
                    _roll = value;
                    JoyStickElevator = Convert.ToDouble(Roll);
                    OnPropertyChange("Roll");
                }
            }
        }

        string _pitch;
        public string Pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                if (_pitch != value)
                {
                    _pitch = value;
                    JoyStickElevator = Convert.ToDouble(Pitch);
                    OnPropertyChange("Pitch");
                }
            }
        }

        string _thorottle;
        public string Thorottle
        {
            get
            {
                return _thorottle;
            }
            set
            {
                if (_thorottle != value)
                {
                    _thorottle = value;
                    JoyStickThrottle = Convert.ToDouble(_thorottle);
                    OnPropertyChange("JoyStickThrottle");
                    OnPropertyChange("Thorottle");

                }
            }
        }

        // First val is actual value taken from flightgear second value is what we have converted 
        // to percentage for movment to be done relative to mean 
        private List<double> AileronList = new List<double> { 0, 125 };

        public Dictionary<string, Tuple<double, double>> rangeDict = new Dictionary<string, Tuple<double, double>>()
        {
            {"aileron",Tuple.Create(0.34,-0.244)},
            {"elevator",Tuple.Create(0.4,-0.4)}
        };
        public double JoyStickAileron
        {
            get { return AileronList[1]; }

            set
            {
                AileronList[0] = Convert.ToDouble(Aileron);
                var change = calcPercentageAndMovementA(this.rangeDict["aileron"], AileronList[0]);

                change = change + 125;
                AileronList[1] = change;
                OnPropertyChange("JoyStickAileron");
            }

        }

        //Returns difference in percentage based of of mean 
        // Important to not we return as a negative or positive based of if we 
        // are below starting positon (mean) or above and based off of that I know 
        // how much to move in which direction!
        private int calcPercentageAndMovementA(Tuple<double, double> rT, double val)
        {
            // return a negative value he is going left
            if (val >= 0)
            {
                var percent = ((val / rT.Item1) * 100);
                var tmpL = -1 * ((percent / 100) * 25);
                return (int)tmpL;
            }

            //return a positive value he is going right we started with a negative value
            var NegativePercent = (-1 * ((val / rT.Item2) * 100));
            var tmpR = 1 * ((NegativePercent / -100) * 25);
            return (int)tmpR;


        }
        // Their the exact same function but its very confusing to put for both therfore for easier understanding 
        // I decided to rewrite the function twice 
        private int calcPercentageAndMovementE(Tuple<double, double> rT, double val)
        {

            // return a negative value he is going Down
            if (val >= 0)
            {
                var percent = ((val / rT.Item1) * 100); var tmpL = -1 * ((percent / 100) * 25);
                return (int)tmpL;
            }
            //return a positive value he is UP we started with a negative value
            var NegativePercent = (-1 * ((val / rT.Item2) * 100)); var tmpR = 1 * ((NegativePercent / -100) * 25);
            return (int)tmpR;


        }

        private List<double> ElevatorList = new List<double> { 0, 125 };
        public double JoyStickElevator
        {
            get { return ElevatorList[1]; }

            set
            {
                ElevatorList[0] = value;
                var change = calcPercentageAndMovementE(this.rangeDict["elevator"], ElevatorList[0]);
                change = change + 125 - 10;
                ElevatorList[1] = change;
                if (change > 150) { ElevatorList[1] = 150; }
                if (change < 100) { ElevatorList[1] = 100; }
                OnPropertyChange("JoyStickElevator");
            }
        }

        

        private List<double> ThrottleList = new List<double> { 0, 170 };
        
        int min_Val = 170;
        public double JoyStickThrottle
        {
            get { return ThrottleList[1]; }

            set
            {
                ThrottleList[0] = value;
                var change = -1 * (ThrottleList[0] * 120);
                var finalNum = min_Val + change;
                ThrottleList[1] = finalNum;
                OnPropertyChange("JoyStickThrottle");
            }
        }

        string _feature = "aileron"; //defult value
        public string Feature
        {
            get
            {
                return _feature;
            }
            set
            {
                if (_feature != value)
                {
                    _feature = value;
                    OnPropertyChange("Feature");
                    OnPropertyChange("CorrelatedFeature");


                }
            }
        }

        
        string _correlatedFeature = "longitude-deg";

        public string CorrelatedFeature
        {
            get
            {
                return _correlatedFeature;
            }
            set
            {
                if (_correlatedFeature != value)
                {
                    _correlatedFeature = value;
                    OnPropertyChange("CorrelatedFeature");

                }
            }
        }


        //convert Nan and infinity to actual values
        private void validateValues(ref double a, double x)
        {
            if (Double.IsNaN(a))
            {
                a = x;
            }
            else if (Double.IsInfinity(a))
            {
                a = x;
            } else
            {
                return;
            }
        }

        //for every feature and correlated feature, calculate their linear regression curve
        //and save it in a dict
        public void initialCurves()
        {
            foreach (var pair in correlationDict)
            {
                
                var xdata = csvParser.DataDict[pair.Key].ToArray();
                var ydata = csvParser.DataDict[pair.Value].ToArray();

                //compute the linear curve
                double[] p = Fit.Polynomial(xdata, ydata, 1);
                var a = p[1]; 
                var b = p[0];
                
                validateValues(ref a, xdata.Max());
                validateValues(ref b, ydata.Max());
                

                //save the linear curve as a func
                Func<double, double> linearCurve = (x) => a * x + b;

                linearRegressionCurves.Add(pair.Key, linearCurve);

            }
        }

        //for every feature find his correlated feature and save it in dict
        public void initialCorrelatedFeatures()
        {
            double currentCorrelation;
            double bestCorrelation = -1;
            string mostCorrelativeFeature = "";

            for (int i = 0; i < this.csvParser.DataDict.Count(); i++)
            {
                for (int j = 0; j < this.csvParser.DataDict.Count(); j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    currentCorrelation = Correlation.Pearson(this.csvParser.DataDict.ElementAt(i).Value,
                                                this.csvParser.DataDict.ElementAt(j).Value);

                    if (Double.IsNaN(currentCorrelation))
                    {
                        currentCorrelation = 0;
                    }

                    if (currentCorrelation > bestCorrelation)
                    {
                        bestCorrelation = currentCorrelation;
                        mostCorrelativeFeature = convertNumToFeature(j);
                    }
                }
                correlationDict.Add(convertNumToFeature(i), mostCorrelativeFeature);
                bestCorrelation = -1;
                mostCorrelativeFeature = "";

            }
        }

        public InfoServer()
        {
            this.csvParser = new FileParser();
            this.csvParser.initializeData();
            initialCorrelatedFeatures();
            initialCurves();
            initalGraphBounderis();
        }
        // open server with the ip and port we decleared
        public void Open()
        {
            if (listener != null) Close();
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse(LOCAL_HOST), PORT));
            listener.Start();
        }

        //close the server
        public void Close() { client.Close(); listener.Stop(); Connected = false; }

        // read simulator input and return it to the model
        public string[] Read()
        {
            if (!Connected)
            {
                Connected = true;
                client = listener.AcceptTcpClient();
                //ConsoleAllocator.ShowConsoleWindow();
                //Console.WriteLine("Connected!");
                reader = new BinaryReader(client.GetStream());
            }

            string input = ""; 
            char s;

            while ((s = reader.ReadChar()) != '\n') input += s; // read untill end of line
            string[] param = input.Split(','); // split features by ,
            string[] ret = { param[0], param[1], param[2], param[3], param[4],
                            param[5], param[6], param[7], param[8], param[9],
                            param[10], param[11], param[12], param[13], param[14],
                            param[15], param[16], param[17], param[18], param[19],
                            param[20], param[21], param[22], param[23], param[24],
                            param[25], param[26], param[27], param[28], param[29],
                            param[30], param[31], param[32], param[33], param[34],
                            param[35], param[36], param[37], param[38], param[39], param[40], param[41]};

            Aileron = param[0];
            Elevator = param[1];
            Thorottle = param[0];
            Roll = param[17];
            Pitch = param[18];
            AirSpeed = param[21];
            AltMeter = param[16];
            Yaw = param[20];
            Direction = param[37];




            return ret;

        }

        //getting feature by his index
        private string convertNumToFeature(int index)
        {
            List<string> features = csvParser.DataDict.Keys.ToList();
            return features.ElementAt(index);
        }

        //getting feature index by his name
        private int convertFeatureToNumber(string featureName)
        {
            List<string> features = csvParser.DataDict.Keys.ToList();
            return features.IndexOf(featureName);
            
        }

        //reading data from the fg and set the latest points 
        void StartRead()
        {

            while (!Stop)
            {
                string[] args = Read();

                LastFeaturePoint = new DataPoint(time += 0.1, Convert.ToDouble(args[convertFeatureToNumber(Feature)]));
                LastCorrelatedFeaturePoint = new DataPoint(time, Convert.ToDouble(args[convertFeatureToNumber(CorrelatedFeature)]));
                LastLinearRegressionPoint = new DataPoint(Convert.ToDouble(args[convertFeatureToNumber(Feature)]),
                                                            Convert.ToDouble(args[convertFeatureToNumber(CorrelatedFeature)]));
                Thread.Sleep(100);
            }
        }

        //starting the reading thread
        public void ReadThread()
        {
            getInfoThread = new Thread(() => StartRead());
            if (!(getInfoThread.ThreadState == ThreadState.Running))

            {
                getInfoThread.Start();
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                if (propertyName == "CorrelatedFeature")
                {
                    CorrelatedFeature = correlationDict[Feature];
                }
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

