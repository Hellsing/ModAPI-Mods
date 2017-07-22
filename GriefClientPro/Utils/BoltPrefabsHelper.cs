using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bolt;
using UnityEngine;

namespace GriefClientPro.Utils
{
    public class BoltPrefabsHelper
    {
        private static readonly Dictionary<string, PrefabId> BoltPrefabsDictionary = new Dictionary<string, PrefabId>();

        public static Dictionary<string, PrefabId> Prefabs => new Dictionary<string, PrefabId>(BoltPrefabsDictionary);

        static BoltPrefabsHelper()
        {
            LoadBoltPrefabs();
        }

        private static void LoadBoltPrefabs()
        {
            var sortedDictionary = new SortedDictionary<string, PrefabId>();
            var fields = typeof(BoltPrefabs).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields.Where(fieldInfo => fieldInfo.Name != "player_net"))
            {
                sortedDictionary.Add(fieldInfo.Name, (PrefabId) fieldInfo.GetValue(null));
            }
            BoltPrefabsDictionary.Clear();
            BoltPrefabsDictionary.Add("Fake Player", BoltPrefabs.player_net);
            foreach (var current in sortedDictionary)
            {
                BoltPrefabsDictionary.Add(current.Key, current.Value);
            }
        }

        public static void Spawn(PrefabId prefabId, Transform transform)
        {
            Spawn(prefabId, transform.position, transform.rotation);
        }

        public static void Spawn(PrefabId prefabId, Vector3 position, Quaternion rotation)
        {
            if (prefabId == BoltPrefabs.Log)
            {
                var dropItem = DropItem.Create(GlobalTargets.OnlyServer);
                dropItem.PrefabId = prefabId;
                dropItem.Position = position;
                dropItem.Rotation = rotation;
                PacketQueue.Add(dropItem);
                return;
            }

            BoltNetwork.Instantiate(prefabId, position, rotation);
        }

        /* From BoltNetwork
        public static BoltEntity Instantiate(GameObject prefab, IProtocolToken token, Vector3 position, Quaternion rotation)
        {
            BoltEntity component = prefab.GetComponent<BoltEntity>();
            if (!(bool) ((UnityEngine.Object) component))
                return (BoltEntity) null;
            if (component.serializerGuid == UniqueId.None)
                return (BoltEntity) null;
            return BoltCore.Instantiate(new PrefabId(component._prefabId), Factory.GetFactory(component.serializerGuid).TypeId, position, rotation, InstantiateFlags.ZERO, (BoltConnection) null, token);
        }
        */

        /* From BoltCore
        internal static BoltEntity Instantiate(PrefabId prefabId, TypeId serializerId, Vector3 position, Quaternion rotation, InstantiateFlags instanceFlags, BoltConnection controller, IProtocolToken attachToken)
        {
            BoltEntity component = BoltCore.PrefabPool.LoadPrefab(prefabId).GetComponent<BoltEntity>();
            if (BoltCore.isClient && !component._allowInstantiateOnClient)
                throw new BoltException("This prefab is not allowed to be instantiated on clients");
            if (component._prefabId != prefabId.Value)
                throw new BoltException("PrefabId for BoltEntity component did not return the same value as prefabId passed in as argument to Instantiate");
            Entity @for = Entity.CreateFor(prefabId, serializerId, position, rotation);
            @for.Initialize();
            @for.AttachToken = attachToken;
            @for.Attach();
            return @for.UnityObject;
        }
        */
    }
}
