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
            //string str ="";
            if (Math.Abs((dt.Date - DateTime.Now.Date).Days) < 7)
            {
                return getDeltaDays(dt);
            }
            else
            {
                return getDeltaWeeks(dt);
            }

            //return str;

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
                return getDeltaDays(dt);
            }

            //return str;

        }

        private static string getDeltaDays(DateTime dt)
        {

            if (dt.Date == DateTime.Now.Date)
                return AppResources.today;

            if (dt.Date == DateTime.Now.Date.AddDays(1))
                return AppResources.tomorrow;

            if (dt.Date == DateTime.Now.Date.AddDays(-1))
                return AppResources.yesterday;
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            string str = "";

            int delta = ts.Days % 7;
            if (delta == 0)
            {
                return "";
            }

            if (CultureInfo.CurrentUICulture.Name.Contains("de"))
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
            if (CultureInfo.CurrentUICulture.Name.Contains("en"))
            {
                if (ts.Days > 0 && ts.Days <= 7)
                    str += (AppResources.futureTime + " ");
                if (ts.Days > 7 || ts.Days < -7)
                    str += (AppResources.and + " ");

                str += (Math.Abs(delta).ToString() + " "); 
                if (Math.Abs(delta) > 1)
                    str += AppResources.DayPlural;
                else
                    str += AppResources.DaySingular;

                if ((-1) > ts.Days)
                    str += (" "+ AppResources.pastTime);
            }

            return str;
        }



        private static string getDeltaWeeks(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;

            if (ts.Days < 7 && ts.Days > -7)
            {
                return "";
            }
            else
            {
                int delta = Math.Abs((int)ts.Days / 7);
                string str = "";

                if (CultureInfo.CurrentUICulture.Name.Contains("de"))
                {
                    if (ts.Days <= (-7))
                        str += (AppResources.pastTime + " ");
                    else
                        str += (AppResources.futureTime + " ");

                    str += delta.ToString() + " ";
                    if (delta > 1)
                        str += AppResources.WeekPlural;
                    else
                        str += AppResources.WeekSingular;
                }

                if (CultureInfo.CurrentUICulture.Name.Contains("en"))
                {
                    if (ts.Days > 0)
                        str += (AppResources.futureTime + " ");

                    str += delta.ToString() + " ";
                    if (delta > 1)
                        str += AppResources.WeekPlural;
                    else
                        str += AppResources.WeekSingular;
                }

                return str;
            }
        }
    }
}
