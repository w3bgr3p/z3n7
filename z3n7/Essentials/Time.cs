using System;
using System.CodeDom;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using z3nIO;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO
{
    public class Time
    {
        public class Deadline
        {
            private long Init { get; set; }
            public Deadline()
            {
                Reset();
            }
            public double Check(double limitSec)
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                double differenceMs = (double)(currentTime - Init);
                double differenceSec = differenceMs / 1000.0;

                if (differenceSec > limitSec)
                    throw new TimeoutException($"Deadline Exception: {limitSec}s");

                return differenceSec;
            }
            public void Reset()
            {
                Init = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

        }

        public class Sleeper
        {
            private readonly int _min;
            private readonly int _max;
            private readonly Random _random;


            /// <param name="min">Min ms</param>
            /// <param name="max">Max ms</param>
            public Sleeper(int min, int max)
            {
                if (min < 0)
                    throw new ArgumentException("Min не может быть отрицательным", nameof(min));

                if (max < min)
                    throw new ArgumentException("Max не может быть меньше Min", nameof(max));

                _min = min;
                _max = max;


                _random = new Random(Guid.NewGuid().GetHashCode());
            }
            
            /// <param name="multiplier">Множитель для задержки (например, 2.0 = в 2 раза дольше)</param>
            public void Sleep(double multiplier = 1.0)
            {
                int delay = _random.Next(_min, _max + 1);
                Thread.Sleep((int)(delay * multiplier));
            }

        }
        
        public static string Now(string format = "unix") // unix|iso
        {
            if (format == "unix")
                return ((long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds))
                    .ToString(); //Unix Epoch
            else if (format == "iso") return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); // ISO 8601 
            else if (format == "short") return DateTime.UtcNow.ToString("MM-ddTHH:mm");
            else if (format == "utcToId") return (DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString();
            throw new ArgumentException("Invalid format. Use: 'unix|iso|short|UtcNow'");
        }

        public static string Cd(object input = null, string o = "iso")
        {
            DateTime t = DateTime.UtcNow;
            if (input == null)
            {
                t = t.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else if (input is string s && s == "nextH")
            {
                t  = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0).AddHours(1).AddMinutes(1);
            }
            else if (input is decimal || input is int)
            {
                decimal minutes = Convert.ToDecimal(input);
                if (minutes == 0m) minutes = 999999999m;
                long secondsToAdd = (long)Math.Round(minutes * 60);
                t = t.AddSeconds(secondsToAdd);
            }
            else if (input is string timeString)
            {
                TimeSpan parsedTime = TimeSpan.Parse(timeString);
                t = t.Add(parsedTime);
            }

            if (o == "unix")
                return ((long)(t - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            else if (o == "iso")
                return t.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); // ISO 8601
            else
                throw new ArgumentException($"unexpected format {o}");
        }

        public static long Elapsed(long startTime = 0, bool useMs = false)
        {
            if (startTime != 0)
            {
                long currentTime = useMs
                    ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return (currentTime - startTime);
            }
            else
            {
                return useMs
                    ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

    }

}


namespace z3nIO
{
    public static partial class ProjectExtensions
    {
        public static int TimeElapsed(this IZennoPosterProjectModel project, string varName = "varSessionId")
        {
            var start = project.Variables[varName].Value;
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long startTime = long.Parse(start);
            int difference = (int)((currentTime - startTime) / 1000);
            return difference;
        }

        public static T Age<T>(this IZennoPosterProjectModel project)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            long start;
            try
            {
                start = long.Parse(project.Variables["varSessionId"].Value);
            }
            catch
            {
                project.Variables["varSessionId"].Value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                start = long.Parse(project.Variables["varSessionId"].Value);
            }

            long ageMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;
            long ageSec = ageMs / 1000;

            if (typeof(T) == typeof(string))
            {
                string result = TimeSpan.FromMilliseconds(ageMs).ToString();
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                return (T)(object)TimeSpan.FromMilliseconds(ageMs);
            }
            else
            {
                return (T)Convert.ChangeType(ageSec, typeof(T));
            }
        }

        public static void TimeOut(this IZennoPosterProjectModel project, int min = 0)
        {
            if (min == 0) 
            {
                try { min = int.Parse(project.Var("timeOut")); }
                catch { throw new ArgumentException("timeout value not provided in project vars"); }
            }
            
            if (project.TimeElapsed() > 60 * min)
                throw new Exception($"GlobalTimeout {min}min, after {project.LastExecutedActionId}");
        }

        public static int Deadline(this IZennoPosterProjectModel project, int sec = 0, bool log = false)
        {

            if (sec != 0)
            {
                var start = project.Variables["t0"].Value;
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long startTime = long.Parse(start);
                int difference = (int)(currentTime - startTime);
                
                if (difference > sec) throw new Exception($"Deadline Exception: {sec}s, after {project.LastExecutedActionId}");
                if (log) project.log($"{difference}s");
                return difference;
            }
            else
            {
                project.Variables["t0"].Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                return 0;
            }
        }
        public static void StartSession(this IZennoPosterProjectModel project) 
        {
            Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(1000));
            project.Var("varSessionId", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
        }
    }

}
