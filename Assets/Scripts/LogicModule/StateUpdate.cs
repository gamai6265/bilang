using Cloth3D.Interfaces;
using UnityEngine;
namespace Cloth3D.Logic {
    internal class StateUpdate : State {
        public StateUpdate(string name) : base(name) {
        }

        protected override void Init(Fsm fsm) {
        }

        public override void EnterState(Fsm fsm) {
           LogSystem.Debug("state update. enter");
            var update = ComponentMgr.Instance.FindComponent(ComponentNames.ComUpdate) as IUpdateMgr;
            if (update!=null && null!=update.OnUpdateBegin) {
                update.OnUpdateBegin();
            }
            fsm.SetParamValue("doneUpdate", true);
        }

        public override void LeaveState(Fsm fsm) {
            fsm.SetParamValue("doneUpdate", false);
        }

        public override void Excute(Fsm fsm) {
            // check if updateMgr is done update, then set doneUpdate flag.
        }
    }
}