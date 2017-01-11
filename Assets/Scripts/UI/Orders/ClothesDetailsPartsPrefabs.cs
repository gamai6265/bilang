using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class ClothesDetailsPartsPrefabs : ViewPresenter {
        public UILabel LblFabric1;
        public UILabel LblFabric2;
        public UILabel LblPartName;
        public UILabel LblPartStyleName;
//        public UILabel LblRemark;
        public UITexture TexClothesDetail;

        public UILabel LblFabricName;
//        public UILabel LblRemarkName;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblFabricName.text = StrConfigProvider.Instance.GetStr("cd_fabric");
//            LblRemarkName.text = StrConfigProvider.Instance.GetStr("cd_remark");
            LblFabricName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
//            LblRemarkName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}