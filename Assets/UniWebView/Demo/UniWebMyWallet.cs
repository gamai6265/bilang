using UnityEngine;
using Cloth3D;
using Cloth3D.Ui;

public class UniWebMyWallet : MonoBehaviour {
    public static UniWebMyWallet Instance;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8

    private UniWebView _webView;
    void Awake() {
        Instance = this;
    }
    void Start() {

    }
    void OnDestroy() {
        Instance = null;
    }
    public void OpenWeb(string myWallerUrl) {
        _webView = GetComponent<UniWebView>();
        if (_webView == null) {
            _webView = gameObject.AddComponent<UniWebView>();
            _webView.OnLoadComplete += OnLoadComplete;
            _webView.OnWebViewShouldClose += OnWebViewShouldClose;
            _webView.InsetsForScreenOreitation += InsetsForScreenOreitation;           
        }
        _webView.url = myWallerUrl;
        // UniWebView.SetUserAgent("app");
#if UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        _webView.Load();
#endif
    }

    public void CloseWeb() {
        _webView.Hide();
        _webView.CleanCache();
        Destroy(_webView);
        _webView.OnLoadComplete -= OnLoadComplete;
        _webView.OnWebViewShouldClose -= OnWebViewShouldClose;
        _webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
        _webView = null;
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
        if (success) {
            webView.Show();
        } else {
            LogSystem.Error("Something wrong in webview loading:{0}" , errorMessage);
        }
    }

    public void ShowAlertInWebview(float time, bool first) {
        if (first) {
            _webView.EvaluatingJavaScript("sample(" + time + ")");
        }
    }

    bool OnWebViewShouldClose(UniWebView webView) {
        if (webView == _webView) {
            CloseWeb();
            EventCenter.Instance.Broadcast(EventId.MyWallet);
            return true;
        }
        return false;
    }

    public void OnBack() {
        if (_webView.CanGoBack()) {
            _webView.GoBack();
        } else {
             CloseWeb();
             EventCenter.Instance.Broadcast(EventId.MyWallet); 
        }


    }

    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {
        int bottomInset = (int)(UniWebViewHelper.screenHeight * 0.5f);

        if (orientation == UniWebViewOrientation.Portrait) {
            return new UniWebViewEdgeInsets((int)(UniWebViewHelper.screenHeight * 0.06f), 0, 0, 0);
        } else {
            return new UniWebViewEdgeInsets(0, 0, 0, 0);
        }
    }
#else //End of #if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
	void Start() {
		Debug.LogWarning("UniWebView only works on iOS/Android/WP8. Please switch to these platforms in Build Settings.");
	}
#endif

}
