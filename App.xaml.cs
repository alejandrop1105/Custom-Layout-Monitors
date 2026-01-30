using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Windows.Input;
using CustomLayoutMonitors.ViewModels;

namespace CustomLayoutMonitors
{
    public partial class App : Application
    {
        private NotifyIcon? _notifyIcon;
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            try 
            {
                base.OnStartup(e);

                _mainWindow = new MainWindow();
                
                // Set window icon safely
                try {
                    _mainWindow.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Assets/app_icon.png"));
                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Window Icon Error: {ex.Message}");
                }

                _notifyIcon = new NotifyIcon();
                _notifyIcon.Text = "Monitor de Layouts Premium";
                _notifyIcon.Visible = true;
                _notifyIcon.Icon = SystemIcons.Application; // Default fallback

                // Try to load custom tray icon safely
                try
                {
                    var uri = new Uri("pack://application:,,,/Assets/app_icon.png");
                    var streamInfo = Application.GetResourceStream(uri);
                    if (streamInfo != null)
                    {
                        using (var stream = streamInfo.Stream)
                        using (var bitmap = new Bitmap(stream))
                        using (var smallBitmap = new Bitmap(bitmap, new System.Drawing.Size(32, 32)))
                        {
                            _notifyIcon.Icon = Icon.FromHandle(smallBitmap.GetHicon());
                        }
                    }
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Tray Icon Error: {ex.Message}");
                }

                _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();

                var contextMenu = new ContextMenuStrip();
                var designsMenuItem = new ToolStripMenuItem("Diseños Registrados");
                contextMenu.Items.Add(designsMenuItem);
                contextMenu.Items.Add("Configuración", null, (s, args) => ShowMainWindow());
                contextMenu.Items.Add(new ToolStripSeparator());
                contextMenu.Items.Add("Salir", null, (s, args) => ExitApplication());
                _notifyIcon.ContextMenuStrip = contextMenu;

                contextMenu.Opening += (s, args) =>
                {
                    designsMenuItem.DropDownItems.Clear();
                    var viewModel = _mainWindow.DataContext as CustomLayoutMonitors.ViewModels.MainViewModel;
                    if (viewModel != null)
                    {
                        foreach (var profile in viewModel.Profiles)
                        {
                            var visualItems = Utils.ProfileVisualizer.GetVisualItems(profile);
                            var preview = Utils.ProfileVisualizer.CreatePreviewBitmap(visualItems, 64, 32);
                            var item = new ToolStripMenuItem(profile.Name, preview);
                            item.Click += (sender, clickArgs) => 
                            {
                                //viewModel.SelectedProfile = profile;
                                viewModel.ApplyCommand.Execute(profile);
                            };
                            designsMenuItem.DropDownItems.Add(item);
                        }
                        if (viewModel.Profiles.Count == 0)
                            designsMenuItem.DropDownItems.Add(new ToolStripMenuItem("No hay diseños") { Enabled = false });
                    }
                };

                _mainWindow.Show();
            }
            catch (Exception fatalEx)
            {
                System.Windows.MessageBox.Show($"Error fatal al iniciar: {fatalEx.Message}\n\n{fatalEx.StackTrace}", "Error de Inicio");
                Shutdown();
            }
        }

        private void ShowMainWindow()
        {
            if (_mainWindow == null) return;
            if (_mainWindow.IsVisible)
            {
                if (_mainWindow.WindowState == WindowState.Normal || _mainWindow.WindowState == WindowState.Maximized)
                {
                    _mainWindow.Activate();
                }
                else
                {
                    _mainWindow.WindowState = WindowState.Normal;
                    _mainWindow.Activate();
                }
            }
            else
            {
                _mainWindow.BeginAnimation(UIElement.OpacityProperty, null);
                _mainWindow.Opacity = 1;
                _mainWindow.Show();
            }
        }

        private void ExitApplication()
        {
            _notifyIcon?.Dispose();
            _notifyIcon = null;
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
