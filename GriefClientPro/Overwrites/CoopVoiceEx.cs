using System;
using System.Linq;
using GriefClientPro.Utils;
using Steamworks;
using UdpKit;

namespace GriefClientPro.Overwrites
{
    public class CoopVoiceEx : CoopVoice
    {
        public static string LastTalkedAs;

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

        public override void ReceiveVoiceData(byte[] packet, int o)
        {
            try
            {
                var length = Blit.ReadI32(packet, ref o);
                var numArray = new byte[length];
                Blit.ReadBytes(packet, ref o, numArray, 0, length);
                ReceiveVoiceData_Unpacked(numArray, length);
            }
            catch (Exception e)
            {
                Logger.Exception("Failed to receive voice data!", e);
            }
        }

        protected override void SendVoiceData(byte[] voice, int size, BoltConnection sendTo)
        {
            var entity = GetComponent<BoltEntity>();
            if (entity == null || !entity.isAttached)
            {
                if (BoltNetwork.entities.Count() > 1)
                {
                    BoltEntity abused = null;
                    try
                    {
                        var ownerName = SteamFriends.GetFriendPersonaName(CoopLobby.Instance.Info.OwnerSteamId);
                        foreach (var boltEntity in BoltNetwork.entities)
                        {
                            if (boltEntity.isAttached && boltEntity.StateIs<IPlayerState>() && boltEntity.GetState<IPlayerState>().name != ownerName)
                            {
                                abused = boltEntity;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Exception("Error while trying to get the user to abuse", e);
                    }

                    if (abused != null)
                    {
                        if (LastTalkedAs != abused.GetState<IPlayerState>().name)
                        {
                            LastTalkedAs = abused.GetState<IPlayerState>().name;
                            Logger.Info("Talking as {0}!", LastTalkedAs);
                        }

                        try
                        {
                            var num = 0;
                            var numArray = new byte[size + 12];
                            Blit.PackU64(numArray, ref num, abused.networkId.PackedValue);
                            Blit.PackI32(numArray, ref num, size);
                            Blit.PackBytes(numArray, ref num, voice, 0, size);
                            sendTo.StreamBytes(VoiceChannel, numArray);
                        }
                        catch (Exception e)
                        {
                            Logger.Exception("Failed to send voice data to host!", e);
                        }
                    }
                }

                // Return because we are not attached
                return;
            }

            base.SendVoiceData(voice, size, sendTo);
        }
    }
}
