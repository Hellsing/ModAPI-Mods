using System;
using System.Collections.Generic;
using System.Timers;
using Bolt;
using GriefClientPro.Utils;

namespace GriefClientPro
{
    public static class PacketQueue
    {
        private static readonly List<Event> CurrentQueue = new List<Event>();

        static PacketQueue()
        {
            var timer = new Timer(200);
            timer.Elapsed += OnElapsed;
            timer.Start();
        }

        private static void OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (CurrentQueue)
            {
                if (CurrentQueue.Count > 0)
                {
                    var packets = CurrentQueue.GetRange(0, Math.Min(100, CurrentQueue.Count));
                    CurrentQueue.RemoveRange(0, Math.Min(100, CurrentQueue.Count));

                    Logger.Info("Sending {0} packet(s)...", packets.Count);

                    foreach (var packet in packets)
                    {
                        try
                        {
                            packet.Send();
                        }
                        catch (Exception e)
                        {
                            Logger.Exception("Error while trying to send packet!", e);
                        }
                    }
                }
            }
        }

        public static void Add(Event packet)
        {
            lock (CurrentQueue)
            {
                CurrentQueue.Add(packet);
            }
        }
    }
}
