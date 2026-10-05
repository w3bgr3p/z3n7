using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Counts traffic per labelled step of a project run and reports it as JSON. Steps are kept in
    /// <c>project.Context</c>.
    /// </summary>
    public static class TrafficCounter
    {
        private const string ContextKey = "trafficMap";

        /// <summary>Turns on traffic monitoring and reads the traffic recorded so far once.</summary>
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
        /// <summary>
        /// Adds a step: the summed request and response body sizes of the traffic returned by
        /// <c>ActiveTab.GetTraffic()</c> (blocked requests are skipped). Errors are written to the log as
        /// warnings.
        /// </summary>
        /// <param name="label">Step name.</param>
        /// <returns>Bytes counted for this step.</returns>
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
        // для HTTP-запросов вне браузера (AI clients, ZennoPoster.HTTP.Request)
        /// <summary>
        /// Adds a step for traffic outside the browser, counted as the UTF-8 size of <c>responseText</c>.
        /// </summary>
        /// <param name="label">Step name.</param>
        /// <param name="responseText">Response text.</param>
        public static void Add(IZennoPosterProjectModel project, string label, string responseText)
        {
            long bytes = Encoding.UTF8.GetByteCount(responseText ?? "");

            var map = project.Context[ContextKey] as List<TrafficStep>
                      ?? new List<TrafficStep>();

            map.Add(new TrafficStep { T = ShortEpoch(), Label = label, Bytes = bytes });

            project.Context[ContextKey] = map;
        }
        /// <summary>
        /// Merges the steps of an earlier report with the current ones, sorted by time, and builds a new
        /// report.
        /// </summary>
        /// <param name="existingJson">Report from <c>ReportJson</c> or this method; ignored when empty or unreadable.</param>
        /// <returns>
        /// JSON <c>{ total_kb, steps: [{ t, label, kb }] }</c>; <c>t</c> is seconds since 2020-01-01 UTC.
        /// </returns>
        public static string MergeAndReport(IZennoPosterProjectModel project, string existingJson)
        {
            var currentSteps = GetMap(project);

            var previousSteps = new List<TrafficStep>();
            if (!string.IsNullOrEmpty(existingJson))
            {
                try
                {
                    var prev = JsonConvert.DeserializeObject<dynamic>(existingJson);
                    foreach (var s in prev.steps)
                    {
                        previousSteps.Add(new TrafficStep
                        {
                            T     = (long)s.t,
                            Label = (string)s.label,
                            Bytes = (long)((double)s.kb * 1024)
                        });
                    }
                }
                catch { }
            }

            var merged = previousSteps.Concat(currentSteps).OrderBy(s => s.T).ToList();
            long total = merged.Sum(s => s.Bytes);

            var report = new
            {
                total_kb = System.Math.Round(total / 1024.0, 1),
                steps = merged.Select(s => new
                {
                    t     = s.T,
                    label = s.Label,
                    kb    = System.Math.Round(s.Bytes / 1024.0, 1)
                }).ToList()
            };

            return JsonConvert.SerializeObject(report, Formatting.Indented);
        }
        /// <summary>Builds a report from the current steps.</summary>
        /// <returns>
        /// JSON <c>{ total_kb, steps: [{ t, label, kb }] }</c>; <c>t</c> is seconds since 2020-01-01 UTC.
        /// </returns>
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
        

        /// <summary>One counted step.</summary>
        public class TrafficStep
        {
            /// <summary>Time, seconds since 2020-01-01 UTC.</summary>
            public long   T     { get; set; }
            /// <summary>Step name.</summary>
            public string Label { get; set; }
            /// <summary>Counted bytes.</summary>
            public long   Bytes { get; set; }
        }
        
        
    }
}
