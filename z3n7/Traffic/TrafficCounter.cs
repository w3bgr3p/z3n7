using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public static class TrafficCounter
    {
        private const string ContextKey = "trafficMap";

        public static void Init(Instance instance)
        {
            instance.UseTrafficMonitoring = true;
            instance.ActiveTab.GetTraffic();
        }

        // short epoch = секунды с 2020-01-01 (умещается в 9 цифр надолго)
        private static long ShortEpoch()
        {
            var origin = new System.DateTime(2020, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            return (long)(System.DateTime.UtcNow - origin).TotalSeconds;
        }

        private static List<TrafficStep> GetMap(IZennoPosterProjectModel project)
        {
            try
            {
                return project.Context[ContextKey] as List<TrafficStep> ?? new List<TrafficStep>();
            }
            catch
            {
                return new List<TrafficStep>();
            }
        }
        public static long Checkpoint(Instance instance, IZennoPosterProjectModel project, string label)
        {
            long bytes = 0;
            try
            {
                var rawTraffic = instance.ActiveTab.GetTraffic();
                foreach (var item in rawTraffic)
                {
                    try
                    {
                        if (item.IsBlocked) continue;
                        bytes += (item.RequestBody?.Length ?? 0) + (item.ResponseBody?.Length ?? 0);

                    }
                    catch (Exception ex)
                    {
                        project.SendWarningToLog($"[TrafficCounter] item err={ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                project.SendWarningToLog($"[TrafficCounter] label={label} err={ex.Message}");
            }

            var map = project.Context[ContextKey] as List<TrafficStep> ?? new List<TrafficStep>();
            map.Add(new TrafficStep { T = ShortEpoch(), Label = label, Bytes = bytes });
            project.Context[ContextKey] = map;

            return bytes;
        }
        // для HTTP-запросов вне браузера (AiClient, ZennoPoster.HTTP.Request)
        public static void Add(IZennoPosterProjectModel project, string label, string responseText)
        {
            long bytes = Encoding.UTF8.GetByteCount(responseText ?? "");

            var map = project.Context[ContextKey] as List<TrafficStep>
                      ?? new List<TrafficStep>();

            map.Add(new TrafficStep { T = ShortEpoch(), Label = label, Bytes = bytes });

            project.Context[ContextKey] = map;
        }

        public static string ReportJson(IZennoPosterProjectModel project)
        {
            var steps = project.Context[ContextKey] as List<TrafficStep>
                        ?? new List<TrafficStep>();

            long total = steps.Sum(s => s.Bytes);

            var report = new
            {
                total_kb = System.Math.Round(total / 1024.0, 1),
                steps = steps.Select(s => new
                {
                    t     = s.T,
                    label = s.Label,
                    kb    = System.Math.Round(s.Bytes / 1024.0, 1)
                }).ToList()
            };

            return JsonConvert.SerializeObject(report, Formatting.Indented);
        }

        public static void LogReport(IZennoPosterProjectModel project)
        {
            project.SendInfoToLog("[Traffic]\n" + ReportJson(project));
        }

        public class TrafficStep
        {
            public long   T     { get; set; }
            public string Label { get; set; }
            public long   Bytes { get; set; }
        }
    }
}