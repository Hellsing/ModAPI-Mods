using System;
using System.IO;
using GriefClientPro.KeyActions;
using GriefClientPro.Utils;
using ModAPI.Attributes;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;
using Input = TheForest.Utils.Input;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GriefClientPro
{
    public class GriefClientPro : MonoBehaviour
    {
        public const string ModName = "GriefClientPro";
        public static readonly string DataFolderLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ModName);
        public static string PlayerName { get; private set; }

        public delegate void TickHandler(object sender, EventArgs args);

        public event TickHandler OnTick;

        private static GameObject DummyObject { get; set; }

        [ExecuteOnGameStart]
        // ReSharper disable once UnusedMember.Local
        private static void Init()
        {
            Logger.Info("Initializing GriefClientPro...");
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

        protected int Tab;

        private static bool _freezeTime;
        public static bool FreezeTime
        {
            get { return _freezeTime; }
            set
            {
                _freezeTime = value;
                _frozenTimeValue = TheForestAtmosphere.Instance.TimeOfDay;
            }
        }
        private static float _frozenTimeValue;
        public static float TimeOfDay
        {
            get { return FreezeTime ? _frozenTimeValue : TheForestAtmosphere.Instance.TimeOfDay; }
            set
            {
                _frozenTimeValue = value;
                TheForestAtmosphere.Instance.TimeOfDay = value;
            }
        }

        public static KeyManager KeyManager { get; private set; }
        public static SphereAction SphereAction { get; private set; }
        public static InstantBuild InstantBuilding { get; private set; }
        public static KillAllPlayers KillAllPlayers { get; private set; }
        public static DestroyBuildings DestroyBuildings { get; private set; }
        public static DestroyTrees DestroyTrees { get; private set; }
        public static Aura Aura { get; set; }

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            try
            {
                // Initialize properties
                Logger.Info("Setting up KeyManager");
                KeyManager = new KeyManager(this);
                Logger.Info("Setting up SphereAction");
                SphereAction = new SphereAction();
                Logger.Info("Setting up InstantBuilding");
                InstantBuilding = new InstantBuild(this);
                Logger.Info("Setting up KillAllPlayers");
                KillAllPlayers = new KillAllPlayers(this);
                Logger.Info("Setting up DestroyBuildings");
                DestroyBuildings = new DestroyBuildings(this);
                Logger.Info("Setting up DestroyTrees");
                DestroyTrees = new DestroyTrees(this);
                Logger.Info("Setting up Aura");
                Aura = new Aura(this);
                Logger.Info("Initialization completed!");
            }
            catch (Exception e)
            {
                Logger.Exception("Error initializing properites", e);
            }
        }

        protected bool LastFreezeTime;
        protected bool LastFreeCam;
        protected float RotationY;

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            try
            {
                if (LocalPlayer.Entity != null && LocalPlayer.Entity.isAttached)
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
                            Logger.Exception("Exception while notifying OnTick listener: " + action.GetType().Name, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception("Exception while looping over OnTick listeners", e);
                }
            }

            if (Menu.Values.Player.Visible && LocalPlayer.Entity != null && !LocalPlayer.Entity.isAttached)
            {
                Utility.AttachLocalPlayer();
            }
            else if (!Menu.Values.Player.Visible && LocalPlayer.Entity != null && LocalPlayer.Entity.isAttached)
            {
                Utility.DetachLocalPlayer();
            }

            if (FreezeTime && !LastFreezeTime)
            {
                Time.timeScale = 0f;
                LastFreezeTime = true;
            }
            if (!FreezeTime && LastFreezeTime)
            {
                Time.timeScale = 1f;
                LastFreezeTime = false;
            }

            if (Menu.Values.Other.FreeCam && !LastFreeCam)
            {
                LocalPlayer.CamFollowHead.enabled = false;
                LocalPlayer.CamRotator.enabled = false;
                LocalPlayer.MainRotator.enabled = false;
                LocalPlayer.FpCharacter.enabled = false;
                LastFreeCam = true;
            }
            if (!Menu.Values.Other.FreeCam && LastFreeCam)
            {
                LocalPlayer.CamFollowHead.enabled = true;
                LocalPlayer.CamRotator.enabled = true;
                LocalPlayer.MainRotator.enabled = true;
                LocalPlayer.FpCharacter.enabled = true;
                LastFreeCam = false;
            }

            if (Menu.Values.Other.FreeCam && !Menu.IsOpen && !LocalPlayer.FpCharacter.Locked)
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
                RotationY += Input.GetAxis("Mouse Y") * 15f;
                RotationY = Mathf.Clamp(RotationY, -80f, 80f);
                Camera.main.transform.localEulerAngles = new Vector3(-RotationY, rotationX, 0);
            }

            if (ModAPI.Input.GetButtonDown("FreezeTime"))
            {
                FreezeTime = !FreezeTime;
            }

            if (ModAPI.Input.GetButtonDown("FreeCam"))
            {
                Menu.Values.Other.FreeCam = !Menu.Values.Other.FreeCam;
            }
            if (ModAPI.Input.GetButton("SphereAction"))
            {
                SphereAction?.OnPrepare();
            }
            else
            {
                SphereAction?.OnTick();
            }
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
    }
}
