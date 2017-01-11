using LitJson;

namespace Cloth3D.Ui {
    public class ClothesPrefabs : ViewPresenter {
        public string OrderId = string.Empty;
        public UILabel LblName;
        public UILabel LblNumber;
        public UILabel LblgoodsNumber;
        public UILabel LblRemarkName;
        public UILabel LblRemark;
        public UITexture TexClothes;

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}