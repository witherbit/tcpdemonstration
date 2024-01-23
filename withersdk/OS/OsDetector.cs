using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using withersdk.OS.Hardware;
using withersdk.OS.Hardware.CPU;
using withersdk.OS.Hardware.Display;
using withersdk.OS.Hardware.GPU;
using withersdk.OS.Hardware.RAM;
using withersdk.OS.OperatingSystem;

namespace withersdk.OS
{
    public static class OsDetector
    {
        public static OS Detect()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OS.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OS.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OS.OSX;
            }
            else
            {
                return OS.Other;
            }
        }
        public static OperatingSystemInfo GetInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsOperatingSystemInfo();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxOperatingSystemInfo();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MacOSXOperatingSystemInfo();
            }
            else
            {
                try
                {
                    return new BSDOperatingSystemInfo();
                }
                catch { }
                try
                {
                    return new UnixOperatingSystemInfo();
                }
                catch { }
                return null;
            }
        }
    }

    public enum OS
    {
        Windows,
        Linux,
        OSX,
        Other
    }
}
