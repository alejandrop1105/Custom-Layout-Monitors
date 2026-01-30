using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomLayoutMonitors.Controls
{
    public partial class Monitor3DView : System.Windows.Controls.UserControl
    {
        public Monitor3DView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Monitor3DView), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(Monitor3DView), new PropertyMetadata(0.0));

        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
    }
}
