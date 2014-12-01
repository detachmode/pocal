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
            if ((dt.Date - DateTime.Now.Date).Days < 7)
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
            if ((dt.Date - DateTime.Now.Date).Days < 7)
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
                return "heute";

            if (dt.Date == DateTime.Now.Date.AddDays(1))
                return "morgen";

            if (dt.Date == DateTime.Now.Date.AddDays(-1))
                return "gestern";
            TimeSpan ts = dt.Date - DateTime.Now.Date;
            string str = "";

            int delta = ts.Days % 7;
            if (delta == 0)
            {
                return "";
            }
            if (ts.Days > 7)
                str += "und ";
            else
                str += "in ";

            str += delta.ToString() + " Tag";
            if (delta > 1)
                str += "en";

            return str;
        }



        private static string getDeltaWeeks(DateTime dt)
        {
            TimeSpan ts = dt.Date - DateTime.Now.Date;

            if (ts.Days < 7)
            {
                return "";
            }
            else
            {
                int delta = (int)ts.Days / 7;
                return "in " + delta.ToString() + " Wochen";
            }
        }
    }
}
