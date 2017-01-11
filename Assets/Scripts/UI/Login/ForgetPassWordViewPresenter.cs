using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ForgetPassWordViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UILabel LblReSet;
        public UILabel LblPhone;
        public UILabel LblGetCaptcha;
        public UILabel LblCaptcha;
        public UILabel LblPw;
        public UILabel LblPw2;
        public UILabel LblTip;

        public UIButton BtnBack;
        public UIButton BtnCaptcha;
        public UIButton BtnReSet;
        public UIButton BtnShow;
        public UIInput IpCaptcha;
        public UIInput IpPassWord;
        public UIInput IpPassWord2;

        public UIInput IpPhone;

        private int _leaveSeconds = 60;

        public event MyAction ClickBack;
        public event MyAction<string,string,string,string> ClickReSet;
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
            LblTitle.text = StrConfigProvider.Instance.GetStr("fp_title");
            LblReSet.text = StrConfigProvider.Instance.GetStr("fp_reset");
            LblPhone.text = StrConfigProvider.Instance.GetStr("fp_phone");
            LblGetCaptcha.text = StrConfigProvider.Instance.GetStr("fp_getcaptcha");
            LblCaptcha.text = StrConfigProvider.Instance.GetStr("fp_captcha");
            LblPw.text = StrConfigProvider.Instance.GetStr("fp_pw");
            LblPw2.text = StrConfigProvider.Instance.GetStr("fp_pw2");
            LblTip.text = StrConfigProvider.Instance.GetStr("fp_tip");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblReSet.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblGetCaptcha.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCaptcha.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPw.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPw2.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblTip.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnCaptcha.onClick.Add(new EventDelegate(OnBtnCaptchaClick));
            BtnShow.onClick.Add(new EventDelegate(OnBtnShowClick));
            BtnReSet.onClick.Add(new EventDelegate(OnBtnReSetClick));
        }

        private void OnBtnReSetClick() {
            if (null != ClickReSet) {
                ClickReSet(IpPhone.value.Trim(), IpPassWord.value.Trim(), IpPassWord2.value.Trim(),
                    IpCaptcha.value.Trim());
            }
        }

        private void OnBtnShowClick() {
            ShowPassword();
        }

        private void OnBtnCaptchaClick() {
            if (null != ClickCaptcha) {
                ClickCaptcha(IpPhone.value.Trim());
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void ShowPassword() {
            IpPassWord.inputType = IpPassWord.inputType == UIInput.InputType.Password
                ? UIInput.InputType.Standard
                : UIInput.InputType.Password;
            IpPassWord2.inputType = IpPassWord2.inputType == UIInput.InputType.Password
                ? UIInput.InputType.Standard
                : UIInput.InputType.Password;
            IpPassWord.UpdateLabel();
            IpPassWord2.UpdateLabel();
        }

        public void ShowCountdown() {
            BtnCaptcha.GetComponent<Collider>().enabled = false;
            BtnCaptcha.GetComponentInChildren<UILabel>().color = Color.gray;
            InvokeRepeating("DoCountDown", 1f, 1f);
        }

        public void ResetCountdown() {
            BtnCaptcha.GetComponentInChildren<UILabel>().text = "获取验证码";
            BtnCaptcha.GetComponent<Collider>().enabled = true;
            BtnCaptcha.GetComponentInChildren<UILabel>().color = new Color(35f/255f, 68f/255f, 151f/255f);
            IpPhone.GetComponent<Collider>().enabled = true;
            _leaveSeconds = 60;
            CancelInvoke("DoCountDown");
        }

        private void DoCountDown() {
            _leaveSeconds--;
            BtnCaptcha.GetComponentInChildren<UILabel>().text = _leaveSeconds + "秒后再次获取";
            if (_leaveSeconds == 0) {
                ResetCountdown();
            }
        }
    }
}