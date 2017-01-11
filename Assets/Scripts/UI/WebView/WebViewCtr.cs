using System.Collections.Generic;
using UnityEngine;

namespace Cloth3D.Ui {
    public class WebViewCtr : IController {
        private WebViewViewPresenter _view;

        public bool Init() {
            EventCenter.Instance.AddListener(EventId.WebView,CloseWebView);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<WebViewViewPresenter>(WindowId.WebView);
                _view.ClickBack += OnBackClick;
            }
            UiManager.Instance.ShowWindow(WindowId.WebView);
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
                UniWebDemo.Instance.CloseWeb();
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.Home).Show();
#endif
                return true;
            }
             return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.WebView);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
               new KeyValuePair<WindowId, string>(WindowId.WebView, "ui/WebView"),
            };
        }

        public void Update() {

        }
        void OnBackClick() {
            OnBack(-1);
        }

        private void CloseWebView() {
           Hide();
           UiControllerMgr.Instance.GetController(WindowId.Home).Show();
        }

    }
}
