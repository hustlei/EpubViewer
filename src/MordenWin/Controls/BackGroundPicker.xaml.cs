using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lei.UI
{
    public partial class BackGroundPicker : MordenWindow
    {
        public BackGroundPicker()
        {
            this.InitializeComponent();
            SetInitialState();
        }

        public Color? Color
        {
            get { return colorCanvas.SelectedColor; }
            set { colorCanvas.SelectedColor = value; }
        }

        private void SetInitialState()
        {
            //this.WindowStyle = WindowStyle.ToolWindow;
            //this.ResizeMode = ResizeMode.NoResize;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}