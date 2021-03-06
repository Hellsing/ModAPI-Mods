﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GriefClientPro.KeyActions;
using GriefClientPro.Utils;
using Steamworks;
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
            public static class Self
            {
                public static Vector2 ScrollPosition = Vector2.zero;

                public static bool InstaRevive;
                public static bool Visible;
                public static bool GodMode = true;
                public static bool FireTrail;
                public static bool FlyMode;
                public static bool NoClip;
                public static bool InstantTree;
                public static bool InstaDestroy;
                public static float SpeedMultiplier = 1;
                public static float JumpMultiplier = 1;
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
        }

        private static GUIStyle LabelStyle { get; set; }
        private static float Y { get; set; }
        private static Rect MenuRect { get; set; }

        private static Dictionary<ulong, Vector3> LastFirePositions { get; } = new Dictionary<ulong, Vector3>();

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
            GUI.skin = ModAPI.Gui.Skin;

            // Create the menu
            MenuRect = new Rect((Camera.main.pixelWidth - Width) / 2, 10, Width, Height);
            GUI.Box(MenuRect, "", GUI.skin.window);
            CurrentTab = GUI.Toolbar(new Rect(MenuRect.xMin, MenuRect.yMin, MenuRect.width, ToolbarHeight), CurrentTab,
                new[]
                {
                    nameof(Values.Self),
                    "Chat",
                    "Voice",
                    "Players",
                    "Spawn",
                    "Sphere",
                    "Aura"
                },
                GUI.skin.GetStyle("Tabs"));

            Y = MenuRect.yMin + ToolbarHeight + Padding;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (CurrentTab)
            {
                #region Self

                case 0:
                {
                    Y -= Padding;
                    //GUI.Box(new Rect(MenuRect.xMin, Y - 1f, Width - 8f, Height - ToolbarHeight), "", GUI.skin.box);
                    Values.Self.ScrollPosition = GUI.BeginScrollView(new Rect(MenuRect.xMin, Y, Width, Height - ToolbarHeight - 5), Values.Self.ScrollPosition, new Rect(0, 0, Width - 24, Height * 2));

                    Y = Padding;

                    AddLabel("OPTIONS", increaseY: true, autoAlign: false);

                    AddLabel("InstantRevive:", autoAlign: false);
                    AddCheckBox(ref Values.Self.InstaRevive, increaseY: true, autoAlign: false);

                    AddLabel("Visible:", autoAlign: false);
                    AddCheckBox(ref Values.Self.Visible, increaseY: true, autoAlign: false);

                    AddLabel("Fire trail:", autoAlign: false);
                    AddCheckBox(ref Values.Self.FireTrail, increaseY: true, autoAlign: false);

                    AddLabel("God mode:", autoAlign: false);
                    AddCheckBox(ref Values.Self.GodMode, increaseY: true, autoAlign: false);

                    AddLabel("Fly mode:", autoAlign: false);
                    AddCheckBox(ref Values.Self.FlyMode, increaseY: true, autoAlign: false);

                    if (Values.Self.FlyMode)
                    {
                        AddLabel("Noclip:", autoAlign: false);
                        AddCheckBox(ref Values.Self.NoClip, increaseY: true, autoAlign: false);
                    }

                    AddLabel("InstantTree:", autoAlign: false);
                    AddCheckBox(ref Values.Self.InstantTree, increaseY: true, autoAlign: false);

                    AddLabel("InstantDestroy:", autoAlign: false);
                    AddCheckBox(ref Values.Self.InstaDestroy, increaseY: true, autoAlign: false);

                    AddLabel("Move speed:", autoAlign: false);
                    AddSlider(ref Values.Self.SpeedMultiplier, 1, 10, increaseY: true, autoAlign: false);

                    AddLabel("Jump power:", autoAlign: false);
                    AddSlider(ref Values.Self.JumpMultiplier, 1, 10, increaseY: true, autoAlign: false);

                    Y += Padding;
                    AddLabel("STATS", autoAlign: false);

                    const float checkBoxPosition = 360;
                    AddLabel("Fix", checkBoxPosition, increaseY: true, autoAlign: false);

                    AddStatSlider("Health:", ref Values.Stats.FixedHealth, ref LocalPlayer.Stats.Health, 0, 100, ref Values.Stats.FixHealth, checkBoxPosition, false);
                    AddStatSlider("Battery charge:", ref Values.Stats.FixedBatteryCharge, ref LocalPlayer.Stats.BatteryCharge, 0, 100, ref Values.Stats.FixBatteryCharge, checkBoxPosition, false);
                    AddStatSlider("Fullness:", ref Values.Stats.FixedFullness, ref LocalPlayer.Stats.Fullness, 0, 1, ref Values.Stats.FixFullness, checkBoxPosition, false);
                    AddStatSlider("Stamina:", ref Values.Stats.FixedStamina, ref LocalPlayer.Stats.Stamina, 0, 100, ref Values.Stats.FixStamina, checkBoxPosition, false);
                    AddStatSlider("Energy:", ref Values.Stats.FixedEnergy, ref LocalPlayer.Stats.Energy, 0, 100, ref Values.Stats.FixEnergy, checkBoxPosition, false);
                    AddStatSlider("Thirst:", ref Values.Stats.FixedThirst, ref LocalPlayer.Stats.Thirst, 0, 1, ref Values.Stats.FixThirst, checkBoxPosition, false);
                    AddStatSlider("Starvation:", ref Values.Stats.FixedStarvation, ref LocalPlayer.Stats.Starvation, 0, 1, ref Values.Stats.FixStarvation, checkBoxPosition, false);
                    AddStatSlider("Body temp:", ref Values.Stats.FixedBodyTemp, ref LocalPlayer.Stats.BodyTemp, 10, 60, ref Values.Stats.FixBodyTemp, checkBoxPosition, false);

                    GUI.EndScrollView();

                    break;
                }

                #endregion

                #region Chat

                case 1:
                {
                    AddLabel("Chat as...", increaseY: true);
                    AddRadioButtons(ref GriefClientPro.ChatManager.ChatAsValue, ChatManager.ChatAsNames, ChatManager.ChatAsCount, width: ChatManager.ChatAsCount * 100, increaseY: true);

                    if (GriefClientPro.ChatManager.CurrentChatAs == ChatManager.ChatAs.Self)
                    {
                        AddLabel("Chat as while invisible...", increaseY: true);
                        AddRadioButtons(ref GriefClientPro.ChatManager.ChatAsInvisibleValue, ChatManager.ChatAsInvisibleNames, ChatManager.ChatAsInvisibleCount,
                            width: ChatManager.ChatAsInvisibleCount * 100, increaseY: true);
                    }

                    AddLabel("Prefix options", increaseY: true);
                    AddCheckBox(ref GriefClientPro.ChatManager.UsePrefixWhenVisible, 20);
                    AddLabel("Visible", 40);
                    AddCheckBox(ref GriefClientPro.ChatManager.UsePrefixWhenInvisible, 100);
                    AddLabel("Invisible", 120);
                    AddLabel("Prefix: ", 190);
                    GriefClientPro.ChatManager.Prefix = GUI.TextField(new Rect(MenuRect.xMin + Padding + 235, Y, 50, 20), GriefClientPro.ChatManager.Prefix);
                    IncreaseY();

                    if (GriefClientPro.ChatManager.CurrentChatAs == ChatManager.ChatAs.Selected ||
                        GriefClientPro.ChatManager.CurrentChatAsInvisible == ChatManager.ChatAsInvisible.Selected)
                    {
                        // Validate current player
                        GriefClientPro.ChatManager.ValidatePlayer();

                        AddLabel("Select a player", increaseY: true);
                        if (GriefClientPro.PlayerManager.Players.Count == 0)
                        {
                            AddLabel("- none found -", 20, increaseY: true);
                        }
                        else
                        {
                            foreach (var player in GriefClientPro.PlayerManager.Players)
                            {
                                // Add checkbox
                                if (GUI.Toggle(new Rect(MenuRect.xMin + Padding + 20, Y, 20, 20), GriefClientPro.ChatManager.LastChattedAs?.SteamId == player.SteamId, ""))
                                {
                                    // Apply player
                                    GriefClientPro.ChatManager.LastChattedAs = player;
                                }

                                // Add player name
                                AddLabel(player.FriendlyName, 40, increaseY: true);
                            }
                        }
                    }

                    break;
                }

                #endregion

                #region Voice

                case 2:
                {
                    AddLabel("Sendng voice as...", increaseY: true);
                    AddRadioButtons(ref GriefClientPro.VoiceManager.ChatAsValue, VoiceManager.ChatAsNames, VoiceManager.ChatAsCount, width: VoiceManager.ChatAsCount * 100, increaseY: true);

                    if (GriefClientPro.VoiceManager.CurrentChatAs == VoiceManager.VoiceChatAs.Self)
                    {
                        AddLabel("Chat as while invisible...", increaseY: true);
                        AddRadioButtons(ref GriefClientPro.VoiceManager.ChatAsInvisibleValue, VoiceManager.ChatAsInvisibleNames, VoiceManager.ChatAsInvisibleCount,
                            width: VoiceManager.ChatAsInvisibleCount * 100, increaseY: true);
                    }

                    if (GriefClientPro.VoiceManager.CurrentChatAs == VoiceManager.VoiceChatAs.Selected ||
                        GriefClientPro.VoiceManager.CurrentChatAsInvisible == VoiceManager.VoiceChatAsInvisible.Selected)
                    {
                        // Validate current player
                        GriefClientPro.VoiceManager.ValidatePlayer();

                        AddLabel("Select a player", increaseY: true);
                        if (GriefClientPro.PlayerManager.Players.Count == 0)
                        {
                            AddLabel("- none found -", 20, increaseY: true);
                        }
                        else
                        {
                            foreach (var player in GriefClientPro.PlayerManager.Players)
                            {
                                // Add checkbox
                                if (GUI.Toggle(new Rect(MenuRect.xMin + Padding + 20, Y, 20, 20), GriefClientPro.VoiceManager.LastChattedAs?.SteamId == player.SteamId, ""))
                                {
                                    // Apply player
                                    GriefClientPro.VoiceManager.LastChattedAs = player;
                                }

                                // Add player name
                                AddLabel(player.FriendlyName, 40, increaseY: true);
                            }
                        }
                    }

                    break;
                }

                #endregion

                #region Players

                case 3:
                {
                    // Kill, Fire trail, teleport, trap, steam
                    AddLabel("Player Name");
                    AddLabel("Kill", 160);
                    AddLabel("Fire trail", 220, increaseY: true);

                    if (GriefClientPro.PlayerManager.Players.Count == 0)
                    {
                        AddLabel("- none found -", 20, increaseY: true);
                    }
                    else
                    {
                        foreach (var player in GriefClientPro.PlayerManager.Players)
                        {
                            // Add player name
                            AddLabel(player.FriendlyName);

                            if (player.SteamId > 0)
                            {
                                // Add kill checkbox
                                if (GUI.Toggle(new Rect(MenuRect.xMin + Padding + 160, Y, 20, 20), GriefClientPro.KillAllPlayers.PermaKillPlayers.Contains(player.SteamId), ""))
                                {
                                    GriefClientPro.KillAllPlayers.AddPlayerToPermaKill(player);
                                }
                                else
                                {
                                    GriefClientPro.KillAllPlayers.RemovePlayerToPermaKill(player);
                                }

                                // Add fire trail checkbox
                                if (GUI.Toggle(new Rect(MenuRect.xMin + Padding + 220, Y, 20, 20), LastFirePositions.ContainsKey(player.SteamId), ""))
                                {
                                    if (!LastFirePositions.ContainsKey(player.SteamId))
                                    {
                                        LastFirePositions.Add(player.SteamId, Vector3.zero);
                                    }
                                }
                                else
                                {
                                    LastFirePositions.Remove(player.SteamId);
                                }
                            }

                            // Add teleport button
                            if (AddButton("Teleport", 300, 75))
                            {
                                LocalPlayer.Transform.position = player.Position;
                            }

                            // Add trap extreme button
                            if (AddButton("Trap", 385, 60))
                            {
                                if (SpawnOffsets == null)
                                {
                                    SpawnOffsets = new List<Vector3>();

                                    const int size = 5;
                                    for (var x = -size; x < size; x++)
                                    {
                                        for (var y = -1; y < size / 2; y++)
                                        {
                                            for (var z = -size; z < size; z++)
                                            {
                                                SpawnOffsets.Add(new Vector3(x * 8, y * 8, z * 8));
                                            }
                                        }
                                    }

                                    // Order offsets closest first
                                    SpawnOffsets = SpawnOffsets.OrderBy(o => Vector3.Distance(Vector3.zero, o)).ThenBy(o => Vector3.Angle(Vector3.zero, o)).ToList();
                                }

                                foreach (var offset in SpawnOffsets)
                                {
                                    // Set the rotation of spawned objects facing the target player
                                    var rotation = Quaternion.LookRotation(new Vector3(offset.z, offset.y, -offset.x) - Vector3.zero);

                                    // Spawn the traps
                                    //BoltPrefabsHelper.Spawn(BoltPrefabs.Trap_TripWireExplosiveBuilt, position, rotation);
                                    BoltPrefabsHelper.Spawn(BoltPrefabs.Bomb_Timed_Placed, player.Position + offset, rotation);
                                }
                            }

                            // Add steam profile button
                            if (player.SteamId > 0 && AddButton("Steam", 455, 75))
                            {
                                SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(player.SteamId));
                            }

                            Y += StepY;
                        }
                    }

                    break;
                }

                #endregion

                #region Spawn

                case 4:
                {

                    break;
                }

                #endregion

                #region Sphere

                case 5:
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

        private List<Vector3> SpawnOffsets { get; set; }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            foreach (var player in
                GriefClientPro.PlayerManager.Players.Where(
                    player => LastFirePositions.ContainsKey(player.SteamId) &&
                              Vector3.Distance(LastFirePositions[player.SteamId], player.Position) > 2))
            {
                LastFirePositions[player.SteamId] = player.Position;
                BoltPrefabsHelper.Spawn(BoltPrefabs.Fire, player.Position - new Vector3(0, 4, 0), player.Transform.rotation);
            }
        }

        private static void OnKeyDown(KeyManager sender, KeyManager.KeyEventArgs args)
        {
            if (BoltNetwork.isRunning && args.Key == KeyManager.Keys.OpenMenu)
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

        private static void AddRadioButtons(ref int updateValue, string[] texts, int xCount, float x = Padding, float width = 250, float height = 20, bool increaseY = false, bool autoAlign = true)
        {
            updateValue = GUI.SelectionGrid(new Rect((autoAlign ? MenuRect.xMin : 0) + Padding + x, Y, width, height), updateValue, texts, xCount, GUI.skin.GetStyle("Button"));
            IncreaseY(increaseY);
        }

        private static void AddLabel(string text, float x = Padding, float width = 150, float height = 20, bool increaseY = false, bool autoAlign = true)
        {
            GUI.Label(new Rect((autoAlign ? MenuRect.xMin : 0) + Padding + x, Y, width, height), text, LabelStyle);
            IncreaseY(increaseY);
        }

        private static bool AddCheckBox(ref bool updateValue, float x = 160, float width = 20, float height = 30, bool increaseY = false, bool autoAlign = true)
        {
            return AddCheckBox(ref updateValue, string.Empty, x, width, height, increaseY, autoAlign);
        }

        private static bool AddCheckBox(ref bool updateValue, string text, float x = 160, float width = 20, float height = 30, bool increaseY = false, bool autoAlign = true)
        {
            updateValue = GUI.Toggle(new Rect((autoAlign ? MenuRect.xMin : 0) + Padding + x, Y, width, height), updateValue, text);
            IncreaseY(increaseY);
            return updateValue;
        }

        private static float AddSlider(ref float updateValue, float minValue, float maxValue, float x = 160, float width = 210, float height = 30, bool increaseY = false, bool autoAlign = true)
        {
            updateValue = GUI.HorizontalSlider(new Rect((autoAlign ? MenuRect.xMin : 0) + Padding + x, Y + 3f, width, height), updateValue, minValue, maxValue);
            IncreaseY(increaseY);
            return updateValue;
        }

        private static bool AddButton(string text, float x = Padding, float width = 180, float height = 20, bool increaseY = false, bool autoAlign = true)
        {
            var result = GUI.Button(new Rect((autoAlign ? MenuRect.xMin : 0) + Padding + x, Y, width, height), text);
            IncreaseY(increaseY);
            return result;
        }

        private static bool AddStatSlider(
            string text,
            ref float updateValueActive,
            ref float updateValueInactive,
            float min,
            float max,
            ref bool updateFixValue,
            float checkBoxPosition,
            bool autoAlign = true)
        {
            AddLabel(text, autoAlign: autoAlign);
            if (updateFixValue)
            {
                if (updateValueActive < 0)
                {
                    updateValueActive = updateValueInactive;
                }
                AddSlider(ref updateValueActive, min, max, width: 160, autoAlign: autoAlign);
            }
            else
            {
                updateValueActive = -1;
                AddSlider(ref updateValueInactive, min, max, width: 160, autoAlign: autoAlign);
            }
            AddLabel(Mathf.RoundToInt(updateFixValue ? updateValueActive : updateValueInactive).ToString(CultureInfo.InvariantCulture), checkBoxPosition - 30, 40, autoAlign: autoAlign);
            AddCheckBox(ref updateFixValue, checkBoxPosition, height: 20, increaseY: true, autoAlign: autoAlign);
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
