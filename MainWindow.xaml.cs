using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CustomLayoutMonitors.ViewModels;

namespace CustomLayoutMonitors
{
    public partial class MainWindow : Window
    {
        private double _targetScrollOffset = 0;
        private const double SCROLL_AMOUNT = 220;
        private const double ANIMATION_DURATION_MS = 300;

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

        private void CarouselScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CarouselScroll != null)
            {
                // Convert vertical scroll to horizontal with animation
                double scrollAmount = e.Delta > 0 ? -SCROLL_AMOUNT : SCROLL_AMOUNT;
                AnimateScroll(scrollAmount);
                e.Handled = true;
            }
        }

        private void AnimateScroll(double delta)
        {
            if (CarouselScroll == null) return;

            // Calculate target offset
            _targetScrollOffset = CarouselScroll.HorizontalOffset + delta;
            
            // Clamp to valid range
            _targetScrollOffset = Math.Max(0, Math.Min(_targetScrollOffset, CarouselScroll.ScrollableWidth));

            // Use CompositionTarget.Rendering for smooth animation
            var startOffset = CarouselScroll.HorizontalOffset;
            var endOffset = _targetScrollOffset;
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromMilliseconds(ANIMATION_DURATION_MS);

            EventHandler? handler = null;
            handler = (s, args) =>
            {
                var elapsed = DateTime.Now - startTime;
                var progress = Math.Min(elapsed.TotalMilliseconds / duration.TotalMilliseconds, 1.0);
                
                // Apply easing (cubic ease out)
                var easedProgress = 1 - Math.Pow(1 - progress, 3);
                
                var currentOffset = startOffset + (endOffset - startOffset) * easedProgress;
                CarouselScroll.ScrollToHorizontalOffset(currentOffset);

                if (progress >= 1.0)
                {
                    System.Windows.Media.CompositionTarget.Rendering -= handler;
                }
            };

            System.Windows.Media.CompositionTarget.Rendering += handler;
        }

        // These handlers are kept for potential future use but do nothing now
        private void CarouselScroll_Loaded(object sender, RoutedEventArgs e) { }
        private void CarouselScroll_ScrollChanged(object sender, ScrollChangedEventArgs e) { }
        private void CarouselScroll_SizeChanged(object sender, SizeChangedEventArgs e) { }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            // Simple Fade Out Animation
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(UIElement.OpacityProperty, anim);
            await System.Threading.Tasks.Task.Delay(200);
            Close();
        }
    }
}

