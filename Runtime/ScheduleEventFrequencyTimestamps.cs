using UnityEngine;

namespace Slax.Schedule
{
    /// <summary>
    /// A useful struct for handling events based on certain timestamps
    /// </summary>
    [System.Serializable]
    public struct Timestamp
    {
        public Days Day;
        [Range(1, 31)]
        public int Date;
        [Range(0, 23)]
        public int Hour;
        [Range(0, 59)]
        public int Minutes;
        public int Year;
        public Month Month;
        public Season Season;

        public Timestamp(Days day, int date, int hour, int minutes, int year, Month month, Season season)
        {
            Date = date;
            Day = day;
            Hour = hour;
            Minutes = minutes;
            Year = year;
            Month = month;
            Season = season;
        }

        /// <summary>Returns the Time in total minutes</summary>
        public int GetTime() => (Hour * 60) + Minutes;
        /// <summary>Returns the date in the year without the year</summary>
        public int GetDate() => (((int)Month) * 44640) + (Date * 1440);
        /// <summary>Returns the full date, year included</summary>
        public int GetFullDate() => (Year * 525600) + ((int)Season * 129600) + (((int)Month) * 44640) + (Date * 1440);

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap around
            {
                int hash = 17;

                hash = hash * 31 + Day.GetHashCode();
                hash = hash * 31 + Date.GetHashCode();
                hash = hash * 31 + Hour.GetHashCode();
                hash = hash * 31 + Minutes.GetHashCode();
                hash = hash * 31 + Year.GetHashCode();
                hash = hash * 31 + Month.GetHashCode();
                hash = hash * 31 + Season.GetHashCode();

                return hash;
            }
        }
    }

    public struct DailyTimestamp
    {
        public int Hour;
        public int Minutes;

        public DailyTimestamp(int hour, int minutes)
        {
            Hour = hour;
            Minutes = minutes;
        }

        public override int GetHashCode()
        {
            return Hour * 60 + Minutes;
        }
    }

    // Weekly timestamp, only need to store the day and time
    public struct WeeklyTimestamp
    {
        public int Day;
        public int Hour;
        public int Minutes;

        public WeeklyTimestamp(int day, int hour, int minutes)
        {
            Day = day;
            Hour = hour;
            Minutes = minutes;
        }

        public override int GetHashCode()
        {
            return (Day * 1440) + (Hour * 60) + Minutes;
        }
    }

    // Monthly timestamp, store the date, time, and month
    public struct MonthlyTimestamp
    {
        public int Month;
        public int Date;
        public int Hour;
        public int Minutes;

        public MonthlyTimestamp(int month, int date, int hour, int minutes)
        {
            Month = month;
            Date = date;
            Hour = hour;
            Minutes = minutes;
        }

        public override int GetHashCode()
        {
            return (Month * 44640) + (Date * 1440) + (Hour * 60) + Minutes;
        }
    }

    // Annual timestamp, store the date, time, month (season)
    public struct AnnualTimestamp
    {
        public int Date;
        public int Hour;
        public int Minutes;
        public int Month;
        public int Season;

        public AnnualTimestamp(int date, int hour, int minutes, int month, int season)
        {
            Date = date;
            Hour = hour;
            Minutes = minutes;
            Month = month;
            Season = season;
        }

        public override int GetHashCode()
        {
            return (Season * 129600) + (Month * 44640) + (Date * 1440) + (Hour * 60) + Minutes;
        }
    }

    /// <summary>
    /// We need to build this struct as there are other elements on the Timestamp
    /// that can mess with the search in the dictionnary
    /// </summary>
    public struct UniqueTimestamp
    {
        public int Date;
        public int Hour;
        public int Minutes;
        public int Month;
        public int Season;
        public int Year;

        public UniqueTimestamp(int date, int hour, int minutes, int month, int season, int year)
        {
            Date = date;
            Hour = hour;
            Minutes = minutes;
            Month = month;
            Season = season;
            Year = year;
        }

        public override int GetHashCode()
        {
            return (Year * 525600) + (Season * 129600) + (Month * 44640) + (Date * 1440) + (Hour * 60) + Minutes;
        }
    }
}