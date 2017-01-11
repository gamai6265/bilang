using System;
using System.Collections;
using UnityEngine;

namespace Cloth3D.Ui {
    public class FabricViewPresenter : ViewPresenter {
        public GameObject FabricFigurePrefab;
        public UIGrid FabricGrid;
        public GameObject FabricItemPrefabs;

        public UIScrollView FabricScrollView;
        public UIGrid SelectionGrid;

        public UIScrollView SelectionScrollView;
        public UIButton BtnRotationFabric;
        public UISprite[] RotationFabricSpr;

        public GameObject FabricObj;
        public GameObject FabricFigureObj;
        public GameObject RotationFabricBtn;

        private string _startindex;

        public event MyAction<string, Texture2D> ClickFabricItem;
        public event MyAction SvDragFinished;
        public event MyAction<string> ClickFabricFigure;
        public event MyAction ClickBtnRotation;

        public UIButton BtnFabricDetail;
        public UILabel LblCodeDetail;
        public UILabel LblColorDetail;
        public UILabel LblPatternDetail;
        public UILabel LblMaterialDetail;
        public UILabel LblThicknessDetail;
        public UIButton BtnClose;
        public UITexture TexFabric1;
        public UITexture TexFabric2;
//        public event MyAction ClickBtnClose;
        public event MyAction ClickBtnFabricDetail;
        public TweenPosition TPFabricDetail;
        private Vector3 _from;
        private Vector3 _to;

        protected override void AwakeUnityMsg() {
            FabricScrollView.onDragFinished += OnDragFinished;
            SelectionScrollView.onDragFinished += OnSelectionScrollViewDragFinished;
            BtnRotationFabric.onClick.Add(new EventDelegate(OnBtnRotationClick));
            BtnFabricDetail.onClick.Add(new EventDelegate(OnBtnFabricDetailClick));
            BtnClose.onClick.Add(new EventDelegate(OnBtnCloseClick));
            _from = TPFabricDetail.from;
            _to = TPFabricDetail.to;
        }

        protected override void StartUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.PopUp;
        }

        private void OnDragFinished() {
            var constraint = FabricScrollView.panel.CalculateConstrainOffset(FabricScrollView.bounds.min,
                FabricScrollView.bounds.max);
            if (constraint.x > 1f) {
                if (null != SvDragFinished) {
                    SvDragFinished();
                }
            }
        }

        private void OnSelectionScrollViewDragFinished() {
            var minDistance = float.MaxValue;
            string fabricStyleIndex = string.Empty;
            FabricFigurePrefabs item = null;
            var lstChildren = SelectionGrid.transform.GetComponentsInChildren<FabricFigurePrefabs>(true);
            for (var i = 0; i < lstChildren.Length; i++) {
                var trans = lstChildren[i].transform;
                var pos = transform.InverseTransformPoint(trans.TransformPoint(Vector3.zero));
                //TODO transform???
                if (Math.Abs(pos.x) < Math.Abs(minDistance)) {
                    minDistance = pos.x;
                    item = lstChildren[i];
                    fabricStyleIndex = item.CodeValue;
                }
            }
            if (fabricStyleIndex != string.Empty) {
                if (fabricStyleIndex != _startindex) {
                    _OnFabricStyleClick(lstChildren, item);
                } else {
                    ChangeFontSizeAndColor(lstChildren, fabricStyleIndex);
                    SvMoveRelative(fabricStyleIndex);
                }
            }
        }

        private void OnBtnRotationClick() {
            if (null != ClickBtnRotation)
                ClickBtnRotation();
        }

        private void OnBtnCloseClick() {
            DockAnimation(false);
        }

        private void OnBtnFabricDetailClick() {
            if (null != ClickBtnFabricDetail)
                ClickBtnFabricDetail();
        }

        public void AddPrefabs(string pname, Texture2D texture) {
            var obj = NGUITools.AddChild(FabricGrid.gameObject, FabricItemPrefabs);

            var fip = obj.GetComponent<FabricItemPrefabs>();
            obj.name = fip.LblClothName.text = pname;
            fip.ClothTexture.mainTexture = texture;

            UIEventListener.Get(obj).onClick += OnFabricPrefabsClick;

            FabricGrid.Reposition();
            FabricGrid.repositionNow = true;
            NGUITools.SetDirty(FabricGrid);
        }

        private void OnFabricPrefabsClick(GameObject go) {
            var fip = go.GetComponent<FabricItemPrefabs>();
            if (null != fip && null != ClickFabricItem) {
                ClickFabricItem(fip.name, (Texture2D)fip.ClothTexture.mainTexture);
            }
        }

        public void DeleteAllFabricFigurePrefabs() {
            SelectionGrid.transform.DestroyChildren();
        }
        public void AddFabricFigurePrefabs(string code, string fname) {
            var obj = NGUITools.AddChild(SelectionGrid.gameObject, FabricFigurePrefab);

            var ffp = obj.GetComponent<FabricFigurePrefabs>();
            ffp.CodeName = fname;
            ffp.CodeValue = code;
            UIEventListener.Get(obj).onClick += OnFabricStyleClick;
            SelectionGrid.Reposition();
            SelectionGrid.repositionNow = true;
            NGUITools.SetDirty(SelectionGrid);
        }

        public void SelectFabricStyle(string styleCode) {
            var lstChildren = SelectionGrid.transform.GetComponentsInChildren<FabricFigurePrefabs>(true);
            FabricFigurePrefabs foundItem = null;
            for (int i = 0; i < lstChildren.Length; i++) {
                if (lstChildren[i].CodeValue == styleCode) {
                    foundItem = lstChildren[i];
                    break;
                } 
            }
            if(foundItem!=null)
                _OnFabricStyleClick(lstChildren, foundItem);
        }

        public void OnFabricStyleClick2(string code) {
            for (var i = 0; i < SelectionGrid.transform.childCount; i++) {
                var obj = SelectionGrid.transform.GetChild(i).gameObject;
                var ffp = obj.GetComponent<FabricFigurePrefabs>();
                if (ffp != null && ffp.CodeValue == code) {
                    var lstChildren = SelectionGrid.transform.GetComponentsInChildren<FabricFigurePrefabs>(true);
                    ChangeFontSizeAndColor(lstChildren, ffp.CodeValue);
                    SvMoveRelative(ffp.CodeValue);
                    _startindex = ffp.CodeValue;
                }
            }
        }

        private void OnFabricStyleClick(GameObject go) {
            var ffp = go.GetComponent<FabricFigurePrefabs>();
            if (ffp != null) {
                var lstChildren = SelectionGrid.transform.GetComponentsInChildren<FabricFigurePrefabs>(true);
                _OnFabricStyleClick(lstChildren, ffp);
            }
        }
        private void _OnFabricStyleClick(FabricFigurePrefabs[] lstItems, FabricFigurePrefabs ffp) {
            ChangeFontSizeAndColor(lstItems, ffp.CodeValue);
            SvMoveRelative(ffp.CodeValue);
            _startindex = ffp.CodeValue;
            if (null != ClickFabricFigure) {
                ClickFabricFigure(ffp.CodeValue);
            }
        }

        public void DeleteAllFabric() {
            SpringSvPanel(1);
            FabricGrid.transform.DestroyChildren();
        }

        public void SpringSvPanel(int index) {
            GetComponentInChildren<SpringPanel>().target.x = -1094 * (index - 1);//0;
            GetComponentInChildren<SpringPanel>().enabled = true;
        }


        private void ChangeFontSizeAndColor(FabricFigurePrefabs[] lstItems, string fabricStyleCode) {//更改按钮大小透明的
            for (int i = 0; i < lstItems.Length; i++) {
                if (fabricStyleCode == lstItems[i].CodeValue) {
                    lstItems[i].LblSelection.color = new Color(1, 1, 1, 1);
                    lstItems[i].LblSelection.fontSize = 50;
                } else {
                    lstItems[i].LblSelection.color = new Color(1, 1, 1, 0.5f);
                    lstItems[i].LblSelection.fontSize = 40;
                }
            }
        }

        private void SvMoveRelative(string fabricStyleCode) {//移动sv
            var lstChildren = SelectionGrid.transform.GetComponentsInChildren<FabricFigurePrefabs>(true);
            for (int i = 0; i < lstChildren.Length; i++) {
                if (fabricStyleCode == lstChildren[i].CodeValue) {
                    var trans = lstChildren[i].transform;
                    var pos = transform.InverseTransformPoint(trans.TransformPoint(Vector3.zero));
                    StartCoroutine(MoveBtns(-pos.x));
                    break;
                }
            }
        }

        private IEnumerator MoveBtns(float distance) {
            for (var i = 0; i < 10; i++) {
                SelectionScrollView.MoveRelative(new Vector3(distance/10, 0, 0));
                yield return new WaitForFixedUpdate();
            }
        }
        
        public void ChangeRotationFabricSpr(int index) {
            for (var i = 0; i < RotationFabricSpr.Length; i++) {
                if (i == index) {
                    RotationFabricSpr[i].color = Color.gray;
                }else {
                    RotationFabricSpr[i].color = Color.white;
                }
            }
        }

        public void SetActiveFabricObj(bool tof) {
            FabricObj.SetActive(tof);
        }

        public void SetActiveFabricFigureObj(bool tof) {
            FabricFigureObj.SetActive(tof);
        }

        public void SetActiveRotationFabricBtn(bool tof) {
//            ChangeRotationFabricSpr(-1);
//            RotationFabricBtn.SetActive(tof);
        }

        public void SetActiveBtnFabricDetail(bool tof) {
            BtnFabricDetail.gameObject.SetActive(tof);
        }

        public void SetFabricDetail(string[] fabricDetails, Texture2D texture2D) {
            LblCodeDetail.text = fabricDetails[0];
            LblColorDetail.text = fabricDetails[1];
            LblPatternDetail.text = fabricDetails[2];
            LblMaterialDetail.text = fabricDetails[3];
            LblThicknessDetail.text = fabricDetails[4];
            TexFabric1.mainTexture = TexFabric2.mainTexture = texture2D;
        }

        public void DockAnimation(bool tof) {
            if (tof) {
                TPFabricDetail.PlayForward();
            }else {
                TPFabricDetail.PlayReverse();
            }

//            if (backWard) {
//                transform.localPosition = _to;
//                TweenPosition.Begin(gameObject, TPFabricDetail.duration, _from);
//            } else {
//                transform.localPosition = _from;
//                TweenPosition.Begin(gameObject, TPFabricDetail.duration, _to);
//            }
        }
    }
}