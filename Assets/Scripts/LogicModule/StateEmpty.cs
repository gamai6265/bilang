using Cloth3D.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cloth3D.Logic {
    internal class StateEmpty : State {
        private AsyncOperation _asynObj;
        public StateEmpty(string name) : base(name) {
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
            //_isSceneLoaded = false;
           LogSystem.Debug("StateEmpty enter.");
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic) {
                if (null != logic.OnBeforeStageEnter) {
                    logic.OnBeforeStageEnter(StageType.StageEmpty);
                }
                _asynObj = SceneManager.LoadSceneAsync(logic.Config.GetSetting("SceneStage", "empty"), LoadSceneMode.Single);
            }
        }

        public override void LeaveState(Fsm fsm) {
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic && null != logic.OnBeforeStageLeave) {
                logic.OnBeforeStageLeave(StageType.StageEmpty);
            }
        }

        public override void Excute(Fsm fsm) {
            if (null!= _asynObj && _asynObj.isDone) {
                var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
                if (null != logic && null != logic.OnAfterStageEnter) {
                    logic.OnAfterStageEnter(StageType.StageEmpty);
                }
                _asynObj = null;
            }
        }
    }
}