using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloth3D.Interfaces;

namespace Cloth3D.Design {
    class StateSelectStyle :State {
        private DesignWorld _designWorld=null;
        public StateSelectStyle(string name, DesignWorld designWorld) : base(name) {
            _designWorld = designWorld;
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
            //fsm.SetParamValue("selectStyle", false);
            _designWorld.ChangeStateEvent(DesignState.StateStyle);
        }

        public override void LeaveState(Fsm fsm) {
        }

        public override void Excute(Fsm fsm) {
            _designWorld.CameraRotaGoAnimation.Update();
        }
    }
}
