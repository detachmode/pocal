using System;
using Windows.ApplicationModel.Appointments;

namespace Pocal.Helper
{
    public static class TimeFrameChecker
    {

        public static bool IsInTimeFrameOfDay(Appointment appt, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.Add(TimeSpan.FromMinutes(24 * 60));
            return (IsInTimeFrame(appt, startOfDay, endOfDay));
        }

        public static bool IsInTimeFrame(Appointment appt, DateTime start, DateTime end)
        {
            if (appt.AllDay)
            {
                return start.Date == appt.StartTime.Date;
            }
            return IsInTimeFrameOfStartEnd(appt, start, end);

        }

        public static bool IsInTimeFrameOfStartEnd(Appointment appt, DateTimeOffset start, DateTimeOffset end)
        {
            var thisStartTime = appt.StartTime;
            var thisEndTime = appt.StartTime + appt.Duration;

            var endsBeforeThis = (thisEndTime <= start);
            var beginsAfterThis = (thisStartTime > end);

            var isNotInTimeFrame = (endsBeforeThis || beginsAfterThis);

            return !isNotInTimeFrame;
        }
    }

}
