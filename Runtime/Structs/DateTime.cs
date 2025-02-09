using UnityEngine;
using Slax.Schedule.Utility;

namespace Slax.Schedule
{
    /// <summary>
    /// The core struct of the Time and Schedule system. It holds information
    /// about the time of day, the date, and offers many helper properties and
    /// methods for the classes using this struct
    /// </summary>
    [System.Serializable]
    public struct DateTime
    {
        #region Properties
        private Days _day;
        /// <summary>The current Day of the week</summary>
        public Days Day => _day;
        private int _date;
        /// <summary>The current Date (day # in the month)</summary>
        public int Date => _date;
        private int _year;
        /// <summary>The current Year</summary>
        public int Year => _year;

        private int _hour;
        /// <summary>Tue current Hour</summary>
        public int Hour => _hour;
        private int _minutes;
        /// <summary>The current minutes</summary>
        public int Minutes => _minutes;

        private Season _season;
        /// <summary>The current season</summary>
        public Season Season => _season;

        private Month _month;
        /// <summary>The current Month</summary>
        public Month Month => _month;

        //private int _totalNumDays;
        //public int TotalNumDays => _totalNumDays;
        //private int _totalNumWeeks;
        //public int TotalNumWeeks => _totalNumWeeks;
        /// <summary>Current week on a 4 seasons of 28 days basis</summary>
        //public int CurrentWeek => _totalNumWeeks % 16 == 0 ? 16 : _totalNumWeeks % 16;

        private float _currentDayProgress;
        /// <summary>Progress of the day, ignoring day configuration so from 00:00 to 23:59</summary>
        public float CurrentDayProgress => _currentDayProgress;

        private DayConfiguration _dayConfiguration;
        public DayConfiguration DayConfiguration => _dayConfiguration;
        #endregion

        public DateTime(int date, int month, int season, int year, int hour, int minutes, DayConfiguration dayConfiguration)
        {
            _day = DateUtils.GetDaysOfWeek(date, month, year);
            _date = date;
            _season = (Season)season;
            _month = (Month)month;

            _year = year;

            _hour = hour;
            _minutes = minutes;

            //_totalNumDays = (int)_season > 0 ? date + (28 * (int)_season) : date;
            //_totalNumDays = year > 1 ? _totalNumDays + ((28 * 4) * (year - 1)) : _totalNumDays;

            //_totalNumWeeks = 1 + _totalNumDays / 7;

            int totalMinutesElapsed = (_hour * 60) + _minutes;
            _currentDayProgress = (float)totalMinutesElapsed / 1440; // 1440 = 24 * 60 mins

            _dayConfiguration = dayConfiguration;
        }

        #region Time Advancement

        /// <summary>
        /// Advances the time by a day and configures it to
        /// the start of the morning from the day configuration
        /// </summary>
        public AdvanceTimeStatus SetNewDay()
        {
            _hour = _dayConfiguration.MorningStartHour;
            RecalculateCurrentDayProgress();
            return AdvanceDay(new AdvanceTimeStatus());
        }

        /// <summary>
        /// Advances the time by a certain amount of minutes
        /// </summary>
        public AdvanceTimeStatus AdvanceMinutes(int minutes)
        {
            AdvanceTimeStatus status = new AdvanceTimeStatus();
            if (_minutes + minutes >= 60)
            {
                _minutes = (_minutes + minutes) % 60;
                return AdvanceHour(status);
            }
            else _minutes += minutes;
            status.AdvancedMinutes = true;
            RecalculateCurrentDayProgress();
            return status;
        }

        /// <summary>
        /// Called when the advance minutes notices
        /// a change in hour is needed
        /// </summary>
        private AdvanceTimeStatus AdvanceHour(AdvanceTimeStatus status)
        {
            if ((_hour + 1) == 24)
            {
                _hour = 0;
                return AdvanceDay(status);
            }
            else _hour++;
            RecalculateCurrentDayProgress();
            status.AdvancedHour = true;
            return status;
        }

        /// <summary>
        /// Called when the advance hour notices
        /// a change in day is needed
        /// </summary>
        private AdvanceTimeStatus AdvanceDay(AdvanceTimeStatus status)
        {
            if (_day + 1 > (Days)6)
            {
                _day = 0;
                //_totalNumWeeks++;
            }
            else
            {
                _day++;
            }

            _date++;

            if (_date > DaysInMonth(_month, _year))
            {
                _date = 1;
                return AdvanceMonth(status);
            }

            //_totalNumDays++;
            status.AdvancedDay = true;
            RecalculateCurrentDayProgress();
            return status;
        }

        /// <summary>
        /// Called when the advance day notices
        /// a change in month is needed
        /// </summary>
        private AdvanceTimeStatus AdvanceMonth(AdvanceTimeStatus status)
        {
            _month = (Month)(((int)_month + 1) % 12); // Циклічний перехід між місяцями

            if (_month == Month.January)
            {
                return AdvanceYear(status); // Інкремент року при переході на січень
            }

            // Тут можна додати логіку для автоматичного оновлення сезону, якщо потрібно
            if (_month == GetLastMonthOfSeason(_season))
            {
                return AdvanceSeason(status);
            }

            status.AdvancedMonth = true;
            RecalculateCurrentDayProgress();
            return status;
        }

        /// <summary>
        /// Called when the advance Day notices
        /// a change in seasons is needed
        /// </summary>
        private AdvanceTimeStatus AdvanceSeason(AdvanceTimeStatus status)
        {
            _season = (Season)(((int)_season + 1) % 4);

            status.AdvancedSeason = true;
            RecalculateCurrentDayProgress();
            return status;
        }

        /// <summary>
        /// Called when Advance season notices
        /// a change in year is needed
        /// </summary>
        private AdvanceTimeStatus AdvanceYear(AdvanceTimeStatus status)
        {
            _date = 1;
            _year++;
            status.AdvancedYear = true;
            RecalculateCurrentDayProgress();
            return status;
        }

        /// <summary>
        /// Recalculates the ratio of progression in the day between
        /// midnight and 23.59. This is useful for light systems etc
        /// </summary>
        private void RecalculateCurrentDayProgress()
        {
            int totalMinutesElapsed = (_hour * 60) + _minutes;
            _currentDayProgress = (float)totalMinutesElapsed / 1440; // 1440 = 24 * 60 mins
        }

        /// <summary>
        /// Method to get the number of days in a month
        /// </summary>
        private int DaysInMonth(Month month, int year)
        {
            switch (month)
            {
                case Month.February:
                    return IsLeapYear(year) ? 29 : 28;
                case Month.April:
                case Month.June:
                case Month.September:
                case Month.November:
                    return 30;
                default:
                    return 31;
            }
        }

        /// <summary>
        /// Method for checking for leap years
        /// </summary>
        private bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }

        #endregion

        #region Bool Checks
        public bool IsNight => _hour >= _dayConfiguration.NightStartHour || _hour < _dayConfiguration.MorningStartHour;
        public bool IsMorning => _hour >= _dayConfiguration.MorningStartHour && _hour < _dayConfiguration.AfternoonStartHour;
        public bool IsAfternoon => _hour >= _dayConfiguration.AfternoonStartHour && _hour < _dayConfiguration.NightStartHour;
        public bool IsWeekend => _day > Days.Fri;
        public bool IsParticularDay(Days day) => day == _day;
        #endregion

        #region Season Start
        public DateTime SeasonStart(Season season, int year)
        {
            season = (Season)Mathf.Clamp((int)season, 0, 3);
            if (year == 0) year = 1;

            return new DateTime(1, (int)GetFirstMonthOfSeason(season), (int)season, year, _dayConfiguration.MorningStartHour, 0, _dayConfiguration);
        }

        public DateTime SpringStart(int year) => SeasonStart(Season.Spring, year);
        public DateTime SummerStart(int year) => SeasonStart(Season.Summer, year);
        public DateTime AutumnStart(int year) => SeasonStart(Season.Autumn, year);
        public DateTime WinterStart(int year) => SeasonStart(Season.Winter, year);
        #endregion

        #region Month Get Of Season
        /// <summary>
        /// Returns the first month of the season.
        /// Useful when you need to find out the first month in the season
        /// </summary>
        private Month GetFirstMonthOfSeason(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return Month.March;
                case Season.Summer:
                    return Month.June;
                case Season.Autumn:
                    return Month.September;
                case Season.Winter:
                    return Month.December;
                default:
                    return Month.January;
            }
        }

        /// <summary>
        /// Returns the last month of the season.
        /// Useful when you need to find out the last month in the season
        /// </summary>
        private Month GetLastMonthOfSeason(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return Month.May;
                case Season.Summer:
                    return Month.August;
                case Season.Autumn:
                    return Month.November;
                case Season.Winter:
                    return Month.February;
                default:
                    return Month.December;
            }
        }
        #endregion

        /// <summary>
        /// Returns a Schedule Timestamp. Useful for comparing events to the
        /// current timestamp and check if they should run
        /// </summary>
        public Timestamp GetTimestamp() => new Timestamp(_day, _date, _hour, _minutes, _year, _month, _season);

        public override string ToString()
        {
            return $"Date: {DateToString()} Season: {_season} Time: {TimeToString()} ";
                //+ $"\nTotal Days: {_totalNumDays} | Total Weeks: {_totalNumWeeks}";
        }

        public string DateToString() => $"{Day} {Date}";
        public string TimeToString()
        {
            int amPmHour = 0;

            if (_hour == 0) amPmHour = 12;
            else if (_hour == 24) amPmHour = 12;
            else if (_hour >= 13) amPmHour = _hour - 12;
            else amPmHour = _hour;

            string AmPm = _hour < 12 ? "AM" : "PM";

            return $"{amPmHour.ToString("D2")}:{_minutes.ToString("D2")} {AmPm}";
        }
    }

    /// <summary>
    /// Configuration of one in game day
    /// </summary>
    [System.Serializable]
    public struct DayConfiguration
    {
        [Range(0, 23)]
        public int MorningStartHour;
        [Range(0, 23)]
        public int AfternoonStartHour;
        [Range(0, 23)]
        public int NightStartHour;
    }

    /// <summary>
    /// Configuration of one in game day
    /// </summary>
    public enum TimeFlow
    {
        Normal = 1,
        Slow = 4,
    }

    /// <summary>
    /// Days of the week
    /// </summary>
    [System.Serializable]
    public enum Days
    {
        Mon = 0,
        Tue = 1,
        Wed = 2,
        Thu = 3,
        Fri = 4,
        Sat = 5,
        Sun = 6
    }

    /// <summary>
    /// Month
    /// </summary>
    [System.Serializable]
    public enum Month
    {
        January = 0,
        February = 1,
        March = 2,
        April = 3,
        May = 4,
        June = 5,
        July = 6,
        August = 7,
        September = 8,
        October = 9,
        November = 10,
        December = 11
    }

    /// <summary>
    /// Seasons
    /// </summary>
    [System.Serializable]
    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 3
    }

    public class AdvanceTimeStatus
    {
        public bool AdvancedMinutes;
        public bool AdvancedHour;
        public bool AdvancedDay;
        public bool AdvancedMonth;
        public bool AdvancedSeason;
        public bool AdvancedYear;

        public AdvanceTimeStatus()
        {
            AdvancedMinutes = false;
            AdvancedHour = false;
            AdvancedDay = false;
            AdvancedMonth = false;
            AdvancedSeason = false;
            AdvancedYear = false;
        }
    }
}
