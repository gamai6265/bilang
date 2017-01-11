using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class PartCtr : IController{
        private PartsViewPresenter _view;
        private MasksViewPresenter _maskView;
        private INetworkMgr _network;
        private IDesignWorld _designWorld;
        private IOrderCenter _orderCenter;
        private string[] _fabricstr = { "双面面料", "前面料", "后面料" };//TODO 国际化
        private bool _isGlobalPart = false;

        public bool Init() {
            _network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _network) {
                LogSystem.Error("network is null.");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
                LogSystem.Error("design world is null.");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            _designWorld.OnChangeDesignStateEvent += OnChangeDesignState;
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<PartsViewPresenter>(WindowId.Parts);
                _maskView = UiManager.Instance.GetWindow<MasksViewPresenter>(WindowId.Masks);
                _maskView.OnClick += OnMaskClick;
                _view.ClickFabic += OnClickFabicSide;
                _view.ClickPartsConfirm += OnClickPartsConfirm;
                _view.ClickParts += OnClickParts;
            }
            UiManager.Instance.ShowWindow(WindowId.Parts);
            ShowDetail();
        }

        private void ShowDetail() {
            if (_isGlobalPart) {
                _view.SetActiveBtnFabicObj(false);
                FillGlobalPartItem();
            } else {
                FillPartItem();
            }
            if (!_isGlobalPart)
                UpdateFabricSide();
        }


        public void Update() {
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            if (_view != null) {
                _view.ClearList();
                UiManager.Instance.HideWindow(WindowId.Parts);
            }
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Parts, "ui/Parts"),
                new KeyValuePair<WindowId, string>(WindowId.Masks, "ui/Masks"),
            };
        }
        private void OnChangeDesignState(DesignState state) {
            switch (state) {
                case DesignState.StatePreview:
                    _isGlobalPart = false;
                    Hide();
                    break;
                case DesignState.StateDesign:
                    _isGlobalPart = true;
                    _designWorld.GlobalId = -1;
                    Show();
                    break;
                case DesignState.StateStyle:
                    _isGlobalPart = false;
                    _view.ClearList();
                    Show();
                    break;
            }
        }

        private void OnClickParts(int partId, string queryType) {
            if (_isGlobalPart) {
                _designWorld.GlobalId = partId;
                _view.SetSelected(partId);
                if (queryType != "0") {//纽扣.衣缝线===>隐藏面料类型.面料旋转,双面料按钮
                    EventCenter.Instance.Broadcast(EventId.SetActiveDesignFabricObject, true, false, false, false);
                    _view.SetActiveBtnFabicObj(false);
                }else {
                    EventCenter.Instance.Broadcast(EventId.SetActiveDesignFabricObject, true, true, false, true);
                }
                _designWorld.GetFabricInfo("Global", 1, queryType, OnFabricInfoList);
                EventCenter.Instance.Broadcast(EventId.ReSetPage);
            } else {
                SwitchDesignFabricObject(partId);
                if (_designWorld.ChangePart(partId)) {
                    _view.SetSelected(partId); 
                } else {
                    UiControllerMgr.Instance.ShowTips("选择的部件有冲突"); //TODO 告诉与谁冲突了
                }
            }
        }

        private void OnFabricInfoList(bool result, List<FabricInfo> lstFabricInfos) {
            if (result)
                 EventCenter.Instance.Broadcast(EventId.GetButtonOrThreadImage, lstFabricInfos);
        }

        private void OnClickPartsConfirm() {
            if (_isGlobalPart){
                _designWorld.PreviewModel();
            } else{
                EventCenter.Instance.Broadcast(EventId.SetActiveDesignFabricObject, true, true, true, false);
                _designWorld.DesignModel();
            }
        }

        private void OnClickFabicSide() {
            var side = _designWorld.FabricSide;
            var toggleSide = FabricSide.FabricUpside;
            if (side == FabricSide.FabricUpside) {
                toggleSide = FabricSide.FabricFilpside;
            }else if (side == FabricSide.FabricFilpside) {
                toggleSide = FabricSide.FabricUpside|FabricSide.FabricFilpside;
            }
            _designWorld.ToggleFabricSide(toggleSide);
            UpdateFabricSide();
        }

        private void UpdateFabricSide() {
            switch (_designWorld.FabricSide) {
                case FabricSide.FabricUpside:
                    _view.SetLblFabicSide(_fabricstr[1]);
                    break;
                case FabricSide.FabricFilpside:
                    _view.SetLblFabicSide(_fabricstr[2]);
                    break;
                default:
                    _view.SetLblFabicSide(_fabricstr[0]);
                    break;
            }
        }

        private void FillPartItem() {
            CoroutineMgr.Instance.StopAllCoroutines();
            _view.DeleteAllStyle();
            var lstParts = _designWorld.GetPartStyle();
            if (lstParts != null) {
                CoroutineMgr.Instance.StartCoroutine(IntPartStyleItem(lstParts));
            }
        }

        private void FillGlobalPartItem() {
            CoroutineMgr.Instance.StopAllCoroutines();
            _view.DeleteAllStyle();
            var lstParts = _designWorld.GetGlobalPartStyle();
            if (lstParts != null) {
                CoroutineMgr.Instance.StartCoroutine(IntGlobalPartStyleItem(lstParts));
            }
        }

        private IEnumerator IntGlobalPartStyleItem(List<GlobalConfig> lstParts) {
            for (var i = 0; i < lstParts.Count; i++) {
                var part = lstParts[i];
                var www = new WWW(Constants.YangShiTuPian + part.SampleImage); //TODO danie cache
                yield return www;
                Texture2D texture = null;
                if (www.error == null) {
                    if (www.isDone) {
                        texture = www.texture;
                    }
                }else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture = www.texture;
                }
                www.Dispose();
                _view.AddPrefabs(texture, part.Id, part.Instruction, part.QueryType);
            }
            if (!_isGlobalPart) {
                SwitchDesignFabricObject(_designWorld.CurrentPart);
                _view.SetSelected(_designWorld.CurrentPart);
            }
            else
                _view.SetSelected(_designWorld.GlobalId);
        }
        private IEnumerator IntPartStyleItem(List<ModelPart> lstParts) {
            SwitchDesignFabricObject(_designWorld.CurrentPart);
            for (var i = 0; i < lstParts.Count; i++) {
                var part = lstParts[i];
                var www = new WWW(Constants.YangShiTuPian + part.SampleImage); //TODO danie cache
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
                _view.AddPrefabs(newTexture, part.Id, part.Instruction, string.Empty);
            }
            if (!_isGlobalPart) {
                _view.SetSelected(_designWorld.CurrentPart);
            }
        }

        private void OnMaskClick() {
            UiManager.Instance.HideWindow(WindowId.Masks);
            _view.HideInstructions();
        }

        private void SwitchDesignFabricObject(int partId) {
            var part = ModelPartProvider.Instance.GetModelPart(1, partId);//TODO
            if (part != null) {
                if (part.AssociateFabricPart != null || part.MaterialName1 == string.Empty) {//不能换面料
                    EventCenter.Instance.Broadcast(EventId.SetActiveDesignFabricObject, false, false, false, false);
                    _view.SetActiveBtnFabicObj(false);
                }
                else {
                    EventCenter.Instance.Broadcast(EventId.SetActiveDesignFabricObject, true, true, true, true);
                    if (part.MaterialName2 != string.Empty) {//双面料
                        _view.SetActiveBtnFabicObj(true);
                    } else {
                        _view.SetActiveBtnFabicObj(false);
                    }
                }
            }
        }
    }
}