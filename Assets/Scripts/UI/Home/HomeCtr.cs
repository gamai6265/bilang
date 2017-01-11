using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class HomeCtr : IController, IInfoListener {
        private HomeViewPresenter _view;
        private INetworkMgr _networkMgr;
        private IInput _input;
        private ILogicModule _logic;
        private ILoginMgr _login;
        private IOrderCenter _orderCenter;
        private bool _isSwipe;

        private List<string> _lstKey = new List<string>();

        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("network is null.");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.InfoTrigger).AddListener(this);
            _input = ComponentMgr.Instance.FindComponent(ComponentNames.ComInput) as IInput;
            if (_input == null) {
                LogSystem.Error("input is null");
                return false;
            }
            _logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (_logic == null) {
                LogSystem.Error("erro. logic module is null.");
                return false;
            }
            _login = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (_login == null) {
                LogSystem.Error("error. loginmgr is null.");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener(EventId.EnterStageEmpty, () => Show());
            EventCenter.Instance.AddListener(EventId.LeaveStageEmpty, ()=>Hide());
            EventCenter.Instance.AddListener<WindowId>(EventId.BtnsEvent, BtnsEvent);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (Constants.HomeShow) {
                if (_view == null) {
                    _view = UiManager.Instance.GetWindow<HomeViewPresenter>(WindowId.Home);
                    _view.ClickInfo += OnInfoClick;
                    _view.ClickAd += OnAdClick;
                    _view.ClickChooseRecommend += OnRecommendClick;
                    _view.ClickScreen += OnScreenClick;
                    _view.ClickMore += OnMoreClick;
                    GetNoticeList();
                }
                UiManager.Instance.ShowWindow(WindowId.Home);
                var btns = (HomeBtnsCtr) UiControllerMgr.Instance.GetController(WindowId.HomeBtns);
                btns.ShowBtns(WindowId.Home, 1);
//            _input.SwitchSwipe(true, OnFingerGestures);
                _isSwipe = true;
            }
            else {
                Hide();
            }
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            LogSystem.Debug("HomeBtn Back");
            UiControllerMgr.Instance.ShowMessageBox(
                "",
                StrConfigProvider.Instance.GetStr("mb_promptquit"),
                StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    Application.Quit();
                }
                );
            return true;
        }

        public void Hide(MyAction onComplete=null) {
            _lstKey.Clear();
            //            _input.SwitchSwipe(false, OnFingerGestures);
            UiManager.Instance.HideWindow(WindowId.Home);
        }


        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Home, "ui/Home")
            };
        }

        private void OnFingerGestures(string str, float flo1, float flo2) {
            if(!_isSwipe) return;
            if (str == "Left" || str == "UpperLeftDiagonal" || str == "LowerLeftDiagonal") {
                EventCenter.Instance.Broadcast(EventId.ClosePersonalCenter);
                UiControllerMgr.Instance.GetController(WindowId.PersonalCenter).Hide();
            }else if (str == "Right" || str == "UpperRightDiagonal" || str == "LowerRightDiagonal") {
                OnInfoClick();
            }
        }

        private void GetNoticeList() {
            _networkMgr.QuerySender<IInfoSender>().SendGetNoticeListReq();
            _orderCenter.GetClassifyManageListReq("0", string.Empty, GetClassifyManageListCallBack);
        }

        public void OnGetNoticeListRsp(JsonData msg) {
            LogSystem.Debug("OnGetNoticeListRsp");
            IDictionary json = msg;
            if (json.Contains("exit")) {
                CoroutineMgr.Instance.StartCoroutine(AddAdvertisement(msg));
            }
        }

        IEnumerator AddAdvertisement(JsonData json) {//添加顶部轮播信息
            if (json["exit"].ToString() == "0") {
                JsonData jData = json["data"];
                Texture2D[] finaion = new Texture2D[jData.Count];
                string[] str = new string[jData.Count];
                for (int i = 0; i < jData.Count; i++) {
                    str[i] = Utils.GetJsonStr(jData[i]["noticeUrl"]);
                    string tance = Utils.GetJsonStr(jData[i]["titleImg"]);
                    WWW www = new WWW(Constants.ShouYeGongGao + tance);
                    yield return www;
                    Texture2D texture2D = null;
                    if (www.error == null) {
                        texture2D= www.texture;
                    }
                    else {
                        www=new WWW(Constants.DefaultImage);
                        yield return www;
                        texture2D= www.texture;
                    }
                    finaion[i] = texture2D;
                    www.Dispose();
                }
                if (null != _view)
                    _view.SetNoticeList(str, finaion);
            } else {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_requestfail"));
            }
        }

        private void BtnsEvent(WindowId id) {
            if (WindowId.Home == id) {
                Show();
            } else {
                Hide();
            }
        }

        private void OnAdClick(string url) {
#if UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            UniWebDemo.Instance.OpenWeb(url, WebType.home);
            UiControllerMgr.Instance.GetController(WindowId.WebView).Show();
#endif
            Hide();
        }

        private void OnRecommendClick(JsonData productJson) {
            UiControllerMgr.Instance.GetController(WindowId.DesignersRecommendPreview).Show();
            EventCenter.Instance.Broadcast(EventId.DesignersRecommendPreviewData, productJson);
        }

        private void OnScreenClick() {
            UiControllerMgr.Instance.GetController(WindowId.DesignersRecommendSelect).Show();
        }

        private void OnMoreClick(string type) {
            UiControllerMgr.Instance.GetController(WindowId.DesignersRecommend).Show();
            EventCenter.Instance.Broadcast(EventId.DesignersRecommendType, type);
//            Hide();
        }

        private void OnInfoClick() {
            if (_login.IsLogined()) {
                LogSystem.Debug("ui. personal center.");
                //TODO danie 这里可能会直接切换到design，不一定会回来，注意销毁,或者用成员方法表示，不要lambda
                OpenPersonalCenter();
            } else {
                //open login panel.
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    //login cancel(failed) or login succ
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        Show();
                        OpenPersonalCenter();
                    } else {
                        Show();
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
                Hide();
            }
        }

        private void OpenPersonalCenter() {
            var btns = (HomeBtnsCtr)UiControllerMgr.Instance.GetController(WindowId.HomeBtns);
            MyAction closePersonalAction = null;
            closePersonalAction = () => {
                EventCenter.Instance.RemoveListener(EventId.ClosePersonalCenter, closePersonalAction);
                if (null != _view)
                    _view.DockAnimation(true);
                btns.ReverseAnimation();
                _isSwipe = true;
            };
            EventCenter.Instance.AddListener(EventId.ClosePersonalCenter, closePersonalAction);
            //open personal center.                
            _isSwipe = false;
            UiControllerMgr.Instance.GetController(WindowId.PersonalCenter).Show();
            if (null != _view)
                _view.DockAnimation();
            btns.PlayAnimation();
        }
        public void OnFashionListSearchRsp(JsonData msg) {
        }public void OnFashionDetailRsp(JsonData msg) {
        }public void OnGetRecommendDetailRsp(JsonData msg) {
        }public void OnGetDefaultProductInfoRsp(JsonData msg) {
        }public void OnGetRecommendListRsp(JsonData msg) {
        }

        private void GetClassifyManageListCallBack(bool result, JsonData msg) {
            if (result) {
                _view.DeleteAllDesignersRecommendItem();
                var classifyManageList = msg["classifyManageList"];
                if (classifyManageList.Count > 0) {
                    _lstKey.Clear();
                    for (var i = 0; i < classifyManageList.Count; i++) {
                        var key = Utils.GetJsonStr(classifyManageList[i]["name"]);
                        if (!_lstKey.Contains(key))
                            _lstKey.Add(key);
                        if (i == 0)
                            _orderCenter.GetDesignerProductListReq("0", "4", key, string.Empty, string.Empty, string.Empty,
                                PlatformRecommendCallBack);
                    }
                }
            }
        }

        private void PlatformRecommendCallBack(bool result, JsonData msg) {
            if (result) {
                var designerProductList = msg["designerProductList"];
                if (designerProductList.Count > 0) {
                    CoroutineMgr.Instance.StartCoroutine(InitPlatformRecommendPrefabs(designerProductList));
                    var key = Utils.GetJsonStr(designerProductList[0]["type"]);
                    if (_lstKey.Contains(key))
                        _lstKey.Remove(key);
                    if (_lstKey.Count > 0)
                        _orderCenter.GetDesignerProductListReq("0", "2", _lstKey[0], string.Empty, string.Empty, string.Empty, GetDesignerProductListCallback);
                }
            }
        }

        IEnumerator InitPlatformRecommendPrefabs(JsonData data) {
            Texture2D[] texture2Ds = new Texture2D[data.Count];
            for (var i = 0; i < data.Count; i++) {
                var item = data[i];
                var www = new WWW(Constants.KuanShiCanKao + Utils.GetJsonStr(item["mainPic"]));
                yield return www;
                if (www.error == null) {
                    texture2Ds[i] = www.texture;
                } else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture2Ds[i] = www.texture;
                }
                www.Dispose();
            }
            _view.AddPlatformRecommendPrefabs(data, texture2Ds);
        }


        private void GetDesignerProductListCallback(bool result, JsonData msg) {
            if (result) {
                var designerProductList = msg["designerProductList"];
                if (designerProductList.Count > 0) {
                    CoroutineMgr.Instance.StartCoroutine(InitProductPrefabs(designerProductList));
//                    var key = Utils.GetJsonStr(designerProductList[0]["type"]);
//                    if (_lstKey.Contains(key))
//                        _lstKey.Remove(key);
//                    if(_lstKey.Count>0)
//                        _orderCenter.GetDesignerProductListReq("0", "2", _lstKey[0], string.Empty, GetDesignerProductListCallback);
                }
            }
        }

        IEnumerator InitProductPrefabs(JsonData data) {
            Texture2D[] texture2Ds = new Texture2D[data.Count];
            for (var i = 0; i < data.Count; i++) {
                var item = data[i];
                var www = new WWW(Constants.KuanShiCanKao + Utils.GetJsonStr(item["mainPic"]));
                yield return www;
                if (www.error == null) {
                    texture2Ds[i] = www.texture;
                }
                else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture2Ds[i] = www.texture;
                }
                www.Dispose();
            }
            _view.AddClassifyManageListPrefabs(data, texture2Ds);
            yield return null;
            var key = Utils.GetJsonStr(data[0]["type"]);
            if (_lstKey.Contains(key))
                _lstKey.Remove(key);
            if (_lstKey.Count > 0)
                _orderCenter.GetDesignerProductListReq("0", "2", _lstKey[0], string.Empty, string.Empty, string.Empty, GetDesignerProductListCallback);
        }
    }
}