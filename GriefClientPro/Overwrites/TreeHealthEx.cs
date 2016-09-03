namespace GriefClientPro.Overwrites
{
    public class TreeHealthEx : TreeHealth
    {
        protected override void Hit()
        {
            if (Menu.Values.Player.InstantTree)
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
