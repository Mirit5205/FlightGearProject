using FlightSimulatorProject.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlightSimulatorProject.Models
{
    public class PanelModel : IPanelModel
    {
        bool isPlay = false;
        const string LOCAL_HOST = "127.0.0.1";
        const int REMOTE_PORT = 5400;
        const string FLIGHTS_DATA = "reg_flight.csv";

        const int MAX_SLEEP_TIME = 160;
        const int MIN_SLEEP_TIME = 40;
        const int INITIAL_SLEEP_TIME = 100;
        const int SLEEP_TIME_STEP = 20;
        const int FAST_SLEEP = 60;

        const int PLAY = 0;
        const int FAST_FORWARD = 1;
        const int FAST_BACKWARD = 2;
        const int PAUSE = 3;

        double aileron;
        double elevator;
        int sleepTime = INITIAL_SLEEP_TIME;

        List<double> aileronList = new List<double>();

        IServer infoServer;


        public int SleepTime
        {
            get
            {
                return sleepTime;
            }
            set
            {

            }
        }
        public double Aileron
        {
            get { return aileron; }
            set
            {
                if (aileron != value)
                {
                    aileron = value;
                    OnPropertyChange("Aileron");

                }
            }
        }



        IClient client;
        //IServer infoServer;


        Thread playThread;

        int mode = -1;

        bool isConnect = false;

        double playSpeed = 1;
        public double PlaySpeed
        {
            get { return playSpeed; }
            set
            {
                if (playSpeed != value)
                {
                    playSpeed = value;
                    OnPropertyChange("PlaySpeed");

                }
            }
        }
        
        string[] dataLines;

        public int DataLength
        {
            get
            {
                return dataLines.Length;
            }
        }

        int dataIndex = 0;
        public int DataIndex
        {
            get
            {
                return dataIndex;
            }
            set
            {
                if (dataIndex != value)
                {
                    dataIndex = value;
                    OnPropertyChange("DataIndex");

                }
            }
        }

        public PanelModel(IClient c, IServer s)
        {
            client = c;
            infoServer = s;
        }


        private void SendDataForward(int index, int modeNum)
        {

            for (int i = index; i < dataLines.Length; i++)
            {
                if (mode != modeNum)
                {
                    break;
                }
                sendData(i);
                DataIndex++;
            }

        }

        private void SendDataBackward(int index, int modeNum)
        {


            for (int i = index; i >= 0; i--)
            {
                if (mode != modeNum)
                {
                    break;
                }
                sendData(i);
                dataIndex--;
            }
        }

        public void sendData(int i)
        {
            client.SendMessage(dataLines[i]);
            Thread.Sleep(sleepTime);
        }
        public void Stop()
        {
            if (isConnect)
            {
                client.DisConnect();
                playThread.Abort();
                isConnect = false;
                infoServer.Close();
            }
        }

        public void run()
        {
            while (isConnect)
            {
                if (mode == PLAY)
                {
                    sleepTime = INITIAL_SLEEP_TIME;
                    SendDataForward(DataIndex, PLAY);
                }
                else if (mode == FAST_FORWARD)
                {
                    //sleepTime = 20;
                    SendDataForward(DataIndex, FAST_FORWARD);
                }
                else if (mode == FAST_BACKWARD)
                {
                    sleepTime = INITIAL_SLEEP_TIME;
                    SendDataBackward(DataIndex, FAST_BACKWARD);


                }
                else if (mode == PAUSE)
                {
                    //do noting == stop
                }
                else
                {


                }

            }
        }
        public void Play()
        {
            if (!isPlay)
            {
                isPlay = true;
                infoServer.ReadThread();
            }
            mode = PLAY;
        }

        public void Pause()
        {
            mode = PAUSE;
        }

        public void FastForward()
        {
            sleepTime = FAST_SLEEP;
            PlaySpeed = 1.5;

            mode = FAST_FORWARD;
        }

        public void FastBackward()
        {
            sleepTime = FAST_SLEEP;
            PlaySpeed = 1.5;

            mode = FAST_BACKWARD;
        }

        public void Faster()
        {
            if (sleepTime >= MIN_SLEEP_TIME)
            {
                PlaySpeed += 0.25;
                sleepTime -= SLEEP_TIME_STEP;
            }
        }

        public void Slower()
        {
            if (sleepTime < MAX_SLEEP_TIME)
            {
                PlaySpeed -= 0.25;
                sleepTime += SLEEP_TIME_STEP;
            }
        }


        private void LoadCSV()
        {
            dataLines = System.IO.File.ReadAllLines(FLIGHTS_DATA);
        }
        public void Connect()
        {
            if (!isConnect)
            {
                isConnect = true;
                LoadCSV();
                client.Connect("localhost", 1); //the arguments is not neccesary
                playThread = new Thread(() => run());
                if (!(playThread.ThreadState == ThreadState.Running))
                {
                    playThread.Start();
                };
            }

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
