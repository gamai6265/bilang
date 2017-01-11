using Cloth3D;
using Cloth3D.Ui;
using UnityEngine;
public enum WebType {
    home,
    DesignersRecommend,
}

public class UniWebDemo : MonoBehaviour {

    public static UniWebDemo Instance;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 ||UNITY_EDITOR
    private UniWebView _webView;
    private WebType _webType;
    
    void Awake() {
        Instance = this;
    }
    void Start() {

    }
    void OnDestroy() {
        Instance = null;
    }
    #region
    public void OpenWeb(string url, WebType type) {
        _webView = GetComponent<UniWebView>();
        if (_webView == null) {
            _webView = gameObject.AddComponent<UniWebView>();
            _webView.OnLoadComplete += OnLoadComplete;
            _webView.OnWebViewShouldClose += OnWebViewShouldClose;
            _webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
        }
        _webView.url = url;
        _webType = type;
        //  UniWebView.SetUserAgent("app");
        //
        //#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
        _webView.Load();
//#endif
    }
    #endregion
    #region
    public void CloseWeb() {
        _webView.Hide();
        _webView.CleanCache();
        Destroy(_webView);
        _webView.OnLoadComplete -= OnLoadComplete;
        _webView.OnWebViewShouldClose -= OnWebViewShouldClose;
        _webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
        _webView = null;
    }
    #endregion

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
        if (success) {
            webView.Show();
        } else {
            LogSystem.Error("Something wrong in webview loading:{0}",errorMessage);
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
            EventCenter.Instance.Broadcast(EventId.WebView);
            return true;
        }
        return false;
    }

    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {
        int bottomInset = (int)(UniWebViewHelper.screenHeight * 0.5f);

        if (orientation == UniWebViewOrientation.Portrait) {
            if (_webType == WebType.home) {
                return new UniWebViewEdgeInsets((int)(UniWebViewHelper.screenHeight * 0.06f), 0, 0, 0);
            }
            else if(_webType == WebType.DesignersRecommend) {
                return new UniWebViewEdgeInsets((int) (UniWebViewHelper.screenHeight*0.06f), 0,
                    (int) (UniWebViewHelper.screenHeight*0.085f), 0);
            }
            else {
                return new UniWebViewEdgeInsets(0, 0, 0, 0);
            }
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
