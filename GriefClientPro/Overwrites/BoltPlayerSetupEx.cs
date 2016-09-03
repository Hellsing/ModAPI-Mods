using System;
using Bolt;
using GriefClientPro.Utils;
using TheForest.Networking;
using TheForest.Utils;

namespace GriefClientPro.Overwrites
{
    public class BoltPlayerSetupEx : BoltPlayerSetup
    {
        protected override void Update()
        {
            try
            {
                if (Menu.Values.Other.InstaRevive && RespawnDeadTrigger != null && RespawnDeadTrigger.activeSelf && RespawnDeadTrigger.GetComponent<RespawnDeadTrigger>() != null)
                {
                    LocalPlayer.Tuts.HideReviveMP();
                    var playerHealed = PlayerHealed.Create(GlobalTargets.Others);
                    playerHealed.HealingItemId = RespawnDeadTrigger.GetComponent<RespawnDeadTrigger>()._healItemId;
                    playerHealed.HealTarget = entity;
                    playerHealed.Send();
                    RespawnDeadTrigger.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Logger.Exception("Exception while reviving player '{0}'", e, entity?.GetState<IPlayerState>()?.name);
            }

            base.Update();
        }
    }
}
