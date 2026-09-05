using Dota2GSI.EventMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dota2GSI
{
    public class EventDispatcher<T> where T : BaseEvent
    {
        /// <summary>
        /// Delegate for handing game events.
        /// </summary>
        /// <param name="game_event">The new game event.</param>
        public delegate void GameEventHandler(T game_event);

        /// <summary>
        /// Event for handing game events.
        /// </summary>
        public event GameEventHandler GameEvent = delegate { };

        private readonly object subscriptions_lock = new object();

        private Dictionary<Type, HashSet<Action<T>>> subscriptions = new Dictionary<Type, HashSet<Action<T>>>();
        private Dictionary<Type, HashSet<Func<T, T>>> pre_processors = new Dictionary<Type, HashSet<Func<T, T>>>();

        // Dedup: bounded recent-key cache to prevent double-firing from multiple listeners
        private const int DedupWindowSize = 50;
        private readonly HashSet<string> _recentKeys = new HashSet<string>();
        private readonly Queue<string> _keyQueue = new Queue<string>();

        public EventDispatcher()
        {
            Subscribe<T>(RaiseOnGameEventHandler);
        }

        ~EventDispatcher()
        {
            Unsubscribe<T>(RaiseOnGameEventHandler);
        }

        private string ExtractPlayerIds(T gameEvent)
        {
            // Try common player ID fields across event types
            var ids = new List<string>();

            // Try Player property (FullPlayerDetails with PlayerID)
            var playerProp = gameEvent.GetType().GetProperty("Player");
            if (playerProp != null)
            {
                var player = playerProp.GetValue(gameEvent);
                if (player != null)
                {
                    var playerIdProp = player.GetType().GetProperty("PlayerID");
                    if (playerIdProp != null)
                    {
                        ids.Add(playerIdProp.GetValue(player)?.ToString() ?? "-1");
                    }
                }
            }

            // Try KillerPlayerID
            var killerProp = gameEvent.GetType().GetProperty("KillerPlayerID");
            if (killerProp != null)
            {
                var killerId = killerProp.GetValue(gameEvent);
                if (killerId != null && !ids.Contains(killerId.ToString()))
                {
                    ids.Add(killerId.ToString());
                }
            }

            // Try EntityID
            var entityProp = gameEvent.GetType().GetProperty("EntityID");
            if (entityProp != null)
            {
                var entityId = entityProp.GetValue(gameEvent);
                if (entityId != null && !ids.Contains(entityId.ToString()))
                {
                    ids.Add(entityId.ToString());
                }
            }

            return string.Join("|", ids.ToArray());
        }

        private string BuildDedupKey<MessageType>(MessageType message) where MessageType : T
        {
            string eventTypeName = typeof(MessageType).Name;
            string playerIds = ExtractPlayerIds(message);
            return $"{eventTypeName}|{playerIds}";
        }

        private void RaiseOnGameEventHandler(T game_event)
        {
            foreach (Delegate d in GameEvent.GetInvocationList())
            {
                if (d.Target is ISynchronizeInvoke)
                {
                    (d.Target as ISynchronizeInvoke).BeginInvoke(d, new object[] { game_event });
                }
                else
                {
                    d.DynamicInvoke(game_event);
                }
            }
        }

        public void RegisterPreProcessor<MessageType>(Func<T, T> callback) where MessageType : T
        {
            var event_type = typeof(MessageType);

            lock (subscriptions_lock)
            {
                if (!pre_processors.ContainsKey(event_type))
                {
                    pre_processors.Add(event_type, new HashSet<Func<T, T>>());
                }

                pre_processors[event_type].Add(callback);
            }
        }

        public void UnregisterPreProcessor<MessageType>(Func<T, T> callback) where MessageType : T
        {
            var event_type = typeof(MessageType);

            lock (subscriptions_lock)
            {
                if (!subscriptions.ContainsKey(event_type))
                {
                    return;
                }

                pre_processors[event_type].Remove(callback);
            }
        }


        public void Subscribe<MessageType>(Action<T> callback) where MessageType : T
        {
            var event_type = typeof(MessageType);

            lock (subscriptions_lock)
            {
                if (!subscriptions.ContainsKey(event_type))
                {
                    subscriptions.Add(event_type, new HashSet<Action<T>>());
                }

                subscriptions[event_type].Add(callback);
            }
        }

        public void Unsubscribe<MessageType>(Action<T> callback) where MessageType : T
        {
            var event_type = typeof(MessageType);

            lock (subscriptions_lock)
            {
                if (!subscriptions.ContainsKey(event_type))
                {
                    return;
                }

                subscriptions[event_type].Remove(callback);
            }
        }

        public void Broadcast<MessageType>(MessageType message) where MessageType : T
        {
            var event_type = typeof(MessageType);
            T msg = message;

            lock (subscriptions_lock)
            {
                // --- Dedup check ---
                string dedupKey = BuildDedupKey(message);
                if (_recentKeys.Contains(dedupKey))
                {
                    // Same key seen within window — skip dispatch to avoid double-fire
                    return;
                }
                _recentKeys.Add(dedupKey);
                _keyQueue.Enqueue(dedupKey);
                if (_keyQueue.Count > DedupWindowSize)
                {
                    string oldKey = _keyQueue.Dequeue();
                    _recentKeys.Remove(oldKey);
                }
                // ----------------

                if (subscriptions.ContainsKey(event_type))
                {
                    // Run pre-processors first
                    if (pre_processors.ContainsKey(event_type))
                    {
                        foreach (var pre_processor in pre_processors[event_type])
                        {
                            msg = pre_processor(msg);

                            if (msg == null)
                            {
                                // The message was handled.
                                return;
                            }
                        }
                    }

                    foreach (var callback in subscriptions[event_type])
                    {
                        callback.Invoke(msg);
                    }
                }

                if (event_type != typeof(T))
                {
                    Broadcast(msg);
                }
            }
        }
    }
}
