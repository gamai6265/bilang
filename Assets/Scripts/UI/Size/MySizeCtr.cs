using UnityEngine;
using System.Collections.Generic;
using Cloth3D.Interfaces;

namespace Cloth3D.Ui {

    public class MySizeCtr : IController{
        private MySizeViewPresenter _view;
        private IUserCenter _user;
        public bool Init() {
            _user=ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_user == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<MySizeViewPresenter>(WindowId.MySize);
                _view.ClickBack += OnBackClick;
                _view.ClickSize += OnSizeClick;
            }
            _view.SetSizeLabel(_user.UserInfo.UserSize);
            UiManager.Instance.ShowWindow(WindowId.MySize);
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
            UiManager.Instance.HideWindow(WindowId.MySize);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.MySize, "ui/MySize"),
            };
        }
        
        private void OnBackClick() {
            OnBack(-1);
        }

        void OnSizeClick(MysizeEntry entry) {
            EventCenter.Instance.Broadcast(EventId.MySizeEntry, entry);
            Hide();
        }
    }
}
