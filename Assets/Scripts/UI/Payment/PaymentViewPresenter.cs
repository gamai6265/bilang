using System;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    //银联支付渠道 upacp 微信支付渠道 wx 支付宝支付渠道 alipay
    public enum PaymenType {
        wx,
        upacp,
        alipay,
    }

    public class PaymentViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UIButton BtnWeChat;
        public UIButton BtnUnionpay;
        public UIButton BtnAlipay;
        public UIButton BtnPay;

        public UISprite SpriteWeChat;
        public UISprite SpriteUnionpay;
        public UISprite SpriteAlipay;

        public UILabel LblPrice;

        public UILabel LblTitle;
        public UILabel LblChoose;
        public UILabel LblPriceName;
        public UILabel LblPay;

        public event MyAction ClickBack;
        public event MyAction ClickWeChat;
        public event MyAction ClickUnionpay;
        public event MyAction ClickAlipay;
        public event MyAction ClickPay;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }
        protected override void StartUnityMsg() {
            BtnsListener();
            SetSprite(PaymenType.alipay);
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("p_title");
            LblChoose.text = StrConfigProvider.Instance.GetStr("p_choose");
            LblPriceName.text = StrConfigProvider.Instance.GetStr("p_price");
            LblPay.text = StrConfigProvider.Instance.GetStr("p_pay");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblChoose.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPriceName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPay.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnWeChat.onClick.Add(new EventDelegate(OnBtnWeChattClick));
            BtnUnionpay.onClick.Add(new EventDelegate(OnBtnUnionpayClick));
            BtnAlipay.onClick.Add(new EventDelegate(OnBtnAlipayClick));
            BtnPay.onClick.Add(new EventDelegate(OnBtnPayClick));
        }

        private void OnBtnPayClick() {
            if (null != ClickPay) {
                ClickPay();
            }
        }

        private void OnBtnUnionpayClick() {
            if (null != ClickUnionpay) {
                ClickUnionpay();
            }
        }

        private void OnBtnWeChattClick() {
            if (null != ClickWeChat) {
                ClickWeChat();
            }
        }

        private void OnBtnAlipayClick() {
            if (null != ClickAlipay) {
                ClickAlipay();
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public void SetPrice(string str) {
            LblPrice.text = str;
        }

        public void SetSprite(PaymenType pt) {
            if (pt == PaymenType.wx) {
                SpriteWeChat.spriteName = "Select";
                SpriteUnionpay.spriteName = SpriteAlipay.spriteName = "All";
            } else if (pt == PaymenType.upacp) {
                SpriteWeChat.spriteName = SpriteAlipay.spriteName = "All";
                SpriteUnionpay.spriteName = "Select";
            }else if(pt == PaymenType.alipay) {
                SpriteWeChat.spriteName = SpriteUnionpay.spriteName = "All";
                SpriteAlipay.spriteName = "Select";
            }
        }
    }
}