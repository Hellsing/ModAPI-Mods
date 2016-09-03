using TheForest.Items.Inventory;
using TheForest.Utils;

namespace GriefClientPro.Overwrites
{
    public class PlayerInventoryEx : PlayerInventory
    {
        public override void Attack()
        {
            // Only handle our player
            if (this == LocalPlayer.Inventory && Menu.IsOpen)
            {
                return;
            }

            base.Attack();
        }
    }
}
