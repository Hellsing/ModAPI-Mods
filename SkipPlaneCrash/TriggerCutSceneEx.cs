using ModAPI;
using Input = TheForest.Utils.Input;

namespace SkipPlaneCrash
{
    public class TriggerCutSceneEx : TriggerCutScene
    {
        protected override void Update()
        {
            if (!skipOpening && Clock.planecrash)
            {
                SpaceTut.SetActive(false);
                LightsFlight.SetActive(false);
                pmTrigger.SendEvent("toSkipOpening");
                skipOpening = true;
            }
            else
            {
                Input.LockMouse();
            }
        }
    }
}
