using System;
using Avalonia.Controls;

namespace BitLifeClone.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Sizing logic belongs here, applied directly to the Desktop Window
            var screen = this.Screens.Primary;
            if (screen != null)
            {
                double areaScaleFactor = Math.Sqrt(0.4); 
                
                this.Width = (screen.WorkingArea.Width / screen.Scaling) * areaScaleFactor;
                this.Height = (screen.WorkingArea.Height / screen.Scaling) * areaScaleFactor;
            }
        }
    }
}