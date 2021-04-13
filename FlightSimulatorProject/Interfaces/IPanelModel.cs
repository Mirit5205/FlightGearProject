using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorProject.Interfaces
{
    public interface IPanelModel : INotifyPropertyChanged
    {
        double Aileron
        {
            get;
            set;

        }

        double PlaySpeed
        {
            get;
            set;

        }

        int SleepTime
        {
            get;
            set;

        }

        int DataIndex
        {
            get;
            set;

        }
        void Play();
        void Pause();
        void Stop();
        void Faster();
        void Slower();



        void Connect();

        void FastForward();

        void FastBackward();

        //void OpenServer();

        //void Read();


    }
}

