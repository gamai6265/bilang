using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class MeasuringSizeCtr : IController {
        private MeasuringSizeViewPresenter _view;
        private IUserCenter _userCenter;
        private IOrderCenter _orderCenter;
        private IDesignWorld _designWorld;
        //private INetworkMgr _networkMgr;
        private MysizeEntry _mysize;
        private bool _isMysize; //true=身体尺寸.false=衣服尺寸
        private string _part;
        private string _spriteName;
        private int _minSize;
        private int _maxSize;
        private float _currentSize;
        private string _titleName;
        private string _message;
        private string _unit;
        private string _propertiesCode;

        public bool Init() {
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
                LogSystem.Error("_designWorld is null");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }

            EventCenter.Instance.AddListener<MysizeEntry>(EventId.MySizeEntry, ShowMySize);//测量身体尺寸
            EventCenter.Instance.AddListener<string>(EventId.Measurement, ShowMeasuringSizeSize); //测量模型尺寸
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<MeasuringSizeViewPresenter>(WindowId.MeasuringSize);
                _view.ClickBack += OnBackClick;
                _view.ClickYes += OnOkClick;
            }
            UiManager.Instance.ShowWindow(WindowId.MeasuringSize);
            _view.DeleteAllPrefabs();
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
            UiManager.Instance.HideWindow(WindowId.MeasuringSize);
            if (!_isMysize) {
                UiControllerMgr.Instance.GetController(WindowId.Measurement).Show();
            }else {
                UiControllerMgr.Instance.GetController(WindowId.MySize).Show();
            }
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.MeasuringSize, "ui/MeasuringSize"),
            };
        }

        void OnBackClick() {
            OnBack(-1);
        }

        void OnOkClick(string sizeAtid, float sizeValue) {
            if (_isMysize) {
                _userCenter.ModifyUserSize(_mysize, sizeValue.ToString(), (result) => {
                    if (result) {
                        UiControllerMgr.Instance.GetController(WindowId.MySize).Show();
                        Hide();
                    }
                    else {
                        UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_testerror"));
                    }
                });
            }
            else {
                foreach (var sizeItem in _orderCenter.DicSizeInfo) {
                    if (_propertiesCode == sizeItem.Value.PropertiesCode) {
                        sizeItem.Value.SizeValue = sizeItem.Value.ShowValue = sizeValue.ToString();
                        break;
                    }
                }
                Hide();
            }
        }


        private void ShowMeasuringSizeSize(string propertiesCode) {
            Show();
            _isMysize = false;
            _propertiesCode = propertiesCode;
            foreach (var sizeItem in _orderCenter.DicSizeInfo) {
                if (_propertiesCode == sizeItem.Value.PropertiesCode) {
                    _orderCenter.GetMeasureByPropertiesCode(_propertiesCode, GetMeasureByPropertiesCodeCallBack);
                    var value = sizeItem.Value.SizeValue + sizeItem.Value.unit;
                    var length = value.Length - sizeItem.Value.unit.Length;
                    string currentSize = value.Substring(0, length);
                    _view.SetRulerSize(sizeItem.Value.Atid, Single.Parse(currentSize), sizeItem.Value);
                    _view.ShowRuler();
                }
            }
        }

        private void GetMeasureByPropertiesCodeCallBack(bool result, JsonData jsonData) {
            if (result) {
                if (jsonData["propertiesMeasure"].Count > 0) {
                    var str = Utils.GetJsonStr(jsonData["propertiesMeasure"][0]["measureMethod"]);
                    var path= Utils.GetJsonStr(jsonData["propertiesMeasure"][0]["measureImg"]);
                    CoroutineMgr.Instance.StartCoroutine(GetTexture(str, path));
                }
            }
        }

        private IEnumerator GetTexture(string str, string path) {
            var www = new WWW(Constants.MeasureImg + path);
            yield return www;
            if (www.error == null) {
                if (www.isDone) {
                    var texture = www.texture;
                    _view.SetMeasurementTexture(str, texture);
                }
            }
            else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                _view.SetMeasurementTexture(str, www.texture);
            }
            www.Dispose();
        }


        private void ShowMySize(MysizeEntry entry) {
            float tem;
            float tem2;
            _spriteName = null;
            switch (entry) {
                case MysizeEntry.Height:
                    _minSize = 40;
                    _maxSize = 210;
                    _spriteName = "Height";
                    _part = "customer_height";
                    _unit = "cm";
                    float.TryParse(_userCenter.UserInfo.UserSize.Height, out tem);
                    float.TryParse(string.Format("{0:0.0}", tem), out tem2);
                    _currentSize = tem2;
                    break;
                case MysizeEntry.Weight:
                    _minSize = 50;
                    _maxSize = 210;
                    _spriteName = "Weight";
                    _part = "customer_weight";
                    _unit = "kg";
                    float.TryParse(_userCenter.UserInfo.UserSize.Weight, out tem);
                    float.TryParse(string.Format("{0:0.0}", tem), out tem2);
                    _currentSize = tem2;
                    break;
                case MysizeEntry.Shoulder:
                    _minSize = 60;
                    _maxSize = 240;
                    _spriteName = "Shoulder";
                    _part = "size_jian_k";
                    _unit = "cm";
                    float.TryParse(_userCenter.UserInfo.UserSize.Shoulder, out tem);
                    float.TryParse(string.Format("{0:0.0}", tem), out tem2);
                    _currentSize = tem2;
                    break;
                case MysizeEntry.Bust:
                    _minSize = 40;
                    _maxSize = 210;
                    _spriteName = "Bust";
                    _part = "size_xiong_w";
                    _unit = "cm";
                    float.TryParse(_userCenter.UserInfo.UserSize.Bust, out tem);
                    float.TryParse(string.Format("{0:0.0}", tem), out tem2);
                    _currentSize = tem2;
                    break;
                case MysizeEntry.Waistline:
                    _minSize = 40;
                    _maxSize = 210;
                    _spriteName = "Waistline2";
                    _part = "size_yao_w";
                    _unit = "cm";
                    float.TryParse(_userCenter.UserInfo.UserSize.WaistLine, out tem);
                    float.TryParse(string.Format("{0:0.0}", tem), out tem2);
                    _currentSize = tem2;
                    break;
                case MysizeEntry.Hipline:
                    _minSize = 40;
                    _maxSize = 210;
                    _spriteName = "Hipline2";
                    _part = "size_dun_w";
                    _unit = "cm";
                    float.TryParse(_userCenter.UserInfo.UserSize.HipLine, out tem);
                    float.TryParse(string.Format("{0:0.0}", tem), out tem2);
                    _currentSize = tem2;
                    break;
            }
            _mysize = entry;
            _isMysize = true;
            SetUpParameter();
            _view.ShowMySize(_spriteName);
        }

        private void SetUpParameter() {
            Show();
            _view.SetMySize(_currentSize, _minSize, _maxSize, _unit);
            _view.ShowRuler();
        }
    }
}
