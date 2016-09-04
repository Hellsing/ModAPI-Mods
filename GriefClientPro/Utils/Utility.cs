using BoltInternal;
using TheForest.Utils;

namespace GriefClientPro.Utils
{
    public static class Utility
    {
        public static BoltEntity AttachLocalPlayer(string name = null)
        {
            if (LocalPlayer.GameObject.GetComponent<BoltEntity>() != null && LocalPlayer.GameObject.GetComponent<BoltEntity>().isAttached)
            {
                return LocalPlayer.GameObject.GetComponent<BoltEntity>();
            }
            LocalPlayer.GameObject.AddComponent<BoltPlayerSetup>();
            LocalPlayer.GameObject.AddComponent<BoltEntity>();
            using (var settingsModifier = LocalPlayer.GameObject.GetComponent<BoltEntity>().ModifySettings())
            {
                settingsModifier.prefabId = BoltPrefabs.player_net;
                settingsModifier.serializerId = StateSerializerTypeIds.IPlayerState;
                settingsModifier.allowInstantiateOnClient = true;
                settingsModifier.persistThroughSceneLoads = true;
                settingsModifier.clientPredicted = false;
                settingsModifier.updateRate = 1;
            }
            var attachedEntity = BoltNetwork.Attach(LocalPlayer.GameObject).GetComponent<BoltEntity>();
            attachedEntity.GetState<IPlayerState>().name = !string.IsNullOrEmpty(name) ? name : GriefClientPro.PlayerName;
            LocalPlayer.Entity = attachedEntity;
            BoltNetwork.SetCanReceiveEntities(true);
            return attachedEntity;
        }

        public static void DetachLocalPlayer()
        {
            if (LocalPlayer.Entity != null && LocalPlayer.Entity.isAttached)
            {
                BoltNetwork.Detach(LocalPlayer.Entity);
            }
        }
    }
}
