using System;
using System.Globalization;
using Pocal.Resources;

namespace Pocal.Helper
{
    public static class DeltaTimeStringCreator
    {
        public static string GetDeltaTime(DateTime dt)
        {
            if (dt.Date == DateTime.Now.Date)
                return AppResources.today;

            if (dt.Date == DateTime.Now.Date.AddDays(1))
                return AppResources.tomorrow;

            if (dt.Date == DateTime.Now.Date.AddDays(-1))
                return AppResources.yesterday;

            var ts = dt.Date - DateTime.Now.Date;

            return ts.Days < 0 ? GetPastDeltaTime(dt) : GetFutureDeltaTime(dt);
        }

        private static string GetPastDeltaTime(DateTime dt)
        {
            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
            {
                var str = "";
                str += AppResources.pastTime;
                str += " ";
                str += GetDeltaWeeks(dt);
                str += AddLineBreakAnd(dt);
                str += GetDeltaDays(dt);
                return str;
            }
            else
            {
                var str = "";
                str += GetDeltaWeeks(dt);
                str += AddAndLineBreak(dt);
                str += GetDeltaDays(dt);
                str += " " + AppResources.pastTime;
                return str;
            }
        }

        private static string GetFutureDeltaTime(DateTime dt)
        {
            var str = "";
            str += AppResources.futureTime + " ";
            str += GetDeltaWeeks(dt);
            str += AddLineBreakAnd(dt);
            str += GetDeltaDays(dt);
            return str;
        }

        private static string AddLineBreakAnd(DateTime dt)
        {
            var ts = dt.Date - DateTime.Now.Date;
            if (AndBeforeLineBreak(ts))
            {
                return (Environment.NewLine + AppResources.and + " ");
            }
            return "";
        }

        private static string AddAndLineBreak(DateTime dt)
        {
            var ts = dt.Date - DateTime.Now.Date;
            if (AndBeforeLineBreak(ts))
            {
                return (" " + AppResources.and + Environment.NewLine);
            }
            return "";
        }

        private static bool AndBeforeLineBreak(TimeSpan ts)
        {
            return ts.Days%7 != 0 && (ts.Days < -7 || ts.Days > 7);
        }

        private static string GetDeltaDays(DateTime dt)
        {
            var str = "";
            var ts = dt.Date - DateTime.Now.Date;

            var delta = ts.Days%7;
            if (delta == 0)
                return str;

            str += (Math.Abs(delta) + " ");
            if (Math.Abs(delta) > 1)
                str += AppResources.DayPlural;
            else
                str += AppResources.DaySingular;


            return str;
        }

        private static string GetDeltaWeeks(DateTime dt)
        {
            var ts = dt.Date - DateTime.Now.Date;
            var delta = Math.Abs(ts.Days/7);
            var str = "";

            if (ts.Days < 7 && ts.Days > -7)
                return str;

            str += delta + " ";
            if (delta > 1)
                str += AppResources.WeekPlural;
            else
                str += AppResources.WeekSingular;

            return str;
        }
    }
}