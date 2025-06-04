namespace Library.Globalization;

public static class DateTimeFamiliar
{
    public static string GetDisplayString(DateTime dateTime)
    {
        if (dateTime.Date == DateTime.Now.Date)
        {
            var timeSpan = DateTime.Now - dateTime;
            return timeSpan < new TimeSpan(0, 1, 0, 0)
                ? timeSpan.Minutes == 0 ? "همین الان" : timeSpan.Minutes + " دقیقه پیش"
                : dateTime.ToString("t");
        }
        var span = DateTime.Now.Date - dateTime.Date;
        if (span < new TimeSpan(2, 0, 0, 0))
        {
            return string.Format("دیروز | {0}", dateTime.ToString("t"));
        }

        if (span < new TimeSpan(7, 0, 0, 0))
        {
            var output = PersianTools.GetWeekDayName(dateTime);
            return $"{dateTime:t} | {output}";
        }

        return $"{dateTime:t} | {dateTime:d}";
    }

    public static string GetDisplayString(TimeSpan timeSpan)
    {
        var time = string.Empty;
        if (timeSpan.Days != 0)
        {
            time += $"{Math.Abs(timeSpan.Days)} روز |";
        }

        if (timeSpan.Hours != 0)
        {
            time += $"{Math.Abs(timeSpan.Hours)} ساعت  |";
        }

        if (timeSpan.Minutes != 0)
        {
            time += $"{Math.Abs(timeSpan.Minutes)} دقیقه |";
        }

        if (timeSpan.TotalMinutes < 0)
        {
            time += "مانده";
        }
        else if (timeSpan.TotalMinutes > 1)
        {
            time += "گذشته";
        }
        else
        {
            time += "الان";
        }
        return time;
    }

    public static string GetDisplayString(DateTime? dateTime)
        => dateTime.HasValue ? GetDisplayString(dateTime.Value) : "";

    public static string GetDisplayString2(DateTime dateTime)
    {
        if (dateTime.Date == DateTime.Now.Date)
        {
            var timeSpan = DateTime.Now - dateTime;
            return timeSpan < new TimeSpan(0, 1, 0, 0)
                ? timeSpan.Minutes == 0 ? "همین الان" : timeSpan.Minutes + " دقیقه پیش"
                : dateTime.ToString("t");
        }
        var span = DateTime.Now.Date - dateTime.Date;
        return span < new TimeSpan(2, 0, 0, 0)
            ? "دیروز"
            : span < new TimeSpan(5, 0, 0, 0)
                ? (int)DateTime.Now.Subtract(dateTime).TotalDays + " روز قبل "
                : string.Format("{1}- {0} ", dateTime.ToString("M/Y"),
                dateTime.GetPersianDayOfMonth());
    }

    public static string GetElapsedTime(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return "";
        }

        var dateDiff = DateTime.Now.Subtract(dateTime.Value);

        return dateDiff.TotalDays > 365
            ? string.Format("بیشتر از {0} سال پیش", dateDiff.Days / 365)
            : dateDiff.TotalDays > 30
            ? string.Format("بیشتر از {0} ماه پیش", dateDiff.Days / 30)
            : dateDiff.TotalHours > 24
            ? string.Format("{0} روز پیش", dateDiff.Days)
            : dateDiff.TotalMinutes > 60
                ? string.Format("حدود {0} ساعت پیش", (int)(dateDiff.TotalMinutes / 60))
                : string.Format("حدود {0} دقیقه پیش", (int)dateDiff.TotalMinutes);
    }

    public static DateTime GetLastMomentOfDay(DateTime dateTime) => dateTime.Date.AddDays(1).AddTicks(-1);
}