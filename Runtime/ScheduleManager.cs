using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Slax.Schedule
{
    /// <summary>
    /// This class is responsible for handling Schedule events for NPCs
    /// and any other registered gameplay event based on time
    /// </summary>
    public class ScheduleManager : TickObserver
    {
        /// <summary>Fires a static event with all the Schedule Events happening at that tick</summary>
        public static UnityAction<List<ScheduleEvent>> OnScheduleEvents = delegate { };

        [Header("When set to true, will fire received In Between Tick Events")]
        public bool HasInBetweenTickEvents = false;
        public static UnityAction OnInBetweenTickFired = delegate { };

        [Header("Schedule Event Checks")]
        public ScheduleEventCheckAssociationSO EventCheckAssociationData;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// On every tick received from the TimeManager, gets the events
        /// for the corresponding timestamp if any and raises a UnityAction
        /// with the events for any observer to pickup and process
        /// </summary>
        protected override void CheckEventOnTick(DateTime date)
        {
            Timestamp timestamp = date.GetTimestamp();

            List<ScheduleEvent> eventsToStart = _scheduleEvents.GetEventsForTimestamp(timestamp);

            eventsToStart.RemoveAll(e => !e.IsValid(timestamp));

            if (eventsToStart.Count == 0) return;

            // Run Checks on events
            if (EventCheckAssociationData != null)
            {
                List<ScheduleEvent> checkedEvents = EventCheckAssociationData.RunChecksAndGetPassedEvents(eventsToStart);
                OnScheduleEvents.Invoke(checkedEvents);
                return;
            }

            OnScheduleEvents.Invoke(eventsToStart);
        }

        /// <summary>
        /// On every in between tick received from the TimeManager, fires
        /// a static void event for any observer to pickup and process.
        /// </summary>
        protected override void FireInBetweenTick()
        {
            if (HasInBetweenTickEvents) OnInBetweenTickFired.Invoke();
        }

        /// <summary>
        /// Retrieves a list of all the schedule events that are supposed to happen
        /// during the day of the provided date time.
        /// </summary>
        [Obsolete("Deprecated in favour of using a dictionary. Kept for reference and debugging")]
        protected virtual List<ScheduleEvent> GetTodayEvents(DateTime date)
        {
            List<ScheduleEvent> eventsToday = _scheduleEvents.Events.FindAll(e =>
                !e.SkipSeason(date.Season) &&
                (
                    e.Frequency == ScheduleEventFrequency.DAILY ||
                    (
                        e.Timestamp.Date == date.Date &&
                        (
                            (e.Frequency == ScheduleEventFrequency.WEEKLY && e.Timestamp.Day == date.Day) ||
                            (e.Frequency == ScheduleEventFrequency.MONTHLY) ||
                            (e.Frequency == ScheduleEventFrequency.ANNUAL && e.Timestamp.Season == date.Season)
                        )
                    )
                )
            );

            return eventsToday;
        }
    }
}