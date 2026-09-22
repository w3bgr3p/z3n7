using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.CommandCenter;
using System.Net.Http;
using ZennoLab.InterfacesLibrary.Enums.Browser;
using ZennoLab.BrowserProfiles;
namespace z3n7
{
    public static class BetterBrowser
    {
         public static void PrepareSession(this IZennoPosterProjectModel project, Instance instance)
        {
            try{
                
                instance.GetCookies(project);
                project.Profile.Email = project.Profile.NickName + "@mailflashx.online";
                project.Profile.Password = Rnd.RndPass(12);
                instance.UseTrafficMonitoring = true;
                instance.SetWindowSize(1280, 720);
                project.Var("ts0", Time.Now());
                project.ImproveBrowser(instance);
            }
            catch (Exception ex)
            {
                project.SendErrorToLog(ex.Message, true);
                throw;
            }
        }
        
        public static void ImproveBrowser(this IZennoPosterProjectModel project, Instance instance)
        {
            // 1. Точка выхода прокси глазами самого браузера: из C# запрос
            //    ушёл бы напрямую и вернул не тот адрес.
            string exitIp = "", chromeMajor = "";
            try
            {
                instance.Go("https://check.z3n.pro/api/ip");
                var body = instance.ActiveTab.FindElementByTag("body", 0).GetAttribute("innertext");
                var echo = JObject.Parse(body);
                exitIp = (string)echo["ip"] ?? "";
                // Версию движка сервер вычитывает из присланного User-Agent:
                // у Tab такого свойства нет, а подставлять в UA чужую версию
                // нельзя — Sec-CH-UA разойдётся с navigator.userAgentData.
                chromeMajor = (string)echo["chromeMajor"] ?? "";
            }
            catch (Exception e)
            {
                project.SendWarningToLog("точка выхода не определена: " + e.Message, false);
            }

            // 2. Готовый документ профиля под этот выход.
            // HttpGet помечен obsolete. Современный вызов — ZennoPoster.HTTP.Request.
            // Запрос идёт НАПРЯМУЮ, без прокси: это наш сервис, а точку выхода
            // мы уже узнали глазами браузера и передаём параметром.
            var specUrl = $"https://check.z3n.pro/api/profile?country={project.Var("proxy_iso")}&ip={exitIp}&chrome={chromeMajor}";
            var spec = JObject.Parse(ZennoPoster.HTTP.Request(
                ZennoLab.InterfacesLibrary.Enums.Http.HttpMethod.GET,
                specUrl,
                "",
                "application/json",
                "",
                "UTF-8",
                ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly,
                20000));

            // 3. Профиль применяется ЦЕЛИКОМ и ПЕРВЫМ. Частичное заполнение не
            //    работает: незаполненный блок не пропускается, а раскатывается
            //    как есть и откатывает таймзону с экраном на хостовые. По той
            //    же причине всё, что ставится на инстансе, идёт ПОСЛЕ.
            var bp = (BrowserProfile)project.Profile.BrowserProfile;
            string applied;
            try
            {
                // Через рефлексию намеренно. У FromJson два перегруза, и второй
                // принимает JObject из сборки Global.dll (ZennoPoster держит
                // Newtonsoft внутри себя под этим именем). Чтобы выбрать
                // перегруз, компилятору нужно видеть оба, а Global в ссылках
                // проекта нет — отсюда CS0012. Рефлексия выбирает строковый
                // перегруз и снимает требование ссылки: общий код остаётся
                // подключаемым в любой проект без правки его настроек.
                var fromJson = bp.GetType().GetMethod("FromJson", new[] { typeof(string) });
                if (fromJson == null) throw new Exception("BrowserProfile.FromJson(string) не найден");
                // Без ToString(Formatting): этого перегруза нет в той Newtonsoft,
                // что подгружает ZennoPoster в рантайме — код компилируется против
                // одной версии, исполняется с другой, и получается
                // MissingMethodException. Отступы в JSON никому не мешают.
                fromJson.Invoke(bp, new object[] { spec["zennoProfile"].ToString() });
                applied = bp.TryApplyTo(instance) ? "ok" : "false";
            }
            catch (Exception e)
            {
                applied = e.GetType().Name + ": " + e.Message;
            }

            // 4. Canvas — единственное, чего в профиле нет вообще: среди 16
            //    блоков эмуляции его нет. SuperEmulation эта сборка молча
            //    отвергает и оставляет Allow, работает только Emulate.
            // Таймзона: блок профиля сам по себе её не включает — при переносе
            // на профиль зона откатилась на хостовую. Режим ставится на инстансе.
            instance.TimezoneWorkMode = TimezoneMode.Emulate;

            instance.CanvasRenderMode = CanvasMode.Emulate;
            instance.CanvasRenderSeed = (string)spec["canvasSeed"];
            instance.CanvasMissMode   = CanvasMissModeEnum.Emulate;

            instance.SetWindowSize((int)spec["window"]["width"], (int)spec["window"]["height"]);

            // ШРИФТЫ НЕ ПОДДАЮТСЯ, и это не наша ошибка. Измерено:
            //   instance.GetFonts() == 0 всегда, до и после HideFont;
            //   Fonts.CheckDefaults(instance, true) список не наполняет;
            //   Fonts.ApplyTo(ShowFont, HideFont, GetFonts, CloseAllTabs) без эффекта;
            //   installedfonts в профиле применяется (TryApplyTo -> ok), но
            //   страница отдаёт прежние 55 шрифтов и тот же хэш во всех прогонах.
            // В поставке ZennoPoster каталоги шрифтов есть только у Firefox-
            // инстансов (FFInstance, ServiceXulRunner); у CDP-Chromium их нет.
            // Похоже, подсистема шрифтов реализована не для этого движка.

            var diag = new JObject();
            diag["applied"]    = applied;
            diag["exitIp"]     = exitIp;
            diag["chrome"]     = chromeMajor;
            diag["fonts"]      = bp.Fonts.VisibleFonts.Count;
            diag["tz"]         = bp.Timezone.IanaZone;
            diag["screen"]     = bp.Screen.Width + "x" + bp.Screen.Height + " avail " + bp.Screen.AvailWidth + "x" + bp.Screen.AvailHeight;
            diag["canvasMode"] = instance.CanvasRenderMode.ToString();
            diag["webrtc"]     = bp.WebRTC.WorkMode + " " + bp.WebRTC.Ipv4Address;
            diag["ts"]         = DateTime.UtcNow.ToString("HH:mm:ss");
            try
            {
                System.IO.File.AppendAllText(
                    @"W:\work_hard\zenoposter\CURRENT_JOBS\simroute\z3n-diag.jsonl",
                    System.Text.RegularExpressions.Regex.Replace(diag.ToString(), @"\s+", " ") + System.Environment.NewLine);
            }
            catch { }

            instance.Go("https://check.z3n.pro/r/zennoposter");
            var deadline = DateTime.Now.AddSeconds(60);
            string status;
            while ((status = instance.HeGet(("z3n-status", "id"))) != "ready" && status != "error" && DateTime.Now < deadline)
                Thread.Sleep(500);

            if (status != "ready") { project.SendWarningToLog("z3nCheck: " + status); return; }

            var sum = JObject.Parse(instance.HeGet(("z3n-summary", "id")));
            foreach (var f in (JArray)sum["findings"])
                project.SendWarningToLog($"z3nCheck {f["severity"]}: {f["id"]} — {f["message"]}");
        }
    }
}

namespace z3n7
{
    public static class SimrouteProxy
    {
        private static readonly object RandomLock = new object();
        private static readonly Random Random = new Random();

        private static string IsAvaliable(string iso, string pathToJson)
        {
            iso = iso.Trim();

            var routes = Newtonsoft.Json.Linq.JArray.Parse(
                System.IO.File.ReadAllText(pathToJson));

            var node = routes.FirstOrDefault(x =>
                string.Equals(((string)x["iso2"])?.Trim(), iso,
                    StringComparison.OrdinalIgnoreCase));

            if (node == null)
                throw new Exception("ISO не найден: [" + iso + "]");

            string proxy = ((string)node["proxy"])?.Trim();

            return string.Equals(proxy, "true", StringComparison.OrdinalIgnoreCase)
                ? iso
                : proxy;
        }

        public static string Get(IZennoPosterProjectModel project,  string iso )
        {
            var host = project.ReadEnv("SIMROUTE_PROXY_HOST");
            var port = project.ReadEnv("SIMROUTE_PROXY_PORT");
            
            iso = IsAvaliable(iso,  project.ReadEnv("SIMROUTE_ROUTES_PATH"));
            
            var login = $"{project.ReadEnv("SIMROUTE_PROXY_LOGIN")}-country-{iso}-session-1{Random.Next(10000, 100000)}";
            var password = project.ReadEnv("SIMROUTE_PROXY_PASS");
            var proxyString = $"socks5://{login}:{password}@{host}:{port}";
            project.Var("proxy", proxyString);
            return proxyString;
        }
        public static string Get(IZennoPosterProjectModel project)
        {
            var host = project.ReadEnv("SIMROUTE_PROXY_HOST");
            var port = project.ReadEnv("SIMROUTE_PROXY_PORT");
            var login = $"{project.ReadEnv("SIMROUTE_PROXY_LOGIN")}-session-1{Random.Next(10000, 100000)}";
            var password = project.ReadEnv("SIMROUTE_PROXY_PASS");
            var proxyString = $"socks5://{login}:{password}@{host}:{port}";
            project.Var("proxy", proxyString);
            return proxyString;
        }
    }
}