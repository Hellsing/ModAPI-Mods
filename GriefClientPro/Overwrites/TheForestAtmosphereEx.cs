using UnityEngine;

namespace GriefClientPro.Overwrites
{
    public class TheForestAtmosphereEx : TheForestAtmosphere
    {
        protected override void Update()
        {
            if (Menu.Values.World.CaveLight > 0f && InACave)
            {
                CaveAddLight1 = new Color(Menu.Values.World.CaveLight, Menu.Values.World.CaveLight, Menu.Values.World.CaveLight);
                CaveAddLight2 = new Color(Menu.Values.World.CaveLight, Menu.Values.World.CaveLight, Menu.Values.World.CaveLight);

                CaveAddLight1Intensity = Menu.Values.World.CaveLight;
                CaveAddLight2Intensity = Menu.Values.World.CaveLight;
                base.Update();
            }
            else
            {
                base.Update();
            }
        }
    }
}
