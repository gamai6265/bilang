using System;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class SetUserInfoCtr : IController{
        private SetUserInfoViewPresenter _view;
        private ILoginMgr _loginMgr;
        private CodeSet _codeSet;
        public bool Init() {
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (_loginMgr == null) {
                LogSystem.Error("login module is null.");
                return false;
            }
            _loginMgr.OnRegister += OnRegister;
            EventCenter.Instance.AddListener<string>(EventId.CheckRegisterCodeDone, SetPhone);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<SetUserInfoViewPresenter>(WindowId.SetUserInfo);
                _view.ClickBack += OnBackClick;
                _view.ClickRegister += OnRegisterClick;
                _view.ClickShow += OpenShowPasswordClick;
                _view.ClickSex += OpenSex;
                _view.ClickAgreement += OnAgreementClick;
                _view.ChooseSexClick += OnChooseSexClick;
            }
            UiManager.Instance.ShowWindow(WindowId.SetUserInfo);
        }
        
        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.RegistrVerification).Show();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.SetUserInfo);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.SetUserInfo, "ui/SetUserInfo"),
            };
        }

        void OnDestory() {
            _loginMgr.OnRegister -= OnRegister;
        }

        private void SetPhone(string phone) {
            _view.SetPhoneText(phone);
        }

        private void OnChooseSexClick(string codeName) {
            _view.SetSexText(codeName);
            _view.DeleteAll();
        }

        private void OnAgreementClick() {
            UiControllerMgr.Instance.GetController(WindowId.Agreement).Show();
        }

        private void OpenSex() {
            CodeSetProvider.Instance.QueryCodeset("GENDER", OnGetCodeset);
        }
        private void OnGetCodeset(CodeSet codeset) {
            _codeSet = codeset;
            _view.AddSexBtn(codeset);
        }

        private void OpenShowPasswordClick() {
            _view.PassWordShow();
        }

        private void OnRegisterClick(string phone, string passWord, string passWord2, string name,  string sex) {
            if ( (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(passWord)
                                && !string.IsNullOrEmpty(passWord2) && !string.IsNullOrEmpty(name)
                                && !string.IsNullOrEmpty(sex))) {
                if (passWord != passWord2) {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_passwordiffer"));
                } else {
                    if (LoginUtils.IsHandset(phone)) {
                        _loginMgr.Register(phone, _codeSet.GetName2Data()[sex], phone, passWord, name);
                    } else {
                        LogSystem.Warn("username need phone number");
                    }
                }
            }
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnRegister(bool result) {
            LogSystem.Debug("onRegiser" + result);
            if (result) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_registersuccess"));
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            } else {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_registerfail"));
            }
        }
    }
}