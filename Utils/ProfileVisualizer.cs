using System.Collections.Generic;
using System.Linq;
using CustomLayoutMonitors.Models;

namespace CustomLayoutMonitors.Utils
{
    public static class ProfileVisualizer
    {
        public static IEnumerable<MonitorVisualItem> GetVisualItems(DisplayProfile? profile)
        {
            return GetVisualItems(profile, 300, 120);
        }

        public static IEnumerable<MonitorVisualItem> GetVisualItems(DisplayProfile? profile, double canvasWidth, double canvasHeight)
        {
            if (profile == null || profile.Paths == null || profile.Modes == null)
                return Enumerable.Empty<MonitorVisualItem>();

            var items = new List<MonitorVisualItem>();
            
            // Map source IDs to target IDs (to handle clones)
            var targetsBySource = new Dictionary<uint, List<uint>>();
            foreach (var path in profile.Paths)
            {
                if (!targetsBySource.ContainsKey(path.SourceInfo.id))
                    targetsBySource[path.SourceInfo.id] = new List<uint>();
                targetsBySource[path.SourceInfo.id].Add(path.TargetInfo.id);
            }

            // Map each unique target ID to a stable number for the UI
            int monitorCounter = 1;
            var targetNumbers = new Dictionary<uint, int>();
            foreach (var path in profile.Paths)
            {
                if (!targetNumbers.ContainsKey(path.TargetInfo.id))
                    targetNumbers[path.TargetInfo.id] = monitorCounter++;
            }

            foreach (var sourceId in targetsBySource.Keys)
            {
                // Find Source Mode info (Type 1 is Source)
                var mode = profile.Modes.FirstOrDefault(m => m.InfoType == 1 && m.Id == sourceId);
                if (mode == null) continue;

                var sourceMode = mode.ModeInfo.sourceMode;
                var targets = targetsBySource[sourceId];
                var displayNumbers = targets.Select(t => targetNumbers[t]).OrderBy(n => n);
                
                // Get rotation from path target info
                var path = profile.Paths.FirstOrDefault(p => p.SourceInfo.id == sourceId);
                uint rotation = path?.TargetInfo.rotation ?? 0;
                
                // Rotation values: 0 = Identity, 1 = 90°, 2 = 180°, 3 = 270°
                uint rotationDegrees = rotation switch
                {
                    1 => 0,   // Identity
                    2 => 90,  // 90
                    3 => 180, // 180
                    4 => 270, // 270
                    _ => 0
                };
                
                // Portrait if rotated 90° or 270°
                var orientation = (rotationDegrees == 90 || rotationDegrees == 270) 
                    ? MonitorOrientation.Portrait 
                    : MonitorOrientation.Landscape;
                
                // Swap dimensions if portrait (so visual rect matches rotation)
                double finalWidth = orientation == MonitorOrientation.Portrait ? sourceMode.height : sourceMode.width;
                double finalHeight = orientation == MonitorOrientation.Portrait ? sourceMode.width : sourceMode.height;

                items.Add(new MonitorVisualItem
                {
                    Content = string.Join("|", displayNumbers),
                    X = sourceMode.position.x,
                    Y = sourceMode.position.y,
                    Width = finalWidth,
                    Height = finalHeight,
                    IsPrimary = sourceMode.position.x == 0 && sourceMode.position.y == 0,
                    Orientation = orientation,
                    RotationDegrees = (int)rotationDegrees
                });
            }

            if (!items.Any()) return items;

            // Normalize coordinates (translate to origin)
            double minX = items.Min(i => i.X);
            double minY = items.Min(i => i.Y);

            foreach (var item in items)
            {
                item.X -= minX;
                item.Y -= minY;
            }

            // AUTO-ALIGNMENT: If items form a single row (no horizontal overlap), align them to the bottom
            bool hasOverlap = false;
            for (int i = 0; i < items.Count; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    double left = System.Math.Max(items[i].X, items[j].X);
                    double right = System.Math.Min(items[i].X + items[i].Width, items[j].X + items[j].Width);
                    if (left < right) // Overlap exists
                    {
                        hasOverlap = true;
                        break;
                    }
                }
                if (hasOverlap) break;
            }

            if (!hasOverlap && items.Count > 1)
            {
                double maxY = items.Max(i => i.Y + i.Height);
                foreach (var item in items)
                {
                    // Shift Y so that (Y + Height) = maxY
                    item.Y = maxY - item.Height;
                }
            }

            // Calculate total size
            double totalWidth = items.Max(i => i.X + i.Width);
            double totalHeight = items.Max(i => i.Y + i.Height);

            // Scale to fit in canvas with padding
            const double padding = 10;

            double availableWidth = canvasWidth - (padding * 2);
            double availableHeight = canvasHeight - (padding * 2);

            // 2. Normalization Logic (Fixed Scale Factor)
            // Use a less aggressive normalization to keep monitors visible
            // Cap to 2x monitors worth so single monitors aren't gigantic but still visible
            double standardTwoMonitorWidth = (1920 * 2) + 20; // ~3860px
            double normalizationWidth = System.Math.Max(totalWidth, standardTwoMonitorWidth);

            // Calculate Scale based on available space vs normalization width
            double scale = System.Math.Min(availableWidth / normalizationWidth, availableHeight / totalHeight);
            
            // Adjust scale slightly up if only 1 monitor to fill a bit more but not too much (user preference balance)
            // Actually user said "conserve el tamaño que tendria si fuera mas de unos", so strictly use normalizationWidth.
            // Maybe add a small safety cap just in case.
            
            // 3. Calculate Tilt for 3D Effect (if > 2 monitors)
            bool applyTilt = items.Count > 2;
            var sortedItems = items.OrderBy(i => i.X).ToList();

            foreach (var item in items)
            {
                // Simple Tilt Logic: Outer monitors get angled inwards
                if (applyTilt)
                {
                    if (item == sortedItems.First()) item.TiltAngle = -15;  // Left Monitor tilts inward (toward center)
                    else if (item == sortedItems.Last()) item.TiltAngle = 15; // Right Monitor tilts inward (toward center)
                    else item.TiltAngle = 0;
                }
                else
                {
                    item.TiltAngle = 0;
                }

                item.X *= scale;
                item.Y *= scale;
                item.Width *= scale;
                item.Height *= scale;
                
                // 5. Adjust Y position for tilted monitors to align top edges
                // The skew transform causes visual displacement that needs compensation
                if (item.TiltAngle != 0)
                {
                    double angleRad = System.Math.Abs(item.TiltAngle) * System.Math.PI / 180;
                    double verticalOffset = item.Height * System.Math.Tan(angleRad) * 0.65; // 65% of the calculated offset
                    item.Y += verticalOffset;
                }
            }

            // 4. Compact monitors horizontally (make them overlap significantly)
            if (items.Count > 1)
            {
                var orderedItems = items.OrderBy(i => i.X).ToList();
                
                for (int i = 1; i < orderedItems.Count; i++)
                {
                    var prev = orderedItems[i - 1];
                    var curr = orderedItems[i];
                    
                    // Determine overlap percentage based on orientation
                    // Portrait monitors are narrower, so use less overlap to avoid excessive superposition
                    bool bothPortrait = prev.Orientation == MonitorOrientation.Portrait && 
                                        curr.Orientation == MonitorOrientation.Portrait;
                    bool anyPortrait = prev.Orientation == MonitorOrientation.Portrait || 
                                       curr.Orientation == MonitorOrientation.Portrait;
                    
                    // Use smaller overlap for portrait monitors
                    double overlapPercent = bothPortrait ? 0.10 : (anyPortrait ? 0.15 : 0.25);
                    double overlapAmount = curr.Width * overlapPercent;
                    double targetX = prev.X + prev.Width - overlapAmount;
                    double shift = curr.X - targetX;
                    
                    // Shift this and all subsequent monitors to the left
                    for (int j = i; j < orderedItems.Count; j++)
                    {
                        orderedItems[j].X -= shift;
                    }
                }
            }

            // Center the group strictly based on its visual dimensions (recalculated after compaction)
            double actualMaxX = items.Max(i => i.X + i.Width);
            double actualMinX = items.Min(i => i.X);
            double visualWidth = actualMaxX - actualMinX;
            double visualHeight = totalHeight * scale;
            
            double offsetX = (availableWidth - visualWidth) / 2;
            double offsetY = (availableHeight - visualHeight) / 2;

            foreach (var item in items)
            {
                item.X += offsetX;
                item.Y += offsetY;
            }

            return items;
        }

        public static System.Drawing.Bitmap CreatePreviewBitmap(IEnumerable<MonitorVisualItem> items, int width = 32, int height = 16)
        {
            var bitmap = new System.Drawing.Bitmap(width, height);
            if (!items.Any()) return bitmap;

            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                double maxX = items.Max(i => i.X + i.Width);
                double maxY = items.Max(i => i.Y + i.Height);
                double scaleX = (width - 4) / maxX;
                double scaleY = (height - 4) / maxY;
                double scale = System.Math.Min(scaleX, scaleY);

                foreach (var item in items)
                {
                    var brush = item.IsPrimary ? new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(147, 51, 234)) : new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(100, 255, 255, 255));
                    float rectX = (float)(item.X * scale) + 2;
                    float rectY = (float)(item.Y * scale) + 2;
                    float rectW = (float)(item.Width * scale);
                    float rectH = (float)(item.Height * scale);

                    g.FillRectangle(brush, rectX, rectY, rectW, rectH);
                    g.DrawRectangle(System.Drawing.Pens.Black, rectX, rectY, rectW, rectH);
                    
                    brush.Dispose();
                }
            }
            return bitmap;
        }
    }
}
