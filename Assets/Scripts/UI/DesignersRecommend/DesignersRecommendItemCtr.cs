namespace Cloth3D.Ui {
    public class DesignersRecommendItemCtr : ViewPresenter {
        public UIButton BtnMore;
        public UIGrid RecommendGrid;
        public UILabel LblTitle;

        public string DRType = string.Empty;
        public event MyAction<string> ClickMore;
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        protected override void AwakeUnityMsg() {
            BtnMore.onClick.Add(new EventDelegate(OnBtnMoreClick));
        }

        private void OnBtnMoreClick() {
            if (null != ClickMore) {
                ClickMore(DRType);
            }
        }
    }
}