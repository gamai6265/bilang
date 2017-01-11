namespace Cloth3D.Ui {
    public class WebViewViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public event MyAction ClickBack;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }
        
        public override void InitWindowData() {
            WinData.WindowType= UiWindowType.Normal;
            WinData.ColliderMode= UiWindowColliderMode.Normal;
        }
    }
}