using System.Collections.Generic;
using UnityEngine;

namespace Cloth3D.Data {
    class ModelsTypeConfig : IData {
        public int Id = -1;
        public string TypeName = string.Empty;
        public List<int> LstModels = new List<int>();
        public List<Vector3> LstModelsPos = new List<Vector3>();
        public Vector3 ModelPos = Vector3.zero;
        public Vector3 ModelDefaultView = Vector3.zero;
        public float MinDistance;
        public float MaxDistance;
        public Vector2 Yscope = Vector2.zero;
        public string SampleImage = string.Empty;
        public string QueryType = string.Empty;

        public bool CollectDataFromDbc(DbcRow row) {
            Id = DbcUtils.ExtractNumeric(row, "Id", -1);
            TypeName = DbcUtils.ExtractString(row, "TypeName", string.Empty);
            var modelsStr = DbcUtils.ExtractString(row, "Models", string.Empty);
            var itemStrVec = Utils.SplitAsList(modelsStr, new[]{"|"});
            for (var i = 0; i < itemStrVec.Count; i++) {
                var itemContens = Utils.SplitAsList(itemStrVec[i], new[] { ";" });
                if (itemContens.Count == 2) {
                    int tmpModelId;
                    int.TryParse(itemContens[0], out tmpModelId);
                    var modelPositions = Utils.SplitAsNumericList<float>(itemContens[1], new[] {","});
                    if (modelPositions != null && modelPositions.Count == 3) {
                        LstModels.Add(tmpModelId);
                        LstModelsPos.Add(new Vector3() {
                            x = modelPositions[0],
                            y = modelPositions[1],
                            z = modelPositions[2]
                        });
                    }
                } else {
                    LogSystem.Warn("models load failed. data:"+ itemStrVec[i]);
                }
            }
            var modelPosStr = DbcUtils.ExtractNumericList<float>(row, "ModelPos", 0);
            if (modelPosStr != null && modelPosStr.Count == 3) {
                ModelPos = new Vector3() {
                    x = modelPosStr[0],
                    y = modelPosStr[1],
                    z = modelPosStr[2]
                };
            }
            
            var lstViewpos = DbcUtils.ExtractNumericList<float>(row, "DefaultView", 0);
            if (lstViewpos != null && lstViewpos.Count == 3) {
                ModelDefaultView = new Vector3() {
                    x = lstViewpos[0],
                    y = lstViewpos[1],
                    z = lstViewpos[2]
                };
            }
            MinDistance = DbcUtils.ExtractNumeric<float>(row, "MinDistance", 0);
            MaxDistance = DbcUtils.ExtractNumeric<float>(row, "MaxDistance", 0);
            var lstYscopePos = DbcUtils.ExtractNumericList<float>(row, "Yscope", 0);
            if (lstYscopePos != null && lstYscopePos.Count == 2) {
                Yscope = new Vector2() {
                    x = lstYscopePos[0],
                    y = lstYscopePos[1],
                };
            }
            SampleImage = DbcUtils.ExtractString(row, "SampleImage", string.Empty);
            QueryType = DbcUtils.ExtractString(row, "QueryType", string.Empty);
            return true;
        }

        public int GetId() {
            return Id;
        }
    }
    class ModelsTypeConfigProvider : SingletonDataMgr<ModelsTypeConfig> {
    }
}
