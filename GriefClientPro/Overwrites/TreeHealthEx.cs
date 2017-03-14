namespace GriefClientPro.Overwrites
{
    public class TreeHealthEx : TreeHealth
    {
        protected override void Hit()
        {
            if (Menu.Values.Self.InstantTree)
            {
                Explosion(100f);
            }
            else
            {
                base.Hit();
            }
        }
    }
}
