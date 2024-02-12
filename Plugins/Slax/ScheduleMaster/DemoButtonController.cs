using UnityEngine;
using Slax.Schedule;

public class DemoButtonController : MonoBehaviour
{
        public void OnButtonClickedFlowNormal()
        {
            TimeManager.OnTimeFlowChanged(TimeFlow.Normal);
        }

        public void OnButtonClickedFlowSlow()
        {
            TimeManager.OnTimeFlowChanged(TimeFlow.Slow);
        }
}
