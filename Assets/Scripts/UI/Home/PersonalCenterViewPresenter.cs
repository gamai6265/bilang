using System;
using UnityEngine;
using System.Collections;
using Cloth3D;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class PersonalCenterViewPresenter : ViewPresenter ,IWindowAnimation {
        public UILabel LblName;
//        public UILabel LblInfo;
        public UILabel LblLogout;
        public UILabel LblQuit;
        public UILabel LblOrder;
        public UILabel LblSize;
        public UILabel LblCollect;
        public UILabel LblShare;
        public UILabel LblWallet;

        public UITexture TexHeadPortrait;

        public UIButton BtnInfo;
        public UIButton BtnOrders;
        public UIButton BtnSize;
        public UIButton BtnCollect;
        public UIButton BtnLogout;
        public UIButton BtnQuit;
        public UIButton BtnBack;
        public UIButton BtnShare;
        public UIButton BtnWallet;

        public TweenPosition TwPersonalCenter;
        private Vector3 _from;
        private Vector3 _to;

        public event MyAction ClickInfo;
        public event MyAction ClickOrders;
        public event MyAction ClickSize;
        public event MyAction ClickCollect;
        public event MyAction ClickLogout;
        public event MyAction ClickQuit;
        public event MyAction ClickBack;
        public event MyAction ClickShare;
        public event MyAction ClickWallet;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
            _from = TwPersonalCenter.from;
            _to = TwPersonalCenter.to;
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public override void ShowWindow(MyAction onComplete = null) {
            LogSystem.Debug("PersonalCenterViewPresenter.ShowWindow");
            ResetAnimation();
            ShowWindowImmediately();
            EnterAnimation(()=> {
                if (onComplete != null)
                    onComplete();
            });
        }

        public override void HideWindow(MyAction action = null) {
            QuitAnimation(() => {
                base.HideWindow(action);
            });
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        protected override void OnDestroyUnityMsg() {
        }

        private void ChangeLanguage() {
//            LblInfo.text = StrConfigProvider.Instance.GetStr("pc_info");
            LblLogout.text = StrConfigProvider.Instance.GetStr("pc_logout");
            LblQuit.text = StrConfigProvider.Instance.GetStr("pc_quit");
            LblOrder.text = StrConfigProvider.Instance.GetStr("pc_order");
            LblSize.text = StrConfigProvider.Instance.GetStr("pc_size");
            LblCollect.text = StrConfigProvider.Instance.GetStr("pc_collect");
            LblShare.text = StrConfigProvider.Instance.GetStr("pc_share");
            LblWallet.text = StrConfigProvider.Instance.GetStr("pc_wallet");

//            LblInfo.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblLogout.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblQuit.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblOrder.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSize.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCollect.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblShare.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblWallet.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnInfo.onClick.Add(new EventDelegate(OnBtnInfoClick));
            BtnOrders.onClick.Add(new EventDelegate(OnBtnOrdersClick));
            BtnSize.onClick.Add(new EventDelegate(OnBtnSizeClick));
            BtnCollect.onClick.Add(new EventDelegate(OnBtnCollectClick));
            BtnLogout.onClick.Add(new EventDelegate(OnBtnLogoutClick));
            BtnQuit.onClick.Add(new EventDelegate(OnBtnQuitClick));
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnShare.onClick.Add(new EventDelegate(OnBtnShareClick));
            BtnWallet.onClick.Add(new EventDelegate(OnBtnWebClick));
        }
        private void OnBtnShareClick() {
            if (null != ClickShare) {
                ClickShare();
            }
        }
        private void OnBtnInfoClick() {
            if (null != ClickInfo) {
                ClickInfo();
            }
        }
        private void OnBtnOrdersClick() {
            if (null != ClickOrders) {
                ClickOrders();
            }
        }
        private void OnBtnSizeClick() {
            if (null != ClickSize) {
                ClickSize();
            }
        }
        private void OnBtnCollectClick() {
            if (null != ClickCollect) {
                ClickCollect();
            }
        }
        private void OnBtnLogoutClick() {
            if (null != ClickLogout) {
                ClickLogout();
            }
        }
        private void OnBtnQuitClick() {
            if (null != ClickQuit) {
                ClickQuit();
            }
        }
        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        public void OnBtnWebClick() {
            if (null != ClickWallet) {
                ClickWallet();
            }
        }

        public void SetUserInfo(string str, Texture2D tex) {
            LblName.text = str;
            SetHead(tex, str);
        }

        public void SetHead(Texture2D texture2D, string userName) {
            TexHeadPortrait.mainTexture = texture2D;
            LblName.text = userName;
        }

        public void EnterAnimation(EventDelegate.Callback onComplete) {
            transform.localPosition = _from;
            TweenPosition.Begin(gameObject, TwPersonalCenter.duration, _to);
            if (onComplete != null) 
                EventDelegate.Set(TwPersonalCenter.onFinished, onComplete);
        }

        public void QuitAnimation(EventDelegate.Callback onComplete) {
            transform.localPosition = _to;
            TweenPosition.Begin(gameObject, TwPersonalCenter.duration, _from);
            if (onComplete != null)
                EventDelegate.Set(TwPersonalCenter.onFinished, onComplete);
        }

        public void ResetAnimation() {

        }
    }
}