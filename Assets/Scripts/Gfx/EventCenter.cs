using System;
using System.Collections.Generic;
using Cloth3D.Ui;
using UnityEngine;

namespace Cloth3D {
/*
    modify by danie,use messgeId instead of string  //TODO check add and send msg type in debug mode.
* Advanced C# messenger by Ilya Suzdalnitski. V1.0 
* 
* Based on Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
* 
* Features:
* Prevents a MissingReferenceException because of a reference to a destroyed message handler.
* Option to log all messages
* Extensive error detection, preventing silent bugs
* 
* Usage examples:
1. EventCenter.AddListener<GameObject>("prop collected", PropCollected);
EventCenter.Broadcast<GameObject>("prop collected", prop);
2. EventCenter.AddListener<float>("speed changed", SpeedChanged);
EventCenter.Broadcast<float>("speed changed", 0.5f);
* 
* EventCenter cleans up its evenTable automatically upon loading of a new level.
* 
* Don't forget that the messages that should survive the cleanup, should be marked with EventCenter.MarkAsPermanent(string)
* 
*/

    //#define LOG_ALL_MESSAGES
    //#define LOG_ADD_LISTENER
    //#define LOG_BROADCAST_MESSAGE
    public class EventCenter:Singleton<EventCenter> {
        private Dictionary<EventId, Delegate> _eventTable = new Dictionary<EventId, Delegate>();

        public void Cleanup() {
            _eventTable.Clear();
        }

        public void PrintEventTable() {
          LogSystem.Debug("\t\t\t=== MESSENGER PrintEventTable ===");

            foreach (var pair in _eventTable) {
                LogSystem.Debug("\t\t\t{0}\t\t{1}",pair.Key,pair.Value);
            }

           LogSystem.Debug("\n");
        }

        public void OnListenerAdding(EventId eventIdType, Delegate listenerBeingAdded) {
            if (!_eventTable.ContainsKey(eventIdType)) {
                _eventTable.Add(eventIdType, null);
            }

            var d = _eventTable[eventIdType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
                throw new ListenerException(
                    string.Format(
                        "Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}",
                        eventIdType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        public void OnListenerRemoving(EventId eventIdType, Delegate listenerBeingRemoved) {
            if (_eventTable.ContainsKey(eventIdType)) {
                var d = _eventTable[eventIdType];

                if (d == null) {
                    throw new ListenerException(
                        string.Format(
                            "Attempting to remove listener with for event type \"{0}\" but current listener is null.",
                            eventIdType));
                }
                if (d.GetType() != listenerBeingRemoved.GetType()) {
                    throw new ListenerException(
                        string.Format(
                            "Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}",
                            eventIdType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            } else {
                throw new ListenerException(
                    string.Format(
                        "Attempting to remove listener for type \"{0}\" but EventCenter doesn't know about this event type.",
                        eventIdType));
            }
        }

        public void OnListenerRemoved(EventId eventIdType) {
            if (_eventTable[eventIdType] == null) {
                _eventTable.Remove(eventIdType);
            }
        }

        public void OnBroadcasting(EventId eventIdType) {
        }

        public BroadcastException CreateBroadcastSignatureException(EventId eventIdType) {
            return
                new BroadcastException(
                    string.Format(
                        "Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.",
                        eventIdType));
        }

        //No parameters
        public void Broadcast(EventId eventIdType) {
            OnBroadcasting(eventIdType);

            Delegate d;
            if (_eventTable.TryGetValue(eventIdType, out d)) {
                var callback = d as MyAction;

                if (callback != null) {
                    callback();
                } else {
                    throw CreateBroadcastSignatureException(eventIdType);
                }
            }
        }

        //Single parameter
        public void Broadcast<T>(EventId eventIdType, T arg1) {
            OnBroadcasting(eventIdType);

            Delegate d;
            if (_eventTable.TryGetValue(eventIdType, out d)) {
                var callback = d as MyAction<T>;

                if (callback != null) {
                    callback(arg1);
                } else {
                    throw CreateBroadcastSignatureException(eventIdType);
                }
            }
        }

        //Two parameters
        public void Broadcast<T, TU>(EventId eventIdType, T arg1, TU arg2) {
            OnBroadcasting(eventIdType);

            Delegate d;
            if (_eventTable.TryGetValue(eventIdType, out d)) {
                var callback = d as MyAction<T, TU>;

                if (callback != null) {
                    callback(arg1, arg2);
                } else {
                    throw CreateBroadcastSignatureException(eventIdType);
                }
            }
        }

        //Three parameters
        public void Broadcast<T, TU, TV>(EventId eventIdType, T arg1, TU arg2, TV arg3) {
            OnBroadcasting(eventIdType);

            Delegate d;
            if (_eventTable.TryGetValue(eventIdType, out d)) {
                var callback = d as MyAction<T, TU, TV>;

                if (callback != null) {
                    callback(arg1, arg2, arg3);
                } else {
                    throw CreateBroadcastSignatureException(eventIdType);
                }
            }
        }

        //Four parameters
        public void Broadcast<T, TU, TV,TS>(EventId eventIdType, T arg1, TU arg2, TV arg3,TS arg4) {
            OnBroadcasting(eventIdType);

            Delegate d;
            if (_eventTable.TryGetValue(eventIdType, out d)) {
                var callback = d as MyAction<T, TU, TV,TS>;

                if (callback != null) {
                    callback(arg1, arg2, arg3,arg4);
                } else {
                    throw CreateBroadcastSignatureException(eventIdType);
                }
            }
        }

        //Five parameters
        public void Broadcast<T, TU, TV, TS,TW>(EventId eventIdType, T arg1, TU arg2, TV arg3, TS arg4,TW arg5) {
            OnBroadcasting(eventIdType);

            Delegate d;
            if (_eventTable.TryGetValue(eventIdType, out d)) {
                var callback = d as MyAction<T, TU, TV, TS, TW>;

                if (callback != null) {
                    callback(arg1, arg2, arg3, arg4,arg5);
                } else {
                    throw CreateBroadcastSignatureException(eventIdType);
                }
            }
        }

        public class BroadcastException : Exception {
            public BroadcastException(string msg)
                : base(msg) {
            }
        }

        public class ListenerException : Exception {
            public ListenerException(string msg)
                : base(msg) {
            }
        }

        #region AddListener

        //No parameters
        public void AddListener(EventId eventIdType, MyAction handler) {
            OnListenerAdding(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction) _eventTable[eventIdType] + handler;
        }

        //Single parameter
        public void AddListener<T>(EventId eventIdType, MyAction<T> handler) {
            OnListenerAdding(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T>) _eventTable[eventIdType] + handler;
        }

        //Two parameters
        public void AddListener<T, TU>(EventId eventIdType, MyAction<T, TU> handler) {
            OnListenerAdding(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T, TU>) _eventTable[eventIdType] + handler;
        }

        //Three parameters
        public void AddListener<T, TU, TV>(EventId eventIdType, MyAction<T, TU, TV> handler) {
            OnListenerAdding(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T, TU, TV>) _eventTable[eventIdType] + handler;
        }

        //Foue parameters
        public void AddListener<T, TU, TV,TS>(EventId eventIdType, MyAction<T, TU, TV,TS> handler) {
            OnListenerAdding(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T, TU, TV,TS>)_eventTable[eventIdType] + handler;
        }

        //Five parameters
        public void AddListener<T, TU, TV, TS, TW>(EventId eventIdType, MyAction<T, TU, TV, TS, TW> handler) {
            OnListenerAdding(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T, TU, TV, TS ,TW>)_eventTable[eventIdType] + handler;
        }

        #endregion

        #region RemoveListener

        //No parameters
        public void RemoveListener(EventId eventIdType, MyAction handler) {
            OnListenerRemoving(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction) _eventTable[eventIdType] - handler;
            OnListenerRemoved(eventIdType);
        }

        //Single parameter
        public void RemoveListener<T>(EventId eventIdType, MyAction<T> handler) {
            OnListenerRemoving(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T>) _eventTable[eventIdType] - handler;
            OnListenerRemoved(eventIdType);
        }

        //Two parameters
        public void RemoveListener<T, TU>(EventId eventIdType, MyAction<T, TU> handler) {
            OnListenerRemoving(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T, TU>) _eventTable[eventIdType] - handler;
            OnListenerRemoved(eventIdType);
        }

        //Three parameters
        public void RemoveListener<T, TU, TV>(EventId eventIdType, MyAction<T, TU, TV> handler) {
            OnListenerRemoving(eventIdType, handler);
            _eventTable[eventIdType] = (MyAction<T, TU, TV>) _eventTable[eventIdType] - handler;
            OnListenerRemoved(eventIdType);
        }

        #endregion
    }
}