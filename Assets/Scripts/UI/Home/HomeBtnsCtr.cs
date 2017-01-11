using System;
using System.Collections.Generic;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public enum HomeCtrBtn {
        BtnHome,
        BtnShopcar,
        BtnDesign,
        BtnAppointment,
        BtnStory,
        TotalBtns
    }

    public class HomeBtnsCtr : IController{
        private HomeBtnsViewPresenter _view;
        private ILoginMgr _loginMgr;
        private ILogicModule _logic;
        private INetworkMgr _networkMgr;

        public bool Init() {
            //EventCenter.Instance.AddListener<WindowId, int>(EventId.ShowBtns, ShowBtns);
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (_loginMgr == null) {
                LogSystem.Error("login mgr is null");
                return false;
            }
            _logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (_logic == null) {
                LogSystem.Error("erro. logic is null.");
                return false;
            }
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("network is null.");
                return false;
            }
            EventCenter.Instance.AddListener(EventId.LeaveStageEmpty, () => Hide());
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (Constants.HomeBtnsShow) {
                if (_view == null) {
                    _view = UiManager.Instance.GetWindow<HomeBtnsViewPresenter>(WindowId.HomeBtns);
                    _view.ClickHome += OnHomeClick;
                    _view.ClickShopCar += OnShopcarClick;
                    _view.ClickDesign += OnDesignClick;
                    _view.ClickAppointment += OnAppointmentClick;
                    _view.ClickStory += OnStoryClick;
                }
                UiManager.Instance.ShowWindow(WindowId.HomeBtns);
                _view.ChangeBtnSprite(0);
            }
            else {
                Hide();
            }
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.HomeBtns);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.HomeBtns, "ui/HomeBtns"),
            };
        }

        public void PlayAnimation() {
            _view.PlayAnimation(false);
        }

        public void ReverseAnimation() {
            _view.PlayAnimation(true);
        }

        public void ShowBtns(WindowId refWinId, int offset) {
            Show();
            if (refWinId == WindowId.Home) {
                _view.ChangeBtnSprite((int)HomeCtrBtn.BtnHome);
            } else if (refWinId == WindowId.BrandStory) {
                _view.ChangeBtnSprite((int)HomeCtrBtn.BtnStory);
            }else if (refWinId == WindowId.Appointment) {
                _view.ChangeBtnSprite((int)HomeCtrBtn.BtnAppointment);
            }else if (refWinId == WindowId.ShopCar) {
                _view.ChangeBtnSprite((int)HomeCtrBtn.BtnShopcar);
            }
            UiManager.Instance.AdjustZorderBaseAnother(_view, refWinId, offset);
        }
        private void OnHomeClick() {
            Constants.HomeShow = true;
            EventCenter.Instance.Broadcast(EventId.BtnsEvent, WindowId.Home);
        }

        private void OnStoryClick() {
            Constants.HomeShow = true;
            EventCenter.Instance.Broadcast(EventId.BtnsEvent, WindowId.BrandStory);
        }

        private void OnAppointmentClick() {
            Constants.HomeShow = true;
            EventCenter.Instance.Broadcast(EventId.BtnsEvent, WindowId.Appointment);
        }

        private void OnDesignClick() {
            Constants.HomeShow = true;
            _networkMgr.QuerySender<IInfoSender>().SendGetDefaultProductInfo("1");
            UiManager.Instance.HideAllShownWindow(); //TODO danie 
//            UiControllerMgr.Instance.GetController(WindowId.ChooseModels).Show();
        }

        private void OnShopcarClick() {
            if (_loginMgr.IsLogined()) {
                Constants.IsShopCar = true;
                EventCenter.Instance.Broadcast(EventId.BtnsEvent, WindowId.ShopCar);
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        Constants.IsShopCar = true;
                        EventCenter.Instance.Broadcast(EventId.BtnsEvent, WindowId.ShopCar);
                    } else {
                        EventCenter.Instance.Broadcast(EventId.BtnsEvent, WindowId.Home);
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
                UiControllerMgr.Instance.GetController(WindowId.Home).Hide();//TODO danie 这里不一定是home在上面，无故被人hide了
            }
        }
    }
}