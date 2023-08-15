using UnityEngine;

namespace Slax.Schedule
{
    [CreateAssetMenu(menuName = ScheduleMasterConfiguration.ScriptableObjectAssetMenu + "Event Checks/Is Specific Day")]
    public class ScheduleEventCheckIsParticularDaySO : ScheduleEventCheckBase
    {
        public Days Day;
        protected Days _currentDay;

        protected virtual void OnEnable()
        {
            TimeManager.OnAwake += SetCurrentDay;
            TimeManager.OnNewDay += SetCurrentDay;
        }

        protected virtual void OnDisable()
        {
            TimeManager.OnAwake -= SetCurrentDay;
            TimeManager.OnNewDay -= SetCurrentDay;
        }

        protected virtual void SetCurrentDay(DateTime date)
        {
            _currentDay = date.Day;
        }

        public override bool CheckEvent(ScheduleEvent scheduleEvent)
        {
            bool result = _currentDay == Day;
            Debug.Log($"Ran check on {scheduleEvent.Name} and status is : {result}");
            return _currentDay == Day;
        }
    }
}
