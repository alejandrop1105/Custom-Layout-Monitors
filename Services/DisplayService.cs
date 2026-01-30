using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CustomLayoutMonitors.Models;
using CustomLayoutMonitors.Services.Native;

namespace CustomLayoutMonitors.Services
{
    public interface IDisplayService
    {
        DisplayProfile GetCurrentProfile(string name);
        void ApplyProfile(DisplayProfile profile);
    }

    public class DisplayService : IDisplayService
    {
        public DisplayProfile GetCurrentProfile(string name)
        {
            uint numPathArrayElements = 0;
            uint numModeInfoArrayElements = 0;

            // First call to get buffer sizes
            int result = NativeMethods.GetDisplayConfigBufferSizes(
                NativeMethods.QDC_ONLY_ACTIVE_PATHS,
                out numPathArrayElements,
                out numModeInfoArrayElements);

            if (result != NativeMethods.ERROR_SUCCESS)
                throw new Exception($"GetDisplayConfigBufferSizes failed with error {result}");

            var pathArray = new NativeMethods.DISPLAYCONFIG_PATH_INFO[numPathArrayElements];
            var modeInfoArray = new NativeMethods.DISPLAYCONFIG_MODE_INFO[numModeInfoArrayElements];

            // Query the current configuration - pass IntPtr.Zero for topology (not needed)
            result = NativeMethods.QueryDisplayConfig(
                NativeMethods.QDC_ONLY_ACTIVE_PATHS,
                ref numPathArrayElements,
                pathArray,
                ref numModeInfoArrayElements,
                modeInfoArray,
                IntPtr.Zero);

            if (result != NativeMethods.ERROR_SUCCESS)
                throw new Exception($"QueryDisplayConfig failed with error {result}");

            // Convert to DTOs for serialization
            var profile = new DisplayProfile
            {
                Name = name,
                Paths = pathArray.Take((int)numPathArrayElements).Select(p => new PathInfoDTO
                {
                    SourceInfo = p.sourceInfo,
                    TargetInfo = p.targetInfo,
                    Flags = p.flags
                }).ToList(),
                Modes = modeInfoArray.Take((int)numModeInfoArrayElements).Select(m => new ModeInfoDTO
                {
                    InfoType = m.infoType,
                    Id = m.id,
                    AdapterId = m.adapterId,
                    ModeInfo = m.modeInfo
                }).ToList()
            };

            return profile;
        }

        public void ApplyProfile(DisplayProfile profile)
        {
            if (profile.Paths == null || profile.Modes == null) return;

            var pathArray = profile.Paths.Select(p => new NativeMethods.DISPLAYCONFIG_PATH_INFO
            {
                sourceInfo = p.SourceInfo,
                targetInfo = p.TargetInfo,
                flags = p.Flags
            }).ToArray();

            var modeInfoArray = profile.Modes.Select(m => new NativeMethods.DISPLAYCONFIG_MODE_INFO
            {
                infoType = m.InfoType,
                id = m.Id,
                adapterId = m.AdapterId,
                modeInfo = m.ModeInfo
            }).ToArray();

            uint flags = NativeMethods.SDC_APPLY | NativeMethods.SDC_USE_SUPPLIED_DISPLAY_CONFIG | NativeMethods.SDC_SAVE_TO_DATABASE | NativeMethods.SDC_ALLOW_CHANGES;

            int result = NativeMethods.SetDisplayConfig(
                (uint)pathArray.Length,
                pathArray,
                (uint)modeInfoArray.Length,
                modeInfoArray,
                flags);

            if (result != NativeMethods.ERROR_SUCCESS)
                throw new Exception($"SetDisplayConfig failed with error {result}");
        }
    }
}
