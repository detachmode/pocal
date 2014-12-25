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
        public static string getFirstLine(DateTime dt)
        {
            return getDeltaTime(dt);

        }

        public static string getSecondLine(DateTime dt)
        {
            //string str ="";
            if (Math.Abs((dt.Date - DateTime.Now.Date).Days) < 7)
            {
                return "";
            }
            else
            {
                return getDeltaTime(dt);
            }

            //return str;

        }

        private static string getDeltaTime(DateTime dt)
        {


            if (dt.Date == DateTime.Now.Date)
                return AppResources.today;

            if (dt.Date == DateTime.Now.Date.AddDays(1))
                return AppResources.tomorrow;

            if (dt.Date == DateTime.Now.Date.AddDays(-1))
                return AppResources.yesterday;

            string str = "";

            str = getDeltaWeeks(dt);
            str += addAndBeforeLineBreak(dt);
            str += Environment.NewLine;
            str += getDeltaDays(dt);

            return str;
        }

        private static string addAndBeforeLineBreak(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            if (AndBeforeLineBreak(ts))
            {
                  return (" "+AppResources.and);
            }
            return "";
        }

        private static bool AndBeforeLineBreak(TimeSpan ts)
        {
            return CultureInfo.CurrentUICulture.Name.Contains("en-") && ts.Days < -7 && ts.Days % 7 != 0;
        }

        private static string getDeltaDays(DateTime dt)
        {
            string str = "";
            TimeSpan ts = dt.Date - DateTime.Now.Date;

            int delta = ts.Days % 7;
            if (delta == 0)
            {
                return str;
            }

            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
            {
                if ((-7) < ts.Days && ts.Days < 0)
                    str += (AppResources.pastTime + " ");
                if (ts.Days > 0 && ts.Days <= 7)
                    str += (AppResources.futureTime + " ");
                if (ts.Days > 7 || ts.Days < -7)
                    str += (AppResources.and + " ");

                str += (Math.Abs(delta).ToString() + " ");
                if (Math.Abs(delta) > 1)
                    str += AppResources.DayPlural;
                else
                    str += AppResources.DaySingular;

            }
            if (CultureInfo.CurrentUICulture.Name.Contains("en-"))
            {
                if (ts.Days > 0 && ts.Days <= 7)
                    str += (AppResources.futureTime + " ");
                if (ts.Days > 7 || ts.Days < -7)
                {
                    if (!AndBeforeLineBreak(ts))
                        str += (AppResources.and + " ");
                }
                    

                str += (Math.Abs(delta).ToString() + " ");
                if (Math.Abs(delta) > 1)
                    str += AppResources.DayPlural;
                else
                    str += AppResources.DaySingular;

                if ((-1) > ts.Days)
                    str += (" " + AppResources.pastTime);
            }

            return str;
        }



        private static string getDeltaWeeks(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            int delta = Math.Abs((int)ts.Days / 7);
            string str = "";

            if (ts.Days < 7 && ts.Days > -7)
                return str;
 
            
            if (CultureInfo.CurrentUICulture.Name.Contains("de-"))
            {
                if (ts.Days <= (-7))
                    str += (AppResources.pastTime + " ");
                else
                    str += (AppResources.futureTime + " ");
            }

            if (CultureInfo.CurrentUICulture.Name.Contains("en-"))
            {
                if (ts.Days > 0)
                    str += (AppResources.futureTime + " ");

            }

            str += delta.ToString() + " ";
            if (delta > 1)
                str += AppResources.WeekPlural;
            else
                str += AppResources.WeekSingular;

            return str;

        }
    }
}
