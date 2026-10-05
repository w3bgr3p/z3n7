using System;
using System.CodeDom;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading;

using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>Time helpers: timestamps, deadlines, random pauses.</summary>
    public class Time
    {
        /// <summary>Stopwatch that throws once a time limit is exceeded.</summary>
        public class Deadline
        {
            private long Init { get; set; }
            /// <summary>Starts the stopwatch.</summary>
            public Deadline()
            {
                Reset();
            }
            /// <summary>Returns the seconds elapsed since start or the last <c>Reset</c>.</summary>
            /// <param name="limitSec">Limit in seconds; exceeding it throws <c>TimeoutException</c>.</param>
            public double Check(double limitSec)
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                double differenceMs = (double)(currentTime - Init);
                double differenceSec = differenceMs / 1000.0;

                if (differenceSec > limitSec)
                    throw new TimeoutException($"Deadline Exception: {limitSec}s");

                return differenceSec;
            }
            /// <summary>Restarts the stopwatch.</summary>
            public void Reset()
            {
                Init = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

        }

        /// <summary>Random pause within a fixed range.</summary>
        public class Sleeper
        {
            private readonly int _min;
            private readonly int _max;
            private readonly Random _random;


            /// <summary>
            /// Creates a sleeper for pauses of <c>min</c> … <c>max</c> milliseconds, both inclusive.
            /// </summary>
            /// <param name="min">Minimum, ms. Must not be negative.</param>
            /// <param name="max">Maximum, ms. Must not be less than <c>min</c>.</param>
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
            
            /// <summary>Blocks the thread for a random time within the range.</summary>
            /// <param name="multiplier">Scale factor for the pause, e.g. 2.0 waits twice as long.</param>
            public void Sleep(double multiplier = 1.0)
            {
                int delay = _random.Next(_min, _max + 1);
                Thread.Sleep((int)(delay * multiplier));
            }

        }
        
        /// <summary>Current UTC time as text.</summary>
        /// <param name="format">
        /// <c>unix</c> — milliseconds since epoch; <c>iso</c> — <c>yyyy-MM-ddTHH:mm:ss.fffZ</c>; <c>short</c> —
        /// <c>MM-ddTHH:mm</c>; <c>utcToId</c> — seconds since epoch. Anything else throws.
        /// </param>
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

        /// <summary>Returns a point in time counted from now (UTC), for cooldowns stored in the database.</summary>
        /// <param name="input">
        /// <c>null</c> — today 23:59:59; <c>"nextH"</c> — one minute past the next hour;
        /// <c>int</c>/<c>decimal</c> — that many minutes from now (0 means practically never); other text — a
        /// <c>TimeSpan</c> such as <c>"02:30:00"</c> added to now.
        /// </param>
        /// <param name="o"><c>iso</c> (<c>yyyy-MM-ddTHH:mm:ss.fffZ</c>) or <c>unix</c> (seconds). Anything else throws.</param>
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

        /// <summary>Time since <c>startTime</c>, or the current Unix time when <c>startTime</c> is 0.</summary>
        /// <param name="startTime">Start as Unix time in the same unit as <c>useMs</c> selects.</param>
        /// <param name="useMs">Milliseconds instead of seconds.</param>
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

        /// <summary>Seconds left until the next full hour, local time.</summary>
        public static int TillNextHour()
        {
            var now = DateTime.Now;
            var endOfHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59).AddSeconds(1);
            var secondsToEndOfHour = (int)(endOfHour - now).TotalSeconds;
            return secondsToEndOfHour;
        }


    }

}


namespace z3n7
{
    public static partial class ProjectExtensions
    {
        /// <summary>Seconds since the time stored (as Unix milliseconds) in a project variable.</summary>
        /// <param name="varName">Variable with the start time; default is the session start, <c>varSessionId</c>.</param>
        public static int TimeElapsed(this IZennoPosterProjectModel project, string varName = "varSessionId")
        {
            var start = project.Variables[varName].Value;
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long startTime = long.Parse(start);
            int difference = (int)((currentTime - startTime) / 1000);
            return difference;
        }

        /// <summary>
        /// Age of the session: time since the Unix milliseconds stored in <c>var</c>. When the variable does
        /// not hold a number, it is set to now first.
        /// <c>string</c> returns <c>TimeSpan.ToString()</c>, <c>TimeSpan</c> returns the span, any other type
        /// receives whole seconds converted with <c>Convert.ChangeType</c>.
        /// </summary>
        /// <param name="var">Variable with the start time; default <c>varSessionId</c>.</param>
        public static T Age<T>(this IZennoPosterProjectModel project, string var = null)
        {
            var var0 =  var ?? "varSessionId";
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            long start;
            try
            {
                start = long.Parse(project.Variables[var0].Value);
            }
            catch
            {
                project.Variables[var0].Value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                start = long.Parse(project.Variables[var0].Value);
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

        /// <summary>
        /// Throws once the session (<c>varSessionId</c>) is older than <c>min</c> minutes. The message names
        /// the last executed action.
        /// </summary>
        /// <param name="min">Limit in minutes; 0 reads it from the <c>timeOut</c> variable.</param>
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

        /// <summary>
        /// Two-call deadline based on the <c>t0</c> variable. With <c>sec</c> = 0 it stores the current time
        /// and returns 0; with <c>sec</c> &gt; 0 it returns the seconds since then and throws when they exceed
        /// <c>sec</c>.
        /// </summary>
        /// <param name="sec">Limit in seconds, or 0 to start.</param>
        /// <param name="log">Write the elapsed seconds to the log.</param>
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
        /// <summary>Waits a random 0–1 s and stores the current Unix milliseconds in <c>varSessionId</c>.</summary>
        public static void StartSession(this IZennoPosterProjectModel project) 
        {
            Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(1000));
            project.Var("varSessionId", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
        }
    }

}
