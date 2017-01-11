using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class RegistrVerificationViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UILabel LblRegistr;
        public UILabel LblVerification;
        public UILabel LblPhone;
        public UILabel LblCaptcha;
        public UILabel LblPw;

        public UIButton BtnBack;
        public UIButton BtnRegistr;
        public UIButton BtnVerification;
        public UIButton BtnShow;
        public UIInput IpCaptcha;
        public UIInput IpPassWord;

        public UIInput IpPhone;

        private int _leaveSeconds = 60;

        public event MyAction ClickBack;
        public event MyAction<string,string,string> ClickVerification;
        public event MyAction<string> ClickCaptcha;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
        protected override void StartUnityMsg() {
            BtnsListener();
        }

        protected override void OnDestroyUnityMsg() {
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("r_title");
            LblRegistr.text = StrConfigProvider.Instance.GetStr("r_registr");
            LblVerification.text = StrConfigProvider.Instance.GetStr("r_verification");
            LblPhone.text = StrConfigProvider.Instance.GetStr("r_phone");
            LblCaptcha.text = StrConfigProvider.Instance.GetStr("r_captcha");
            LblPw.text = StrConfigProvider.Instance.GetStr("r_password");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblRegistr.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblVerification.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect(); 
            LblCaptcha.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPw.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnVerification.onClick.Add(new EventDelegate(OnBtnVerificationClick));
            BtnRegistr.onClick.Add(new EventDelegate(OnBtnCaptchaClick));
            BtnShow.onClick.Add(new EventDelegate(OnBtnShowClick));
        }

        private void OnBtnCaptchaClick() {
            LogSystem.Debug("BtnCaptcha click");
            if (null != ClickCaptcha) {
                ClickCaptcha(IpPhone.value.Trim());
            }
        }

        private void OnBtnVerificationClick() {
            if (null != ClickVerification) {
                ClickVerification(IpPhone.value.Trim(), IpCaptcha.value.Trim(), IpPassWord.value.Trim());
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }
        
        private void OnBtnShowClick() {
            IpPassWord.inputType = IpPassWord.inputType == UIInput.InputType.Password
                ? UIInput.InputType.Standard
                : UIInput.InputType.Password;
            IpPassWord.UpdateLabel();
        }

        private void DoCountDown() {
            _leaveSeconds--;
            BtnRegistr.GetComponentInChildren<UILabel>().text = _leaveSeconds + "秒后再次获取";
            if (_leaveSeconds == 0) {
                ResetCountdown();
            }
        }

        public void ShowCountdown() {
            BtnRegistr.GetComponent<Collider>().enabled = false;
            BtnRegistr.GetComponentInChildren<UILabel>().color = Color.gray;
            IpPhone.GetComponent<Collider>().enabled = false;
            InvokeRepeating("DoCountDown", 1f, 1f);
        }

        public void ResetCountdown() {
            BtnRegistr.GetComponentInChildren<UILabel>().text = "获取验证码";
            BtnRegistr.GetComponent<Collider>().enabled = true;
            BtnRegistr.GetComponentInChildren<UILabel>().color = new Color(35f / 255f, 68f / 255f, 151f / 255f);
            IpPhone.GetComponent<Collider>().enabled = true;
            _leaveSeconds = 60;
            CancelInvoke("DoCountDown");
        }
    }
}