﻿using System;
using System.IO;
using Bolt;
using GriefClientPro.KeyActions;
using GriefClientPro.Utils;
using ModAPI.Attributes;
using TheForest.Networking;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;
using Input = TheForest.Utils.Input;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GriefClientPro
{
    public class GriefClientPro : MonoBehaviour
    {
        public const string ModName = nameof(GriefClientPro);
        public static readonly string DataFolderLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ModName);
        public static string PlayerName { get; private set; }

        public delegate void TickHandler(object sender, EventArgs args);

        public event TickHandler OnTick;

        public static GriefClientPro Instance { get; set; }

        private static GameObject DummyObject { get; set; }
        private static Vector3 LastFirePos { get; set; }
        private static int LastFireTime { get; set; }

        public static KeyManager KeyManager { get; private set; }
        public static PlayerManager PlayerManager { get; private set; }
        public static SphereAction SphereAction { get; private set; }
        public static InstantBuild InstantBuilding { get; private set; }
        public static KillAllPlayers KillAllPlayers { get; private set; }
        public static DestroyBuildings DestroyBuildings { get; private set; }
        public static DestroyTrees DestroyTrees { get; private set; }
        public static Aura Aura { get; private set; }
        public static ChatManager ChatManager { get; private set; }
        public static VoiceManager VoiceManager { get; private set; }

        [ExecuteOnGameStart]
        // ReSharper disable once UnusedMember.Local
        private static void Init()
        {
            Logger.Info($"Initializing {nameof(GriefClientPro)}...");
            Logger.Info("Dummy object: " + (DummyObject == null ? "null (good)" : "already set (bad)"));

            try
            {
                if (DummyObject == null)
                {
                    DummyObject = new GameObject($"__{ModName}__");
                    DummyObject.AddComponent<GriefClientPro>();
                    DummyObject.AddComponent<Menu>();
                }
            }
            catch (Exception e)
            {
                Logger.Exception("Failed to initialize dummy object", e);
            }

            Logger.SaveLogToFile();
        }

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            try
            {
                Instance = this;

                // Initialize properties
                Logger.Info($"Setting up {nameof(KeyManager)}");
                KeyManager = new KeyManager(this);
                Logger.Info($"Setting up {nameof(PlayerManager)}");
                PlayerManager = new PlayerManager(this);
                Logger.Info($"Setting up {nameof(SphereAction)}");
                SphereAction = new SphereAction();
                Logger.Info($"Setting up {nameof(InstantBuilding)}");
                InstantBuilding = new InstantBuild(this);
                Logger.Info($"Setting up {nameof(KillAllPlayers)}");
                KillAllPlayers = new KillAllPlayers(this);
                Logger.Info($"Setting up {nameof(DestroyBuildings)}");
                DestroyBuildings = new DestroyBuildings(this);
                Logger.Info($"Setting up {nameof(DestroyTrees)}");
                DestroyTrees = new DestroyTrees(this);
                Logger.Info($"Setting up {nameof(Aura)}");
                Aura = new Aura(this);
                Logger.Info($"Setting up {nameof(ChatManager)}");
                ChatManager = new ChatManager(this);
                Logger.Info($"Setting up {nameof(VoiceManager)}");
                VoiceManager = new VoiceManager(this);
                Logger.Info("Initialization completed!");
            }
            catch (Exception e)
            {
                Logger.Exception("Error initializing properites", e);
            }
        }

        public bool FreeCam;
        private bool _lastFreeCam;
        private float _rotationY;

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            try
            {
                if (BoltNetwork.isRunning && LocalPlayer.Entity != null && LocalPlayer.Entity.isAttached)
                {
                    PlayerName = LocalPlayer.Entity.GetState<IPlayerState>().name;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (ChatBox.IsChatOpen)
            {
                return;
            }

            if (OnTick != null)
            {
                try
                {
                    foreach (var action in OnTick.GetInvocationList())
                    {
                        try
                        {
                            action.DynamicInvoke(this, EventArgs.Empty);
                        }
                        catch (Exception e)
                        {
                            Logger.Exception($"Exception while notifying {nameof(OnTick)} listener: " + action.GetType().Name, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception($"Exception while looping over {nameof(OnTick)} listeners", e);
                }
            }

            if (BoltNetwork.isRunning)
            {
                // Visible player
                if (Menu.Values.Self.Visible && LocalPlayer.Entity != null && !LocalPlayer.Entity.isAttached)
                {
                    Utility.AttachLocalPlayer();
                }
                // Invisible player
                else if (!Menu.Values.Self.Visible && LocalPlayer.Entity != null && LocalPlayer.Entity.isAttached)
                {
                    Utility.DetachLocalPlayer();
                }

                // Add fire trail to movement
                if (Menu.Values.Self.FireTrail)
                {
                    var feetPos = LocalPlayer.Transform.position - new Vector3(0, 4, 0);
                    if (Vector3.Distance(LastFirePos, feetPos) > 2 || Environment.TickCount - LastFireTime > 5000)
                    {
                        LastFirePos = feetPos;
                        LastFireTime = Environment.TickCount;
                        BoltPrefabsHelper.Spawn(BoltPrefabs.Fire, feetPos, LocalPlayer.Transform.rotation);
                    }
                }
            }

            if (FreeCam && !_lastFreeCam)
            {
                LocalPlayer.CamFollowHead.enabled = false;
                LocalPlayer.CamRotator.enabled = false;
                LocalPlayer.MainRotator.enabled = false;
                LocalPlayer.FpCharacter.enabled = false;
                _lastFreeCam = true;
            }
            if (!FreeCam && _lastFreeCam)
            {
                LocalPlayer.CamFollowHead.enabled = true;
                LocalPlayer.CamRotator.enabled = true;
                LocalPlayer.MainRotator.enabled = true;
                LocalPlayer.FpCharacter.enabled = true;
                _lastFreeCam = false;
            }

            if (FreeCam && !Menu.IsOpen && !LocalPlayer.FpCharacter.Locked)
            {
                var button1 = Input.GetButton("Crouch");
                var button2 = Input.GetButton("Run");
                var button3 = Input.GetButton("Jump");
                var multiplier = 0.1f;
                if (button2)
                {
                    multiplier = 2f;
                }

                var vector3 = Camera.main.transform.rotation * (
                                  new Vector3(Input.GetAxis("Horizontal"),
                                      0f,
                                      Input.GetAxis("Vertical")
                                  ) * multiplier);
                if (button3)
                {
                    vector3.y += multiplier;
                }
                if (button1)
                {
                    vector3.y -= multiplier;
                }
                Camera.main.transform.position += vector3;

                var rotationX = Camera.main.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * 15f;
                _rotationY += Input.GetAxis("Mouse Y") * 15f;
                _rotationY = Mathf.Clamp(_rotationY, -80f, 80f);
                Camera.main.transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0);
            }

            if (ModAPI.Input.GetButtonDown("FreeCam"))
            {
                FreeCam = !FreeCam;
            }

            if (ModAPI.Input.GetButton("SphereAction"))
            {
                SphereAction?.OnPrepare();
            }
            else
            {
                SphereAction?.OnTick();
            }

            if (LocalPlayer.Stats != null)
            {
                if (Menu.Values.Stats.FixBodyTemp)
                {
                    LocalPlayer.Stats.BodyTemp = Menu.Values.Stats.FixedBodyTemp;
                }
                if (Menu.Values.Stats.FixBatteryCharge)
                {
                    LocalPlayer.Stats.BatteryCharge = Menu.Values.Stats.FixedBatteryCharge;
                }
                if (Menu.Values.Stats.FixEnergy)
                {
                    LocalPlayer.Stats.Energy = Menu.Values.Stats.FixedEnergy;
                }
                if (Menu.Values.Stats.FixHealth)
                {
                    LocalPlayer.Stats.Health = Menu.Values.Stats.FixedHealth;
                }
                if (Menu.Values.Stats.FixStamina)
                {
                    LocalPlayer.Stats.Stamina = Menu.Values.Stats.FixedStamina;
                }
                if (Menu.Values.Stats.FixFullness)
                {
                    LocalPlayer.Stats.Fullness = Menu.Values.Stats.FixedFullness;
                }
                if (Menu.Values.Stats.FixStarvation)
                {
                    LocalPlayer.Stats.Starvation = Menu.Values.Stats.FixedStarvation;
                }
                if (Menu.Values.Stats.FixThirst)
                {
                    LocalPlayer.Stats.Thirst = Menu.Values.Stats.FixedThirst;
                }
            }

            if (BoltNetwork.isRunning && Menu.Values.Self.InstaRevive)
            {
                foreach (var player in PlayerManager.Players)
                {
                    var triggerObject = player.DeadTriggerObject;
                    if (triggerObject != null && triggerObject.activeSelf)
                    {
                        var trigger = triggerObject.GetComponent<RespawnDeadTrigger>();

                        //Logger.Info("Reviving " + player.Entity.GetState<IPlayerState>().name);

                        // Send revive packet
                        var playerHealed = PlayerHealed.Create(GlobalTargets.Others);
                        playerHealed.HealingItemId = trigger._healItemId;
                        playerHealed.HealTarget = player.Entity;
                        PacketQueue.Add(playerHealed);

                        // Set revive trigger inactive
                        trigger.SendMessage("SetActive", false);
                    }
                }
            }
        }
    }
}
