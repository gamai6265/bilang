using System.Collections.Generic;

namespace Cloth3D.Data {
    public class StrConfig : IData {
        public int Id;
        public string Key = string.Empty;
        public string Value = string.Empty;

        public bool CollectDataFromDbc(DbcRow node) {
            Id = DbcUtils.ExtractNumeric(node, "Id", 0);
            Key = DbcUtils.ExtractString(node, "Key", string.Empty);
            Value = DbcUtils.ExtractString(node, "Value", string.Empty);
            return true;
        }

        public int GetId() {
            return Id;
        }

    }

    public class StrConfigProvider : DataMgr<StrConfig> {
        private static StrConfigProvider _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<string, StrConfig> _dicName2Data = new Dictionary<string, StrConfig>();

        public static StrConfigProvider Instance {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null)
                            _instance = new StrConfigProvider();
                    }
                }
                return _instance;
            }
        }

        public string GetStr(string name) {
            if (string.IsNullOrEmpty(name)) {
               LogSystem.Warn("warn. getstr return string.empty");
                return string.Empty;
            }
            if (_dicName2Data.ContainsKey(name)) {
                return _dicName2Data[name].Value;
            }
           LogSystem.Warn("warn. getstr return string.empty");
            return string.Empty;
        }

        protected override void AddRecord(int id, StrConfig data) {
            if (!_dicName2Data.ContainsKey(data.Key)) {
                _dicName2Data.Add(data.Key, data);
            } else {
               LogSystem.Warn("!!!!!error. has the same name:{0}",data.Key);
            }
        }

        public override void Clear() {
            base.Clear();
            _dicName2Data.Clear();
        }
    }
}