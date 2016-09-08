using System;
using System.Linq;
using GriefClientPro.Utils;
using Steamworks;
using TheForest.Utils;
using UdpKit;

namespace GriefClientPro.Overwrites
{
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
                            if (LocalPlayer.GameObject.GetComponent<BoltEntity>() != null && !LocalPlayer.GameObject.GetComponent<BoltEntity>().isAttached)
                            {
                                Logger.Info("Sending voice data while invisible!");
                            }
                            SendVoiceData(vc_cmp, (int) nBytesWritten, BoltNetwork.server);
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
                foreach (var boltEntity in BoltNetwork.entities)
                {
                    try
                    {
                        var num = 0;
                        var numArray = new byte[size + 12];
                        Blit.PackU64(numArray, ref num, boltEntity.networkId.PackedValue);
                        Blit.PackI32(numArray, ref num, size);
                        Blit.PackBytes(numArray, ref num, voice, 0, size);
                        sendTo.StreamBytes(VoiceChannel, numArray);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to send voice data!", ex);
                    }
                }
                return;
            }

            base.SendVoiceData(voice, size, sendTo);
        }
    }
}
