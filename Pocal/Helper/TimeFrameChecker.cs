using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Helper
{
    public static class TimeFrameChecker
    {

        public static bool isInTimeFrameOfDay(Appointment appt, DateTime date)
        {
            DateTime startOfDay = date.Date;
            DateTime endOfDay = startOfDay.Add(TimeSpan.FromMinutes(24 * 60));
            return (isInTimeFrame(appt, startOfDay, endOfDay));
        }

        public static bool isInTimeFrame(Appointment appt, DateTime start, DateTime end)
        {
            if (appt.AllDay)
            {
                if (start.Date == appt.StartTime.Date)
                    return true;
                else
                    return false;
            }
            return isInTimeFrameOFStartEnd(appt, start, end);

        }

        public static bool isInTimeFrameOFStartEnd(Appointment appt, DateTimeOffset start, DateTimeOffset end)
        {
            DateTimeOffset thisStartTime = appt.StartTime;
            DateTimeOffset thisEndTime = appt.StartTime + appt.Duration;

            bool endsBeforeThis = (thisEndTime <= start);
            bool beginsAfterThis = (thisStartTime > end);

            bool isNotInTimeFrame = (endsBeforeThis || beginsAfterThis);

            return !isNotInTimeFrame;
        }
    }

}
