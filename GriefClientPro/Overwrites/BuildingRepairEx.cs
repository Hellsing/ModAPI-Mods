using System;
using ModAPI;
using TheForest.Buildings.World;

namespace GriefClientPro.Overwrites
{
    public class BuildingRepairEx : BuildingRepair
    {
        protected override void Update()
        {
            if (Menu.Values.Player.InstantBuild && Input.GetButton("InstantBuild"))
            {
                this.RepairBuildingInstantly();
            }
            else
            {
                base.Update();
            }
        }
    }

    public static class BuildingRepairHelper
    {
        public static void RepairBuildingInstantly(this BuildingRepair building)
        {
            try
            {
                for (var i = 0; i < building._target.CalcMissingRepairLogs(); i++)
                {
                    building._target.AddRepairMaterial(true);
                }
                for (var i = 0; i < building._target.CalcMissingRepairMaterial(); i++)
                {
                    building._target.AddRepairMaterial(false);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
