using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth3D.Ui {
    public class AgreementCtr : IController{
        private AgreementViewPresenter _view;
        public bool Init() {
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<AgreementViewPresenter>(WindowId.Agreement);
                _view.ClickBack += ClickBackBtn;
            }
            UiManager.Instance.ShowWindow(WindowId.Agreement);
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
            UiManager.Instance.HideWindow(WindowId.Agreement);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Agreement, "ui/Agreement"),
            };
        }
        private void ClickBackBtn() {
            OnBack(-1);
        }
    }
}
