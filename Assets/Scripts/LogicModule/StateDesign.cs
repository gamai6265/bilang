using System;
using Cloth3D.Design;
using Cloth3D.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cloth3D.Logic {
    internal class StateDesign : State {
        private AsyncOperation _asynObj;
        public StateDesign(string name) : base(name) {
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
           LogSystem.Debug("StateDesign enter.");
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic) {
                if (null != logic.OnBeforeStageEnter) {
                    logic.OnBeforeStageEnter(StageType.StageDesign);
                }
                _asynObj = SceneManager.LoadSceneAsync(logic.Config.GetSetting("SceneStage", "design"), LoadSceneMode.Single);
            }
        }

        public override void LeaveState(Fsm fsm) {
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic && null != logic.OnBeforeStageLeave) {
                logic.OnBeforeStageLeave(StageType.StageDesign);
            }
        }

        public override void Excute(Fsm fsm) {
            if (null != _asynObj) {
                if (_asynObj.isDone) {
                    var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
                    if (null != logic && null != logic.OnAfterStageEnter) {
                        logic.OnAfterStageEnter(StageType.StageDesign);
                    }
                    //Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                    _asynObj = null;
                }
            } 
        }
    }
}