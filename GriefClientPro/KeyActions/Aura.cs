using System;
using GriefClientPro.Utils;
using TheForest.Buildings.Interfaces;
using TheForest.Utils;
using UnityEngine;

namespace GriefClientPro.KeyActions
{
    public class Aura
    {
        public const float MinRadius = 10;
        public const float MaxRadius = 50;

        public static bool Active { get; set; }
        public static float _radius = MinRadius;
        public static float Radius
        {
            get { return _radius; }
            set { _radius = Mathf.Max(MinRadius, Mathf.Min(MaxRadius, value)); }
        }

        private static int _lastPulse;

        public static class Enabled
        {
            public static bool RepairBuildings = true;
            public static bool KillEnemies = true;
            public static bool KillPlayers;
        }

        public Aura(GriefClientPro instance)
        {
            // Listen to required events
            GriefClientPro.KeyManager.OnKeyUp += OnKeyUp;
            instance.OnTick += OnTick;
        }

        private static void OnKeyUp(KeyManager sender, KeyManager.KeyEventArgs args)
        {
            if (args.Key == KeyManager.Keys.Aura)
            {
                Active = !Active;
            }
        }

        private static void OnTick(object sender, EventArgs args)
        {
            if (Active && Environment.TickCount - _lastPulse > 250)
            {
                _lastPulse = Environment.TickCount;

                var hits = Physics.SphereCastAll(LocalPlayer.MainCam.transform.position, Radius, new Vector3(1f, 0f, 0f));
                if (hits != null && hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
                        if (Enabled.RepairBuildings)
                        {
                            var entity = hit.collider.GetComponent<BoltEntity>();
                            if (entity != null && entity.isAttached && entity.StateIs<IBuildingDestructibleState>() && entity.GetState<IBuildingDestructibleState>().repairTrigger)
                            {
                                Logger.Info("Found destructable structure!");

                                var structure = entity.GetComponentInChildren<TheForest.Buildings.World.BuildingHealth>() ??
                                                entity.GetComponentInChildren<TheForest.Buildings.World.FoundationHealth>() as IRepairableStructure;

                                if (structure != null)
                                {
                                    Logger.Info("Found repairable structure!");

                                    var missingMaterials = structure.CalcTotalRepairMaterial() - structure.RepairMaterial;
                                    var missingLogs = structure.CollapsedLogs - structure.RepairLogs;

                                    if (missingMaterials == 0 && missingLogs == 0)
                                    {
                                        missingMaterials = 1;
                                        missingLogs = 1;
                                    }

                                    if (missingMaterials > 0)
                                    {
                                        for (var i = 0; i < missingMaterials; i++)
                                        {
                                            structure.AddRepairMaterial(false);
                                        }
                                    }
                                    if (missingLogs > 0)
                                    {
                                        for (var i = 0; i < missingLogs; i++)
                                        {
                                            structure.AddRepairMaterial(true);
                                        }
                                    }
                                }
                            }
                        }
                        if (Enabled.KillEnemies)
                        {
                            var entity = hit.collider.GetComponent<BoltEntity>();
                            if (entity != null && entity.isAttached && (entity.StateIs<IMutantState>() || entity.StateIs<IAnimalState>() || entity.StateIs<IAnimalDeerState>()))
                            {
                                try
                                {
                                    var playerHitEnemy = PlayerHitEnemy.Create(Bolt.GlobalTargets.OnlyServer);
                                    playerHitEnemy.Target = entity;
                                    playerHitEnemy.Burn = true;
                                    playerHitEnemy.getStealthAttack = true;
                                    playerHitEnemy.Hit = 1000;
                                    playerHitEnemy.takeDamage = 1000;
                                    playerHitEnemy.HitAxe = true;
                                    playerHitEnemy.Send();
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                        }
                        if (Enabled.KillPlayers)
                        {
                            var player = hit.collider.GetComponent<CoopPlayerRemoteSetup>();
                            if (player != null)
                            {
                                KillAllPlayers.KillSinglePlayer(player);
                            }
                        }
                    }
                }
            }
        }
    }
}
