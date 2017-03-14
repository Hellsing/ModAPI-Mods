namespace GriefClientPro.Overwrites
{
    public class MyKeepAboveTerrain : KeepAboveTerrain
    {
        protected override void Awake()
        {
            // Increase build height "slightly"
            maxAirBorneHeight = 800f;
            maxBuildingHeight = 3000f;

            base.Awake();
        }
    }
}
