using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class OrderConfirmPrefabs : ViewPresenter {
        public UILabel LbllClothName;
        public UILabel LblClothQuantity;
        public UITexture TexCloth;
        public UIButton BtnChoose;
        public UIButton BtnChooseSize;
        public UILabel LblQuantity;

        public string ClothCode;
        public string modelId;
        public event MyAction<string,string> ClickChoose;
        public event MyAction<string, string> ClickChooseSize;

        protected override void AwakeUnityMsg() {                    
            LblQuantity.text = StrConfigProvider.Instance.GetStr("or_p_quantity");
            LblQuantity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public void AddListener() {
            BtnChooseSize.onClick.Add(new EventDelegate(OnBtnChooseSizeClick));
            BtnChoose.onClick.Add(new EventDelegate(OnBtnChooseClick));
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                ClickChoose(ClothCode, LblClothQuantity.text);
            }
        }

        private void OnBtnChooseSizeClick() {
            if (null != ClickChooseSize) {
                ClickChooseSize(modelId, ClothCode);
            }
        }
    }
}
