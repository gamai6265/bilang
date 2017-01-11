using System.Collections.Generic;
using Cloth3D.Interfaces;

namespace Cloth3D.Ui {
    public class ChooseSizeTypeCtr : IController {
        private ChooseSizeTypeViewPresenter _view;

        public bool Init() {
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<ChooseSizeTypeViewPresenter>(WindowId.ChooseSizeType);
                _view.ClickAppointment += OnClickAppointment;
                _view.ClickMySize += OnClickMySize;
                _view.ClickBack += OnClickBack;
            }
            UiManager.Instance.ShowWindow(WindowId.ChooseSizeType);
        }
        
        public void Update() {
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.ChooseSizeType);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.ChooseSizeType, "ui/ChooseSizeType"),
            };
        }
        
        private void OnClickBack() {
            OnBack(-1);
        }

        private void OnClickMySize() {
            Hide();
            UiControllerMgr.Instance.GetController(WindowId.MySize).Show();
        }

        private void OnClickAppointment() {
            Hide();
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