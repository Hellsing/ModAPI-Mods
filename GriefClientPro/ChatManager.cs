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

        public enum ChatAs
        {
            Self,
            Random,
            Selected
        }

        public enum ChatAsInvisible
        {
            Random,
            Selected
        }

        public static readonly string[] ChatAsNames = Enum.GetNames(typeof(ChatAs));
        public static readonly string[] ChatAsInvisibleNames = Enum.GetNames(typeof(ChatAsInvisible));

        public static readonly int ChatAsCount = ChatAsNames.Length;
        public static readonly int ChatAsInvisibleCount = ChatAsInvisibleNames.Length;

        public class ChatEventArgs : EventArgs
        {
            public bool CustomHandler { get; set; }
            public string Text { get; set; }
            public NetworkId SenderId { get; set; }
            public GlobalTargets Target { get; set; } = GlobalTargets.OnlyServer;
        }

        private GriefClientPro Instance { get; set; }

        public bool UsePrefixWhenVisible;
        public bool UsePrefixWhenInvisible = true;
        public string Prefix = DefaultPrefix;

        public int ChatAsValue = (int) ChatAs.Self;
        public int ChatAsInvisibleValue = (int) ChatAsInvisible.Random;

        public ChatAs CurrentChatAs => (ChatAs) ChatAsValue;
        public ChatAsInvisible CurrentChatAsInvisible => (ChatAsInvisible) ChatAsInvisibleValue;

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

        public void OnSubmit(ChatEventArgs chatEvent)
        {
            if (!string.IsNullOrEmpty(chatEvent.Text))
            {
                try
                {
                    if (!BoltNetwork.isRunning)
                    {
                        return;
                    }

                    // Check if player is detached
                    var isAttached = LocalPlayer.Entity?.isAttached ?? false;

                    // Validate player
                    ValidatePlayer();

                    // Check if we need to randomize
                    if (CurrentChatAs == ChatAs.Random || (CurrentChatAs == ChatAs.Self && !isAttached && CurrentChatAsInvisible == ChatAsInvisible.Random))
                    {
                        RandomizePlayer();

                        // Validate
                        if (LastChattedAs == null)
                        {
                            Logger.Warning("Player still null even after refresh (all invisible?)");
                            return;
                        }
                        if (!LastChattedAs.Entity.isAttached)
                        {
                            Logger.Warning("Random player is not attached (disconnecting?)");
                            return;
                        }
                    }

                    // Apply values
                    if (!isAttached)
                    {
                        chatEvent.Target = GlobalTargets.Everyone;
                    }
                    chatEvent.CustomHandler = true;
                    chatEvent.SenderId = CurrentChatAs == ChatAs.Self && isAttached ? LocalPlayer.Entity.networkId : LastChattedAs.NetworkId;
                    chatEvent.Text = ((isAttached && UsePrefixWhenVisible) || (!isAttached && UsePrefixWhenInvisible) ? GriefClientPro.ChatManager.Prefix : string.Empty) + chatEvent.Text;
                }
                catch (Exception e)
                {
                    Logger.Exception("Exception while processing chat message to send!", e);
                }
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
                return;
            }

            // Create local event
            var chatEvent = new ChatManager.ChatEventArgs
            {
                Text = _input.value
            };

            // Process the event
            GriefClientPro.ChatManager.OnSubmit(chatEvent);

            if (!chatEvent.CustomHandler || string.IsNullOrEmpty(chatEvent.Text))
            {
                base.OnSubmit();
            }
            else
            {
                // Create the chat event
                var @event = ChatEvent.Create(chatEvent.Target);
                @event.Message = chatEvent.Text;
                @event.Sender = chatEvent.SenderId;

                // Send the message
                PacketQueue.Add(@event);

                _input.value = null;
                _mustClose = true;
                _lastInteractionTime = Time.time;
            }
        }
    }
}
