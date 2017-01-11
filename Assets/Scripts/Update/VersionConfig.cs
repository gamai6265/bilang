using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D{
    [Serializable]
    public class VersionConfig : IData {
        public string MainNum = string.Empty; 
        public string SubNum = string.Empty;
        public string StepNum = string.Empty;
        public string DateNum = string.Empty;
        public string GreekSymbols = string.Empty;
        public bool CollectDataFromDbc(DbcRow node) {
            MainNum = DbcUtils.ExtractString(node, "MainNum", "1");
            SubNum = DbcUtils.ExtractString(node, "SubNum", "0");
            StepNum = DbcUtils.ExtractString(node, "StepNum", "0");
            DateNum = DbcUtils.ExtractString(node, "DateNum", "0");
            GreekSymbols = DbcUtils.ExtractString(node, "GreekSymbols", "base");
            return true;
        }
        public int GetId() {
            int tmp;
            int.TryParse(MainNum, out tmp);
            return tmp;
        }
    }

    public class VersionConfigProvider:SingletonDataMgr<VersionConfig> {
        public string GetVersionNum() {
            string game_version = "0";
            var dic = GetData();
            foreach (object element in dic.Values) {
                VersionConfig version = element as VersionConfig;
                if (null != version) {
                    game_version = version.MainNum + "-" + version.SubNum + "-" + version.StepNum + "-" + version.DateNum + "-" + version.GreekSymbols;
                    break;
                }
            }
            return game_version;
        }
    }
}
