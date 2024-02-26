using Slax.Schedule;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class ScheduleInstaller : MonoInstaller
{
    [SerializeField] private ScheduleEventsSO _scheduleEvents;
    [SerializeField] private TimeConfigurationSO _timeConfigurationSO;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<TimeManager>()
            .AsSingle()
            .OnInstantiated<TimeManager>((ctx, timeManager) =>
                {
                    timeManager.SetTimeConfigurationSO(_timeConfigurationSO);
                    timeManager.Initialize();
                    timeManager.Play();
                }
            )
            .NonLazy();

        Container.BindInterfacesAndSelfTo<ScheduleManager>()
            .AsSingle()
            .OnInstantiated<ScheduleManager>((ctx, scheduleManager) =>
            {
                scheduleManager.SetScheduleEvents(_scheduleEvents);
            })
            .NonLazy();
    }
}