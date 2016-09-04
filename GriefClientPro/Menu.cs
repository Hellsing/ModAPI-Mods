using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GriefClientPro.KeyActions;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;

namespace GriefClientPro
{
    public class Menu : MonoBehaviour
    {
        public const float Width = 550;
        public const float Height = 450;
        public const float ToolbarHeight = 30;
        public const float Padding = 10;
        public const float StepY = 30;

        public static bool IsOpen { get; set; }
        public static int CurrentTab { get; set; }

        public static class Values
        {
            public static class Player
            {
                public static bool Visible;
                public static bool GodMode = true;
                public static bool FlyMode;
                public static bool NoClip;
                public static bool InstantTree;
                public static bool InstantBuild = true;
                public static bool InstaKill = true;
                public static bool InstaDestroy;
                public static float SpeedMultiplier = 1;
                public static float JumpMultiplier = 1;
            }

            public static class World
            {
                public static bool FreezeTime;
                public static float TimeOfDay;
                public static float CaveLight = 1;
                public static bool FreezeWeather;
                public static int ForceWeather = -1;
            }

            public static class Stats
            {
                public static bool FixHealth;
                public static float FixedHealth = -1;
                public static bool FixBatteryCharge = true;
                public static float FixedBatteryCharge = 100;
                public static bool FixFullness;
                public static float FixedFullness = -1;
                public static bool FixStamina;
                public static float FixedStamina = -1;
                public static bool FixEnergy;
                public static float FixedEnergy = -1;
                public static bool FixThirst;
                public static float FixedThirst = -1;
                public static bool FixStarvation;
                public static float FixedStarvation = -1;
                public static bool FixBodyTemp;
                public static float FixedBodyTemp = -1;
            }

            public static class Other
            {
                public static bool FreeCam;
                public static bool InstaRevive;
            }
        }

        private static GUIStyle LabelStyle { get; set; }
        private static float Y { get; set; }
        private static Rect MenuRect { get; set; }

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            // Listen to required events
            GriefClientPro.KeyManager.OnKeyDown += OnKeyDown;
        }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            if (LabelStyle == null)
            {
                // Setup label style
                LabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            }

            if (!IsOpen || ChatBox.IsChatOpen)
            {
                return;
            }

            // Lock the view
            LocalPlayer.FpCharacter.LockView();

            // Apply the skin
            GUI.skin = ModAPI.GUI.Skin;

            // Create the menu
            MenuRect = new Rect((Camera.main.pixelWidth - Width) / 2, 10, Width, Height);
            GUI.Box(MenuRect, "", GUI.skin.window);
            CurrentTab = GUI.Toolbar(new Rect(MenuRect.xMin, MenuRect.yMin, MenuRect.width, ToolbarHeight), CurrentTab,
                new[]
                {
                    new GUIContent("Player"),
                    new GUIContent("World"),
                    new GUIContent("Stats"),
                    new GUIContent("Other"),
                    new GUIContent("Sphere"),
                    new GUIContent("PermaKill"),
                    new GUIContent("Aura")
                },
                GUI.skin.GetStyle("Tabs"));

            Y = MenuRect.yMin + ToolbarHeight + Padding;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (CurrentTab)
            {
                #region Player

                case 0:
                    {
                        AddLabel("Visible:");
                        AddCheckBox(ref Values.Player.Visible, increaseY: true);

                        AddLabel("God mode:");
                        AddCheckBox(ref Values.Player.GodMode, increaseY: true);

                        AddLabel("Fly mode:");
                        AddCheckBox(ref Values.Player.FlyMode, increaseY: true);

                        if (Values.Player.FlyMode)
                        {
                            AddLabel("Noclip:");
                            AddCheckBox(ref Values.Player.NoClip, increaseY: true);
                        }

                        AddLabel("InstantTree:");
                        AddCheckBox(ref Values.Player.InstantTree, increaseY: true);

                        AddLabel("InstantBuild:");
                        AddCheckBox(ref Values.Player.InstantBuild, increaseY: true);

                        AddLabel("InstantKill:");
                        AddCheckBox(ref Values.Player.InstaKill, increaseY: true);

                        AddLabel("InstantDestroy:");
                        AddCheckBox(ref Values.Player.InstaDestroy, increaseY: true);

                        AddLabel("Move speed:");
                        AddSlider(ref Values.Player.SpeedMultiplier, 1, 10, increaseY: true);

                        AddLabel("Jump power:");
                        AddSlider(ref Values.Player.JumpMultiplier, 1, 10, increaseY: true);

                        break;
                    }

                #endregion

                #region World

                case 1:
                    {
                        AddLabel("Time Settings", Padding * 2, increaseY: true);

                        AddLabel("Freeze time:");
                        AddCheckBox(ref Values.World.FreezeTime, increaseY: true);

                        AddLabel("Speed of time:");
                        AddSlider(ref TheForestAtmosphere.Instance.RotationSpeed, 0, 10, increaseY: true);
                        if (AddButton("Reset", 270, 100, increaseY: true))
                        {
                            TheForestAtmosphere.Instance.RotationSpeed = 0.13f;
                        }

                        AddLabel("Time:");
                        AddSlider(ref Values.World.TimeOfDay, 0, 360, increaseY: true);

                        AddLabel("Cave light:");
                        AddSlider(ref Values.World.CaveLight, 0, 1, increaseY: true);

                        AddLabel("Weather Settings", Padding * 2, increaseY: true);

                        AddLabel("Freeze Weather:");
                        AddCheckBox(ref Values.World.FreezeWeather, increaseY: true);

                        if (AddButton("Clear Weather", width: 180, increaseY: true))
                        {
                            Values.World.ForceWeather = 0;
                        }

                        if (AddButton("Cloudy", width: 180, increaseY: true))
                        {
                            Values.World.ForceWeather = 4;
                        }

                        if (AddButton("Light Rain", width: 180, increaseY: true))
                        {
                            Values.World.ForceWeather = 1;
                        }

                        if (AddButton("Medium Rain", width: 180, increaseY: true))
                        {
                            Values.World.ForceWeather = 2;
                        }

                        if (AddButton("Heavy Rain", width: 180, increaseY: true))
                        {
                            Values.World.ForceWeather = 3;
                        }

                        break;
                    }

                #endregion

                #region Stats

                case 2:
                    {
                        const float checkBoxPosition = 360;
                        AddLabel("Fix", checkBoxPosition, increaseY: true);

                        AddStatSlider("Health:", ref Values.Stats.FixedHealth, ref LocalPlayer.Stats.Health, 0, 100, ref Values.Stats.FixHealth, checkBoxPosition);
                        AddStatSlider("Battery charge:", ref Values.Stats.FixedBatteryCharge, ref LocalPlayer.Stats.BatteryCharge, 0, 100, ref Values.Stats.FixBatteryCharge, checkBoxPosition);
                        AddStatSlider("Fullness:", ref Values.Stats.FixedFullness, ref LocalPlayer.Stats.Fullness, 0, 1, ref Values.Stats.FixFullness, checkBoxPosition);
                        AddStatSlider("Stamina:", ref Values.Stats.FixedStamina, ref LocalPlayer.Stats.Stamina, 0, 100, ref Values.Stats.FixStamina, checkBoxPosition);
                        AddStatSlider("Energy:", ref Values.Stats.FixedEnergy, ref LocalPlayer.Stats.Energy, 0, 100, ref Values.Stats.FixEnergy, checkBoxPosition);
                        AddStatSlider("Thirst:", ref Values.Stats.FixedThirst, ref LocalPlayer.Stats.Thirst, 0, 1, ref Values.Stats.FixThirst, checkBoxPosition);
                        AddStatSlider("Starvation:", ref Values.Stats.FixedStarvation, ref LocalPlayer.Stats.Starvation, 0, 1, ref Values.Stats.FixStarvation, checkBoxPosition);
                        AddStatSlider("Body temp:", ref Values.Stats.FixedBodyTemp, ref LocalPlayer.Stats.BodyTemp, 10, 60, ref Values.Stats.FixBodyTemp, checkBoxPosition);

                        break;
                    }

                #endregion

                #region Other

                case 3:
                    {
                        AddLabel("Free cam:");
                        AddCheckBox(ref Values.Other.FreeCam, increaseY: true);

                        AddLabel("InstantRevive:");
                        AddCheckBox(ref Values.Other.InstaRevive, increaseY: true);

                        break;
                    }

                #endregion

                #region Sphere

                case 4:
                    {
                        AddLabel("Enabled actions", Padding * 2, increaseY: true);
                        Y += Padding;

                        AddLabel("BluePrints:");
                        AddCheckBox(ref SphereAction.Enabled.BluePrints, increaseY: true);
                        AddLabel("BreakableCrates:");
                        AddCheckBox(ref SphereAction.Enabled.BreakableCrates, increaseY: true);
                        AddLabel("Buildings:");
                        AddCheckBox(ref SphereAction.Enabled.Buildings, increaseY: true);
                        AddLabel("Bushes:");
                        AddCheckBox(ref SphereAction.Enabled.Bushes, increaseY: true);
                        AddLabel("SuitCases:");
                        AddCheckBox(ref SphereAction.Enabled.SuitCases, increaseY: true);
                        AddLabel("Trees:");
                        AddCheckBox(ref SphereAction.Enabled.Trees, increaseY: true);
                        AddLabel("TreeStumps:");
                        AddCheckBox(ref SphereAction.Enabled.TreeStumps, increaseY: true);
                        AddLabel("KillPlayers:");
                        AddCheckBox(ref SphereAction.Enabled.KillPlayers, increaseY: true);

                        break;
                    }

                #endregion

                #region PermaKill

                case 5:
                    {
                        AddLabel("Kill selected players", Padding * 2, increaseY: true);
                        Y += Padding;

                        var currentPlayers = new HashSet<string>(GriefClientPro.KillAllPlayers.PermaKillPlayers);
                        GriefClientPro.KillAllPlayers.PermaKillPlayers.Clear();
                        foreach (
                            var playerName in
                                BoltNetwork.entities.Where(current => current.StateIs<IPlayerState>() && current.GetState<IPlayerState>().name != GriefClientPro.PlayerName)
                                    .Select(current => current.GetState<IPlayerState>().name))
                        {
                            AddLabel(playerName + ":");
                            if (GUI.Toggle(new Rect(MenuRect.xMin + Padding + 160, Y, 20, 30), currentPlayers.Contains(playerName), ""))
                            {
                                GriefClientPro.KillAllPlayers.PermaKillPlayers.Add(playerName);
                            }
                            Y += StepY;
                        }
                        break;
                    }

                #endregion

                #region Aura

                case 6:
                    {
                        AddLabel("Execute selected actions around camera", increaseY: true);
                        AddLabel("Status: " + (Aura.Active ? "Active" : "Offline"), Padding * 3, increaseY: true);
                        Y += Padding;

                        AddLabel("Radius: " + Math.Round(Aura.Radius));
                        Aura.Radius = AddSlider(ref Aura._radius, Aura.MinRadius, Aura.MaxRadius, width: MenuRect.width - Padding * 3 - 160, increaseY: true);

                        AddLabel("KillEnemies:");
                        AddCheckBox(ref Aura.Enabled.KillEnemies, increaseY: true);

                        AddLabel("KillPlayers:");
                        AddCheckBox(ref Aura.Enabled.KillPlayers, increaseY: true);

                        AddLabel("RepairBuildings:");
                        AddCheckBox(ref Aura.Enabled.RepairBuildings, increaseY: true);
                        break;
                    }

                    #endregion
            }
        }

        private static void OnKeyDown(KeyManager sender, KeyManager.KeyEventArgs args)
        {
            if (args.Key == KeyManager.Keys.OpenMenu)
            {
                // Invert open menu status
                IsOpen = !IsOpen;

                // Check
                if (IsOpen)
                {
                    LocalPlayer.FpCharacter.LockView();
                }
                else
                {
                    LocalPlayer.FpCharacter.UnLockView();
                }
            }
        }

        private static void AddLabel(string text, float x = Padding, float width = 150, float height = 20, bool increaseY = false)
        {
            GUI.Label(new Rect(MenuRect.xMin + Padding + x, Y, width, height), text, LabelStyle);
            IncreaseY(increaseY);
        }

        private static bool AddCheckBox(ref bool updateValue, float x = 160, float width = 20, float height = 30, bool increaseY = false)
        {
            updateValue = GUI.Toggle(new Rect(MenuRect.xMin + Padding + x, Y, width, height), updateValue, "");
            IncreaseY(increaseY);
            return updateValue;
        }

        private static float AddSlider(ref float updateValue, float minValue, float maxValue, float x = 160, float width = 210, float height = 30, bool increaseY = false)
        {
            updateValue = GUI.HorizontalSlider(new Rect(MenuRect.xMin + Padding + x, Y + 3f, width, height), updateValue, minValue, maxValue);
            IncreaseY(increaseY);
            return updateValue;
        }

        private static bool AddButton(string text, float x = Padding, float width = 180, float height = 20, bool increaseY = false)
        {
            var result = GUI.Button(new Rect(MenuRect.xMin + Padding + x, Y, width, height), text);
            IncreaseY(increaseY);
            return result;
        }

        private static bool AddStatSlider(string text, ref float updateValueActive, ref float updateValueInactive, float min, float max, ref bool updateFixValue, float checkBoxPosition)
        {
            AddLabel(text);
            if (updateFixValue)
            {
                if (updateValueActive < 0)
                {
                    updateValueActive = updateValueInactive;
                }
                AddSlider(ref updateValueActive, min, max, width: 160);
            }
            else
            {
                updateValueActive = -1;
                AddSlider(ref updateValueInactive, min, max, width: 160);
            }
            AddLabel(Mathf.RoundToInt(updateFixValue ? updateValueActive : updateValueInactive).ToString(CultureInfo.InvariantCulture), checkBoxPosition - 30, 40);
            AddCheckBox(ref updateFixValue, checkBoxPosition, height: 20, increaseY: true);
            return updateFixValue;
        }

        private static void IncreaseY(bool increaseY = true)
        {
            if (increaseY)
            {
                Y += StepY;
            }
        }
    }
}
