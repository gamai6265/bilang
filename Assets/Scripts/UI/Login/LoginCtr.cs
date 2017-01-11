using System;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class LoginCtr :IController {
        private LoginViewPresenter _view;
        private ILoginMgr _loginMgr;
        private IUserCenter _userCenter;
        public bool Init() {
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (null == _loginMgr) {
                LogSystem.Error("erro, login mgr is null.");
                return false;
            }
            //loginmgr
            //_loginMgr.OnLoginSucc += OnLoginSucc;
            _loginMgr.OnLoginFailed += OnLoginFailed;
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (null == _userCenter) {
                LogSystem.Error("user center is null.");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            LogSystem.Debug("openLogin");
            if (null == _view) {
                _view = UiManager.Instance.GetWindow<LoginViewPresenter>(WindowId.Login);
                _view.ClickBack += OnBackClick;
                _view.ClickLogin += OnLoginClick;
                _view.ClickRegister += OnRegisterClick;
                _view.ClickForgetPw += OnForgetPwClick;
            }
            UiManager.Instance.ShowWindow(WindowId.Login, Zorder.TopMost, 0, onComplete);
            _view.ClearAllLabel();
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                LogSystem.Debug("OnBackClick");
                EventCenter.Instance.Broadcast(EventId.LoginDone, false);
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.Login, false, onComplete);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Login, "ui/Login")
            };
        }

        // Use this for initialization

        void OnDestroy() {
            //loginmgr
            //_loginMgr.OnLoginSucc -= OnLoginSucc;
            //_loginMgr.OnLoginFailed -= OnLoginFailed;
        }
        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnLoginClick() {
            LogSystem.Debug("OnLoginClick");
            if (string.IsNullOrEmpty(_view.GetUserName()) || !LoginUtils.IsHandset(_view.GetUserName())) {
                LogSystem.Warn("username erro.");
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_usernameerror"));
            } else if (string.IsNullOrEmpty(_view.GetPassword())) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_passwordnull"));
                LogSystem.Warn("password is null.");
            } else {
                if (null != _loginMgr) {
                    _loginMgr.Login(_view.GetUserName(), _view.GetPassword());
                    _userCenter.OnUserInfoUpdate += OnLoginSucc;
                } else {
                    LogSystem.Error("loing mgr is null");
                }
            }
        }

        private void OnRegisterClick() {
            UiControllerMgr.Instance.GetController(WindowId.RegistrVerification).Show();
            LogSystem.Debug("OnRegisterClick");
        }

        private void OnForgetPwClick() {
            LogSystem.Debug("OnForgetPwClick");
            UiControllerMgr.Instance.GetController(WindowId.ForgetPassWord).Show();
        }

        private void OnLoginSucc(UserInfo info) {
            _userCenter.OnUserInfoUpdate -= OnLoginSucc;
            if (info != null && _loginMgr.IsLogined()) {
                EventCenter.Instance.Broadcast(EventId.LoginDone, true);
                PlayerPrefs.DeleteKey(Constants.UserName);
                PlayerPrefs.DeleteKey(Constants.PassWord);
                PlayerPrefs.SetString(Constants.UserName, _view.GetUserName());
                PlayerPrefs.SetString(Constants.PassWord, _view.GetPassword());
                Hide();
            }
        }

        private void OnLoginFailed(LoginFailedType type) {
            UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_passworderror"));
        }
    }
}