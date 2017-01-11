using System;
using System.Collections.Generic;
using System.Linq;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using Cloth3D.Ui;
using LitJson;
using UnityEngine;

namespace Cloth3D.Design {
    public partial class DesignWorld : IDesignWorld, IDesignClothesListener, ICodeSetListener {
        private class TransInfo {
            public Vector3 Position;
            public Quaternion Rotation;
        }
        private const string StrStatePreview = "StatePreview";
        private const string StrStateDesignModel = "StateDesignModel";
        private const string StrStateSelectStyle = "StateSelectStyle";
        private readonly Dictionary<string, FabricInfo> _dicAllFabric = new Dictionary<string, FabricInfo>();
        private readonly Dictionary<string, FabricStyle> _dicFabricStyle = new Dictionary<string, FabricStyle>();
        private Fsm _fsm;
        private bool _isStop;
        private IInput _input;
        private ILogicModule _logicModule;
        private INetworkMgr _networkMgr;
        private IOrderCenter _orderCenter;

        //design data from network
        private Dictionary<int, List<PartInfo>> _dicOrignPartInfo;
        private List<PartInfo> _lstOrignGlobalInfo;
        private  Dictionary<string, Utils.DownCacheImageInfo> _dicOrignFabricTexture;
        private Utils.AsyncInfo _asyncInfo = null;

        private ModelsTypeConfig _model;
        private Dictionary<int, ModelView> _dicDesignModel;

        private ModelView _currentModel = null;
        private int _currentPart = -1;
        private int _globalId = -1;
        private FabricSide _fabricSide = FabricSide.FabricUpside;
        private Dictionary<int, PartInfo> _dicGlobalPart;

        private TransInfo _orignCameraTransInfo;
        private float _orignHotpointFactor= 1;

        public CameraRotaAnimation CameraRotaGoAnimation = new CameraRotaAnimation();
        public CameraRotaAnimation CameraRotaBackAnimation = new CameraRotaAnimation();
        
        private MyAction<bool> getFabricDetailCallBack;

        public string Name { get; set; }
        private Dictionary<string, string> _dicGlobalStyle = new Dictionary<string, string>();

        public void Tick() {
            if (_isStop)
                return;
            if (_asyncInfo != null) {
                if (_asyncInfo.IsError) {
                    _asyncInfo = null;
                    ////TODO danie，出错啦
                } else if (_asyncInfo.IsDone) {
                    _asyncInfo = null;
                    CreateModel();
                    InitCamera();
                    InitTouchEvent();
                    PreviewModel();
                } else {
                    return;
                }
            }
            
            _fsm.Update();
        }

        public bool Init() {
            _isStop = true;
            _InitDesignState();
            //net
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _networkMgr) {
                LogSystem.Info("network is null.");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.DesignClothesTrigger).AddListener(this);
            _networkMgr.QueryTigger(TriggerNames.CodeSetTrigger).AddListener(this);
            //logic state
            _logicModule = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null == _logicModule) {
                LogSystem.Info("logic module is null");
                return false;
            }
            _input = ComponentMgr.Instance.FindComponent(ComponentNames.ComInput) as IInput;
            if (_input == null) {
                LogSystem.Info("input is null");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            _logicModule.OnBeforeStageEnter += OnBeforeDesignStageEnter;
            _logicModule.OnAfterStageEnter += OnAfterDesignStageEnter;
            _logicModule.OnBeforeStageLeave += OnLeaveDesignStage;
            return true;
        }

        private bool ResolvePartInfo(SerialModelInfo info, out Dictionary<string, Utils.DownCacheImageInfo> dicFabric) {
            dicFabric = new Dictionary<string, Utils.DownCacheImageInfo>();
            bool ret = true;
            var model = ModelsTypeConfigProvider.Instance.GetDataById(info.ModelTypeId);
            if (model != null) {
                _dicOrignPartInfo = new Dictionary<int, List<PartInfo>>();
                _lstOrignGlobalInfo = new List<PartInfo>();
                for (int i = 0; i < info.LstPartInfo.Count; i++) {
                    var partInfo = info.LstPartInfo[i];
                    if (partInfo.Fabric1!=string.Empty && !dicFabric.ContainsKey(partInfo.Fabric1)) {
                        if (_dicAllFabric.ContainsKey(partInfo.Fabric1)) {
                            var fabricInfo = _dicAllFabric[partInfo.Fabric1];
                            dicFabric.Add(partInfo.Fabric1, new Utils.DownCacheImageInfo() {
                                ImageName = WWW.UnEscapeURL(fabricInfo.FabricImage),
                                PathPrefix = "Fabric",
                                Url = Constants.MianLiaoTuPian,
                                AppendInfo = fabricInfo
                            });
                        }
                    }
                    if (partInfo.Fabric2!=string.Empty && !dicFabric.ContainsKey(partInfo.Fabric2)) {
                        if (_dicAllFabric.ContainsKey(partInfo.Fabric2)) {
                            var fabricInfo = _dicAllFabric[partInfo.Fabric2];
                            dicFabric.Add(partInfo.Fabric2, new Utils.DownCacheImageInfo() {
                                ImageName = WWW.UnEscapeURL(fabricInfo.FabricImage),
                                PathPrefix = "Fabric",
                                Url = Constants.MianLiaoTuPian,
                                AppendInfo = fabricInfo
                            });
                        }
                    }
                    bool foundPart = false;
                    for (int j = 0; j < model.LstModels.Count; j++) {
                        var modelId = model.LstModels[j];
                        var part = ModelPartProvider.Instance.GetModelPart(modelId, partInfo.PartId);
                        if (part != null) {
                            if (!_dicOrignPartInfo.ContainsKey(modelId))
                                _dicOrignPartInfo.Add(modelId, new List<PartInfo>());
                            _dicOrignPartInfo[modelId].Add(partInfo);
                            foundPart = true;
                            break;
                        }
                    }
                    if (!foundPart) {
                        //globals
                        var globalConfig = GlobalProvider.Instance.GetDataById(partInfo.PartId);
                        if (globalConfig != null) {
                            _lstOrignGlobalInfo.Add(partInfo);
                        }
                    }
                }
            } else {
                ret = false;
            }
            return ret;
        }

        public void GetScreenShot(MyAction<Texture2D, Texture2D> callback) {
            var obj = GameObject.FindGameObjectWithTag("screenShot");
            if (obj != null) {
                var screenShot = obj.GetComponent<ScreenShotControl>();
                Texture2D textur1 = screenShot.CaptureCamera(_model.ModelDefaultView, _model.ModelPos, Vector3.up,
                                new Rect(Screen.width * 0f, Screen.height * 0f, Screen.width * 1f, Screen.height * 1f));

                screenShot.LightOn(true);
                var dir = _model.ModelPos-_model.ModelDefaultView;
                var targetPos = Vector3.Reflect(dir, Vector3.up)-_model.ModelPos;
                Texture2D textur2 = screenShot.CaptureCamera(targetPos, _model.ModelPos, Vector3.up,
                                new Rect(Screen.width*0f, Screen.height*0f, Screen.width*1f, Screen.height*1f));
                screenShot.LightOn(false);
                callback(textur1, textur2);
            }
            else {
                callback(null, null);
            }
        }


        public bool InitDesignModel(SerialModelInfo serialModelInfo) {
            _model = ModelsTypeConfigProvider.Instance.GetDataById(serialModelInfo.ModelTypeId);
            if (_model == null) {
                return false;
            }
            if (!ResolvePartInfo(serialModelInfo, out _dicOrignFabricTexture)) {
                return false;
            }
            _asyncInfo = new Utils.AsyncInfo();
            CoroutineMgr.Instance.StartCoroutine(Utils.DownLoadCacheImags(_asyncInfo, _dicOrignFabricTexture.Values, null));
            _logicModule.SwitchStage(StageType.StageDesign);
            return true;
        }

        private void InitTouchEvent() {
            _input.SwitchDrag(true, OnDrag);
            _input.SwitchTap(true, OnTap);
            _input.SwitchDoubleTap(true, OnDoubleTap);
            _input.SwitchPinch(true, OnPinch);
            _input.FilterNguiEvent(true);
        }

        private void DestroyTouchEvent() {
            _input.SwitchDrag(false, OnDrag);
            _input.SwitchTap(false, OnTap);
            _input.SwitchDoubleTap(false, OnDoubleTap);
            _input.SwitchPinch(false, OnPinch);
        }

        private void InitCamera() {
            var camTrans = Camera.main.transform;
            camTrans.position = _model.ModelDefaultView; 
            CameraMgr.RotateAroundAxies(_model.ModelPos, 0, 0);
            CameraMgr.RotateAroundAxies(_model.ModelPos, 0, 1);
            var pos = Camera.main.cameraToWorldMatrix * (camTrans.right + _model.ModelPos);
            _orignHotpointFactor = (Camera.main.projectionMatrix * pos).x;
        }
        private void CreateModel() {
            _dicGlobalPart = new Dictionary<int, PartInfo>();
            _dicDesignModel = new Dictionary<int, ModelView>();
            for (var i = 0; i < _model.LstModels.Count; i++) {
                var obj = new GameObject("Model");
                var designModel = obj.AddComponent<ModelView>();
                designModel.ModelId = _model.LstModels[i];
                designModel.CreateModelFromData(_dicOrignPartInfo[designModel.ModelId]);
                obj.transform.position = _model.LstModelsPos[i];
                if (!_dicDesignModel.ContainsKey(_model.LstModels[i])) {
                    _dicDesignModel.Add(_model.LstModels[i], designModel);
                }
                var lst = _dicOrignPartInfo[designModel.ModelId];
                //TODO fabric side info
                for (int j = 0; j < lst.Count; j++) {
                    var item = lst[j];
                    var part = ModelPartProvider.Instance.GetModelPart(designModel.ModelId, item.PartId);
                    if (item.Fabric1!=string.Empty && _dicAllFabric.ContainsKey(item.Fabric1)) {
                        var fabric = _dicAllFabric[item.Fabric1];
                        var texinfo = _dicOrignFabricTexture[item.Fabric1];
                        designModel.ChangePartFabric(part, fabric, texinfo.Texture, fabric.ActualWidth,
                           FabricSide.FabricUpside | FabricSide.FabricFilpside);
                        if (item.Fabric1Rotation != string.Empty) {
                            designModel.RotationFabirc(part, item.Fabric1Rotation,
                                FabricSide.FabricUpside | FabricSide.FabricFilpside);
                        }
                    }
                    if (item.Fabric2!=string.Empty && _dicAllFabric.ContainsKey(item.Fabric2)) {
                        var fabric = _dicAllFabric[item.Fabric2];
                        var texinfo = _dicOrignFabricTexture[item.Fabric2];
                        designModel.ChangePartFabric(part, fabric, texinfo.Texture, fabric.ActualWidth,
                           FabricSide.FabricUpside | FabricSide.FabricFilpside);
                        if (item.Fabric2Rotation != string.Empty) {
                            designModel.RotationFabirc(part, item.Fabric2Rotation,
                                FabricSide.FabricUpside | FabricSide.FabricFilpside);//TODO 
                        }
                    }
                }
                var designPart = ModelPartProvider.Instance.GetModel(designModel.ModelId);
                if (designPart != null && designPart.AssociateFabricPart!=null) {
                    designModel._mainTexutre = designModel.GetPartFabric(designPart.AssociateFabricPart);
                    LogSystem.Debug("mainFabric:"+designModel._mainTexutre.Key);
                }
            }
            //globals info
            for (int i = 0; i < _lstOrignGlobalInfo.Count; i++) {
                var item = _lstOrignGlobalInfo[i];
                var globalConfig = GlobalProvider.Instance.GetDataById(item.PartId);
                foreach (var modelViewItem in _dicDesignModel) {
                    if (_dicOrignFabricTexture.ContainsKey(WWW.EscapeURL(item.Fabric1))) {
                        var texinfo = _dicOrignFabricTexture[WWW.EscapeURL(item.Fabric1)];
                        modelViewItem.Value.ChangeGlobal(texinfo.Texture, globalConfig.LstMaterials);
                    }else {
                        CoroutineMgr.Instance.StartCoroutine(LoadGlobalCacheImag(item, modelViewItem.Value,
                            globalConfig.LstMaterials));
                    }
                }
                if (!_dicGlobalPart.ContainsKey(item.PartId)) {
                    _dicGlobalPart.Add(item.PartId, item);
                }
            }
            CountProductInfoPrice();
        }
        

        public bool ChangePart(int partId) { //TODO danie  quetion: differce materials fabric 
            bool ret = true;
            var designModel = _currentModel;
            if (designModel == null)
                return false;
            _currentPart = partId;
            var part = ModelPartProvider.Instance.GetModelPart(designModel.ModelId, partId);
            if (part != null) {
                Dictionary<int, ModelPart> dicChangeParts;
                ret = designModel.ChangeModelPart(part, out dicChangeParts);
                if (ret) {
                    FabricSide side = FabricSide.FabricFilpside | FabricSide.FabricUpside;
                    bool paintFabric;
                    //change parts's fabric
                    if (dicChangeParts != null) { //TODO danie 这里可能会出现顺序问题
                        foreach (var item in dicChangeParts) {
                            var itemPart = item.Value;
                            if (itemPart.IsMountPoint) continue;
                            paintFabric = false;
                            if (itemPart.AssociateFabricPart != null) {
                                // get associatefabric's fabric
                                var fabricKeyvalue = designModel.GetPartFabric(itemPart.AssociateFabricPart);
                                if (fabricKeyvalue.Key != string.Empty) {
                                    paintFabric = true;
                                    _ChangeFabric(designModel, itemPart, fabricKeyvalue.Key, fabricKeyvalue.Value, side, true);
                                }
                            } else if (itemPart.ChildType == PartChildType.ChildOption) {
                                if (itemPart.OwnedParent != null && itemPart.OwnedParent.LastOptionPart != null) {
                                    //find option part's fabric
                                    var fabricKeyvalue = designModel.GetPartFabric(itemPart.OwnedParent.LastOptionPart);
                                    if (fabricKeyvalue.Key != string.Empty) {
                                        paintFabric = true;
                                        _ChangeFabric(designModel, itemPart, fabricKeyvalue.Key, fabricKeyvalue.Value,
                                            side, true);
                                    }
                                }
                            } else {
                                if (itemPart.ParentPart != null && itemPart.ParentPart.LastVisibleStylePart != null) {
                                    //find sibling part's fabric
                                    var fabricKeyvalue =
                                        designModel.GetPartFabric(itemPart.ParentPart.LastVisibleStylePart);
                                    if (fabricKeyvalue.Key != string.Empty) {
                                        paintFabric = true;
                                        _ChangeFabric(designModel, itemPart, fabricKeyvalue.Key, fabricKeyvalue.Value,
                                            side, true);
                                    }
                                }
                            }
                            if (!paintFabric) {
                                //if not found then get the default fabric
                                var fabricKeyvalue = designModel._mainTexutre;
                                if (fabricKeyvalue.Key != string.Empty) {
                                    _ChangeFabric(designModel, itemPart, fabricKeyvalue.Key, fabricKeyvalue.Value, side, true);
                                }
                            }
                        }
                    }
                    CountProductInfoPrice(); //TODO
                    EventCenter.Instance.Broadcast(EventId.SetDesignTitle, part.DisplayName); //TODO danie what's this 
                }
            }
            return ret;
        }

        public bool _ChangeFabric(ModelView model, ModelPart part, string fabricCode, Texture2D texture, FabricSide side, bool force=false) {
            bool ret = false;
            if (_dicAllFabric.ContainsKey(fabricCode)) {
                var fabricInfo = _dicAllFabric[fabricCode];
                var entryPart = ModelPartProvider.Instance.GetModel(model.ModelId);
                if (force|| part.AssociateFabricPart == null || part==entryPart) {
                    model.ChangePartFabric(part, fabricInfo, texture, fabricInfo.ActualWidth,side);
                    ret = true;
                }
            }
            return ret;
        }

        public bool ChangeFabric(string fabricCode, Texture2D texture) {
            bool ret = false;
            if (_currentModel == null) {
                if (GlobalId == -1) {
                    foreach (var item in _dicDesignModel) {
                        item.Value._mainTexutre = new KeyValuePair<string, Texture2D>(fabricCode, texture);
                        var part = ModelPartProvider.Instance.GetModel(item.Value.ModelId);
                        if (part != null)
                            ret = _ChangeFabric(item.Value, part, fabricCode, texture, FabricSide);
                    }
                } else {
                    var globalConfig = GlobalProvider.Instance.GetDataById(GlobalId);
                    if (globalConfig != null && globalConfig.LstMaterials != null) {
                        foreach (var item in _dicDesignModel) {
                            item.Value.ChangeGlobal(texture, globalConfig.LstMaterials);
                        }
                        ret = true;
                        if (!_dicGlobalPart.ContainsKey(GlobalId)) {
                            _dicGlobalPart.Add(GlobalId, new PartInfo() {
                                PartId = GlobalId,
                                Partname = globalConfig.GlobalName,
                                PartType = "2",
                            });
                        }
                        if (_dicAllFabric.ContainsKey(fabricCode)) {
                            var partinfo = _dicGlobalPart[GlobalId];
                            var fabricInfo = _dicAllFabric[fabricCode];
                            partinfo.Fabric1 = fabricInfo.FabricCode;
                            partinfo.MateId1Price = fabricInfo.FabricPrice;
                            partinfo.MateId1SuppType = fabricInfo.SuppType;
                        }
                    }
                }
            } else {
                ModelPart part;
                if (_currentPart == -1) {
                    part = ModelPartProvider.Instance.GetModel(_currentModel.ModelId);
                } else {
                    part = ModelPartProvider.Instance.GetModelPart(_currentModel.ModelId, _currentPart);
                }
                if (part != null) {
                    ret = _ChangeFabric(_currentModel, part, fabricCode, texture, FabricSide);
                }
            }
            CountProductInfoPrice();
            return ret;
        }

        public FabricSide ToggleFabricSide(FabricSide side) {
            if (_currentModel != null) {
                var part = ModelPartProvider.Instance.GetModelPart(_currentModel.ModelId, _currentPart);
                FabricSide = FabricSide.FabricUpside;
                if (side != FabricSide.FabricUpside && part.IsDoubleSide) {
                    FabricSide = side;
                }
            }
            return FabricSide;
        }


        public event MyAction<DesignState> OnChangeDesignStateEvent;

        public void PreviewModel() {
            _fsm.SetCurState(StrStatePreview, true);
            var dir = _model.ModelDefaultView - _model.ModelPos;
            var upDir = Vector3.up - Vector3.Project(Vector3.up, dir);
            Quaternion rot = Quaternion.LookRotation(-dir, upDir);
            CameraRotaBackAnimation.PlayCameraAnimation(Camera.main.transform.position, Camera.main.transform.rotation,
                _model.ModelDefaultView, rot);
            UpdateModel();
        }

        public void DesignModel() {
            _currentModel = null;
            _currentPart = -1;
            if (_fsm.CurState.Name == StrStateSelectStyle) {
                CameraRotaGoAnimation.PlayCameraAnimation(Camera.main.transform.position, Camera.main.transform.rotation,
                    _orignCameraTransInfo.Position, _orignCameraTransInfo.Rotation, UpdateModel);
            }
            _fsm.SetCurState(StrStateDesignModel, true);

            foreach (var item in _dicDesignModel) {
                item.Value.RestoreHideParts();
            }
            UpdateModel();//是否一进来就显示部件热点
        }

        public void StraightenModel() {
            var dir = _model.ModelDefaultView - _model.ModelPos;
            var upDir = Vector3.up - Vector3.Project(Vector3.up, dir);
            Quaternion rot = Quaternion.LookRotation(-dir, upDir);
            CameraRotaBackAnimation.PlayCameraAnimation(Camera.main.transform.position, Camera.main.transform.rotation,
                _model.ModelDefaultView, rot);
        }
        public void DesignPartDetail(int modelId, int partId) {
            if (_dicDesignModel.ContainsKey(modelId)) {
                _currentModel = _dicDesignModel[modelId];
                _currentPart = partId;
                _fsm.SetCurState(StrStateSelectStyle, true);
                var part = ModelPartProvider.Instance.GetModelPart(modelId, partId);
                if (part != null) {
                    if (part.IsDoubleSide) {
                        FabricSide = FabricSide.FabricUpside | FabricSide.FabricFilpside;
                    } else {
                        FabricSide = FabricSide.FabricUpside;
                    }
                    if(part.ParentPart!=null) {
                        _orignCameraTransInfo = new TransInfo() {
                            Position = Camera.main.transform.position,
                            Rotation = Camera.main.transform.rotation
                        };

                        var lstHideModelParts = part.ParentPart.LstHotPointHibdes;
                        if (lstHideModelParts != null && lstHideModelParts.Count > 0) {
                            _currentModel.HideParts(lstHideModelParts);
                        }
                        //StareAtModelPart(part.ParentPart);
                        StareAtModelPart(part.ParentPart);
                        EventCenter.Instance.Broadcast(EventId.SetDesignTitle, part.DisplayName); //TODO danie what's this 
                    }
                } 
                UpdateModel();
            } 
        }

        public FabricInfo GetMainFabbric() {
            var code = _dicDesignModel.First().Value._mainTexutre.Key; //TODO danie 
            if (_dicAllFabric.ContainsKey(code)) {
                return _dicAllFabric[code];
            }
            return new FabricInfo();//TODO danie 
        }

        public string GetShirtName() {
            var shirtName = string.Empty;
            var fabricInfo = GetMainFabbric();
            var model = ModelsTypeConfigProvider.Instance.GetDataById(GetModelId());
            shirtName = fabricInfo.FabricColorName + fabricInfo.FabricFigureName + fabricInfo.FabricTextureName + model.TypeName;
            return shirtName;
        }

        public int GetModelId() {
            return _model.Id;
        }

        public event MyAction<bool, string, string> OnCountPrice;
        private void CountProductInfoPrice() {
            var lstPartInfos = new List<PartInfo>();
            var partInfos = GetPartInfo();
            for (int i = 0; i < partInfos.Count; i++) {
                var partItem = new PartInfo() {
                    PartId = partInfos[i].PartId,
                    PartType = partInfos[i].PartType,
                    PartWeight = partInfos[i].PartWeight,
                    Fabric1 = partInfos[i].Fabric1,
                    Fabric2 = partInfos[i].Fabric2,
                    MateId1Price = partInfos[i].MateId1Price,
                    MateId2Price = partInfos[i].MateId2Price,
                    MateId1SuppType = partInfos[i].MateId1SuppType,
                    MateId2SuppType = partInfos[i].MateId2SuppType
                };
                lstPartInfos.Add(partItem);
            }
            _networkMgr.QuerySender<IDesignClothesSender>().SendCountProductInfoPriceReq(_model.Id.ToString(), lstPartInfos);
        }

        private event MyAction<bool, JsonData> OnModelSize;
        private event MyAction<bool, JsonData> OnUserSize;
        public void GetUserSize(string modelId,MyAction<bool, JsonData> callback) {
            OnUserSize = callback;
            _networkMgr.QuerySender<IDesignClothesSender>().SendGetUserSizeListReq(modelId);
        }

        public event MyAction<bool, JsonData> OnSaveModelSize;
        public void SaveModelSize(string modelId,string atid, string buyerPropertiesJson, MyAction<bool, JsonData> callback) {
            OnSaveModelSize = callback;
            _networkMgr.QuerySender<IDesignClothesSender>().SendSaveUserSizeReq(modelId,atid, buyerPropertiesJson);
        }
        public event MyAction<bool, JsonData> OnDeleteModelSize; 
        public void DeleteUserSize(string ids, MyAction<bool, JsonData> callback) {
            OnDeleteModelSize = callback;
            _networkMgr.QuerySender<IDesignClothesSender>().SendDeleteUserSizeReq(ids);
        }

        public List<PartInfo> GetPartInfo() {
            List<PartInfo> partInfos = new List<PartInfo>();
            foreach (var item in _dicDesignModel) {
                partInfos.AddRange(item.Value.GetPartInfo());
            }
            foreach (var item in _dicGlobalPart) {
                partInfos.Add(item.Value);
            }
            return partInfos;
        }

        public Dictionary<string, Texture2D> GetInUsedFabricList() {
            Dictionary<string, Texture2D> dicFabricInfo = new Dictionary<string, Texture2D>();
            foreach (var item in _dicDesignModel) {
                ModelPart part = ModelPartProvider.Instance.GetModel(item.Key);
                var dicFabric = item.Value.GetInUsedFabricList(part);
                foreach (var fabricItem in dicFabric) {
                    if (!dicFabricInfo.ContainsKey(fabricItem.Key))
                        dicFabricInfo.Add(fabricItem.Key, fabricItem.Value);
                }
            }
            return dicFabricInfo;
        }

        private void StareAtHotPoint(ModelPart part) {
            var center = part.HotPointPos;
            Vector3 hotpointDir;
            if (_currentModel.GetHotpointOriginalDir(part, out hotpointDir)) {
                var upDir = Vector3.up - Vector3.Project(Vector3.up, hotpointDir);
                var tranPos = center + hotpointDir*-1.2f; 
                Quaternion rot = Quaternion.LookRotation(center-tranPos, upDir);
                CameraRotaGoAnimation.PlayCameraAnimation(_orignCameraTransInfo.Position, _orignCameraTransInfo.Rotation,
                    tranPos, rot);
            }
        }

        private void StareAtModelPart(ModelPart part) {
            CameraRotaGoAnimation.PlayCameraAnimation(_orignCameraTransInfo.Position, _orignCameraTransInfo.Rotation,
                    part.ViewPoint, Quaternion.Euler(part.ViewPointRotation));
        }

        public List<ModelPart> GetPartStyle() {
            if (_currentModel==null || _currentPart == -1) return null;
            var lstRet = new List<ModelPart>();
            var part = ModelPartProvider.Instance.GetModelPart(_currentModel.ModelId, _currentPart);
            if (part != null && part.ParentPart != null) {
                for (int i = 0; i < part.ParentPart.SubParts.Count; i++) {
                    var item = part.ParentPart.SubParts[i];
                    if (item.ChildType != PartChildType.ChildFiexed && item.IsValid) {
                        lstRet.Add(item);
                    }
                }
            }
            return lstRet;
        }

        public List<GlobalConfig> GetGlobalPartStyle() {
            List<GlobalConfig> lst = new List<GlobalConfig>();
            lst.Add(new GlobalConfig() {
                Id = -1,
                ModelType = _model.Id,
                SampleImage = _model.SampleImage,
                Instruction = _model.TypeName,
                QueryType = _model.QueryType
            });
            var lstConfig = GlobalProvider.Instance.GetGlobals(_model.Id);
            if (lstConfig != null) {
                lst.AddRange(lstConfig);
            }
            return lst;
        }

        public int CurrentPart {
            get { return _currentPart; }
        }

        public int GlobalId {
            get { return _globalId; }
            set { _globalId = value; }
        }

        public ModelPart GetModelPart(int modelType, int partId) {
            //TODO fixed me too lowbe
            ModelPart part = null;
            var model = ModelsTypeConfigProvider.Instance.GetDataById(modelType);
            if (model!=null) {
                for (int i = 0; i < model.LstModels.Count; i++) {
                    part = ModelPartProvider.Instance.GetModelPart(model.LstModels[i], partId);
                    if (part != null) 
                        break;
                }
            }
            return part;
        }

        public FabricSide FabricSide {
            get {
                return _fabricSide;
            }

            set {
                _fabricSide = value;
            }
        }

        private event MyAction<bool, List<KeyValuePair<string, string>>> OnFabricStyle;
        private event MyAction<bool, List<FabricInfo>> OnFabricList;

        private void _InitDesignState() {
            _fsm = new Fsm("DesignWorld");

            //_fsm.Blackboard.AddData("LogicModule", this);

            // add some states
            _fsm.AddState(new StatePreview(StrStatePreview, this));
            _fsm.AddState(new StateDesignModel(StrStateDesignModel, this));
            _fsm.AddState(new StateSelectStyle(StrStateSelectStyle, this));

            //_fsm.CurState = _fsm.GetState(_statePreview);

            /*//preview state
            _fsm.AddParam(new ParamBoolean("doDesign") {Value = false});
            Fsm.AddTransitionTo(_fsm, _statePreview, _stateDesignModel,
                new List<Condition> {new Condition("doDesign", true, ConditionType.Equals)});
            //selectparts state
            _fsm.AddParam(new ParamBoolean("selectStyle") {Value = false});
            Fsm.AddTransitionTo(_fsm, _stateDesignModel, _stateSelectStyle,
                new List<Condition> {new Condition("selectStyle", true, ConditionType.Equals)});
            // select style state
            _fsm.AddParam(new ParamBoolean("doneSelectStyle") {Value = false});
            Fsm.AddTransitionTo(_fsm, _stateSelectStyle, _stateDesignModel,
                new List<Condition> {new Condition("doneSelectStyle", true, ConditionType.Equals)});*/
        }

        private void OnBeforeDesignStageEnter(StageType stage) {
            if (stage == StageType.StageDesign) {
                _networkMgr.QuerySender<ICodeSetSender>().SendGetPricePropertysReq();
            }
        }

        private void OnAfterDesignStageEnter(StageType stage) {
            if (stage == StageType.StageDesign) {
                _isStop = false;
            }
        }

        private void OnLeaveDesignStage(StageType stage) {
            if (stage == StageType.StageDesign) {
                _dicDesignModel = null;
                _isStop = true;
                DestroyTouchEvent();
                _model = null;
            }
        }

        private bool Raycast(Camera cam, Vector2 screenPos, out RaycastHit hitData) {
            var ray = cam.ScreenPointToRay(screenPos);

            // regular 3D raycast
            var didHit = Physics.Raycast(ray, out hitData, Mathf.Infinity);

            // vizualise ray
#if UNITY_EDITOR
            if (didHit) {
                var hitPos = hitData.point;
                Debug.DrawLine(ray.origin, hitPos, Color.green, 0.5f);
            } else {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction*9999.0f, Color.red, 0.5f);
            }
#endif
            return didHit;
        }

        private void OnTap(Vector2 pos) {
            RaycastHit rayData;
            if (Raycast(Camera.main, pos, out rayData)) {
                foreach (var designModel in _dicDesignModel) {
                    if(designModel.Value.HandelTapEvent(pos, rayData.collider.gameObject))
                        break;
                }
            }
        }

        private void OnDoubleTap() {
            //LogSystem.Debug("double tap");
        }

        private void OnPinch(float delata) {
            float pinchScaleFactor = float.Parse(_logicModule.Config.GetSetting("Design", "pinchscalefactor"));
            var camTrans = Camera.main.transform;
            var dirVec = (camTrans.position - _model.ModelPos);
            var temVec = dirVec*(1 - pinchScaleFactor*delata);
            if (temVec.sqrMagnitude < _model.MinDistance* _model.MinDistance) {
                camTrans.position = _model.MinDistance*temVec.normalized + _model.ModelPos;
            } else if (temVec.sqrMagnitude > _model.MaxDistance* _model.MaxDistance) {
                camTrans.position = _model.MaxDistance*temVec.normalized + _model.ModelPos;
            } else {
                camTrans.position = temVec + _model.ModelPos;
            }
            //var distance = (camTrans.position - model.ModelPos).sqrMagnitude;
            var pos = Camera.main.cameraToWorldMatrix*(camTrans.right + _model.ModelPos);
            float factor = (Camera.main.projectionMatrix*pos).x;
            LogSystem.Debug("factor:{0}/orgin:{1}", factor, _orignHotpointFactor);
            foreach (var item in _dicDesignModel) {
                item.Value.UpdateHotpointsScale(factor/_orignHotpointFactor);
            }
        }

        private void UpdateModel() {
            if (_dicDesignModel != null) {
                bool bShowHotpoint = _fsm.CurState.Name == StrStateDesignModel; 
                foreach (var  item in _dicDesignModel) {
                   item.Value.UpdateHotpoints(bShowHotpoint); 
                }
            }
        }

        //private void 
        private void OnDrag(Vector2 startPos, Vector2 pos, Vector2 deltaMove) {
            if (_fsm.CurState.Name != StrStateSelectStyle) {
                float dragFactor = float.Parse(_logicModule.Config.GetSetting("Design", "dragfactor"));
                if (Math.Abs(pos.x - startPos.x) > Math.Abs(pos.y - startPos.y)) {
                    //move x axies
                    if (Math.Abs(deltaMove.x) > 0)
                        CameraMgr.RotateAroundAxies(Vector3.zero, dragFactor*deltaMove.x, 0);
                } else {
                    if (Math.Abs(deltaMove.y) > 0) {
                        var camTrans = Camera.main.transform;
                        var dirVec = camTrans.position - Vector3.zero;
                        var angle = Vector3.Angle(dirVec, Vector3.up);
                        var deltaAngle = dragFactor*deltaMove.y;
                        var preSetTargetAngle = angle + deltaAngle;
                        float targetAngle = deltaAngle;
                        if (preSetTargetAngle < 90) {
                            if (preSetTargetAngle < _model.Yscope.x) {
                                targetAngle = _model.Yscope.x - angle;
                            }
                        } else {
                            if (preSetTargetAngle > 180 - _model.Yscope.y) {
                                targetAngle = 180 - _model.Yscope.y - angle;
                            }
                        }
                        CameraMgr.RotateAroundAxies(Vector3.zero, -targetAngle, 1);
                    }
                }
                UpdateModel();
            }
        }

        private List<KeyValuePair<string, string>> _GetFabricStyleList() {
            var lstResult = new List<KeyValuePair<string, string>>();
            foreach (var item in _dicFabricStyle) {
                if (!_dicGlobalStyle.ContainsKey(item.Value.StyleCode))
                    lstResult.Add(new KeyValuePair<string, string>(item.Value.StyleCode, item.Value.StyleName));
            }
            return lstResult;
        }
        
        internal void ChangeStateEvent(DesignState state) {
            if (OnChangeDesignStateEvent != null)
                OnChangeDesignStateEvent(state);
        }

        private class FabricStyle {
            public readonly Dictionary<int, List<FabricInfo>> DicFabricPageData =
                new Dictionary<int, List<FabricInfo>>();

            public bool IsPageValid;
            public int PageNum;
            public string StyleCode = string.Empty;
            public string StyleName = string.Empty;
        }

        public void RotationFabirc(string rotation) {
            if (_currentModel != null) {
                var part = ModelPartProvider.Instance.GetModelPart(_currentModel.ModelId, _currentPart);
                _currentModel.RotationFabirc(part, rotation, _fabricSide);
            }
        }

        public Dictionary<string, string> GetGlobalStyle() {
            return _dicGlobalStyle;
        }
    }
}