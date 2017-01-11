using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrderEvaluateCtr : IController {
        private OrderEvaluateViewPresenter _view;

        private int _quality;
        private int _serve;
        private int _size;
        private int _index = 0;
        private List<string> _lstOrderStr = new List<string>();
        private List<string> _lstGoodId = new List<string>();
        private IDictionary<int, Texture2D> textureDic = new Dictionary<int, Texture2D>();

        private IOrderCenter _orderCenter;
        private IUserCenter _userCenter;

        public bool Init() {
            EventCenter.Instance.AddListener<List<string>, List<Texture2D>, List<string>>(EventId.OrderEvaluate, ShowOrderEvaluate);
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("OrderCenter is null");
                return false;
            }

            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("UserCenter is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<OrderEvaluateViewPresenter>(WindowId.OrderEvaluate);
                _view.ClickBack += OnBackClick;
                _view.ClickCamera += OnCameraClick;
                _view.ClickPhoto += OnPhotoClick;
                _view.ClickQuality += OnQualityClick;
                _view.ClickServe += OnServeClick;
                _view.ClickSize += OnSizeClick;
                _view.ClickSubmit += OnSubmitClick;
            }
            UiManager.Instance.ShowWindow(WindowId.OrderEvaluate);
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

        public void Hide(MyAction onComplete = null) {
            _quality = 0;
            _serve = 0;
            _size = 0;
            _index = 0;
            _view.ReSet();
            UiManager.Instance.HideWindow(WindowId.OrderEvaluate);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.OrderEvaluate, "ui/OrderEvaluate"),
            };
        }

        private void ShowOrderEvaluate(List<string> lstStrs, List<Texture2D> texTure2D, List<string> lstGoodId) {
            _lstOrderStr = lstStrs;
            _lstGoodId = lstGoodId;
            _view.SetClothse(lstStrs[0], lstStrs[1], lstStrs[2], lstStrs[3], texTure2D[0]);
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnCameraClick() {
            SDKContorl.Instance.OpenCamera("CameraPhoto", PhotoComplete);
        }

        private void OnPhotoClick() {
            SDKContorl.Instance.OpenPhoto("LocalPhoto", PhotoComplete);
        }

        private void PhotoComplete(string base64) {
            if (_index < 3) {
                #if UNITY_ANDROID
                    CoroutineMgr.Instance.StartCoroutine(LoadTexture(base64));
                #endif

                #if UNITY_IPHONE
                    Texture2D tex = new Texture2D (4, 4, TextureFormat.ARGB32, false);
				    byte[] bytes = System.Convert.FromBase64String(base64);
				    tex.LoadImage(bytes);
				    _view.SetTexture(_index, tex);
				    textureDic.Add(_index, tex);
				    _index++;
                #endif
            }
        }

        IEnumerator LoadTexture(string name) {
            string path = "file://" + "/mnt/sdcard/Android/data/com.bilang.tailor/image/" + name;
            WWW www = new WWW(path);
            yield return www;
            if (www.isDone) {
                Texture2D texture2D = www.texture;
                _view.SetTexture(_index, texture2D);
                textureDic.Add(_index, texture2D);
                _index++;
                www.Dispose();
            }
        }

        private void OnQualityClick(int quality) {
            _quality = quality;
        }

        private void OnServeClick(int serve) {
            _serve = serve;
        }

        private void OnSizeClick(int size) {
            _size = size;
        }

        private void OnSubmitClick(string remark) {
            JsonData jsonData = new JsonData();
            jsonData["orderId"] = _lstOrderStr[0];
//            StringBuilder sb = new StringBuilder();
//            for (var i = 0; i < _lstGoodId.Count; i++) {
//                if (i != 0)
//                    sb.Append(",");
//                sb.Append(_lstGoodId[i]);
//            }
            jsonData["goodsId"] = _lstGoodId[0];//只需传订单里其中一件商品id即可
            jsonData["productQuality"] = _quality;
            jsonData["serviceAttitude"] = _serve;
            jsonData["sizeFit"] = _size;
            jsonData["remark"] = remark;

            IList <KeyValuePair<string, byte[]>> files=new List<KeyValuePair<string, byte[]>>();
            for (int i = 0; i < _index; i++) {
                byte[] bytes = textureDic[i].EncodeToJPG();
                files.Add(new KeyValuePair<string, byte[]>("img", bytes));
            }
            _orderCenter.SubmitOrderEvaluate(jsonData.ToJson(), files, ".jpg", OnSubmitReturn);
        }

        private void OnSubmitReturn(bool retult) {
            if (retult) {
                _orderCenter.ModifyOrderStatus(_lstOrderStr[0], "07", ModifyStatus);
            }
        }

        private void ModifyStatus(bool result, List<OrderInfo> lstOrder) {
            if (result) {
                EventCenter.Instance.Broadcast(EventId.OrderEvaluateDone, _lstOrderStr[0]);
                Hide();
            }
        }
    }
}