using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Logic {
    internal class StateLoadData : State {
        public StateLoadData(string name) : base(name) {
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
           LogSystem.Debug("state loading. enter");
            //1.loading sheet conifg
            LoadData();
            //2.cache resource. TODO danie 
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (logic!=null && logic.OnLoadDataDone!=null) {
                logic.OnLoadDataDone();
            }
            fsm.SetParamValue("doneLoadData", true);
            //3.load scene: assum all scene has less elements, then assign this task to stateEmpty and stateDesign
        }

        public override void LeaveState(Fsm fsm) {
        }

        public override void Excute(Fsm fsm) {
        }

        private void LoadData() {
            //UiConfigProvider.Instance.Clear();
            var persistDir= GlobalVariables.PersistentDataPath + "/" + FilePathDefine.ResSheetCacheDir;
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (logic != null) {
                logic.Config = new ConfigFile(persistDir + FilePathDefine.IniConfig);
            }
            bool ret = true;
            ret &= GlobalProvider.Instance.Load(persistDir + FilePathDefine.GlobalConfig);
            ret &= ModelsConfigProvider.Instance.Load(persistDir + FilePathDefine.ModelsConfig);
            var modesData = ModelsConfigProvider.Instance.GetData();
            ret &= ModelsTypeConfigProvider.Instance.Load(persistDir + FilePathDefine.ModelsTypeConfig);
            var partProvider = ModelPartProvider.Instance;
            foreach (var item in modesData) {
                ret &= partProvider.Load(item.Key, persistDir + item.Value.ModelFile);
            }
            ret &= StrConfigProvider.Instance.Load(persistDir + FilePathDefine.StrDictionary);
            //TODO if error
        }
    }
}