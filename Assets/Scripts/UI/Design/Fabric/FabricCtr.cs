using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;
using LitJson;

namespace Cloth3D.Ui {
    public class FabricCtr : IController {
        private FabricViewPresenter _view;
        private string _fabricFigureCode = string.Empty;
        private int _page = 1;

        private INetworkMgr _network;
        private IDesignWorld _designWorld;
        private IOrderCenter _orderCenter;
        private int _index = 0;
        private readonly string[] _rotations = {"0", "90", "-45", "45"};
        private string _fabricCode = string.Empty;
        private List<KeyValuePair<string, string>> _lstFabricStyle;

        public bool Init() {
            _network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _network) {
                LogSystem.Error("network is null.");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (null == _designWorld) {
                LogSystem.Error("design world is null");
                return false;
            }
            _designWorld.OnChangeDesignStateEvent += OnChangeDesignState;
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<List<FabricInfo>>(EventId.GetButtonOrThreadImage, GetButtonOrThreadImage);
            EventCenter.Instance.AddListener<bool, bool, bool, bool>(EventId.SetActiveDesignFabricObject, SetActiveDesignFabricObj);
            EventCenter.Instance.AddListener(EventId.ReSetPage, ReSetPage);//TODO 
            return true;
        }

        private void OnChangeDesignState(DesignState state) {
            switch (state) {
                case DesignState.StatePreview:
                    Hide();
                    break;
                case DesignState.StateDesign:
                    Show();
                    break;
                case DesignState.StateStyle:
                    Show();
                    _view.SetActiveRotationFabricBtn(true);
                    break;
            }
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<FabricViewPresenter>(WindowId.Fabric);
                _view.ClickFabricItem += OnClickFabricItem;
                _view.SvDragFinished += OnSvDragFinished;
                _view.ClickFabricFigure += OnClickFabricFigure;
                _view.ClickBtnRotation += OnClickBtnRotation;
                _view.ClickBtnFabricDetail += OnClickBtnFabricDetail;
            }

            UiManager.Instance.ShowWindow(WindowId.Fabric);
            _designWorld.GetFabricStyleList(CreateFabricStyleItem);
            _view.SetActiveRotationFabricBtn(false);
        }

        void OnDestroy() {
            EventCenter.Instance.RemoveListener<List<FabricInfo>>(EventId.GetButtonOrThreadImage, GetButtonOrThreadImage);
            EventCenter.Instance.RemoveListener(EventId.ReSetPage, ReSetPage);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Fabric, "ui/Fabric"),
            };
        }

        public void Hide(MyAction onComplete=null) {
            if (_view != null) {
                _index = 0;
                _page = 1;
                _lstFabricStyle.Clear();
                _view.SetActiveFabricObj(true);
                _view.SetActiveFabricFigureObj(true);
                _view.SetActiveRotationFabricBtn(false);
                _view.SetActiveBtnFabricDetail(false);
                UiManager.Instance.HideWindow(WindowId.Fabric);
            }
        }

        private void CreateFabricStyleItem(bool result, List<KeyValuePair<string, string>> lstFabricStyle) {
            if (result) {
                _lstFabricStyle = new List<KeyValuePair<string, string>>();
                _lstFabricStyle = lstFabricStyle;
                _view.DeleteAllFabricFigurePrefabs();
                _view.AddFabricFigurePrefabs("-1", "已选面料");//TODO 国际化
                for (int i = 0; i < lstFabricStyle.Count; i++) {
                    _view.AddFabricFigurePrefabs(lstFabricStyle[i].Key, lstFabricStyle[i].Value);
                }
                if (lstFabricStyle.Count > 0) {
                    _view.SelectFabricStyle(lstFabricStyle[0].Key);
                }
            } else {
                //TODO tips?
            }
        }
        
        private void OnClickFabricFigure(string codeValue) {
            _view.DeleteAllFabric();
            _orderCenter.FabricFigureCode = _fabricFigureCode = codeValue;
            if (codeValue == "-1") {
                var dicFabric = _designWorld.GetInUsedFabricList();
                foreach (var item in dicFabric) {
                    _view.AddPrefabs(item.Key,item.Value);
                    if(!Constants.ImagesDic.ContainsKey(item.Key))
                        Constants.ImagesDic.Add(item.Key, item.Value);
                }
            } else {
                _page = 1;
                _designWorld.GetFabricInfo(_orderCenter.FabricFigureCode, _page, "0", OnFabricInfoList);
            }
        }

        //TODO danie cache
        private IEnumerator IntClothItem(List<FabricInfo> lstFabricInfos, bool isButtonOrThread) {
            UiControllerMgr.Instance.ShowLodingTips(true);
            for (var i = 0; i < lstFabricInfos.Count; i++) {
                var fabricInfo = lstFabricInfos[i];
                if(i==0)
                    _fabricFigureCode = lstFabricInfos[i].FabricFigure;
                string filePath = Application.persistentDataPath + "/" + WWW.UnEscapeURL(fabricInfo.FabricImage);
                if (Constants.ImagesDic.ContainsKey(fabricInfo.FabricCode)) {
                    _view.AddPrefabs(fabricInfo.FabricCode, Constants.ImagesDic[fabricInfo.FabricCode]);
                } else {
                    if (File.Exists(filePath)) {
                        FileStream fs = new FileStream(filePath, FileMode.Open);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length); 
                        fs.Close();
                        fs.Dispose();
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(buffer);
                        _view.AddPrefabs(fabricInfo.FabricCode, texture);
                        Constants.ImagesDic.Add(fabricInfo.FabricCode, texture);
                    } else {
                        var url = string.Empty;
                        if (fabricInfo.FabricFigure == "13") {//TODO to low
                            url = Constants.NiuKouTuPian;
                        }else if(fabricInfo.FabricFigure == "14") {
                            url = Constants.ThreadImage;
                        }else {
                            url = Constants.MianLiaoTuPian;
                        }
                        if (fabricInfo.FabricImage != string.Empty) {
                            var www = new WWW(url + fabricInfo.FabricImage); //WWW.UnEscapeURL(fabricInfo.FabricImage)
                            yield return www;
                            Texture2D newTexture = null;
                            if (www.error == null) {
                                if (www.isDone) {
                                    newTexture = www.texture;
                                }
                            }
                            else {
                                www = new WWW(Constants.DefaultImage);
                                yield return www;
                                newTexture = www.texture;
                            }
                            www.Dispose();
                            _view.AddPrefabs(fabricInfo.FabricCode, newTexture);
                            var pngData = newTexture.EncodeToJPG();
                            File.WriteAllBytes(filePath, pngData); //TODO danie 这里的图片有可能为null,坑死爹了
                            Constants.ImagesDic.Add(fabricInfo.FabricCode, newTexture);
                        }
                    }
                }
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        private void OnFabricInfoList(bool result, List<FabricInfo> lstFabricInfos) {
            if (result) {
                //Load pics
                CoroutineMgr.Instance.StartCoroutine(IntClothItem(lstFabricInfos, false));
                _view.SpringSvPanel(_page);
            }
        }


        private void GetButtonOrThreadImage(List<FabricInfo> lstFabricInfos) {
            _view.DeleteAllFabric();
            if (_orderCenter.FabricFigureCode == "-1") {
                _orderCenter.FabricFigureCode = "01";
                _view.OnFabricStyleClick2(_orderCenter.FabricFigureCode);
            }
            CoroutineMgr.Instance.StartCoroutine(IntClothItem(lstFabricInfos, true));
        }

        private void OnClickFabricItem(string fabricCode, Texture2D texture) {
            //            LogSystem.Debug("OnClickFabricItem");
            _fabricCode = fabricCode;
            if (!_designWorld.ChangeFabric(fabricCode, texture)) {
                UiControllerMgr.Instance.ShowTips("该部件不能更换面料");//TODO 国际化
                _view.SetActiveBtnFabricDetail(false);
            }else {
                if (!fabricCode.Contains("#")) {
                    _view.SetActiveBtnFabricDetail(true);
                }else {
                    _fabricCode = string.Empty;//纽扣
                }
            }
        }

        private void OnClickBtnFabricDetail() {
            if(_fabricCode!=string.Empty)
                _orderCenter.GetFabricDetail(_fabricCode, GetFabricDetailRsp);
        }

        private void OnSvDragFinished() {
            if (_fabricFigureCode != "-1" ) {//非已选面料
                _page++;
                var temDic = _designWorld.GetGlobalStyle();
                if (temDic.ContainsKey(_fabricFigureCode)) {
                    _designWorld.GetFabricInfo("Global", _page, temDic[_fabricFigureCode], OnButtonOrThreadInfoList);
                }else {
                    _designWorld.GetFabricInfo(_orderCenter.FabricFigureCode, _page, "0", OnFabricInfoList);
                }
            }
        }

        private void OnButtonOrThreadInfoList(bool result, List<FabricInfo> lstFabricInfos) {
            if (result)
                CoroutineMgr.Instance.StartCoroutine(IntClothItem(lstFabricInfos, true));
        }

        private void OnClickBtnRotation() {
            _view.ChangeRotationFabricSpr(_index);
            _designWorld.RotationFabirc(_rotations[_index]);
            _index++;
            if (_index==4) {
                _index = 0;
            }
        }

        private void SetActiveDesignFabricObj(bool b1, bool b2, bool b3, bool b4) {//b1整个面料列表,b2面料属性,b3面料旋转,b4面料详情
            _view.SetActiveFabricObj(b1);
            _view.SetActiveFabricFigureObj(b2);
            _view.SetActiveRotationFabricBtn(b3);
            if(_fabricCode!=string.Empty)
                _view.SetActiveBtnFabricDetail(b4);
        }

        private void ReSetPage() {
            _page = 1;
        }

        private void GetFabricDetailRsp(bool result, JsonData jsonData) {
            if (result) {
                var _code = Utils.GetJsonStr(jsonData["mateCode"]);
                var _color = Utils.GetJsonStr(jsonData["fabricColorName"]);
                var _pattern = string.Empty;
                for (var i = 0; i < _lstFabricStyle.Count; i++) {
                    if (_lstFabricStyle[i].Key == Utils.GetJsonStr(jsonData["fabricFigure"])) {
                        _pattern = _lstFabricStyle[i].Value;
                        break;
                    }
                }
                var _material = Utils.GetJsonStr(jsonData["fabricTextureName"]);
                var _thickness = Utils.GetJsonStr(jsonData["fabricPly"]);
                string[] _str = {_code, _color, _pattern, _material, _thickness};
                _view.SetFabricDetail(_str, Constants.ImagesDic[_code]);
                _view.DockAnimation(true);
            }
        }
    }
}