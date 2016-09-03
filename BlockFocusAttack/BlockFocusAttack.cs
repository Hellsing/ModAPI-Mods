using System;
using ModAPI.Attributes;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace BlockFocusAttack
{
    public class BlockFocusAttack : MonoBehaviour
    {
        public const string ModName = "BlockFocusAttack";

        public static int LastFocus { get; set; }
        public static bool ShouldAttack
        {
            get { return Environment.TickCount - LastFocus > 200; }
            set
            {
                if (value)
                {
                    LastFocus = 0;
                }
            }
        }
        
        [ExecuteOnGameStart]
        // ReSharper disable once UnusedMember.Local
        private static void Init()
        {
            var go = new GameObject($"__{ModName}__");
            go.AddComponent<BlockFocusAttack>();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                LastFocus = Environment.TickCount;
            }
        }
    }

    public class PlayerInventoryEx : PlayerInventory
    {
        public override void Attack()
        {
            if (this == LocalPlayer.Inventory)
            {
                if (!BlockFocusAttack.ShouldAttack)
                {
                    BlockFocusAttack.ShouldAttack = true;
                    return;
                }
            }
            base.Attack();
        }
    }
}
