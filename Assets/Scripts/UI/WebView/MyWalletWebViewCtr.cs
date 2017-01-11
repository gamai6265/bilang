using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D;
using Cloth3D.Ui;

public class MyWalletWebViewCtr : IController {


    private MyWalletWebViewViewPresenter _view;

    public bool Init() {
        EventCenter.Instance.AddListener(EventId.MyWallet,CloseMyWaller);
        return true;
    }

    public void Show(MyAction onComplete = null) {
        if (_view == null) {
            _view = UiManager.Instance.GetWindow<MyWalletWebViewViewPresenter>(WindowId.MyWalletWebView);
            _view.ClickBack += OnBackClick;
        }
        UiManager.Instance.ShowWindow(WindowId.MyWalletWebView);
    }

    public bool OnBack(int zorder) {
        if (zorder == 0 || zorder == -1) {
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
            //  UniWebDemo.Instance.CloseWeb();
            // Hide();
#endif
            return true;
        }
        return false;
    }

    public void Hide(MyAction onComplete = null) {
        UiManager.Instance.HideWindow(WindowId.MyWalletWebView);
    }

    public List<KeyValuePair<WindowId, string>> GetWinLst() {
        return new List<KeyValuePair<WindowId, string>>() {
               new KeyValuePair<WindowId, string>(WindowId.MyWalletWebView, "ui/MyWalletWebView"),
            };
    }

    public void Update() {

    }
    void OnBackClick() {
       UniWebMyWallet.Instance.OnBack();
    }

    private void CloseMyWaller() {
       Hide();
    }
}
