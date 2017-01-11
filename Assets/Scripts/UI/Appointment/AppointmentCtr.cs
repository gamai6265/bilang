using System.Collections.Generic;
using System.Diagnostics;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class AppointmentCtr : IController{
        private AppointmentViewPresenter _view;
        public bool Init() {
            EventCenter.Instance.AddListener<WindowId>(EventId.BtnsEvent, BtnsEvent);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<AppointmentViewPresenter>(WindowId.Appointment);
                _view.ClickClose += OnCloseClick;
                _view.ClickCall += OnCallClick;
            }
            UiManager.Instance.ShowWindow(WindowId.Appointment);
            var btns = (HomeBtnsCtr) UiControllerMgr.Instance.GetController(WindowId.HomeBtns);
            btns.ShowBtns(WindowId.Appointment, 1);
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.Home).Show();
                return true;                    
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.Appointment);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Appointment, "ui/Appointment"),
            };
        }
        
        private void BtnsEvent(WindowId id) {
            if (WindowId.Appointment == id) {
                Show();
            } else {
                Hide();
            }
        }
        void OnCloseClick() {
            OnBack(-1);
        }

        void OnCallClick() {
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic) {
                if (null != logic.OnBeforeStageEnter) {
                    logic.OnBeforeStageEnter(StageType.StageNone);
                }
                SDKContorl.Instance.CallPhone(logic.Config.GetSetting("Misc", "phone"));
            }
        }
    }
}