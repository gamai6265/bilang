using System;

namespace Cloth3D.Ui {
    public class AgreementViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UILabel LblAgreement;

        public event MyAction ClickBack;

        protected override void AwakeUnityMsg() {
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode= UiWindowColliderMode.Normal;
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }
    }
}