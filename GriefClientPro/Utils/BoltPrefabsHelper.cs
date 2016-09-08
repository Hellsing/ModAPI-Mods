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
            var fields = typeof (BoltPrefabs).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
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

        public static BoltEntity Spawn(PrefabId prefabId, Transform transform)
        {
            return Spawn(prefabId, transform.position, transform.rotation);
        }

        public static BoltEntity Spawn(PrefabId prefabId, Vector3 position, Quaternion rotation)
        {
            if (prefabId == BoltPrefabs.Log)
            {
                var dropItem = DropItem.Create(GlobalTargets.OnlyServer);
                dropItem.PrefabId = prefabId;
                dropItem.Position = position;
                dropItem.Rotation = rotation;
                dropItem.Send();
                return null;
            }

            return BoltNetwork.Instantiate(prefabId, position, rotation);
        }
    }
}
