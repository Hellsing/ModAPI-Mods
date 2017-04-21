using System;
using Bolt;
using GriefClientPro.Utils;
using Steamworks;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;

namespace GriefClientPro
{
    public class ChatManager
    {
        // Special default prefix for Hellsing: http://steamcommunity.com/id/singhell/
        public static string DefaultPrefix => SteamUser.GetSteamID().m_SteamID == 76561198008774571 ? "Ⓗ " : "<ツ> ";

        private GriefClientPro Instance { get; set; }

        public bool UsePrefix = true;
        public string Prefix = DefaultPrefix;

        public bool ChatAsSelf = true;
        public bool ChatAsRandom;
        public bool ChatAsSelected;
        public bool ChatInvisibleAsRandom = true;
        public bool ChatInvisibleAsSelected;

        public Player LastChattedAs { get; set; }

        public ChatManager(GriefClientPro instance)
        {
            // Apply properties
            Instance = instance;
        }

        public void ValidatePlayer()
        {
            if (LastChattedAs?.Entity == null || !LastChattedAs.Entity.isAttached)
            {
                RandomizePlayer();
            }
        }

        public void RandomizePlayer()
        {
            if (GriefClientPro.PlayerManager.Players.Count > 0)
            {
                LastChattedAs = GriefClientPro.PlayerManager.Players.Shuffle()[0];
            }
        }
    }

    public class ChatBoxEx : ChatBox
    {
        public override void OnSubmit()
        {
            if (GriefClientPro.ChatManager == null)
            {
                base.OnSubmit();
            }
            else
            {
                if (!string.IsNullOrEmpty(_input.value))
                {
                    try
                    {
                        if (!BoltNetwork.isRunning)
                        {
                            return;
                        }

                        // Check if player is detached
                        var attached = LocalPlayer.GameObject?.GetComponent<BoltEntity>()?.isAttached;

                        // Validate player
                        GriefClientPro.ChatManager.ValidatePlayer();

                        // Check if we need to randomize
                        if (GriefClientPro.ChatManager.ChatAsRandom ||
                            (GriefClientPro.ChatManager.ChatAsSelf && attached.HasValue && !attached.Value && GriefClientPro.ChatManager.ChatInvisibleAsRandom))
                        {
                            GriefClientPro.ChatManager.RandomizePlayer();

                            // Validate
                            if (GriefClientPro.ChatManager.LastChattedAs == null)
                            {
                                Logger.Warning("Player still null even after refresh (all invisible?)");
                                return;
                            }
                            if (!GriefClientPro.ChatManager.LastChattedAs.Entity.isAttached)
                            {
                                Logger.Warning("Random player is not attached (disconnecting?)");
                                return;
                            }
                        }

                        // Get the player to chat as
                        var player = GriefClientPro.ChatManager.LastChattedAs;
                        var senderNetworkId = GriefClientPro.ChatManager.ChatAsSelf && attached.HasValue && attached.Value ? LocalPlayer.Entity.networkId : player.NetworkId;

                        // Create the chat event
                        var chatEvent = ChatEvent.Create(GlobalTargets.OnlyServer);
                        chatEvent.Message = (GriefClientPro.ChatManager.UsePrefix ? GriefClientPro.ChatManager.Prefix : "") + _input.value;

                        // Define the sender
                        chatEvent.Sender = senderNetworkId;

                        // Send the message
                        PacketQueue.Add(chatEvent);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception("Exception while processing chat message to send!", e);
                    }

                    _input.value = null;
                }
                _mustClose = true;
                _lastInteractionTime = Time.time;
            }
        }
    }
}
