using System.Collections.Generic;

namespace Cloth3D{
    public class SystemGuideConfig : IData {
        public int Id;
        public List<int> Operations;

        public bool CollectDataFromDbc(DbcRow node) {
            Id = DbcUtils.ExtractNumeric<int>(node, "Id", 0);
            Operations = DbcUtils.ExtractNumericList<int>(node, "Operation", 0);
            return true;
        }
        public int GetId() {
            return Id;
        }
    }

    public class SystemGuideConfigProvider:SingletonDataMgr<SystemGuideConfig> {
    }
}
