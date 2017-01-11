using System.Collections.Generic;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrdersPrefabs : ViewPresenter {
        public UIButton BtnAppraise;

        public UILabel LblOO;
        public UILabel LblNum;
        public UILabel LblPri;
        public UILabel Lblcurrency;
        public UILabel LblAppraise;
        public UILabel LblCancle;
        public UILabel LblReceipt;
        public UILabel LblPay;

        public UIButton BtnCancel;
        public UIButton BtnChooce;
        public UIButton BtnPay;
        public UIButton BtnReceipt;
        public UIButton BtnRemind;

        public UILabel LblNumber;
        public UILabel LblOrderCode;
        public UILabel LblOrderName;
        public UILabel LblPrice;
        public UILabel LblState;
        public string OrderId;
        public OrderInfo OrderInfoItem;
//        public UITexture TexOrder1;
//        public UITexture TexOrder2;
//        public UITexture TexOrder3;

        public UITexture[] Textures = new UITexture[3];
        public List<string> LstGoodId = new List<string>();

        public event MyAction<string> ClickCancel;
        public event MyAction<string, string, string, string> ClickPay;
        public event MyAction<string> ClickReceipt;
        public event MyAction<List<string>, List<Texture2D>, List<string>> ClickAppraise;
        public event MyAction<OrderInfo, List<Texture2D>> ClickChooce;
        public event MyAction ClickRemind;

        protected override void AwakeUnityMsg() {
            BtnCancel.onClick.Add(new EventDelegate(OnBtnCancelClick));
            BtnPay.onClick.Add(new EventDelegate(OnBtnPayClick));
            BtnReceipt.onClick.Add(new EventDelegate(OnBtnReceiptClick));
            BtnAppraise.onClick.Add(new EventDelegate(OnBtnAppraiseClick));
            BtnChooce.onClick.Add(new EventDelegate(OnBtnChooceClick));
            BtnRemind.onClick.Add(new EventDelegate(OnBtnRemindClick));
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblOO.text = StrConfigProvider.Instance.GetStr("or_p_oo");
            LblNum.text = StrConfigProvider.Instance.GetStr("or_p_mun");
            LblPri.text = StrConfigProvider.Instance.GetStr("or_p_pri");
            Lblcurrency.text = StrConfigProvider.Instance.GetStr("or_p_currency");

            LblOO.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblNum.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPri.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            Lblcurrency.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnChooceClick() {
            if (null != ClickChooce) {
                List<Texture2D> texture2 = new List<Texture2D>();
                for (int i = 0; i < Textures.Length; i++) {
                    texture2.Add((Texture2D)Textures[i].mainTexture);
                }
                ClickChooce(OrderInfoItem, texture2);
            }
        }

        private void OnBtnAppraiseClick() {
            if (null != ClickAppraise) {
                List<Texture2D> texture2=new List<Texture2D>();
                for (int i = 0; i < Textures.Length; i++) {
                    texture2.Add((Texture2D)Textures[i].mainTexture);
                }
                List<string> strs=new List<string>();
                strs.Add(OrderId);
                strs.Add(LblOrderName.text);
                strs.Add(LblOrderCode.text);
                strs.Add(LblNumber.text);

                ClickAppraise(strs, texture2, LstGoodId);
            }
        }

        private void OnBtnReceiptClick() {
            if (null != ClickReceipt) {
                ClickReceipt(OrderId);
            }
        }

        private void OnBtnPayClick() {
            if (null != ClickPay) {
                var subject = string.Empty;
                if (int.Parse(OrderInfoItem.ClothNumber) > 1) {
                    subject = OrderInfoItem.Order_Goods.Count.ToString() + "件衬衣";
                } else {
                    subject = "比朗衬衣"; //OrderInfoItem.Order_Goods[0].GoodsName;//TODO 商品名字
                }
                ClickPay(LblOrderCode.text, OrderId, LblPrice.text, subject);
            }
        }

        private void OnBtnCancelClick() {
            if (null != ClickCancel) {
                ClickCancel(OrderId);
            }
        }

        private void OnBtnRemindClick() {
            if (null != ClickRemind)
                ClickRemind();
        }

        protected override
            void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
        }

        public void SetBtns(string str) {
            switch (str) {
                case "11":
                    BtnCancel.gameObject.SetActive(true);
                    LblCancle.text = StrConfigProvider.Instance.GetStr("or_p_cancle");
                    LblCancle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
                    BtnPay.gameObject.SetActive(true);
                    LblPay.text = StrConfigProvider.Instance.GetStr("or_p_pay");
                    LblPay.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
                    break;
                case "05":
                    BtnReceipt.gameObject.SetActive(true);
                    LblReceipt.text = StrConfigProvider.Instance.GetStr("or_p_receipt");
                    LblReceipt.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
                    break;
                case "06":
                    BtnAppraise.gameObject.SetActive(true);
                    LblAppraise.text = StrConfigProvider.Instance.GetStr("or_p_appraise");
                    LblAppraise.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
                    break;
                case "01":
                    BtnRemind.gameObject.SetActive(true);
                    break;
            }
        }
    }
}