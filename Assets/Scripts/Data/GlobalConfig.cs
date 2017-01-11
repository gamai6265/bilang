using System.Collections.Generic;

namespace Cloth3D.Data {
    public class GlobalConfig : IData {
        public int Id = -1;
        public int ModelType = -1;
        public string GlobalName = string.Empty;
        public string SampleImage  = string.Empty;
        public List<string> LstMaterials = null;
        public string Instruction = string.Empty;
        public string QueryType = string.Empty;
        public string FabricStyle = string.Empty;
        public bool CollectDataFromDbc(DbcRow row) {
            Id = DbcUtils.ExtractNumeric(row, "Id", -1);
            ModelType = DbcUtils.ExtractNumeric(row, "ModelType", -1);
            GlobalName = DbcUtils.ExtractString(row, "Name", string.Empty);
            SampleImage = DbcUtils.ExtractString(row, "SampleImage", string.Empty);
            Instruction= DbcUtils.ExtractString(row, "Instruction", string.Empty);
            QueryType = DbcUtils.ExtractString(row, "QueryType", string.Empty);
            FabricStyle = DbcUtils.ExtractString(row, "FabricStyle", string.Empty);
            var materialStr = DbcUtils.ExtractString(row, "Materials", string.Empty);
            if (materialStr != string.Empty) {
                LstMaterials = Utils.SplitAsList(materialStr, DbcUtils.FieldStringSeperator);
            }
            return true;
        }

        public int GetId() {
            return Id;
        }
    }
    class GlobalProvider : DataMgr<GlobalConfig> {
        private static GlobalProvider _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<int, List<GlobalConfig>> _dicModelGlobals = new Dictionary<int, List<GlobalConfig>>();

        public static GlobalProvider Instance {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null)
                            _instance = new GlobalProvider();
                    }
                }
                return _instance;
            }
        }

        protected override void AddRecord(int id, GlobalConfig data) {
            if (!_dicModelGlobals.ContainsKey(data.ModelType)) {
                _dicModelGlobals.Add(data.ModelType, new List<GlobalConfig>());
            }
            var lst = _dicModelGlobals[data.ModelType];
            if (!lst.Contains(data)) {
                lst.Add(data);
            }else {
                LogSystem.Warn("!!!!!error. has the same name:{0}", data.Id);
            }
        }

        public override void Clear() {
            base.Clear();
            _dicModelGlobals.Clear();
        }

        public List<GlobalConfig> GetGlobals(int modelType) {
            if (_dicModelGlobals.ContainsKey(modelType)) {
                return _dicModelGlobals[modelType];
            }
            return null;
        }
    }
}
