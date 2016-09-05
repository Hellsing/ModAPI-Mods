using System;
using System.Linq;
using Bolt;
using GriefClientPro.Utils;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;

namespace GriefClientPro.Overwrites
{
    public class ChatBoxEx : ChatBox
    {
        private System.Random _random;
        private System.Random Random => _random ?? (_random = new System.Random(DateTime.Now.Millisecond));

        public override void OnSubmit()
        {
            if (!string.IsNullOrEmpty(_input.value))
            {
                Logger.Info("Trying to submit: {0}", _input.value);

                try
                {
                    if (!BoltNetwork.isRunning)
                    {
                        return;
                    }

                    // Check if player is detached
                    var detached = LocalPlayer.GameObject != null && LocalPlayer.GameObject.GetComponent<BoltEntity>() != null && !LocalPlayer.GameObject.GetComponent<BoltEntity>().isAttached;
                    if (detached)
                    {
                        // Pick a random player on the server
                        var player = Players.Keys.ToArray()[Random.Next(Players.Count)];

                        // Write with player name
                        var chatEvent = ChatEvent.Create(GlobalTargets.OnlyServer);
                        chatEvent.Message = "<†> " + _input.value;
                        chatEvent.Sender = player;
                        chatEvent.Send();
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
