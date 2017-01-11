using System.Collections.Generic;

namespace Cloth3D.Data {
    public class UiConfig : IData {
        public int Id;
        public bool IsExclusion;
        public int OffsetBottom = -1;
        public int OffsetLeft = -1;
        public int OffsetRight = -1;
        public int OffsetTop = -1;
        public int SceneId = -1;
        public int ShowType = -1;
        public string WindowName = string.Empty;
        public string WindowPath = string.Empty;

        public bool CollectDataFromDbc(DbcRow row) {
            Id = DbcUtils.ExtractNumeric(row, "Id", 0);
            WindowName = DbcUtils.ExtractString(row, "WindowName", "");
            WindowPath = DbcUtils.ExtractString(row, "WindowPath", "");
            OffsetLeft = DbcUtils.ExtractNumeric(row, "OffsetLeft", -1);
            OffsetRight = DbcUtils.ExtractNumeric(row, "OffsetRight", -1);
            OffsetTop = DbcUtils.ExtractNumeric(row, "OffsetTop", -1);
            OffsetBottom = DbcUtils.ExtractNumeric(row, "OffsetBottom", -1);
            IsExclusion = DbcUtils.ExtractBool(row, "IsExclusion", false);
            ShowType = DbcUtils.ExtractNumeric(row, "ShowType", 0);
            SceneId = DbcUtils.ExtractNumeric(row, "SceneId", 0);
            return true;
        }

        public int GetId() {
            return Id;
        }
    }

    public class UiConfigProvider : DataMgr<UiConfig> {
        private static UiConfigProvider _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<string, UiConfig> _dicName2Data = new Dictionary<string, UiConfig>();

        public static UiConfigProvider Instance {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null)
                            _instance = new UiConfigProvider();
                    }
                }
                return _instance;
            }
        }

        public UiConfig GetData(string name) {
            if (string.IsNullOrEmpty(name)) {
                LogSystem.Warn("error.");
                return null;
            }
            if (_dicName2Data.ContainsKey(name)) {
                return _dicName2Data[name];
            }
            return null;
        }

        protected override void AddRecord(int id, UiConfig data) {
            if (!_dicName2Data.ContainsKey(data.WindowName)) {
                _dicName2Data.Add(data.WindowName, data);
            } else {
               LogSystem.Warn("!!!!!error. has the same name:{0}",data.WindowName);
            }
        }

        public override void Clear() {
            base.Clear();
            _dicName2Data.Clear();
        }
    }
}