using System;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ForgetPassWordCtr : IController{
        private ForgetPassWordViewPresenter _view;
        private ILoginMgr _loginMgr;
        public bool Init() {
            //_loginmgr
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (_loginMgr == null) {
                LogSystem.Error("login module is null.");
                return false;
            }
            _loginMgr.OnGetRestPwCode += OnGetResetPwCode;
            _loginMgr.OnResetPassword += OnRestPassword;
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<ForgetPassWordViewPresenter>(WindowId.ForgetPassWord);
                _view.ClickBack += OnBackClick;
                _view.ClickCaptcha += OnGetVerificationClick;
                _view.ClickReSet += OnReSetPwClick;
            }
            UiManager.Instance.ShowWindow(WindowId.ForgetPassWord);
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.ForgetPassWord);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.ForgetPassWord, "ui/ForgetPassWord"),
            };
        }
        void OnDestroy() {
            //_loginmgr
            _loginMgr.OnGetRestPwCode -= OnGetResetPwCode;
            _loginMgr.OnResetPassword -= OnRestPassword;
        }

        // Update is called once per frame
        public void Update() {
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnReSetPwClick(string phone, string passWord, string passWord2, string captcha) {
            if ((!string.IsNullOrEmpty(phone) &&
                                !string.IsNullOrEmpty(passWord)
                                && !string.IsNullOrEmpty(passWord2) &&
                                !string.IsNullOrEmpty(captcha))) {
                if (passWord != passWord2) {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_passwordiffer"));
                } else {
                    _loginMgr.RestPassword(phone, passWord, captcha);
                }
            }
        }

        private void OnRestPassword(bool result) {
            //重置密码
            if (result) {
                _view.ResetCountdown();
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_modifypassword"));
                UiManager.Instance.ShowWindow(WindowId.Login);
            } else {
                _view.ResetCountdown();
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_iderror")); 
            }
        }

        private void OnGetVerificationClick(string phone) {
            if (!string.IsNullOrEmpty(phone)) {
                if (LoginUtils.IsHandset(phone)) {
                    _loginMgr.GetResetPwCode(phone);
                }
            }
        }

        private  void OnGetResetPwCode(bool result) {
            if (result) {
                _view.ShowCountdown();
            } else {
                _view.ResetCountdown();
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_notregister"));
            }
        }
    }
}