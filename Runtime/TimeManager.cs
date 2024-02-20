using UnityEngine;
using UnityEngine.Events;
using SimpleSaveMaster;

namespace Slax.Schedule
{
    /// <summary>
    /// This class is responsible for handling time progress and invoking Tick update events
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private TimeConfigurationSO _timeConfigurationSO;

        public static event UnityAction<DateTime> OnAwake;
        public static event UnityAction<DateTime> OnNewDay;
        public static event UnityAction<DateTime> OnNewMonth;
        public static event UnityAction<DateTime> OnNewSeason;
        public static event UnityAction<DateTime> OnNewYear;
        public static event UnityAction<DateTime> OnDateTimeChanged;
        /// <summary>Fired if there is time between ticks, can be useful</summary>
        public static event UnityAction OnInBetweenTickFired;
        
        private SerializableDateTime serializableDateTime;
        private DateTime _dateTime;
        
        private bool _isPaused = true;
        
        private int _tickMinutesIncrease = 10;
        private static float _timeBetweenTicks = 1f;
        private float _currentTimeBetweenTicks = 0;

        protected virtual void Start()
        {
            Initialize();
            Play();
        }

        protected virtual void Update()
        {
            if (_isPaused) return;

            _currentTimeBetweenTicks += Time.deltaTime;

            if (_currentTimeBetweenTicks >= _timeBetweenTicks)
            {
                _currentTimeBetweenTicks = 0;
                Tick();
            }
            else
            {
                OnInBetweenTickFired.Invoke();
            }
        }

        public virtual void Initialize()
        {
            Setup();
            CreateDateTime();
        }

        public virtual void Play()
        {
            _isPaused = false;
        }

        public virtual void Pause()
        {
            _isPaused = true;
        }

        public virtual void SetNewDay()
        {
            Pause();
            AdvanceTimeStatus status = _dateTime.SetNewDay();
            if (status.AdvancedDay) OnNewDay?.Invoke(_dateTime);
            if (status.AdvancedDay) OnNewMonth?.Invoke(_dateTime);
            if (status.AdvancedSeason) OnNewSeason?.Invoke(_dateTime);
            if (status.AdvancedYear) OnNewYear?.Invoke(_dateTime);
            Play();
        }

        public virtual void SetTime(Timestamp t)
        {
            serializableDateTime.Season = t.Season;
            serializableDateTime.Month = t.Month;
            serializableDateTime.Date = t.Date;
            serializableDateTime.Year = t.Year;
            serializableDateTime.Hour = t.Hour;
            serializableDateTime.Minutes = t.Minutes;

            CreateDateTime();
        }

        public static void OnTimeFlowChanged(TimeFlow timeFlow)
        {
            _timeBetweenTicks = (int)timeFlow;
        }

        protected virtual void Tick()
        {
            // AdvanceTimeStatus status = _dateTime.AdvanceMinutes(_tickMinutesIncrease);
            AdvanceTimeStatus status = _dateTime.SetNewDay();
            OnDateTimeChanged?.Invoke(_dateTime);
            if (status.AdvancedDay) OnNewDay?.Invoke(_dateTime);
            if (status.AdvancedDay) OnNewMonth?.Invoke(_dateTime);
            if (status.AdvancedSeason) OnNewSeason?.Invoke(_dateTime);
            if (status.AdvancedYear) OnNewYear?.Invoke(_dateTime);
        }

        protected virtual void Setup()
        {
            if (SaveMaster.Exists<SerializableDateTime>())
            {
                serializableDateTime = SaveMaster.Load<SerializableDateTime>();
            }
            else
            {
                serializableDateTime.Season = _timeConfigurationSO.Season;
                serializableDateTime.Month = _timeConfigurationSO.Month;
                serializableDateTime.Year = _timeConfigurationSO.Year;
                serializableDateTime.Date = _timeConfigurationSO.Date;
                serializableDateTime.Hour = _timeConfigurationSO.Hour;
                serializableDateTime.Minutes = _timeConfigurationSO.Minutes;
                serializableDateTime.DayConfiguration = _timeConfigurationSO.DayConfiguration;
            }

            _tickMinutesIncrease = _timeConfigurationSO.TickMinutesIncrease;
            _timeBetweenTicks = _timeConfigurationSO.TimeBetweenTicks;
        }

        protected virtual void CreateDateTime()
        {
            _dateTime = new DateTime(
                serializableDateTime.Date,
                (int)serializableDateTime.Month,
                (int)serializableDateTime.Season,
                serializableDateTime.Year,
                serializableDateTime.Hour,
                serializableDateTime.Minutes,
                serializableDateTime.DayConfiguration);

            // We Invoke here so that other scripts can setup during awake with
            // the starting DateTime
            OnAwake?.Invoke(_dateTime);
            OnDateTimeChanged?.Invoke(_dateTime);
        }

        private void SaveDateTime()
        {
            serializableDateTime.Date = _dateTime.Date;
            serializableDateTime.Month = _dateTime.Month;
            serializableDateTime.Season = _dateTime.Season;
            serializableDateTime.Year = _dateTime.Year;
            serializableDateTime.Hour = _dateTime.Hour;
            serializableDateTime.Minutes = _dateTime.Minutes;
            serializableDateTime.DayConfiguration = _dateTime.DayConfiguration;
            
            SaveMaster.Save(serializableDateTime);
        }

        private void OnDisable()
        {
            SaveDateTime();
        }
    }
}