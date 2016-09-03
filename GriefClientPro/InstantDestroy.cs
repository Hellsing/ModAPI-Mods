using TheForest.Buildings.World;
using TheForest.World;

namespace GriefClientPro
{
    public static class InstantDestroy
    {
        public static LocalizedHitData GetLocalizedHitData(LocalizedHitData data)
        {
            return Menu.Values.Player.InstaDestroy ? new LocalizedHitData(data._position, 100000) : data;
        }
    }

    public class BuildingHealthChunkHitRelayEx : BuildingHealthChunkHitRelay
    {
        public override void LocalizedHit(LocalizedHitData data)
        {
            base.LocalizedHit(InstantDestroy.GetLocalizedHitData(data));
        }
    }

    public class BuildingHealthHitRelayEx : BuildingHealthHitRelay
    {
        public override void LocalizedHit(LocalizedHitData data)
        {
            base.LocalizedHit(InstantDestroy.GetLocalizedHitData(data));
        }
    }

    public class FoundationChunkTierEx : FoundationChunkTier
    {
        public override void LocalizedHit(LocalizedHitData data)
        {
            base.LocalizedHit(InstantDestroy.GetLocalizedHitData(data));
        }
    }
}
