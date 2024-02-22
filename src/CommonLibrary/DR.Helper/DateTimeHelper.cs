using System.Globalization;
using DR.Constant.Enums;

namespace DR.Helper;

public static class DateTimeHelper {

    public static (DateTimeOffset, DateTimeOffset) GetPeriod(DateTimeOffset time, EDateTimePeriod period) {
        if (period == EDateTimePeriod.Day) {
            return (new DateTimeOffset(time.Date), new DateTimeOffset(time.Date.AddDays(1)));
        }

        if (period == EDateTimePeriod.Week) {
            int diffDay = (int)time.DayOfWeek - 1;
            if (diffDay < 0) diffDay = 6;
            var startWeek = time.Date.AddDays(-diffDay);
            return (new DateTimeOffset(startWeek), new DateTimeOffset(startWeek.AddDays(7)));
        }

        if (period == EDateTimePeriod.Month) {
            var startMonth = time.Date.AddDays(-time.Day + 1);
            return (new DateTimeOffset(startMonth), new DateTimeOffset(startMonth.AddMonths(1)));
        }

        if (period == EDateTimePeriod.Year) {
            return (new DateTimeOffset(time.Year, 1, 1, 0, 0, 0, 0, time.Offset), new DateTimeOffset(time.Year, 12, 31, 23, 59, 59, 999, time.Offset));
        }

        return (DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
    }

    public static (DateTimeOffset, DateTimeOffset) GetPreviousPeriod(DateTimeOffset time, EDateTimePeriod period) {
        if (period == EDateTimePeriod.Day) {
            time.AddDays(-1);
            return (new DateTimeOffset(time.Date), new DateTimeOffset(time.Date.AddDays(1)));
        }

        if (period == EDateTimePeriod.Week) {
            time.AddDays(-7);
            int diffDay = (int)time.DayOfWeek - 1;
            if (diffDay < 0) diffDay = 6;
            var startWeek = time.Date.AddDays(-diffDay);
            return (new DateTimeOffset(startWeek), new DateTimeOffset(startWeek.AddDays(7)));
        }

        if (period == EDateTimePeriod.Month) {
            time.AddMonths(-1);
            var startMonth = time.Date.AddDays(-time.Day + 1);
            return (new DateTimeOffset(startMonth), new DateTimeOffset(startMonth.AddMonths(1)));
        }

        return (DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
    }

    public static bool IsStartOfWeek(DateTime date) {
        DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        DateTime startDateOfWeek = date.AddDays(-(int)date.DayOfWeek);
        return date.DayOfWeek == firstDayOfWeek && date.Date == startDateOfWeek.Date;
    }

    public static bool IsFirstDayOfMonth(DateTime date) {
        return date.Day == 1;
    }

    public static long MsInDay(int? day) {
        if (day == null) return 0;
        if (day == 0) return 0;
        return (long)day * 24 * 3600 * 1000;
    }
}
