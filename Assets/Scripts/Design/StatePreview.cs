using Cloth3D.Interfaces;
using UnityEngine;
namespace Cloth3D.Design {
    class StatePreview :State {
        private DesignWorld _designWorld = null;
        public StatePreview(string name, DesignWorld designWorld) : base(name) {
            _designWorld = designWorld;
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
            _designWorld.ChangeStateEvent(DesignState.StatePreview);
        }

        public override void LeaveState(Fsm fsm) {
        }

        public override void Excute(Fsm fsm) {
            _designWorld.CameraRotaBackAnimation.Update();
        }
    }
}
