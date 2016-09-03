namespace GriefClientPro.Overwrites
{
    public class EnemyHealthEx : EnemyHealth
    {
        public override void HitReal(int damage)
        {
            if (Menu.Values.Player.InstaKill)
            {
                base.HitReal(damage * 1000);
            }
            else
            {
                base.HitReal(damage);
            }
        }
    }
}
