using System;
using System.Linq;
using Bolt;
using GriefClientPro.Utils;
using Steamworks;
using TheForest.Utils;
using UdpKit;

namespace GriefClientPro
{
    public class VoiceManager
    {
        public enum VoiceChatAs
        {
            Self,
            Selected
        }

        public enum VoiceChatAsInvisible
        {
            Selected
        }

        public static readonly string[] ChatAsNames = Enum.GetNames(typeof(VoiceChatAs));
        public static readonly string[] ChatAsInvisibleNames = Enum.GetNames(typeof(VoiceChatAsInvisible));

        public static readonly int ChatAsCount = ChatAsNames.Length;
        public static readonly int ChatAsInvisibleCount = ChatAsInvisibleNames.Length;

        public class VoiceChatEventArgs : EventArgs
        {
            public bool CustomHandler { get; set; }
            public NetworkId SenderId { get; set; }
        }

        private GriefClientPro Instance { get; set; }

        public int ChatAsValue = (int) VoiceChatAs.Self;
        public int ChatAsInvisibleValue = (int) VoiceChatAsInvisible.Selected;

        public VoiceChatAs CurrentChatAs => (VoiceChatAs) ChatAsValue;
        public VoiceChatAsInvisible CurrentChatAsInvisible => (VoiceChatAsInvisible) ChatAsInvisibleValue;

        public Player LastChattedAs { get; set; }

        public VoiceManager(GriefClientPro instance)
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

        public void ProcessVoiceSender(VoiceChatEventArgs chatEvent)
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

                // Apply values
                chatEvent.CustomHandler = true;
                chatEvent.SenderId = CurrentChatAs == VoiceChatAs.Self && isAttached ? LocalPlayer.Entity.networkId : LastChattedAs.NetworkId;
            }
            catch (Exception e)
            {
                Logger.Exception("Exception while processing chat message to send!", e);
            }
        }
    }

    public class CoopVoiceEx : CoopVoice
    {
        protected override void Update()
        {
            try
            {
                if (!IsLocal)
                {
                    return;
                }
                if (GetComponentInParent<FirstPersonCharacter>() && WalkieTalkie.gameObject.activeInHierarchy)
                {
                    if (recording)
                    {
                        uint nBytesWritten;
                        uint nUncompressBytesWritten;
                        if (SteamUser.GetVoice(true, vc_cmp, 65536U, out nBytesWritten, false, null, 0U, out nUncompressBytesWritten, 0U) != EVoiceResult.k_EVoiceResultOK || nBytesWritten <= 0U)
                        {
                            return;
                        }
                        if (BoltNetwork.isServer)
                        {
                            ForwardVoiceData(vc_cmp, (int) nBytesWritten);
                        }
                        else
                        {
                            try
                            {
                                SendVoiceData(vc_cmp, (int) nBytesWritten, BoltNetwork.server);
                            }
                            catch (Exception e)
                            {
                                Logger.Exception("SendVoiceData failed!", e);
                                base.SendVoiceData(vc_cmp, (int) nBytesWritten, BoltNetwork.server);
                            }
                        }
                    }
                    else
                    {
                        SteamUser.StartVoiceRecording();
                        SteamFriends.SetInGameVoiceSpeaking(SteamUser.GetSteamID(), true);
                        recording = true;
                    }
                }
                else
                {
                    if (!recording)
                    {
                        return;
                    }
                    recording = false;
                    SteamFriends.SetInGameVoiceSpeaking(SteamUser.GetSteamID(), false);
                    SteamUser.StopVoiceRecording();
                }
            }
            catch (Exception e)
            {
                Logger.Exception("Failed to update CoopVoice", e);
            }
        }

        protected override void SendVoiceData(byte[] voice, int size, BoltConnection sendTo)
        {
            if (GriefClientPro.VoiceManager == null)
            {
                base.SendVoiceData(voice, size, sendTo);
                return;
            }

            // Create local event
            var chatEvent = new VoiceManager.VoiceChatEventArgs();

            // Process the event
            GriefClientPro.VoiceManager.ProcessVoiceSender(chatEvent);

            if (!chatEvent.CustomHandler)
            {
                base.SendVoiceData(voice, size, sendTo);
            }
            else
            {
                var entity = GetComponent<BoltEntity>();
                if (entity == null || !entity.isAttached)
                {
                    if (BoltNetwork.entities.Count() > 1)
                    {
                        var abused = chatEvent.SenderId;
                        try
                        {
                            var num = 0;
                            var numArray = new byte[size + 12];
                            Blit.PackU64(numArray, ref num, abused.PackedValue);
                            Blit.PackI32(numArray, ref num, size);
                            Blit.PackBytes(numArray, ref num, voice, 0, size);
                            sendTo.StreamBytes(VoiceChannel, numArray);
                        }
                        catch (Exception e)
                        {
                            Logger.Exception("Failed to send voice data to host!", e);
                        }
                    }

                    // Return because we are not attached
                    return;
                }

                base.SendVoiceData(voice, size, sendTo);
            }
        }
    }
}
