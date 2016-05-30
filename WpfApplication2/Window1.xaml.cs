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

namespace WpfApplication2
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            POINT p = new POINT();
           
            InitializeComponent();
        }
        POINT DownPosition = new POINT();
        POINT UpPosition = new POINT();
        public struct POINT
        {
            public double X;
            public double Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            Point ppp = (e.Source as FrameworkElement).PointToScreen(pp);//WPF方法
            DownPosition.X = pp.X;
            DownPosition.Y = pp.Y;
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            Point ppp = (e.Source as FrameworkElement).PointToScreen(pp);//WPF方法
            UpPosition.X = pp.X;
            UpPosition.Y = pp.Y;
            TextBoxChange();
        }
        private void TextBoxChange()
        {
           float Dis =  Convert.ToSingle( TwoPointWeight(Convert.ToDouble( DownPosition.X),
                                                           Convert.ToDouble(DownPosition.Y),
                                                           Convert.ToDouble(UpPosition.X),
                                                           Convert.ToDouble(UpPosition.Y)));
            tbWeight.Text = Dis.ToString();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        private double TwoPointWeight(double x1, double y1, double x2, double y2)//求两点之间的距离
        {
            double Weight = 0;
            Weight = System.Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return Weight;
        }
    }
}
