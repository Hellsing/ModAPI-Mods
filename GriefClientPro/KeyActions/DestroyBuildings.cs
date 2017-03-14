using System;
using System.Linq;
using Bolt;
using TheForest.Buildings.Creation;
using TheForest.Buildings.World;
using TheForest.World;

namespace GriefClientPro.KeyActions
{
    public class DestroyBuildings
    {
        public DestroyBuildings(GriefClientPro instance)
        {
            // Listen to required events
            GriefClientPro.KeyManager.OnKeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyManager.KeyEventArgs args)
        {
            if (args.Key == KeyManager.Keys.DestroyEverything)
            {
                Execute();
            }
        }

        public static void Execute()
        {
            if (BoltNetwork.isRunning)
            {
                foreach (var entity in BoltNetwork.entities.Where(entity => entity.isAttached))
                {
                    try
                    {
                        if (entity.GetComponentInChildren<Craft_Structure>())
                        {
                            // Cancel blueprints
                            var cancelBlueprint = CancelBluePrint.Create(GlobalTargets.OnlyServer);
                            cancelBlueprint.BluePrint = entity;
                            PacketQueue.Add(cancelBlueprint);
                        }
                        else if (entity.StateIs<IBuildingDestructibleState>())
                        {
                            // Destroy building
                            var destroyBuilding = DestroyBuilding.Create(GlobalTargets.OnlyServer);
                            destroyBuilding.BuildingEntity = entity;
                            PacketQueue.Add(destroyBuilding);
                        }
                        else if (entity.gameObject.GetComponentInChildren<BuildingHealthHitRelay>() ||
                                 entity.gameObject.GetComponentInChildren<BuildingHealthChunkHitRelay>() ||
                                 entity.gameObject.GetComponentInChildren<FoundationChunkTier>() ||
                                 entity.gameObject.GetComponentInChildren<BuildingHealth>())
                        {
                            entity.gameObject.SendMessage("LocalizedHit", new LocalizedHitData(entity.gameObject.transform.position, 10000f));
                        }
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
