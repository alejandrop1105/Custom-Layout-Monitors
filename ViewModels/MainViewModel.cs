using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CustomLayoutMonitors.Models;
using CustomLayoutMonitors.Services;
using CustomLayoutMonitors.Utils;


namespace CustomLayoutMonitors.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDisplayService _displayService;
        private const string ProfileFileName = "profiles.json";
        
        // Panel state
        private bool _isPanelOpen;
        private string _panelTitle = "Crear Nuevo Perfil";
        private string _panelProfileName = "";
        private DisplayProfile? _editingProfile;
        private ObservableCollection<MonitorVisualItem> _panelPreviewItems = new();

        public ObservableCollection<DisplayProfile> Profiles { get; set; } = new();

        // Panel Properties
        public bool IsPanelOpen
        {
            get => _isPanelOpen;
            set { _isPanelOpen = value; OnPropertyChanged(); }
        }

        public string PanelTitle
        {
            get => _panelTitle;
            set { _panelTitle = value; OnPropertyChanged(); }
        }

        public string PanelProfileName
        {
            get => _panelProfileName;
            set { _panelProfileName = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MonitorVisualItem> PanelPreviewItems
        {
            get => _panelPreviewItems;
            set { _panelPreviewItems = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand ApplyCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand OpenPanelCommand { get; }
        public ICommand ClosePanelCommand { get; }
        public ICommand SavePanelCommand { get; }
        public ICommand ScrollLeftCommand { get; }
        public ICommand ScrollRightCommand { get; }

        public MainViewModel()
        {
            _displayService = new DisplayService();
            LoadProfiles();
            
            ApplyCommand = new RelayCommand<DisplayProfile>(ApplyProfile, p => p != null);
            DeleteCommand = new RelayCommand<DisplayProfile>(DeleteProfile, p => p != null);
            EditProfileCommand = new RelayCommand<DisplayProfile>(EditProfile, p => p != null);
            OpenPanelCommand = new RelayCommand(OpenPanel);
            ClosePanelCommand = new RelayCommand(ClosePanel);
            SavePanelCommand = new RelayCommand(SavePanel);
            ScrollLeftCommand = new RelayCommand(ScrollLeft);
            ScrollRightCommand = new RelayCommand(ScrollRight);
        }

        private void LoadProfiles()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfileFileName);
            var loaded = JsonStorage.Load<ObservableCollection<DisplayProfile>>(path);
            if (loaded != null)
            {
                Profiles.Clear();
                foreach (var p in loaded) Profiles.Add(p);
            }
        }

        private void SaveProfiles()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfileFileName);
            JsonStorage.Save(path, Profiles);
        }

        private void OpenPanel()
        {
            _editingProfile = null;
            PanelTitle = "Crear Nuevo Perfil";
            PanelProfileName = "";
            
            // Get current monitor configuration for preview
            try
            {
                var profile = _displayService.GetCurrentProfile("Preview");
                var items = ProfileVisualizer.GetVisualItems(profile, 300, 180);
                PanelPreviewItems.Clear();
                foreach (var item in items) PanelPreviewItems.Add(item);
            }
            catch { }
            
            IsPanelOpen = true;
        }

        private void EditProfile(DisplayProfile profile)
        {
            if (profile == null) return;
            
            _editingProfile = profile;
            PanelTitle = "Editar Perfil";
            PanelProfileName = profile.Name;
            
            var items = ProfileVisualizer.GetVisualItems(profile, 300, 180);
            PanelPreviewItems.Clear();
            foreach (var item in items) PanelPreviewItems.Add(item);
            
            IsPanelOpen = true;
        }

        private void ClosePanel()
        {
            IsPanelOpen = false;
            _editingProfile = null;
        }

        private void SavePanel()
        {
            if (string.IsNullOrWhiteSpace(PanelProfileName))
            {
                System.Windows.MessageBox.Show("Por favor ingresa un nombre para el perfil.", "Nombre requerido", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_editingProfile != null)
                {
                    // Editing existing profile
                    _editingProfile.Name = PanelProfileName;
                    SaveProfiles();
                }
                else
                {
                    // Creating new profile
                    var profile = _displayService.GetCurrentProfile(PanelProfileName);
                    Profiles.Add(profile);
                    SaveProfiles();
                }
                
                ClosePanel();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al guardar: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ApplyProfile(DisplayProfile profile)
        {
            if (profile == null) return;
            try
            {
                _displayService.ApplyProfile(profile);
                
                // Update Active State
                foreach (var p in Profiles) p.IsActive = false;
                profile.IsActive = true;

                System.Windows.MessageBox.Show("✓ Configuración aplicada", "Éxito", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void DeleteProfile(DisplayProfile profile)
        {
            if (profile == null) return;
            var result = System.Windows.MessageBox.Show($"¿Eliminar '{profile.Name}'?", "Confirmar", 
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                Profiles.Remove(profile);
                SaveProfiles();
            }
        }

        private void ScrollLeft()
        {
            // Scroll will be handled in code-behind or behavior
        }

        private void ScrollRight()
        {
            // Scroll will be handled in code-behind or behavior
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter == null && !typeof(T).IsValueType)
                return _canExecute == null || _canExecute(default(T)!);
            if (parameter is T typedParameter)
                return _canExecute == null || _canExecute(typedParameter);
            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T typedParameter)
                _execute(typedParameter);
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
