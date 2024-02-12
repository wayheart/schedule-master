using Slax.Schedule;
using UnityEngine;

public class DemoLogger : MonoBehaviour
{
    private DateTime dateTime;

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += SetLoggerDate;
    }

    private void OnDisable()
    {
        TimeManager.OnDateTimeChanged -= SetLoggerDate;
    }

    private void SetLoggerDate(DateTime date)
    {
        dateTime = date;
    }

    public void ViewLog()
    {
        Debug.Log(dateTime);
    }
}
