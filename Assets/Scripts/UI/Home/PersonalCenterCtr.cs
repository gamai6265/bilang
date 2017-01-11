using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class PersonalCenterCtr : IController{
        private PersonalCenterViewPresenter _view;
        private IUserCenter _user;
        private ILoginMgr _loginMgr;
        private IInput _input;

        private string url = "https://www.baidu.com/";
           
        public bool Init() {
            EventCenter.Instance.AddListener(EventId.ClosePersonalCenter,ClosePersonalCenter);
            _user = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_user == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (null == _loginMgr) {
                LogSystem.Error("login mgr is null");
                return false;
            }
            _input = ComponentMgr.Instance.FindComponent(ComponentNames.ComInput)as IInput;
            if (_input == null) {
                LogSystem.Error("erro. input is null");
                return false;
            }
            EventCenter.Instance.AddListener<Texture2D, string>(EventId.ModifyUserInfo, SetHeadPortrait);
            EventCenter.Instance.AddListener(EventId.MovePersonalCenterMoveBack, OnBackClick);
            return true;
        }

        public void Show(MyAction onComplete=null) {
             LogSystem.Debug("PersonalCenterCtr.OpenPersonalCenter");
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<PersonalCenterViewPresenter>(WindowId.PersonalCenter);
                _view.ClickInfo += OnInfoClick;
                _view.ClickOrders += OnOrdersClick;
                _view.ClickSize += OnSizeClick;
                _view.ClickCollect += OnCollectClick;
                _view.ClickLogout += OnLogoutClick;
                _view.ClickQuit += OnQuitClick;
                _view.ClickBack += OnBackClick;
                _view.ClickShare += OnClickShare;
                _view.ClickWallet += OnClickWallet;
            }
            UiManager.Instance.ShowWindow(WindowId.PersonalCenter);
            EventCenter.Instance.Broadcast(EventId.OpenPersonalCenter);
            CoroutineMgr.Instance.StartCoroutine(GetUserInfo());
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                LogSystem.Debug("personal cener back.");
                EventCenter.Instance.Broadcast(EventId.ClosePersonalCenter);
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.PersonalCenter);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.PersonalCenter, "ui/PersonalCenter"),
            };
        }

        private IEnumerator GetUserInfo() {
            var www = new WWW(_user.UserInfo.Photo);
            yield return www;
            Texture2D texture = null;
            if (www.error == null) {
                if (www.isDone) {
                    texture = www.texture;
                }
            }
            else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                texture = www.texture;
            }
            www.Dispose();
            _user.UserInfo.Head = texture;
            _view.SetUserInfo(_user.UserInfo.UserName, texture);
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnClickShare() {
            _user.ShareUrl();
        }

        private void SetHeadPortrait(Texture2D texture2D, string userName) {
            _view.SetHead(texture2D, userName);    
        }

        private void OnQuitClick() {
            UiControllerMgr.Instance.ShowMessageBox(
                "",
                StrConfigProvider.Instance.GetStr("mb_promptquit"),
                StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    PlayerPrefs.DeleteKey(Constants.Token);
                    Application.Quit();
                }
            );
        }

        private void ClosePersonalCenter() {
                  Hide();
        }

        private void OnLogoutClick() {
            LoginBoolDelegate logoutAction = null;
            logoutAction = (result) => {
                if (result) {
                    EventCenter.Instance.Broadcast(EventId.ClosePersonalCenter);
                    Hide();
                } else {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_logout"));
                }
            };
            _loginMgr.OnLogout += logoutAction;
            _loginMgr.Logout();
        }

        private void OnCollectClick() {
            UiControllerMgr.Instance.GetController(WindowId.Collect).Show();
        }

        private void OnSizeClick() {
            //           UiControllerMgr.Instance.GetController(WindowId.ChooseSizeType).Show();
            UiControllerMgr.Instance.GetController(WindowId.MySize).Show();
        }

        private void OnOrdersClick() {
            UiControllerMgr.Instance.GetController(WindowId.Orders).Show();
        }

        private void OnInfoClick() {
            UiControllerMgr.Instance.GetController(WindowId.UserInfo).Show();
        }

        private void OnClickWallet() {
            //TODO 打开网页后如果手机屏幕关闭或者软件在后台运行，打开后网页不能返回unity
            UniWebMyWallet.Instance.OpenWeb(url);
            UiControllerMgr.Instance.GetController(WindowId.MyWalletWebView).Show();
        }
    }
}
