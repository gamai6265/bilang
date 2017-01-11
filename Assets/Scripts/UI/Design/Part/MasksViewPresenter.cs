using System;
using System.Collections.Generic;
using UnityEngine;
namespace Cloth3D.Ui {
    class MasksViewPresenter : ViewPresenter {

        public UIButton MasksButton;
        public event MyAction OnClick; 

        protected override void AwakeUnityMsg() {
            MasksButton.onClick.Add(new EventDelegate(OnMoveBack));
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.PopUp;
            //WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public override void HideWindow(MyAction onComplete = null) {
            base.HideWindow(onComplete);
        }

        private void OnMoveBack() {
            LogSystem.Debug("moveback");
            if (null != OnClick) {
                OnClick();
            }
        }
    }
}
