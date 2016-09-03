using System;
using GriefClientPro.Overwrites;
using TheForest.Buildings.World;
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
                            var buildingRepair = hit.collider.GetComponent<BuildingRepair>();
                            buildingRepair?.RepairBuildingInstantly();
                        }
                        if (Enabled.KillEnemies)
                        {
                            var enemy = hit.collider.GetComponent<EnemyHealth>();
                            enemy?.SendMessage("Hit", 100);
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
