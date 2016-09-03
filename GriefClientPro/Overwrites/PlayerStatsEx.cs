using ModAPI.Attributes;

namespace GriefClientPro.Overwrites
{
    public class PlayerStatsEx : PlayerStats
    {
        [Priority(1000)]
        protected override void hitFallDown()
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.hitFallDown();
            }
        }

        [Priority(1000)]
        protected override void HitFire()
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.HitFire();
            }
        }

        [Priority(1000)]
        public override void hitFromEnemy(int getDamage)
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.hitFromEnemy(getDamage);
            }
        }

        [Priority(1000)]
        public override void HitShark(int damage)
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.HitShark(damage);
            }
        }

        [Priority(1000)]
        protected override void FallDownDead()
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.FallDownDead();
            }
        }

        [Priority(1000)]
        protected override void Fell()
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.Fell();
            }
        }

        [Priority(1000)]
        protected override void HitFromPlayMaker(int damage)
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.HitFromPlayMaker(damage);
            }
        }

        protected override void Update()
        {
            if (Menu.Values.Player.GodMode)
            {
                IsBloody = false;
                FireWarmth = true;
                SunWarmth = true;
                IsCold = false;
                Health = 100f;
                Armor = 400;
                Fullness = 1f;
                Stamina = 100f;
                Energy = 100f;
                Hunger = 0;
                Thirst = 0;
                Starvation = 0;
            }
            base.Update();
        }

        protected override void KillPlayer()
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.KillPlayer();
            }
        }

        public override void HitFood()
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.HitFood();
            }
        }

        public override void HitFoodDelayed(int damage)
        {
            if (!Menu.Values.Player.GodMode)
            {
                base.HitFoodDelayed(damage);
            }
        }
    }
}
