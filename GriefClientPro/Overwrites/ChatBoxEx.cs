using System;
using System.Linq;
using Bolt;
using GriefClientPro.Utils;
using Steamworks;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;

namespace GriefClientPro.Overwrites
{
    public class ChatBoxEx : ChatBox
    {
        public static string Prefix => SteamUser.GetSteamID().m_SteamID == 76561198008774571 ? "Ⓗ " : "<ツ> ";

        public override void OnSubmit()
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
                    var detached = LocalPlayer.GameObject?.GetComponent<BoltEntity>()?.isAttached;
                    if (detached.HasValue && !detached.Value)
                    {
                        // Pick a random player on the server
                        var player = Players.Keys.ToList().Shuffle()[0];

                        // Write with player name
                        var chatEvent = ChatEvent.Create(GlobalTargets.Others);
                        chatEvent.Message = Prefix + _input.value;
                        chatEvent.Sender = player;
                        chatEvent.Send();

                        // Display locally
                        AddLine(player, chatEvent.Message, false);
                    }
                    else
                    {
                        // Regular text sending
                        SendLine(_input.value);
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception("Exception while trying to send message!", e);
                }

                _input.value = null;
            }
            _mustClose = true;
            _lastInteractionTime = Time.time;
        }
    }
}
