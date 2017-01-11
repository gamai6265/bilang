using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cloth3D.Ui {
    public class BrandStoryCtr : IController {
        private BrandStoryViewPresenter _view;

        public bool Init() {
            EventCenter.Instance.AddListener<WindowId>(EventId.BtnsEvent, BtnsEvent);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<BrandStoryViewPresenter>(WindowId.BrandStory);
                _view.ClickBack += OnBackClick;
            }
            UiManager.Instance.ShowWindow(WindowId.BrandStory);
            var btns = (HomeBtnsCtr) UiControllerMgr.Instance.GetController(WindowId.HomeBtns);
            btns.ShowBtns(WindowId.BrandStory, 1);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.Home).Show();
                return true;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.BrandStory);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.BrandStory, "ui/BrandStory"),
            };
        }

        private void BtnsEvent(WindowId id) {
            if (WindowId.BrandStory == id) {
                Show();
            } else {
                Hide();
            }
        }

        void OnBackClick() {
            OnBack(-1);
        }
    }
}
