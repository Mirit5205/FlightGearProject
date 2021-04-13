using FlightSimulatorProject.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorProject.ViewModels
{
    public class PanelViewModel : INotifyPropertyChanged
    {
        IPanelModel _panel;

        public IPanelModel Panel
        {
            get { return _panel; }
            set
            {
                if (Equals(_panel, value)) return;
                _panel = value;
                OnPropertyChange(nameof(Panel));
            }
        }
        public PanelViewModel(IPanelModel p)
        {
            _panel = p;
            //notify panel when there is changes 
            _panel.PropertyChanged += OnModelChanged;

        }

        //playSpeed property
        public double PlaySpeed {
            get
            {
                return _panel.PlaySpeed;
            }
            set
            {
                if (_panel.PlaySpeed != value)
                {
                    _panel.PlaySpeed = value;
                    OnPropertyChange("PlaySpeed");
                }
            }
        }

        public int SleepTime
        {
            get
            {
                return _panel.SleepTime;
            }
        }

        //data index (num of row in the csv file) property
        public int DataIndex
        {
            get { return _panel.DataIndex; }
            set
            {
                if (_panel.DataIndex != value)
                {
                    _panel.DataIndex = value;
                    OnPropertyChange("DataIndex");
                }
            }
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
