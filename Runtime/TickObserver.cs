using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Slax.Schedule
{
    /// <summary>
    /// This class is a base class for any class observing the TimeManager directly
    /// although a ScheduleManager class should be present in a scene to have a central
    /// hub of ScheduleEvents invoking. Having this abstract class offers a little bit more
    /// flexibility to do things differently or have additional conditions on
    /// events that can or can't happen.
    ///
    /// Keep in mind that when creating an TickObserver class alongside the Schedule Manager
    /// it will impact performance a little bit as the CheckEventOnTick will happen in both
    /// </summary>
    public abstract class TickObserver : IInitializable, IDisposable
    { 
        protected ScheduleEventsSO _scheduleEvents;

        /// <summary>Event fired when one or multiple events start</summary>
        public event UnityAction<List<ScheduleEvent>> OnTickReceived = delegate { };
        /// <summary>Event fired if the time between ticks configuration is > 1</summary>
        public event UnityAction OnInBetweenTickReceived = delegate { };

        public void SetScheduleEvents(ScheduleEventsSO scheduleEvents) 
        {
            _scheduleEvents = scheduleEvents;
        }
        
        public virtual void Initialize()
        {
            TimeManager.OnDateTimeChanged += CheckEventOnTick;
            TimeManager.OnInBetweenTickFired += FireInBetweenTick;
        }

        public virtual void Dispose()
        {
            TimeManager.OnDateTimeChanged -= CheckEventOnTick;
            TimeManager.OnInBetweenTickFired -= FireInBetweenTick;
        }

        /// <summary>
        /// Goes through the events dictionaries and invokes events if
        /// some need to start
        /// </summary>
        protected virtual void CheckEventOnTick(DateTime date)
        {
            List<ScheduleEvent> eventsToStart = _scheduleEvents.GetEventsForTimestamp(date.GetTimestamp());

            if (eventsToStart.Count == 0) return;

            OnTickReceived.Invoke(eventsToStart);
        }

        /// <summary>
        /// If the Time Manager Configuration has some time between ticks
        /// this method will invoke events for in between tick actions.
        /// </summary>
        protected abstract void FireInBetweenTick();
    }
}
