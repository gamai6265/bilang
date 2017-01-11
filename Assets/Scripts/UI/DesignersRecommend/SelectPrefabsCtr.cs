using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class SelectPrefabsCtr : ViewPresenter {
        public UIButton ScreenPrefabs;
        public UISprite SprSelect;
        public UILabel LblText;

        public SelectInfo SelectItemInfo;
        public event MyAction<SelectInfo> ClickChoose;
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        protected override void AwakeUnityMsg() {
            ScreenPrefabs.onClick.Add(new EventDelegate(OnBtnChooseClick));
        }
        
        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                ClickChoose(SelectItemInfo);
            }
        }
    }
}