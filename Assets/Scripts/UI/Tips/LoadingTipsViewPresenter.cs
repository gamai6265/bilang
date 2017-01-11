using System;

namespace  Cloth3D.Ui {
    class LoadingTipsViewPresenter : ViewPresenter, IWindowAnimation {

        protected override void AwakeUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.PopUp;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public override void HideWindow(MyAction onComplete = null) {
            base.HideWindow(onComplete);
        }

        public void EnterAnimation(EventDelegate.Callback onComplete) {
        }

        public void QuitAnimation(EventDelegate.Callback onComplete) {
        }

        public void ResetAnimation() {
        }
    }
}
