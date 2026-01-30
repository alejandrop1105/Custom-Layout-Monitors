using System;
using System.Collections.Generic;
using CustomLayoutMonitors.Services.Native;

namespace CustomLayoutMonitors.Models
{
    public class DisplayProfile : System.ComponentModel.INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name 
        { 
            get => _name; 
            set 
            { 
                _name = value; 
                OnPropertyChanged(nameof(Name)); 
            } 
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
        public List<PathInfoDTO> Paths { get; set; } = new();
        public List<ModeInfoDTO> Modes { get; set; } = new();

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }

    // DTOs for JSON serialization
    public class PathInfoDTO
    {
        public NativeMethods.DISPLAYCONFIG_PATH_SOURCE_INFO SourceInfo { get; set; }
        public NativeMethods.DISPLAYCONFIG_PATH_TARGET_INFO TargetInfo { get; set; }
        public uint Flags { get; set; }
    }

    public class ModeInfoDTO
    {
        public uint InfoType { get; set; }
        public uint Id { get; set; }
        public NativeMethods.LUID AdapterId { get; set; }
        public NativeMethods.DISPLAYCONFIG_MODE_INFO_UNION ModeInfo { get; set; }
    }
}
