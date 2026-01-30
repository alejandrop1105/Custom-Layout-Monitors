using System;
using System.Collections.Generic;

namespace CustomLayoutMonitors.Models
{
    public enum MonitorOrientation
    {
        Landscape,
        Portrait
    }

    public class MonitorVisualItem
    {
        public string Content { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsPrimary { get; set; }
        public MonitorOrientation Orientation { get; set; }
        public int RotationDegrees { get; set; }
        public double TiltAngle { get; set; } // For 3D visual effect
    }
}
