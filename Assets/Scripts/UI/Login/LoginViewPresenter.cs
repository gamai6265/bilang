using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class LoginViewPresenter : ViewPresenter/*, IWindowAnimation*/{
        public UILabel LblTitle;
        public UILabel LblLogin;
        public UILabel LblUser;
        public UILabel LblPw;

        public UIButton BtnBack;
        public UIButton BtnForgetPw;
        public UIButton BtnLogin;
        public UIButton BtnRegister;
        public UIButton BtnShow;
        public UIInput IpPw;

        public UIInput IpUser;

        public TweenPosition LoginTwe;
        private Vector3 _from;
        private Vector3 _to;

        public event MyAction ClickBack;
        public event MyAction ClickLogin;
        public event MyAction ClickRegister;
        public event MyAction ClickForgetPw;

        public string GetPassword() {
            return IpPw.value.Trim();
        }

        public string GetUserName() {
            return IpUser.value.Trim();
        }

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
            _from = LoginTwe.from;
            _to = LoginTwe.to;
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }
        protected override void OnDestroyUnityMsg() {
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("l_title");
            LblLogin.text = StrConfigProvider.Instance.GetStr("l_login");
            LblUser.text = StrConfigProvider.Instance.GetStr("l_phone");
            LblPw.text = StrConfigProvider.Instance.GetStr("l_password");
            BtnRegister.GetComponentInChildren<UILabel>().text = StrConfigProvider.Instance.GetStr("l_register");
            BtnForgetPw.GetComponentInChildren<UILabel>().text = StrConfigProvider.Instance.GetStr("l_forgetpw");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblLogin.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblUser.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPw.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnRegister.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnForgetPw.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnLogin.onClick.Add(new EventDelegate(OnBtnLoginClick));
            BtnRegister.onClick.Add(new EventDelegate(OnBtnRegisterClick));
            BtnForgetPw.onClick.Add(new EventDelegate(OnBtnForgetPwClick));
            BtnShow.onClick.Add(new EventDelegate(OnBtnShowClick));
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void OnBtnLoginClick() {
            if (null != ClickLogin) {
                ClickLogin();
            }
        }

        private void OnBtnRegisterClick() {
            if (null != ClickRegister) {
                ClickRegister();
            }
        }

        private void OnBtnForgetPwClick() {
            if (null != ClickForgetPw) {
                ClickForgetPw();
            }
        }

        private void OnBtnShowClick() {
            _ShowPassword();
        }

        /*public void PlayAnimation(bool tof = false) {
            if (tof) {
                transform.localPosition = _from;
                TweenPosition.Begin(gameObject, LoginTwe.duration, _to);
            } else {
                transform.localPosition = _to;
                TweenPosition.Begin(gameObject, LoginTwe.duration, _from);
            }
        }*/

        private void _ShowPassword() {
            LogSystem.Debug("{0}",IpPw.inputType);
            IpPw.inputType = IpPw.inputType == UIInput.InputType.Password
                ? UIInput.InputType.Standard
                : UIInput.InputType.Password;
            IpPw.UpdateLabel();
        }

        public void ClearAllLabel() {
            IpUser.value = null;
            IpPw.value = null;
        }
        /*
        public override void ShowWindow(MyAction onComplete = null) {
            ResetAnimation();
            //base.ShowWindow(onComplete);
            ShowWindowImmediately();
            EnterAnimation(() => {
                if (onComplete != null)
                    onComplete();
            });
        }

        public override void HideWindow(MyAction onComplete = null) {
            QuitAnimation(() => {
                base.HideWindow(onComplete);
            });
        }

        public void EnterAnimation(EventDelegate.Callback onComplete) {
            transform.localPosition = _from;
            TweenPosition.Begin(gameObject, LoginTwe.duration, _to);
            if (onComplete != null)
                EventDelegate.Set(LoginTwe.onFinished, onComplete);
        }

        public void QuitAnimation(EventDelegate.Callback onComplete) {
            transform.localPosition = _to;
            TweenPosition.Begin(gameObject, LoginTwe.duration, _from);
            if (onComplete != null)
                EventDelegate.Set(LoginTwe.onFinished, onComplete);
        }

        public void ResetAnimation() {
        }*/
    }
}