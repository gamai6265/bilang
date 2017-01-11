using System;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class RegistrVerificationCtr : IController{
        private RegistrVerificationViewPresenter _view;
        private ILoginMgr _loginMgr;
        private string _phone;
        public bool Init() {
            //_loginMgr
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (_loginMgr == null) {
                LogSystem.Error("LoginMgr is null.");
                return false;
            }
            _loginMgr.OnGetRegisterCode += OnGetRegisterCode;
            _loginMgr.OnCheckRegisterCode += OnCheckRegisterCode;
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<RegistrVerificationViewPresenter>(WindowId.RegistrVerification);
                _view.ClickBack += OnBackClick;
                _view.ClickVerification += OnCheckRegisterCodeClick;
                _view.ClickCaptcha += OnGetVerifyCodeClick;
            }
            UiManager.Instance.ShowWindow(WindowId.RegistrVerification);
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                LogSystem.Debug("back.");
                Hide();
            return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.RegistrVerification);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.RegistrVerification, "ui/RegistrVerification"),
            };
        }

        private void OnDestroy() {
            //_loginMgr
            _loginMgr.OnGetRegisterCode -= OnGetRegisterCode;
            _loginMgr.OnCheckRegisterCode -= OnCheckRegisterCode;
        }

        private void OnGetVerifyCodeClick(string phone) {
            LogSystem.Debug("OnGetVerifyCodeClick");
            if (!string.IsNullOrEmpty(phone) && LoginUtils.IsHandset(phone)) {
                _loginMgr.GetRegisterCode(phone);
                LogSystem.Debug("tips verify code has sended.");
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_sendid"));
            } else {
                LogSystem.Debug(StrConfigProvider.Instance.GetStr("tip_inputphone"));
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_inputphone"));
            }
        }

        private void OnCheckRegisterCodeClick(string phone, string captcha,string password) {
            LogSystem.Debug("OnCheckRegisterCodeClick....");
            if (!string.IsNullOrEmpty(phone) && LoginUtils.IsHandset(phone)) {
                if (!string.IsNullOrEmpty(captcha)) {
                    if (!string.IsNullOrEmpty(password)) {
                        LogSystem.Debug("CheckRegisterCode....");
                        _phone = phone;
                        _loginMgr.CheckRegisterCode(phone, captcha,password);
                    } else {
                        UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_inputpassword"));
                        LogSystem.Info("please input password.");
                    }
                } else {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_inputid"));
                    LogSystem.Info("please input verifycode.");
                }
            } else {
                LogSystem.Info("please input phone");
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_inputphone"));
            }
        }

        private void OnCheckRegisterCode(bool result) {
            if (result) {
                _view.ResetCountdown();
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            } else {
                LogSystem.Error("verify code is wrong.");
                _view.ResetCountdown();
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_iderror"));
            }
        }
        private void OnGetRegisterCode(bool result) {
            if (result) {
                _view.ShowCountdown();
            } else {
                _view.ResetCountdown();
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_phoneregister"));
                LogSystem.Warn("phone has been registed.");
            }
        }

        private void OnBackClick() {
            OnBack(-1);
        }
    }
}