using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    public static class HarFilterTest
    {
        public static string Run(IZennoPosterProjectModel project, Instance instance)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (instance == null) throw new ArgumentNullException("instance");
            if (instance.ActiveTab == null) throw new InvalidOperationException("ActiveTab is null");

            var domain = instance.ActiveTab.Domain ?? "";
            var report = new StringBuilder();

            Test(project, instance, report, "no parameters", tab => tab.GetTraffic());
            Test(project, instance, report, "single literal: a", tab => tab.GetTraffic(new[] { "a" }));
            Test(project, instance, report, "single literal: 0", tab => tab.GetTraffic(new[] { "0" }));
            Test(project, instance, report, "dot", tab => tab.GetTraffic(new[] { "." }));
            Test(project, instance, report, "http", tab => tab.GetTraffic(new[] { "http" }));

            if (!string.IsNullOrWhiteSpace(domain))
                Test(project, instance, report, "domain: " + domain,
                    tab => tab.GetTraffic(new[] { domain }));

            Test(project, instance, report, "settings: GatherAll only", tab =>
                tab.GetTraffic(new ZennoLab.CommandCenter.Classes.GetTrafficSettings
                {
                    GatherAllTraffic = true
                }));

            Test(project, instance, report, "settings: GatherAll + a", tab =>
                tab.GetTraffic(new ZennoLab.CommandCenter.Classes.GetTrafficSettings
                {
                    GatherAllTraffic = true,
                    UrlFilters = new[] { "a" }
                }));

            Test(project, instance, report, "settings: GatherAll + a-z0-9", tab =>
                tab.GetTraffic(new ZennoLab.CommandCenter.Classes.GetTrafficSettings
                {
                    GatherAllTraffic = true,
                    UrlFilters = "abcdefghijklmnopqrstuvwxyz0123456789"
                        .Select(character => character.ToString())
                        .ToArray()
                }));

            var result = report.ToString();
            project.SendInfoToLog(result);
            return result;
        }

        private static void Test(
            IZennoPosterProjectModel project,
            Instance instance,
            StringBuilder report,
            string name,
            Func<Tab, IEnumerable<TrafficItem>> getTraffic)
        {
            instance.F5();
            if (instance.ActiveTab.IsBusy) instance.ActiveTab.WaitDownloading();

            var traffic = getTraffic(instance.ActiveTab).ToList();
            report.AppendLine(name + ": " + traffic.Count);
            foreach (var item in traffic.Take(3))
                report.AppendLine("  " + (item.Url ?? "<null>"));
        }
    }
}
