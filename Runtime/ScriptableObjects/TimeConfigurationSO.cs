using UnityEngine;

namespace Slax.Schedule
{
    /// <summary>
    /// This class is here to hold a configuration setting for the TimeManager
    /// this allows to create different assets for different time configuration.
    /// It allows for easier testing of different values in time easily by switching
    /// assets instead of changing the configuration every time on the TimeManager
    /// </summary>
    [CreateAssetMenu(menuName = "Slax/ScheduleMaster/TimeConfiguration", fileName = "NewTimeConfiguration")]
    public class TimeConfigurationSO : ScriptableObject
    {
        /// <summary>To keep track of what its for</summary>
        [SerializeField] protected string _notes;
        [SerializeField] protected Season _season;
        public Season Season => _season;
        [SerializeField] protected int _maxYears = 99;
        public int MaxYears => _maxYears;
        [SerializeField] protected int _year = 1;
        public int Year => _year;
        [SerializeField] protected Month _month;
        public Month Month => _month;
        [Range(1, 31)]
        [Tooltip("Day in month")]
        [SerializeField] protected int _date = 0;
        public int Date => _date;
        [Range(0, 24)]
        [SerializeField] protected int _hour = 0;
        public int Hour => _hour;
        [SerializeField] protected int _minutes = 0;
        public int Minutes => _minutes;
        [SerializeField] protected DayConfiguration _dayConfiguration;
        public DayConfiguration DayConfiguration => _dayConfiguration;
        [SerializeField] protected int _tickMinutesIncrease = 10;
        public int TickMinutesIncrease => _tickMinutesIncrease;
        [SerializeField] protected float _timeBetweenTicks = 1f;
        public float TimeBetweenTicks => _timeBetweenTicks;
    }
}
