using UnityEngine;

namespace Cloth3D.Data {
    class ModelsConfig :IData{
        public int Id = -1;
        public string ModelName = string.Empty;
        public string ModelFile = string.Empty;

        public bool CollectDataFromDbc(DbcRow row) {
            Id = DbcUtils.ExtractNumeric(row, "Id", -1);
            ModelName = DbcUtils.ExtractString(row, "ModelName", string.Empty);
            ModelFile = DbcUtils.ExtractString(row, "ModelFile", string.Empty);
            return true;
        }

        public int GetId() {
            return Id;
        }
    }

    class ModelsConfigProvider:SingletonDataMgr<ModelsConfig> {
    }
}
