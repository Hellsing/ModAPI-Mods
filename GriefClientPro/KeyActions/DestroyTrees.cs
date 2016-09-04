using System;

namespace GriefClientPro.KeyActions
{
    public class DestroyTrees
    {
        public DestroyTrees(GriefClientPro instance)
        {
            // Listen to required events
            GriefClientPro.KeyManager.OnKeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyManager.KeyEventArgs args)
        {
            if (args.Key == KeyManager.Keys.DestroyTrees)
            {
                Execute();
            }
        }

        public static void Execute()
        {
            // Get all buildings
            var trees = UnityEngine.Object.FindObjectsOfType<TreeHealth>();

            // Destroy buildings
            foreach (var tree in trees)
            {
                try
                {
                    tree.SendMessage("Explosion", 100f);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
