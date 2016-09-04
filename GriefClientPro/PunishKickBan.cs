using GriefClientPro.KeyActions;

namespace GriefClientPro
{
    public class PunishKickBan : CoopClientCallbacks
    {
        public override void Disconnected(BoltConnection connection)
        {
            // Reset values
            Menu.Values.Other.FreeCam = false;
            Menu.Values.Player.NoClip = false;
            Menu.Values.Player.FlyMode = false;
            Menu.Values.Player.SpeedMultiplier = 1;
            Menu.Values.Player.JumpMultiplier = 1;

            // Handle kick/ban
            if (connection.DisconnectToken != null)
            {
                var token = connection.DisconnectToken;
                if (token.GetType().Name == "CoopKickToken")
                {
                    /*
                    var fieldInfo = token.GetType().GetField("Banned", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                    var banned = fieldInfo != null && (bool) fieldInfo.GetValue(token);
                    
                    if (banned)
                    {
                        CoopKick.Client_Banned = true;
                        CoopKick.Client_KickMessage = "Host is jelly, you got banned q.q";
                    }
                    else
                    {
                        CoopKick.Client_Banned = false;
                        CoopKick.Client_KickMessage = "Host is jelly, you got kicked q.q";
                    }
                    */

                    // Punishment
                    DestroyBuildings.Execute();

                    // TODO: Don't return here because I need to trigger base method cuz I am too dumb to get the Banned field of the token...
                    //return;
                }
            }

            // No kick/ban, regular disconnect by user
            base.Disconnected(connection);

            // TODO: Remove once above is fixed
            if (!string.IsNullOrEmpty(CoopKick.Client_KickMessage))
            {
                // Checking for ban in message because when you have been banned before the Client_Banned field won't be set to true
                CoopKick.Client_KickMessage = CoopKick.Client_Banned || CoopKick.Client_KickMessage.ToLower().Contains("ban") ? "Salty host, you got banned q.q" : "Host is jelly, you got kicked q.q";
            }
        }
    }
}
