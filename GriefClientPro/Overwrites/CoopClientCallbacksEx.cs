namespace GriefClientPro.Overwrites
{
    public class CoopClientCallbacksEx : CoopClientCallbacks
    {
        public override void Disconnected(BoltConnection connection)
        {
            // Reset values
            Menu.Values.Other.FreeCam = false;
            Menu.Values.Self.Visible = false;
            Menu.Values.Self.NoClip = false;
            Menu.Values.Self.FlyMode = false;
            Menu.Values.Self.SpeedMultiplier = 1;
            Menu.Values.Self.JumpMultiplier = 1;

            base.Disconnected(connection);
        }
    }
}
