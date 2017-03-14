using System;
using Bolt;

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
            if (BoltNetwork.isRunning)
            {
                // Destroy all trees
                foreach (var tree in UnityEngine.Object.FindObjectsOfType<TreeHealth>())
                {
                    var entity = tree.LodTree.GetComponent<BoltEntity>();
                    //if (entity.isAttached)
                    {
                        try
                        {
                            var destroyTree = DestroyTree.Create(GlobalTargets.OnlyServer);
                            destroyTree.Tree = entity;
                            PacketQueue.Add(destroyTree);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }
            }
        }
    }
}
