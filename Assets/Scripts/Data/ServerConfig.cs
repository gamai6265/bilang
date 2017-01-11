namespace Cloth3D.Data {
    public class ServerConfig : IData {
        public int LogicServerId = -1;
        public string LogicServerName = "";
        public string NodeIp = "";
        public int NodePort = -1;
        public int ServerId = -1;
        public string ServerName = "";

        public bool CollectDataFromDbc(DbcRow node) {
            ServerId = DbcUtils.ExtractNumeric(node, "ServerId", -1);
            ServerName = DbcUtils.ExtractString(node, "ServerName", "");
            NodeIp = DbcUtils.ExtractString(node, "NodeIp", "");
            NodePort = DbcUtils.ExtractNumeric(node, "NodePort", -1);
            LogicServerId = DbcUtils.ExtractNumeric(node, "LogicServerId", -1);
            LogicServerName = DbcUtils.ExtractNumeric(node, "LogicServerName", "");
            return true;
        }

        public int GetId() {
            return ServerId;
        }
    }

    public class ServerConfigProvider : SingletonDataMgr<ServerConfig> {
    }
}