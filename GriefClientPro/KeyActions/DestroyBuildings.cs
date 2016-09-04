using System;
using System.Collections.Generic;
using System.Linq;
using TheForest.Buildings.Creation;
using TheForest.Buildings.World;
using TheForest.World;
using UnityEngine;

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
            // Get all blueprints
            var blueprints = new List<Craft_Structure>(UnityEngine.Object.FindObjectsOfType<Craft_Structure>());

            // Delete all blueprints
            foreach (var blueprint in blueprints)
            {
                try
                {
                    blueprint.SendMessage("CancelBlueprint");
                    blueprint.SendMessage("CancelBlueprintSafe");
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            // Get all buildings
            var buildings = new List<GameObject>(UnityEngine.Object.FindObjectsOfType<destroyStructure>().Select(o => o.gameObject));
            buildings.AddRange(UnityEngine.Object.FindObjectsOfType<BuildingHealthHitRelay>().Select(o => o.gameObject));
            buildings.AddRange(UnityEngine.Object.FindObjectsOfType<BuildingHealthChunkHitRelay>().Select(o => o.gameObject));
            buildings.AddRange(UnityEngine.Object.FindObjectsOfType<FoundationChunkTier>().Select(o => o.gameObject));
            buildings.AddRange(UnityEngine.Object.FindObjectsOfType<BuildingHealth>().Select(o => o.gameObject));

            // Destroy buildings
            foreach (var building in buildings)
            {
                try
                {
                    destroyStructure structure;
                    if ((structure = building.GetComponent<destroyStructure>()) != null)
                    {
                        structure.SendMessage("Hit", structure.health);
                    }
                    else
                    {
                        building.SendMessage("LocalizedHit", new LocalizedHitData(building.transform.position, 10000f));
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
