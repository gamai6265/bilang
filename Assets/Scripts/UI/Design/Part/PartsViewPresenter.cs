using System.Collections.Generic;
using UnityEngine;

namespace Cloth3D.Ui {
    public class PartsViewPresenter : ViewPresenter {
        public UIButton BtnFabic;
        public UIButton BtnPartsConfirm;
        public UILabel LblFabic;
        
        public UIGrid PartsGrid;
        public UIScrollView PartsScrollView;
        public GameObject PartsItemPrefabs;
        
        public event MyAction ClickPartsConfirm;
        public event MyAction ClickFabic;
        public event MyAction<int, string> ClickParts;

        private readonly List<PartItemPrefab> _partidList = new List<PartItemPrefab>();
        private PartItemPrefab _selectedItem = null;
        private Ray _ray;

        protected override void AwakeUnityMsg() {
            _ray = new Ray();
            PartsScrollView.onDragStarted = OnStartDrag;
            BtnPartsConfirm.onClick.Add(new EventDelegate(OnBtnPartsConfirmClick));
//            BtnFabic.onClick.Add(new EventDelegate(OnBtnFabicClick));
        }

        protected override void StartUnityMsg() {
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        public void OnStartDrag() {
            UiManager.Instance.HideWindow(WindowId.PanelInstruction);
        }

        private void OnBtnPartsConfirmClick() {
            ClearList();
            if (null != ClickPartsConfirm) {
                ClickPartsConfirm();
            }
        }

        private void OnBtnFabicClick() {
            if (null != ClickFabic) {
                ClickFabic();
            }
        }

        public void SetLblFabicSide(string str) {
            LblFabic.text = str;
        }

        public void AddPrefabs(Texture2D texture2D, int partId, string instruction, string queryType) {
            var obj = NGUITools.AddChild(PartsGrid.gameObject, PartsItemPrefabs);
            var pip = obj.GetComponent<PartItemPrefab>();
            pip.PartId = partId;
            pip.PartItemPrefabs.mainTexture = texture2D;
            pip.Instruction = instruction;
            pip.QueryType = queryType;
            _partidList.Add(pip);
            UIEventListener.Get(obj).onClick += OnPartItemPrefabsClick;
            PartsGrid.Reposition();
            PartsGrid.repositionNow = true;
            NGUITools.SetDirty(PartsGrid);
        }

        private void OnPartItemPrefabsClick(GameObject go) {
            var pip = go.GetComponent<PartItemPrefab>();
            PartsScrollView.Scroll(0f);
            AdjustIntersectsPos(pip);
            //selected state.
            if (ClickParts != null)
                ClickParts(pip.PartId, pip.QueryType);
        }

        public void DeleteAllStyle() {
            PartsGrid.transform.DestroyChildren();
            GetComponentInChildren<SpringPanel>().target.y = 75; //TODO
            GetComponentInChildren<SpringPanel>().enabled = true;
        }

        public void SetSelected(int id) {
            for (int i = 0; i < _partidList.Count; i++) {
                if (_partidList[i].PartId == id) {
                    var isSelectedBefore = _partidList[i].IsSelected;
                    _partidList[i].SetSelected(true);
                    _selectedItem = _partidList[i];
                    if (isSelectedBefore) {
                        if (_selectedItem.IsFold) {
                            _selectedItem.UnFold(false); 
                        } else {
                            _selectedItem.Fold(false);
                        }
                    }
                } else {
                    _partidList[i].SetSelected(false);
                    if (!_partidList[i].IsFold) {
                        _partidList[i].Fold(true);
                    }
                }
            }
            //if not in eyesight
            UpdateItemPos();
        }

        public void ClearList() {
            _partidList.Clear();
            _selectedItem = null;
        }

        private void UpdateItemPos() {
            //TODO danie 这里加上更新移动代码
        }

        private void AdjustIntersectsPos(PartItemPrefab item) {
            var panel = PartsScrollView.GetComponent<UIPanel>();
            var bounds = item.GetComponent<BoxCollider>().bounds;
            _ray.origin = panel.worldCorners[1];
            _ray.direction = panel.worldCorners[2] - panel.worldCorners[1];
            bool upColided = false;
            //up side intersect
            if (bounds.IntersectRay(_ray)) {
                upColided = true;
                var lbConerPos = panel.localCorners[2];
                lbConerPos.y -= PartsGrid.cellHeight;
                lbConerPos.x -= PartsGrid.cellWidth;

                CenterOn(item.transform, (lbConerPos+ panel.localCorners[2])*0.5f);
            }
            //bottom side intersect
            if (!upColided) {
                _ray.origin = panel.worldCorners[0];
                _ray.direction = panel.worldCorners[3] - panel.worldCorners[0];
                if (bounds.IntersectRay(_ray)) {
                    var luConerPos = panel.localCorners[3];
                    luConerPos.y += PartsGrid.cellHeight;
                    luConerPos.x -= PartsGrid.cellWidth;
                    CenterOn(item.transform, (luConerPos + panel.localCorners[3]) * 0.5f);
                }
            }
        }

        private void CenterOn(Transform target, Vector3 centerPos) {
            var panelTrans = PartsScrollView.panel.cachedTransform;
            Vector3 cp = panelTrans.InverseTransformPoint(target.position);
            Vector3 localOffset = cp - centerPos;
            localOffset.x = 0f;
            localOffset.z = 0f;
            PartsScrollView.MoveRelative(-localOffset);
        }

        public void HideInstructions() {
            if (_selectedItem != null) {
                _selectedItem.Fold(true);
            }
        }

        public void SetActiveBtnFabicObj(bool tof) {
//            BtnFabic.gameObject.SetActive(tof);
        }
    }
}