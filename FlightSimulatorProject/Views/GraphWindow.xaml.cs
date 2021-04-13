using FlightSimulatorProject.Models;
using FlightSimulatorProject.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FlightSimulatorProject.Views
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {

        PanelView pv;
        public GraphWindow(PanelView m)
        {
            InitializeComponent();
            this.pv = m;

        }
        
        
        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null)
            {
                pv.SetFeature((string)item.Content);
            }
        }
        
    }
}