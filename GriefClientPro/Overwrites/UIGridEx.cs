namespace GriefClientPro.Overwrites
{
    /*
    // ReSharper disable once InconsistentNaming
    public class UIGridEx : UIGrid
    {
        public override List<Transform> GetChildList()
        {
            Logger.Info("UIGridEx::GetChildList()");

            return base.GetChildList();

            if (GetComponent<CoopSteamNGUI>())
            {
                Logger.Info("Found CoopSteamNGUI");

                return base.GetChildList()
                    .OrderByDescending(o =>
                    {
                        var gameRow = o.GetComponent<MpGameRow>();
                        if (gameRow != null)
                        {
                            Logger.Info("Found MpGameRow: {0} - {1}", gameRow._gameName.text, gameRow._playerLimit.text);
                            return gameRow._playerLimit.text;
                        }

                        return o.name;
                    })
                    .ToList();
            }
            return base.GetChildList();
        }
    }
    */
}
