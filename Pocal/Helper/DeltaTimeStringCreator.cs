using Pocal.Resources;
using Pocal.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocal.Helper
{
    public static class DeltaTimeStringCreator
    {


        public static string getDeltaTime(DateTime dt)
        {

            if (dt.Date == DateTime.Now.Date)
                return AppResources.today;

            if (dt.Date == DateTime.Now.Date.AddDays(1))
                return AppResources.tomorrow;

            if (dt.Date == DateTime.Now.Date.AddDays(-1))
                return AppResources.yesterday;

            string str = "";
            TimeSpan ts = dt.Date - DateTime.Now.Date;

            if (ts.Days < 0)
            {
                str = getPastDeltaTime(dt);
            }
            else
            {
                str = getFutureDeltaTime(dt);
            }


            return str;
        }

        private static string getPastDeltaTime(DateTime dt)
        {
            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
            {
                string str = "";
                str += AppResources.pastTime;
                str += getDeltaWeeks(dt);
                str += addLineBreakAnd(dt);
                str += getDeltaDays(dt);
                return str;
            }
            else
            {
                string str = "";
                str += getDeltaWeeks(dt);
                str += addAndLineBreak(dt);
                str += getDeltaDays(dt);
                str += " "+AppResources.pastTime;
                return str;

            }
        }

        private static string getFutureDeltaTime(DateTime dt)
        {
            string str = "";
            str += AppResources.futureTime +" ";
            str += getDeltaWeeks(dt);
            str += addLineBreakAnd(dt);
            str += getDeltaDays(dt);
            return str;


        }

        private static string addLineBreakAnd(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            if (AndBeforeLineBreak(ts))
            {
                return (Environment.NewLine + AppResources.and +" ");
            }
            return "";
        }

        private static string addAndLineBreak(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            if (AndBeforeLineBreak(ts))
            {
                return (" " + AppResources.and + Environment.NewLine);
            }
            return "";
        }

        private static bool AndBeforeLineBreak(TimeSpan ts)
        {
            return ts.Days % 7 != 0 && (ts.Days < -7 || ts.Days > 7);
        }

        private static string getDeltaDays(DateTime dt)
        {
            string str = "";
            TimeSpan ts = dt.Date - DateTime.Now.Date;

            int delta = ts.Days % 7;
            if (delta == 0)
                return str;

            str += (Math.Abs(delta).ToString() + " ");
            if (Math.Abs(delta) > 1)
                str += AppResources.DayPlural;
            else
                str += AppResources.DaySingular;


            return str;
        }



        private static string getDeltaWeeks(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            int delta = Math.Abs((int)ts.Days / 7);
            string str = "";

            if (ts.Days < 7 && ts.Days > -7)
                return str;

            str += delta.ToString() + " ";
            if (delta > 1)
                str += AppResources.WeekPlural;
            else
                str += AppResources.WeekSingular;

            return str;

        }
    }
}
