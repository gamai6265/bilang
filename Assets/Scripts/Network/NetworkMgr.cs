using System;
using System.Collections.Generic;
using Cloth3D.Interfaces;

namespace Cloth3D.Network {
    public abstract class AbstractTrigger<T> : ITrigger {
        protected List<object> LstListener = new List<object>();

        public bool Fire(NetMessage evt) {
            var ret = true;
            foreach (var o in LstListener) {
                var listener = (T) o;
                if (listener != null) {
                    if (!FireOneListener(listener, evt)) {
                        ret = false;
                        //break; 丢弃事件处理结果。
                    }
                }
            }
            return ret;
        }

        public virtual List<int> GetEventList() {
            return null;
        }

        public void AddListener(object listener) {
            LstListener.Add(listener);
        }

        public void RemoveListener(object listener) {
            LstListener.Remove(listener);
        }

        public virtual bool FireOneListener(T listener, NetMessage evt) {
            return false;
        }
    }

    public interface IProtolProcessor {
        List<Type> GetSenderList(); //用于注册发消息接口
        List<KeyValuePair<string, ITrigger>> GetTriggerList();
    }

    public class NetworkMgr : INetworkMgr {
        //msg触发器
        private readonly Dictionary<int, ITrigger> _dicMsg2Triggers = new Dictionary<int, ITrigger>();
        private readonly Dictionary<Type, object> _dicSender = new Dictionary<Type, object>();
        private readonly Dictionary<string, ITrigger> _dicTriggers = new Dictionary<string, ITrigger>();
        //消息队列
        private readonly Queue<NetMessage> _queueMsgs = new Queue<NetMessage>();
        //协议处理器
        private Dictionary<int, IProtolProcessor> _processor = new Dictionary<int, IProtolProcessor>();
        private object _lockObj = new object();

        /**********************************************************************************************/ /**
         * @fn  public bool Init()
         *
         * @brief   S this object.
         *
         * @return  true if it succeeds, false if it fails.
         **************************************************************************************************/
        public string Name { get; set; }

        public void Tick() {
            DispathMsg();
        }

        public bool Init() {
            return true;
        }


        /**********************************************************************************************/ /**
         * @fn  public void DispathMsg()
         *
         * @brief   Dispath message.
         *
         **************************************************************************************************/

        public void DispathMsg() {
            lock (_lockObj) {
                while (_queueMsgs.Count > 0) {
                    var msg = _queueMsgs.Dequeue();
                    if (_dicMsg2Triggers.ContainsKey(msg.Id)) {
                        var tirgger = _dicMsg2Triggers[msg.Id];
                        tirgger.Fire(msg);
                    }
                }
            }
        }

        public bool PushMsg(NetMessage msg) {
            lock (_lockObj) {
                _queueMsgs.Enqueue(msg);
            }
            return true;
        }

        /**********************************************************************************************/ /**
         * @fn  public T QuerySender<T>()
         *
         * @brief   Queries the sender.
         *
         * @tparam  T   Generic type parameter.
         *
         * @return  The sender.
         **************************************************************************************************/

        public T QuerySender<T>() {
            if (_dicSender.ContainsKey(typeof(T))) {
                return (T) _dicSender[typeof(T)];
            }
            return default(T);
        }

        public ITrigger QueryTigger(string triggerName) {
            if (_dicTriggers.ContainsKey(triggerName)) {
                return _dicTriggers[triggerName];
            }
            return null;
        }

        public bool RegistDispatcher(IProtolProcessor protolProcessor) {
            var lstTrigger = protolProcessor.GetTriggerList();
            for (var i = 0; i < lstTrigger.Count; i++) {
                AddTrigger(lstTrigger[i].Key, lstTrigger[i].Value);
                AddMsg2Triger(lstTrigger[i].Value);
            }
            var lstSender = protolProcessor.GetSenderList();
            for (var i = 0; i < lstSender.Count; i++) {
                var type = lstSender[i];
                AddSender(type, protolProcessor);
            }
            return true;
        }

        private bool AddTrigger(string tiggerName, ITrigger tigger) {
            var ret = true;
            if (!_dicTriggers.ContainsKey(tiggerName)) {
                _dicTriggers[tiggerName] = tigger;
            } else {
                LogSystem.Warn("NetworkMgr.AddTrigger failed has same sender type:{0}", tiggerName);
                ret = false;
            }
            return ret;
        }

        /**********************************************************************************************/ /**
         * @fn  public bool AddMsg2Triger(ITrigger trigger)
         *
         * @brief   添加事件触发器，不允许同一个事件被多个触发器处理。
         *
         * @param   trigger The trigger.
         *
         * @return  1.注册成功返回true 2.当触发器拥有相同消息时，新的触发器无法添加成功，返回false.
         **************************************************************************************************/

        private bool AddMsg2Triger(ITrigger trigger) {
            var ret = true;
            var msgIds = trigger.GetEventList();
            foreach (var id in msgIds) {
                if (!_dicMsg2Triggers.ContainsKey(id)) {
                    _dicMsg2Triggers.Add(id, trigger);
                } else {
                    LogSystem.Warn("NetworkMgr.AddMsg2Triger failed has same messageId:{0}", id);
                    ret = false;
                }
            }
            return ret;
        }

        /**********************************************************************************************/ /**
         * @fn  private bool AddSender<T>(T sender)
         *
         * @brief   Adds a sender.
         *
         * @tparam  T   Generic type parameter.
         * @param   sender  The sender.
         *
         * @return  true if it succeeds, false if it fails.
         **************************************************************************************************/

        private bool AddSender(Type senderType, object sender) {
            var ret = true;
            if (!_dicSender.ContainsKey(senderType)) {
                _dicSender[senderType] = sender;
            } else {
                LogSystem.Warn("NetworkMgr.AddSender failed has same sender type:{0}", senderType.ToString());
                ret = false;
            }
            return ret;
        }
    }
}