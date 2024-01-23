using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using withersdk.OS;
using withersdk.OS.OperatingSystem;
using withersdk.Utils;

namespace withersdk.Net.General
{
    public sealed class ClientDeviceInfo
    {
        public string OS {  get; set; }
        public string OSVersion { get; set; }
        public string MachineName { get; set; }
        public bool IsMono { get; set; }

        public string Architecture { get; set; }
        public string Runtime {  get; set; }

        public string CPU { get; set; }
        public string GPU { get; set; }

        public string RAM { get; set; }

        public string UserName {  get; set; }

        public string Type { get; set; }

        public string AppName { get; set; }

        public static ClientDeviceInfo Build(string appName)
        {
            var secondary = Environment.OSVersion;
            var info = OsDetector.GetInfo();
            if (info != null)
            {
                var cpus = "";
                var gpus = "";
                foreach (var cpu in info.Hardware.CPUs)
                {
                    cpus += $"{cpu.Name}|{cpu.Brand}|{cpu.PhysicalCores}x{cpu.LogicalCores}|{cpu.Frequency}MHz|{cpu.Architecture}\r\n";
                }
                foreach (var gpu in info.Hardware.GPUs)
                {
                    gpus += $"{gpu.Name}|{gpu.Brand}|{gpu.MemoryTotal / 1024 / 1024}mb\r\n";
                }
                return new ClientDeviceInfo
                {
                    OS = info.Name,
                    OSVersion = secondary.VersionString,
                    MachineName = Environment.MachineName.ToString(),
                    IsMono = info.IsMono,
                    Architecture = info.Architecture,
                    Runtime = info.Runtime,
                    Type = info.OperatingSystemType.ToString(),
                    CPU = cpus,
                    GPU = gpus,
                    RAM = $"{(info.Hardware.RAM.Total - info.Hardware.RAM.Free) / 1024d}mb/{info.Hardware.RAM.Total / 1024d}mb [{info.Hardware.RAM.Free / 1024d}mb]",
                    UserName = Environment.UserName,
                    AppName = appName
                };
            }
            
            return new ClientDeviceInfo
            {
                OS = secondary.Platform.ToString(),
                OSVersion = secondary.VersionString,
                MachineName = Environment.MachineName.ToString(),
                IsMono = false,
                Architecture = Environment.Is64BitOperatingSystem ? "x62" : "x86",
                Runtime = Environment.Version.ToString(),
                Type = OperatingSystemType.Other.ToString(),
                CPU = $"Unknown",
                GPU = $"Unknown",
                RAM = $"Unknown",
                UserName = Environment.UserName,
                AppName = appName
            };
        }

        public static ClientDeviceInfo Deserialize(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<ClientDeviceInfo>(bytes.ToUTF8());
        }
        public static ClientDeviceInfo Deserialize(string str)
        {
            return JsonConvert.DeserializeObject<ClientDeviceInfo>(str);
        }

        public static byte[] BuildSerialized(string appName)
        {
            return Build(appName).Serialize();
        }

        public byte[] Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented).FromUTF8();
        }
    }
}
