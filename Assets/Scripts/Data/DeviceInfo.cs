namespace Cloth3D.Data {
    public class DeviceInfo {
        private string _clientVersion = "0"; // 客户端版本
        private string _clientLoginIp = "127.0.0.1"; // 客户端Ip地址

        public string OperatingSystem { get; set; }

        public string UniqueIdentifier { get; set; }

        public string ClientVersion {
            get { return _clientVersion; }
            set { _clientVersion = value; }
        }

        public string ClientLoginIp {
            get { return _clientLoginIp; }
            set { _clientLoginIp = value; }
        }
    }
}