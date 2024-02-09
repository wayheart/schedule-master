namespace Slax.Schedule
{
    /// <summary>
    /// Provides utility methods for working with dates and calendars.
    /// Includes functions for getting seasons from months,
    /// calculating days of the week, etc.
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// Get the current season from the month
        /// </summary>
        public static Season GetCurrentSeason(Month month)
        {
            switch (month)
            {
                case Month.December:
                case Month.January:
                case Month.February:
                    return Season.Winter;

                case Month.March:
                case Month.April:
                case Month.May:
                    return Season.Spring;

                case Month.June:
                case Month.July:
                case Month.August:
                    return Season.Summer;

                case Month.September:
                case Month.October:
                case Month.November:
                    return Season.Autumn;

                default: return Season.Winter;
            }
        }

        /// <summary>
        /// Get the day of the week
        /// </summary>
        public static Days GetDaysOfWeek(int day, int month, int year)
        {
            month++;

            if (month <= 2)
            {
                year--;
                day += 3;
            }

            int dayOfWeek = 1 + (day + year + year / 4 - year / 100 + year / 400 + (31 * month + 10) / 12) % 7;

            return (Days)dayOfWeek - 1;
        }
    }
}
