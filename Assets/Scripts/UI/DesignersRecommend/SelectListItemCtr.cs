namespace Cloth3D.Ui {
    public class SelectListItemCtr : ViewPresenter {
        public UILabel LblLabel;
        public UIScrollView ItemScrollView;
        public UIGrid ItemGrid;

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}