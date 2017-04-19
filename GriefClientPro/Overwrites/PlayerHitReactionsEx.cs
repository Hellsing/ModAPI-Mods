namespace GriefClientPro.Overwrites
{
    public class PlayerHitReactionsEx : playerHitReactions
    {
        public override void enableExplodeShake(float dist)
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.enableExplodeShake(dist);
            }
        }
    }
}
