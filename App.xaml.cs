using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Windows.Input;
using CustomLayoutMonitors.ViewModels;
using Microsoft.Win32;

namespace CustomLayoutMonitors
{
    public partial class App : Application
    {
        private NotifyIcon? _notifyIcon;
        private MainWindow? _mainWindow;
        private ToolStripMenuItem? _autoStartMenuItem;
        
        private const string APP_NAME = "CustomLayoutMonitors";
        private const string REGISTRY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        protected override void OnStartup(StartupEventArgs e)
        {
            try 
            {
                base.OnStartup(e);

                _mainWindow = new MainWindow();
                
                // Set window icon safely
                try {
                    _mainWindow.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Assets/tray_icon.png"));
                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Window Icon Error: {ex.Message}");
                }

                _notifyIcon = new NotifyIcon();
                _notifyIcon.Text = "Monitor Layout Manager";
                _notifyIcon.Visible = true;
                _notifyIcon.Icon = SystemIcons.Application; // Default fallback

                // Try to load custom tray icon safely
                try
                {
                    var uri = new Uri("pack://application:,,,/Assets/tray_icon.png");
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
                
                // Auto-start option
                _autoStartMenuItem = new ToolStripMenuItem("Iniciar con Windows");
                _autoStartMenuItem.CheckOnClick = true;
                _autoStartMenuItem.Checked = IsAutoStartEnabled();
                _autoStartMenuItem.Click += (s, args) => ToggleAutoStart();
                contextMenu.Items.Add(_autoStartMenuItem);
                
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
                            
                            // Add checkmark if this profile is active
                            if (profile.IsActive)
                            {
                                item.Checked = true;
                            }
                            
                            item.Click += (sender, clickArgs) => 
                            {
                                viewModel.ApplyCommand.Execute(profile);
                            };
                            designsMenuItem.DropDownItems.Add(item);
                        }
                        if (viewModel.Profiles.Count == 0)
                            designsMenuItem.DropDownItems.Add(new ToolStripMenuItem("No hay diseños") { Enabled = false });
                    }
                    
                    // Update auto-start checkbox state
                    if (_autoStartMenuItem != null)
                    {
                        _autoStartMenuItem.Checked = IsAutoStartEnabled();
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

        private bool IsAutoStartEnabled()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, false))
                {
                    if (key != null)
                    {
                        var value = key.GetValue(APP_NAME);
                        return value != null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking auto-start: {ex.Message}");
            }
            return false;
        }

        private void ToggleAutoStart()
        {
            try
            {
                if (IsAutoStartEnabled())
                {
                    // Remove from auto-start
                    using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
                    {
                        key?.DeleteValue(APP_NAME, false);
                    }
                }
                else
                {
                    // Add to auto-start
                    var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
                        {
                            key?.SetValue(APP_NAME, $"\"{exePath}\"");
                        }
                    }
                }
                
                // Update checkbox state
                if (_autoStartMenuItem != null)
                {
                    _autoStartMenuItem.Checked = IsAutoStartEnabled();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al configurar inicio automático: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

