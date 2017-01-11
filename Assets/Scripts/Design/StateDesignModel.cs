using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Design {
    internal class StateDesignModel : State {
        private readonly DesignWorld _designWorld;

        public StateDesignModel(string name, DesignWorld designWorld) : base(name) {
            _designWorld = designWorld;
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
            LogSystem.Debug("StateDesignModel.");
            //fsm.SetParamValue("doDesign", false);
            //fsm.SetParamValue("doneSelectStyle", false);
            _designWorld.ChangeStateEvent(DesignState.StateDesign);
        }

        public override void LeaveState(Fsm fsm) {
        }

        public override void Excute(Fsm fsm) {
            _designWorld.CameraRotaGoAnimation.Update();
        }
    }
}