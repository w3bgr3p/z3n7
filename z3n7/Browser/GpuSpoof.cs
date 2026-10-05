using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    // Структура одной модели GPU
    /// <summary>One GPU model from the PCI ID list.</summary>
    public class GpuModel
    {
        /// <summary>PCI device id, 4 hex digits, e.g. <c>2486</c>.</summary>
        public string DeviceId { get; set; }   // "2486"
        /// <summary>Model name, e.g. <c>GeForce RTX 3060 Ti</c>.</summary>
        public string Name     { get; set; }   // "GeForce RTX 3060 Ti"
        /// <summary>Chip code, e.g. <c>GA104</c>; empty when the list does not give one.</summary>
        public string Chip     { get; set; }   // "GA104"
    }

    // Структура одной архитектуры
    /// <summary>GPU models of one architecture.</summary>
    public class GpuArch
    {
        /// <summary>Architecture name, e.g. <c>Ampere</c>.</summary>
        public string Arch     { get; set; }   // "Ampere"
        /// <summary>Models of this architecture.</summary>
        public List<GpuModel> Models { get; set; } = new List<GpuModel>();
    }

    // Структура одного вендора
    /// <summary>GPU architectures of one vendor.</summary>
    public class GpuVendor
    {
        /// <summary><c>NVIDIA</c>, <c>AMD</c> or <c>Intel</c>.</summary>
        public string Vendor   { get; set; }   // "NVIDIA"
        /// <summary>Architectures of this vendor.</summary>
        public List<GpuArch> Archs { get; set; } = new List<GpuArch>();
    }

    /// <summary>
    /// Picks a plausible WebGL vendor/renderer pair of the same GPU architecture as the machine's card,
    /// from the public PCI ID list.
    /// </summary>
    public static class GpuSpoof
    {
        private static readonly Random _rng = new Random();
        private static readonly object _buildLock = new object();

        // ── Chip-code → Architecture name ──────────────────────────────────────

        private static readonly Dictionary<string, string> _nvidiaArchMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AD1",  "Ada Lovelace" },
            { "GA1",  "Ampere"       },
            { "TU1",  "Turing"       },
            { "GP1",  "Pascal"       },
            { "GM2",  "Maxwell"      },
            { "GM1",  "Maxwell"      },
            { "GK1",  "Kepler"       },
            { "GK2",  "Kepler"       },
            { "GF1",  "Fermi"        },
            { "GT2",  "Tesla"        },
        };

        private static readonly Dictionary<string, string> _amdArchMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Navi 4",   "RDNA 4"  },
            { "Navi 3",   "RDNA 3"  },
            { "Navi 2",   "RDNA 2"  },
            { "Navi 1",   "RDNA 1"  },
            { "Vega",     "Vega"    },
            { "Polaris",  "Polaris" },
            { "Fiji",     "Fiji"    },
            { "Tonga",    "Tonga"   },
            { "Kaveri",   "GCN 2"   },
            { "Bonaire",  "GCN 2"   },
            { "Hawaii",   "GCN 2"   },
            { "Navi",     "RDNA 1"  }, // fallback Navi без номера
        };

        private static string DetectNvidiaArch(string chip)
        {
            // chip = "GA104", "AD102", "TU116" ...
            if (string.IsNullOrEmpty(chip)) return "Unknown";
            string prefix = chip.Length >= 3 ? chip.Substring(0, 3).ToUpper() : chip.ToUpper();
            string val;
            return _nvidiaArchMap.TryGetValue(prefix, out val) ? val : "Unknown";
        }

        private static string DetectAmdArch(string chip)
        {
            if (string.IsNullOrEmpty(chip)) return "Unknown";
            foreach (var kv in _amdArchMap)
                if (chip.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kv.Value;
            return "Unknown";
        }

        private static string DetectIntelArch(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Unknown";
            if (name.IndexOf("Arc",    StringComparison.OrdinalIgnoreCase) >= 0) return "Xe HPG";
            if (name.IndexOf("Xe",     StringComparison.OrdinalIgnoreCase) >= 0) return "Xe";
            if (name.IndexOf("Iris",   StringComparison.OrdinalIgnoreCase) >= 0) return "Iris";
            if (name.IndexOf("UHD",    StringComparison.OrdinalIgnoreCase) >= 0) return "Gen12";
            if (name.IndexOf("HD Gra", StringComparison.OrdinalIgnoreCase) >= 0) return "HD";
            return "Unknown";
        }

        // ── Regex для парсинга строк pci.ids ───────────────────────────────────
        // Формат:  \t{deviceId}  {chip} [{model name}]
        //      или \t{deviceId}  {name} (без скобок — берём всё как имя)

        private static readonly Regex _reDevice = new Regex(
            @"^\t([0-9a-fA-F]{4})\s+(.+)$",
            RegexOptions.Compiled);

        // Вытащить chip из строки вида "GA104 [GeForce RTX 3060 Ti]"
        private static readonly Regex _reChip = new Regex(
            @"^([A-Z]{2,4}\d{1,4}[A-Z]?[A-Z]?[A-Z]?)\s*\[(.+)\]$",
            RegexOptions.Compiled);

        // ── GPU-вендоры которые нас интересуют ────────────────────────────────

        private static readonly Dictionary<string, string> _gpuVendors = new Dictionary<string, string>
        {
            { "10de", "NVIDIA" },
            { "1002", "AMD"    },
            { "8086", "Intel"  },
        };

        // ── Главный парсер ──────────────────────────────────────────────────────

        /// <summary>
        /// Downloads the PCI ID list and groups the GPU models of NVIDIA, AMD and Intel by vendor and
        /// architecture (only GeForce/Quadro/Tesla/RTX, Radeon/Navi/Vega/Polaris and HD
        /// Graphics/UHD/Iris/Xe/Arc entries).
        /// </summary>
        /// <param name="pciIdsUrl">URL of <c>pci.ids</c>.</param>
        /// <returns>JSON <c>[{ Vendor, Archs: [{ Arch, Models: [{ DeviceId, Name, Chip }] }] }]</c>.</returns>
        public static string BuildGpuJson(string pciIdsUrl = "https://pci-ids.ucw.cz/v2.2/pci.ids")
        {
            string raw;
            using (var wc = new WebClient())
                raw = wc.DownloadString(pciIdsUrl);

            var vendors = new List<GpuVendor>();

            string currentVendorId = null;
            string currentVendorName = null;
            // arch → models
            var archBuckets = new Dictionary<string, List<GpuModel>>(StringComparer.OrdinalIgnoreCase);

            Action flushVendor = () =>
            {
                if (currentVendorId == null || archBuckets.Count == 0) return;
                var vendor = new GpuVendor { Vendor = currentVendorName };
                foreach (var kv in archBuckets)
                    vendor.Archs.Add(new GpuArch { Arch = kv.Key, Models = kv.Value });
                vendors.Add(vendor);
                archBuckets.Clear();
            };

            foreach (var line in raw.Split('\n'))
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

                // Строка подустройства (два таба) — пропускаем
                if (line.StartsWith("\t\t")) continue;

                // Строка вендора: начинается с hex без таба
                if (!line.StartsWith("\t"))
                {
                    var parts = line.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2) continue;

                    string vid = parts[0].ToLower();
                    if (_gpuVendors.ContainsKey(vid))
                    {
                        flushVendor();
                        currentVendorId   = vid;
                        currentVendorName = _gpuVendors[vid];
                    }
                    else
                    {
                        flushVendor();
                        currentVendorId   = null;
                        currentVendorName = null;
                    }
                    continue;
                }

                // Строка устройства: один таб
                if (currentVendorId == null) continue;

                var dm = _reDevice.Match(line);
                if (!dm.Success) continue;

                string deviceId  = dm.Groups[1].Value.ToUpper();
                string rawName   = dm.Groups[2].Value.Trim();

                // Фильтр — только GPU строки
                if (!IsGpuEntry(rawName, currentVendorName)) continue;

                string chip      = "";
                string modelName = rawName;

                var cm = _reChip.Match(rawName);
                if (cm.Success)
                {
                    chip      = cm.Groups[1].Value;
                    modelName = cm.Groups[2].Value.Trim();
                }

                string arch = ResolveArch(currentVendorName, chip, modelName);

                if (!archBuckets.ContainsKey(arch))
                    archBuckets[arch] = new List<GpuModel>();

                archBuckets[arch].Add(new GpuModel
                {
                    DeviceId = deviceId,
                    Name     = modelName,
                    Chip     = chip,
                });
            }

            flushVendor();

            return JsonConvert.SerializeObject(vendors, Formatting.Indented);
        }

        private static bool IsGpuEntry(string rawName, string vendor)
        {
            if (vendor == "NVIDIA")
                return rawName.IndexOf("GeForce",  StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Quadro",   StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Tesla",    StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("RTX",      StringComparison.OrdinalIgnoreCase) >= 0;

            if (vendor == "AMD")
                return rawName.IndexOf("Radeon",   StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Navi",     StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Vega",     StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Polaris",  StringComparison.OrdinalIgnoreCase) >= 0;

            if (vendor == "Intel")
                return rawName.IndexOf("HD Graphics", StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("UHD",         StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Iris",        StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Xe",          StringComparison.OrdinalIgnoreCase) >= 0
                    || rawName.IndexOf("Arc",         StringComparison.OrdinalIgnoreCase) >= 0;

            return false;
        }

        private static string ResolveArch(string vendor, string chip, string modelName)
        {
            if (vendor == "NVIDIA") return DetectNvidiaArch(chip);
            if (vendor == "AMD")    return DetectAmdArch(chip.Length > 0 ? chip : modelName);
            if (vendor == "Intel")  return DetectIntelArch(modelName);
            return "Unknown";
        }

        // ── Определение вендора текущей карты ──────────────────────────────────

        /// <summary>
        /// Vendor of the machine's video card from WMI <c>Win32_VideoController</c>; with several cards the
        /// second one is used.
        /// </summary>
        /// <returns>
        /// <c>NVIDIA</c>, <c>AMD</c>, <c>Intel</c>, the first word of another name, or an empty string.
        /// </returns>
        public static string GetCurrentVendor()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                var cards = searcher.Get().Cast<ManagementObject>().ToList();

                int idx = cards.Count > 1 ? 1 : 0;
                if (cards.Count == 0) return "";

                string name = (cards[idx]["Name"]?.ToString() ?? "").ToLower();

                if (name.Contains("nvidia")) return "NVIDIA";
                if (name.Contains("amd"))    return "AMD";
                if (name.Contains("ati"))    return "AMD";
                if (name.Contains("intel"))  return "Intel";

                return cards[idx]["Name"]?.ToString().Split(' ').FirstOrDefault() ?? "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// Full name of the machine's video card from WMI (the second card when there are several); empty on
        /// error.
        /// </summary>
        public static string GetCurrentCardName()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                var cards = searcher.Get().Cast<ManagementObject>().ToList();
                int idx = cards.Count > 1 ? 1 : 0;
                return cards.Count == 0 ? "" : cards[idx]["Name"]?.ToString() ?? "";
            }
            catch { return ""; }
        }

        // ── Определение архитектуры текущей карты из JSON ─────────────────────

        /// <summary>
        /// Architecture of the card in <c>gpuJson</c>: the first model of the card's vendor whose name contains
        /// the card name or is contained in it.
        /// </summary>
        /// <param name="gpuJson">JSON from <c>BuildGpuJson</c>.</param>
        /// <param name="cardName">Card name; default is the current card (<c>GetCurrentCardName</c>).</param>
        /// <returns>Architecture name, or an empty string.</returns>
        public static string DetectCurrentArch(string gpuJson, string cardName = null)
        {
            if (string.IsNullOrEmpty(cardName))
                cardName = GetCurrentCardName();

            if (string.IsNullOrEmpty(cardName)) return "";

            var vendors = JsonConvert.DeserializeObject<List<GpuVendor>>(gpuJson);
            string vendor = GetCurrentVendor();

            var vendorData = vendors?.FirstOrDefault(v =>
                v.Vendor.Equals(vendor, StringComparison.OrdinalIgnoreCase));
            if (vendorData == null) return "";

            foreach (var arch in vendorData.Archs)
                foreach (var model in arch.Models)
                    if (cardName.IndexOf(model.Name, StringComparison.OrdinalIgnoreCase) >= 0
                     || model.Name.IndexOf(cardName, StringComparison.OrdinalIgnoreCase) >= 0)
                        return arch.Arch;

            return "";
        }

        // ── Финальный метод: случайная строка той же архитектуры ───────────────

        /// <summary>
        /// Random model of the same vendor and architecture as the card, formatted as Chrome's ANGLE strings.
        /// </summary>
        /// <param name="gpuJson">JSON from <c>BuildGpuJson</c>.</param>
        /// <param name="cardName">Card name; default is the current card (<c>GetCurrentCardName</c>).</param>
        /// <returns>
        /// <c>["Google Inc. (NVIDIA)", "ANGLE (NVIDIA, NVIDIA {model} (0x0000XXXX) Direct3D11 vs_5_0 ps_5_0,
        /// D3D11)"]</c>; two empty strings when the architecture is unknown.
        /// </returns>
        public static string[] RandomAngleString(string gpuJson, string cardName = null)
        {
            if (string.IsNullOrEmpty(cardName))
                cardName = GetCurrentCardName();

            string vendor  = GetCurrentVendor();
            string arch    = DetectCurrentArch(gpuJson, cardName);
            if (string.IsNullOrEmpty(arch)) return new[] { "", "" };

            var vendors    = JsonConvert.DeserializeObject<List<GpuVendor>>(gpuJson);
            var vendorData = vendors?.FirstOrDefault(v =>
                v.Vendor.Equals(vendor, StringComparison.OrdinalIgnoreCase));
            if (vendorData == null) return new[] { "", "" };

            var archData = vendorData.Archs.FirstOrDefault(a =>
                a.Arch.Equals(arch, StringComparison.OrdinalIgnoreCase));
            if (archData == null || archData.Models.Count == 0) return new[] { "", "" };

            var pick = archData.Models[_rng.Next(archData.Models.Count)];

            string hexId = "0x" + pick.DeviceId.PadLeft(8, '0').ToUpper();

            return new[]
            {
                $"Google Inc. ({vendor})",
                $"ANGLE ({vendor}, {vendor} {pick.Name} ({hexId}) Direct3D11 vs_5_0 ps_5_0, D3D11)",
            };
        }

        // ── Утилита: сохранить/загрузить/обеспечить JSON ──────────────────────

        /// <summary>Writes the GPU JSON to a file (UTF-8).</summary>
        public static void SaveGpuJson(string json, string path)
            => File.WriteAllText(path, json, System.Text.Encoding.UTF8);

        /// <summary>Reads the GPU JSON from a file.</summary>
        public static string LoadGpuJson(string path)
            => File.ReadAllText(path, System.Text.Encoding.UTF8);

        /// <summary>
        /// Reads the GPU JSON from <c>path</c>; when the file does not exist, builds it with
        /// <c>BuildGpuJson</c> and saves it (one builder at a time).
        /// </summary>
        /// <param name="path">Cache file.</param>
        public static string EnsureGpuJson(string path)
        {
            if (!File.Exists(path))
            {
                lock (_buildLock)
                {
                    if (!File.Exists(path))
                    {
                        string json = BuildGpuJson();
                        SaveGpuJson(json, path);
                        return json;
                    }
                }
            }
            return LoadGpuJson(path);
        }
    }

    /// <summary>Extension methods on <c>IZennoPosterProjectModel</c>: WebGL spoofing.</summary>
    public static partial class ProjectExtensions
    {
        /// <summary>
        /// Takes a random WebGL profile (Base64 JSON per line) from <c>{project.Path}/resourses/webgl.txt</c>,
        /// replaces its unmasked vendor and renderer with <c>RandomAngleString</c> (GPU list cached in
        /// <c>resourses/gpu.json</c>), stores it in <c>webgl</c> and loads it into the instance. Does nothing
        /// when <c>webgl.txt</c> is missing.
        /// </summary>
        /// <param name="instance">Browser instance.</param>
        public static void SpoofGpu(this IZennoPosterProjectModel project, Instance instance)
        {
            var jGpuPath  = Path.Combine(project.Path, "resourses", "gpu.json");
            var webglPath = Path.Combine(project.Path, "resourses", "webgl.txt");

            if (!File.Exists(webglPath)) return;

            var gpuJson = GpuSpoof.EnsureGpuJson(jGpuPath);

            var webgls = File.ReadAllLines(webglPath);
            var webgl  = webgls[new Random().Next(webgls.Length)].FromBase64();

            var angle = GpuSpoof.RandomAngleString(gpuJson);

            var jo = JObject.Parse(webgl);
            jo["parameters"]["default"]["UNMASKED_VENDOR_WEBGL"]   = angle[0];
            jo["parameters"]["default"]["UNMASKED_RENDERER_WEBGL"] = angle[1];

            webgl = JsonConvert.SerializeObject(jo);

            project.Var("webgl", webgl);
            instance.WebGLPreferences.Load(webgl);
        }
    }
}