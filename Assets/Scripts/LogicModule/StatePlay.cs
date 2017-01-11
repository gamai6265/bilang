using System.Collections.Generic;
using Cloth3D.Interfaces;

namespace Cloth3D.Logic {
    internal class StatePlay : State {
        private static string _stateEmpty = "StateEmpty";
        private static string _stateDesign = "StateDesign";
        private Fsm _fsm;
        public StatePlay(string name) : base(name) {
            _fsm = new Fsm("FsmPlay");
            // add some states
            _fsm.AddState(new StateEmpty(_stateEmpty));
            _fsm.AddState(new StateDesign(_stateDesign));
            _fsm.CurState = _fsm.GetState(_stateEmpty);

            //empty state
            _fsm.AddParam(new ParamValue("SceneId") { Value = 0});
            Fsm.AddTransitionTo(_fsm, _stateEmpty, _stateDesign, new List<Condition>() { new Condition("SceneId", 1, ConditionType.Equals) });
            //design state
            Fsm.AddTransitionTo(_fsm, _stateDesign, _stateEmpty, new List<Condition>() { new Condition("SceneId", 0, ConditionType.Equals) });
        }

        public void SwitchStage(StageType stage) {
            switch (stage) {
                case StageType.StageEmpty:
                    _fsm.SetParamValue("SceneId", 0);
                    break;
                case StageType.StageDesign:
                    _fsm.SetParamValue("SceneId", 1);
                    break;
            }
        }

        public StageType GetCurrentStage() {
            if (_fsm.GetParamValue<int>("SceneId") == 0) {
                return StageType.StageEmpty;
            }
            return StageType.StageDesign;
        }

        protected override void Init(Fsm fsm) {
            //throw new NotImplementedException();
        }

        public override void EnterState(Fsm fsm) {
        }

        public override void LeaveState(Fsm fsm) {
            //throw new NotImplementedException();
        }

        public override void Excute(Fsm fsm) {
            //throw new NotImplementedException();
            _fsm.Update();
        }
    }
}