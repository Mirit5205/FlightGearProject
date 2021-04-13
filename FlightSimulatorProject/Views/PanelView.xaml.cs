using FlightSimulatorProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlightSimulatorProject.Views
{
    /// <summary>
    /// Interaction logic for PanelView.xaml
    /// </summary>
    public partial class PanelView : UserControl
    {
        MasterViewModel m;
        GraphWindow graphWindow;

        const string EXE_FILE = "fgfs.exe";

        const string FG_SOCKET = "--generic=socket,out,10,127.0.0.1,6400,tcp,playback_small";

        const string FG_SIMULATON = "--fdm=null";

        const string FG_CLIENT = "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small";

        public MasterViewModel M {
            get
            {
                return m;
            }
            set
            {
                m = value;
            }
        }

        public string APIPath {get; set;}

        public PanelView()
        {
            InitializeComponent();
            M = new MasterViewModel();
            //this.DataContext = m;
            this.DataContext = this;


        }

        //public MasterViewModel M { get; set; }
        public void SetFeature(string s)
        {
            m.Feature = s;
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.Play();
        }

        private void FastForward_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.FastForward();

        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.Connect();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.Pause();

        }

        private void fastBackward_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.FastBackward();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.Stop();
        }

        private void VideoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m.VM_Panel.Panel.DataIndex = (int)slValue.Value;

        }

        private void faster_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.Faster();
        }

        private void slower_Click(object sender, RoutedEventArgs e)
        {
            m.VM_Panel.Panel.Slower();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            m.GraphRender();

            if (this.graphWindow == null)
            {
                this.graphWindow = new GraphWindow(this);
                graphWindow.DataContext = m;
                this.graphWindow.Show();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ProcessStartInfo x = new ProcessStartInfo(EXE_FILE, FG_SIMULATON + " " + FG_CLIENT + " " + FG_SOCKET); 
            x.UseShellExecute = true;
            x.WorkingDirectory = @APIPath;
            try 
            {
                Process.Start(x);
            } catch
            {
                MessageBox.Show("please enter valid path");
            }


        }
    }
}
