using System;
using System.Collections.Generic;
using System.Linq;
using GriefClientPro.Utils;
using UnityEngine;

namespace GriefClientPro
{
    public class KeyManager
    {
        public enum Keys
        {
            SphereAction,
            OpenMenu,
            InstantBuild,
            FreezeTime,
            FreeCam,
            DestroyEverything,
            Aura
        }

        public class KeyEventArgs : EventArgs
        {
            public Keys Key { get; private set; }
            public KeyCode KeyCode { get; private set; }

            public KeyEventArgs(Keys key)
            {
                Key = key;
                if (ModAPI.Input.KeyMapping.ContainsKey(key.ToString()))
                {
                    KeyCode = ModAPI.Input.KeyMapping[key.ToString()];
                }
            }
        }

        public delegate void KeyHandler(KeyManager sender, KeyEventArgs args);

        public event KeyHandler OnKeyDown;
        public event KeyHandler OnKeyUp;

        private static HashSet<Keys> KeysDown { get; } = new HashSet<Keys>();

        public KeyManager(GriefClientPro instance)
        {
            // Listen to required events
            instance.OnTick += OnTick;
        }

        private void OnTick(object sender, EventArgs args)
        {
            // Check keys
            foreach (var key in Enum.GetValues(typeof (Keys)).Cast<Keys>())
            {
                var keyDown = ModAPI.Input.GetButton(key.ToString());
                if (keyDown && !KeysDown.Contains(key))
                {
                    KeysDown.Add(key);

                    // Notify listeners for key down
                    if (OnKeyDown != null)
                    {
                        foreach (var action in OnKeyDown.GetInvocationList())
                        {
                            try
                            {
                                action.DynamicInvoke(this, new KeyEventArgs(key));
                            }
                            catch (Exception e)
                            {
                                Logger.Exception("Exception while notifying OnKeyDown listener: " + action.GetType().Name, e);
                            }
                        }
                    }
                }
                else if (!keyDown && KeysDown.Contains(key))
                {
                    KeysDown.Remove(key);

                    // Notify listeners for key up
                    if (OnKeyUp != null)
                    {
                        foreach (var action in OnKeyUp.GetInvocationList())
                        {
                            try
                            {
                                action.DynamicInvoke(this, new KeyEventArgs(key));
                            }
                            catch (Exception e)
                            {
                                Logger.Exception("Exception while notifying OnKeyDown listener: " + action.GetType().Name, e);
                            }
                        }
                    }
                }
            }
        }
    }
}
