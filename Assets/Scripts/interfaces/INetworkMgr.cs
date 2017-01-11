using System.Collections.Generic;

namespace Cloth3D.Interfaces {
    public class NetMessage {
        public object Content = null;
        public int Id = -1;
    }

    public interface ITrigger {
        bool Fire(NetMessage evt);
        List<int> GetEventList();
        void AddListener(object listener);
        void RemoveListener(object listener);
    }

    internal interface INetworkMgr : IComponent {
        T QuerySender<T>();
        ITrigger QueryTigger(string triggerName);
        void DispathMsg();
        bool PushMsg(NetMessage msg);
    }
}