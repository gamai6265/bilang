using System;
using System.Collections.Generic;
using Cloth3D;
using Cloth3D.Ui;
using LitJson;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;

public class SDKContorl : Singleton<SDKContorl> {
	[DllImport ("__Internal")]
	private static extern void _iosOpenPhotoLibrary();
	[DllImport ("__Internal")]
	private static extern void _iosOpenPhotoAlbums();
	[DllImport ("__Internal")]
	private static extern void _iosOpenCamera();
	[DllImport ("__Internal")]
	private static extern void _iosOpenPhotoLibrary_allowsEditing();    
	[DllImport ("__Internal")]
	private static extern void _iosOpenPhotoAlbums_allowsEditing(); 
	[DllImport ("__Internal")]
	private static extern void _iosOpenCamera_allowsEditing();
	[DllImport ("__Internal")]
	private static extern void _iosSaveImageToPhotosAlbum(string readAddr);
	[DllImport ("__Internal")]
	private static extern void _iosCall(string number);
    [DllImport("__Internal")]
    private static extern void _ReservationModification(string orderNo, string subject, string body, string amount, string channel, string token);


    private string[] _strings;
    private Dictionary<string, MyAction<JsonData>> _dicFuns;
    private MyAction<string> _onPhotoComplete;
    public SDKContorl() {
        _dicFuns = new Dictionary<string, MyAction<JsonData>>();

        _dicFuns.Add("PaySucceed", PaySucceed);
        _dicFuns.Add("PayFailed", PayFailed);
        _dicFuns.Add("PhotoAlbumCallBack", PhotoAlbumCallBack);
    }

    /**
     * PayWrapper_mm_Handler
	 * 
	 * @param orderNo
	 *            订单编号 必须在商户系统内唯一
	 * @param subject
	 *            商品的标题 该参数最长为 32 个 Unicode 字符
	 * @param body
	 *            商品的描述信息，该参数最长为 128 个 Unicode 字符
	 * @param amount
	 *            订单总金额,单位为对应币种的最小货币单位，例如：人民币为分
     * @channel
	 *            支付方式 银联支付渠道 upacp 微信支付渠道 wx 支付宝支付渠道 alipay
	 */

    public void Pay(string orderNo, string subject, string body, string amount, string channel) {
        var str = (orderNo.Length < 26) ? orderNo + GetRandomString() : orderNo;
        _strings = new string[] { str, subject, body, amount, channel };

        #if UNITY_ANDROID
            using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
                   jo.Call("payment", str, subject, body, amount, channel);
                    UiControllerMgr.Instance.ShowLodingTips(true);
                }
            }
        #endif

        #if UNITY_IPHONE
            _ReservationModification(str, subject, body, amount, channel, PlayerPrefs.GetString(Constants.Token, string.Empty));
        #endif
    }
    
    private string  GetRandomString() {
        StringBuilder sb = new StringBuilder();
//        sb.Append("OL");
        sb.Append(DateTime.Now.Year.ToString().Substring(2));
        sb.Append(IsMoreTen(DateTime.Now.Month));
        sb.Append(IsMoreTen(DateTime.Now.Day));
        sb.Append(IsMoreTen(DateTime.Now.Hour));
        sb.Append(IsMoreTen(DateTime.Now.Minute));
//        string[] strKu = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
//            , "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
//        for (var i = 0; i < 4; i++) {
//            sb.Append(strKu[UnityEngine.Random.Range(0, strKu.Length - 1)]);
//        }
        return sb.ToString();
    }
    private string IsMoreTen(int i) {
        var temp = (i >= 10) ? i.ToString() : "0" + i;
        return temp;
    }


    private void PaySucceed(JsonData json) {
        UiControllerMgr.Instance.GetController(WindowId.PayResult).Show();
        EventCenter.Instance.Broadcast(EventId.PayResult, _strings, true);
        EventCenter.Instance.Broadcast(EventId.MovePersonalCenterMoveBack);
        UiControllerMgr.Instance.ShowLodingTips(false);
    }

    private void PayFailed(JsonData json) {
        UiControllerMgr.Instance.GetController(WindowId.PayResult).Show();
        EventCenter.Instance.Broadcast(EventId.PayResult, _strings, false);
        EventCenter.Instance.Broadcast(EventId.MovePersonalCenterMoveBack);
        UiControllerMgr.Instance.ShowLodingTips(false);
    }

    public void CallPhone(string number) {
        #if UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
                    jo.Call("callTellPhone", number);
                }
            }
        #endif

        #if UNITY_IPHONE
            _iosCall(number);
        #endif
    }

    public void OpenCamera(string name, MyAction<string> onComplete) {
        _onPhotoComplete = onComplete;
        #if UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
               using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
                 jo.Call("takePhoto", new object[] {"takePhoto", name});
                }
            }
        #endif

        #if UNITY_IPHONE
            _iosOpenCamera_allowsEditing();
        #endif
    }

    public void OpenPhoto(string name,MyAction<string> onComplete) {
        _onPhotoComplete = onComplete;
        #if UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
                    jo.Call("takePhoto", new object[] { "takeSave", name });
                }
            }
        #endif

        #if UNITY_IPHONE
            _iosOpenPhotoLibrary_allowsEditing();
        #endif
    }

    private void PhotoAlbumCallBack(JsonData json) {
        var str = Utils.GetJsonStr(json["fileName"]);
        if (_onPhotoComplete != null) {
            _onPhotoComplete(str);
            _onPhotoComplete = null;
        }
    }

    public void SdkCallBack(JsonData json) {
        var functionName = Utils.GetJsonStr(json["function"]);
        MyAction<JsonData> d;
        if (_dicFuns.TryGetValue(functionName, out d)) {
            d(json);
        }
    }
}