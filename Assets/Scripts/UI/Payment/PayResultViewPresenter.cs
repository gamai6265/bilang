using UnityEngine;
using System.Collections;
using System;
using Cloth3D;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class PayResultViewPresenter : ViewPresenter {
        public UIButton BtnBack;

        public UISprite SprResult;
        public UILabel LblResult;

        public UILabel LblOrderCode;
        public UILabel LblPrice;
        public UILabel LblChannel;

        public UILabel LblTitle;
        public UILabel LblNumber;
        public UILabel LblPriceName;
        public UILabel LblWay;
        public UILabel LblPayNew;
       

        public UIButton BtnSure;
        public UIButton BtnBackHome;

        public event MyAction ClickBack;
        public event MyAction ClickSure;
        public event MyAction ClickBackHome; 

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnSure.onClick.Add(new EventDelegate(OnBtnSureClick));
            BtnBackHome.onClick.Add(new EventDelegate(OnBtnBackHome));
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("or_ps_title");
            LblNumber.text = StrConfigProvider.Instance.GetStr("or_ps_number");
            LblPriceName.text = StrConfigProvider.Instance.GetStr("or_ps_price");
            LblWay.text = StrConfigProvider.Instance.GetStr("or_ps_way");
            LblPayNew.text = StrConfigProvider.Instance.GetStr("or_ps_pnew");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblNumber.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPriceName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblWay.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPayNew.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnSureClick() {
            if (null != ClickSure) {
                ClickSure();
            }
        }

        private void OnBtnBackHome() {
            if (null != ClickBackHome) {
                ClickBackHome();
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

        public void SetResult(bool tof) {
            if (tof) {
                SprResult.spriteName = "Succeed";
                LblResult.text = StrConfigProvider.Instance.GetStr("or_ps_succeed");
                BtnSure.gameObject.SetActive(false);
                BtnBackHome.gameObject.SetActive(true);
                LblResult.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            } else {
                SprResult.spriteName = "Fail";
                LblResult.text = StrConfigProvider.Instance.GetStr("or_ps_fail");
                BtnBackHome.gameObject.SetActive(false);
                BtnSure.gameObject.SetActive(true);
                LblResult.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            }
        }

        public void SetLabel(string orderid, string price, string channel) {
            LblOrderCode.text = orderid;
            float tem;
            float.TryParse(price, out tem);
            LblPrice.text = string.Format("{0:F}", (tem / 100));
            if (channel == PaymenType.wx.ToString()) {
                LblChannel.text = StrConfigProvider.Instance.GetStr("or_ps_wx");
            } else if (channel == PaymenType.upacp.ToString()) {
                LblChannel.text = StrConfigProvider.Instance.GetStr("or_ps_upacp");
            }
            else if(channel==PaymenType.alipay.ToString()) {
                LblChannel.text = StrConfigProvider.Instance.GetStr("or_ps_alipay");
            }
            else {
                LblChannel.text = channel;
            }
            LblChannel.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }
    }
}