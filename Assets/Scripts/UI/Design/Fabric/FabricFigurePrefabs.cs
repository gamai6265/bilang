namespace Cloth3D.Ui {
    public class FabricFigurePrefabs : ViewPresenter {
        public string CodeName {
            get {
                return LblSelection.text;
            }
            set {
                LblSelection.text = value;
            }
        }
        public string CodeValue;
        public UILabel LblSelection;
        public override void InitWindowData() {
        }
    }
}