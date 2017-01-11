using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D.Ui{
    class MessageBoxCtr :IController {
        private MessageBoxPresenter _view;
        private event MyAction _leftAction;
        private event MyAction _rightAction;
        public bool Init() {
            return true;
        }
        public void Show(MyAction onComplete = null) {
            InitView();
            UiManager.Instance.ShowWindow(WindowId.MessageBox);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.MessageBox);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.MessageBox, "ui/MessageBox"),
            };
        }

        public void ShowMessageBox(string title, string msg, string leftStr, MyAction leftAction, string rightStr, MyAction rightAction) {
            InitView();
            _view.SetMsg(title, msg, leftStr, rightStr);
            _leftAction = leftAction;
            _rightAction = rightAction;
            Show();
        }

        private void InitView() {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<MessageBoxPresenter>(WindowId.MessageBox);
                _view.ClickRight += OnRightClick;
                _view.ClickLeft += OnLeftClick;
            }
        }

        private void OnLeftClick() {
            Hide();
            if (_leftAction != null) {
                _leftAction();
                _leftAction = null;
            }
        }

        private void OnRightClick() {
            Hide();
            if (_rightAction != null) {
                _rightAction();
                _rightAction = null;
            }
        }
    }
}
