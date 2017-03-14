using ModAPI.Attributes;

namespace GriefClientPro.Overwrites
{
    public class PlayerStatsEx : PlayerStats
    {
        [Priority(1000)]
        protected override void hitFallDown()
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.hitFallDown();
            }
        }

        [Priority(1000)]
        protected override void HitFire()
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.HitFire();
            }
        }

        [Priority(1000)]
        public override void hitFromEnemy(int getDamage)
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.hitFromEnemy(getDamage);
            }
        }

        [Priority(1000)]
        public override void HitShark(int damage)
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.HitShark(damage);
            }
        }

        [Priority(1000)]
        protected override void FallDownDead()
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.FallDownDead();
            }
        }

        [Priority(1000)]
        protected override void Fell()
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.Fell();
            }
        }

        [Priority(1000)]
        protected override void HitFromPlayMaker(int damage)
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.HitFromPlayMaker(damage);
            }
        }

        protected override void Update()
        {
            if (Menu.Values.Self.GodMode)
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
            if (!Menu.Values.Self.GodMode)
            {
                base.KillPlayer();
            }
        }

        public override void HitFood()
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.HitFood();
            }
        }

        public override void HitFoodDelayed(int damage)
        {
            if (!Menu.Values.Self.GodMode)
            {
                base.HitFoodDelayed(damage);
            }
        }
    }
}
