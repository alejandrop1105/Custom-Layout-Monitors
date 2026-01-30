using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CustomLayoutMonitors.ViewModels;

namespace CustomLayoutMonitors
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Minimize to Tray behavior
            e.Cancel = true;
            this.Hide();
        }

        private void ClosePanel_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ClosePanelCommand.Execute(null);
            }
        }

        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            if (CarouselScroll != null)
            {
                CarouselScroll.ScrollToHorizontalOffset(CarouselScroll.HorizontalOffset - 220);
            }
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            if (CarouselScroll != null)
            {
                CarouselScroll.ScrollToHorizontalOffset(CarouselScroll.HorizontalOffset + 220);
            }
        }

        private void CarouselScroll_Loaded(object sender, RoutedEventArgs e) => UpdateScrollButtons();
        private void CarouselScroll_ScrollChanged(object sender, ScrollChangedEventArgs e) => UpdateScrollButtons();
        private void CarouselScroll_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateScrollButtons();

        private void UpdateScrollButtons()
        {
            if (CarouselScroll == null || BtnScrollLeft == null || BtnScrollRight == null) return;

            // Show buttons only if content is wider than viewport
            bool hasOverflow = CarouselScroll.ScrollableWidth > 0;
            
            // Optionally, we can also hide left button at start and right button at end
            // But per user request "appear if profiles exceed screen width", we mainly care about overflow existence.
            // Let's implement full UX: Hide Left if at start, Hide Right if at end, Hide both if no overflow.
            
            if (!hasOverflow)
            {
                BtnScrollLeft.Visibility = Visibility.Hidden;
                BtnScrollRight.Visibility = Visibility.Hidden;
                return;
            }

            // If we have overflow, handle individual visibility
            BtnScrollLeft.Visibility = CarouselScroll.HorizontalOffset > 0 ? Visibility.Visible : Visibility.Hidden;
            BtnScrollRight.Visibility = CarouselScroll.HorizontalOffset < CarouselScroll.ScrollableWidth ? Visibility.Visible : Visibility.Hidden;
        }
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            // Simple Fade Out Animation
            var anim = new System.Windows.Media.Animation.DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(UIElement.OpacityProperty, anim);
            await System.Threading.Tasks.Task.Delay(200);
            Close();
        }
    }
}