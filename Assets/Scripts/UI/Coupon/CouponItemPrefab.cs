namespace Cloth3D.Ui {
    public class CouponItemPrefab : ViewPresenter {
        public string CouponId = string.Empty;
        public string CouponPrice = string.Empty;
        public string Price = string.Empty;
//        public UILabel LblPrice;
//        public UILabel LblTitle;
//        public UILabel LblStart;
//        public UILabel LblEnd;
        public UITexture TexCoupon;
        public UIButton BtnChooce;
        public event MyAction<string, string, string> ClickChoose;

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        protected override void AwakeUnityMsg() {
            BtnChooce.onClick.Add(new EventDelegate(OnBtnChooseClick));
        }

        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                ClickChoose(CouponId, CouponPrice, "满"+ Price+"减"+ CouponPrice);
            }
        }
    }
}
