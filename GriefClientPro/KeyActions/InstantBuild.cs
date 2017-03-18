using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using TheForest.Buildings.Creation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GriefClientPro.KeyActions
{
    public class InstantBuild
    {
        public static InstantBuild Instance { get; private set; }

        public static int ActionsPerSecond { get; set; } = 100;

        public List<Craft_Structure> CurrentQueue { get; set; }
        public Dictionary<Craft_Structure, int> OverflowDictionary { get; set; }

        private int _lastAction;

        public InstantBuild(GriefClientPro instance)
        {
            // Initialize properties
            Instance = this;
            CurrentQueue = new List<Craft_Structure>();
            OverflowDictionary = new Dictionary<Craft_Structure, int>();

            // Listen to required events
            instance.OnTick += OnTick;
        }

        private void OnTick(object sender, EventArgs args)
        {
            // Remove old overflows
            if (OverflowDictionary.Count > 0)
            {
                foreach (var entry in OverflowDictionary.ToArray().Where(entry => Environment.TickCount - entry.Value > 5000))
                {
                    OverflowDictionary.Remove(entry.Key);
                }
            }

            if (ModAPI.Input.GetButton("InstantBuild"))
            {
                var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                ray.origin += Camera.main.transform.forward * 1f;
                var raycastHits = Physics.RaycastAll(ray, 100f);
                if (raycastHits.Length > 0)
                {
                    foreach (var structure in raycastHits.Select(raycastHit => raycastHit.collider.GetComponent<Craft_Structure>()).Where(structure => structure != null))
                    {
                        AddBlueprintToQueue(structure);
                    }
                }
            }

            if (CurrentQueue.Count > 0 && Environment.TickCount - _lastAction > 1f / ActionsPerSecond * 1000)
            {
                _lastAction = Environment.TickCount;
                InstantlyFinishBlueprint(CurrentQueue[0]);
                CurrentQueue.RemoveAt(0);
            }
        }

        public static void AddBlueprintToQueue(Craft_Structure structure)
        {
            if (Instance != null && !Instance.CurrentQueue.Contains(structure) && !Instance.OverflowDictionary.ContainsKey(structure))
            {
                Instance.OverflowDictionary.Add(structure, Environment.TickCount);
                Instance.CurrentQueue.Add(structure);
            }
        }

        public static void InstantlyFinishBlueprint(Craft_Structure structure)
        {
            try
            {
                if (structure._requiredIngredients.Count > 0)
                {
                    for (var itemNumber = 0; itemNumber < structure._requiredIngredients.Count; itemNumber++)
                    {
                        var requiredItems = structure._requiredIngredients[itemNumber];
                        if (structure.GetPresentIngredients().Length > itemNumber)
                        {
                            var presentItems = structure.GetPresentIngredients()[itemNumber];

                            if (presentItems._amount >= requiredItems._amount)
                            {
                                continue;
                            }

                            for (var i = 0; i < requiredItems._amount - presentItems._amount; i++)
                            {
                                if (BoltNetwork.isRunning)
                                {
                                    var addIngredient = AddIngredient.Create(GlobalTargets.OnlyServer);
                                    addIngredient.IngredientNum = itemNumber;
                                    addIngredient.ItemId = requiredItems._itemID;
                                    addIngredient.Construction = structure.entity;
                                    PacketQueue.Add(addIngredient);
                                }
                                else
                                {
                                    structure.AddIngrendient_Actual(itemNumber, true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void AddAllBlueprintsToQueue()
        {
            var structures = Object.FindObjectsOfType<Craft_Structure>();
            if (structures != null && structures.Length > 0)
            {
                foreach (var structure in structures)
                {
                    AddBlueprintToQueue(structure);
                }
            }
        }

        public static void Clear()
        {
            Instance?.CurrentQueue.Clear();
        }
    }
}
