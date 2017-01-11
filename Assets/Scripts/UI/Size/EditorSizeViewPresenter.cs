using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Data;

namespace  Cloth3D.Ui {
    public class EditorSizeViewPresenter:ViewPresenter {
        public UIButton BtnAdd;
        public UIButton BtnBack;
        public UIButton BtnHistory;
        public UILabel LblTitle;

        public UIScrollView EditorSizeScrollView;
        public UIGrid EditorSizeGrid;
        public GameObject EditorSizePrefabs;
        public GameObject SizePrefabs;

        public event MyAction ClickAddSize; 
        public event MyAction ClickBack;
        public event MyAction ClickHistory;
        public event MyAction<string, List<SizeInfo>> ClickChoose;
        public event MyAction<string> ClickDelete;
        public event MyAction<string, List<SizeInfo>> ClickEditor;

        protected override void AwakeUnityMsg() {
            BtnAdd.onClick.Add(new EventDelegate(OnBtnAddNewSize));
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnHistory.onClick.Add(new EventDelegate(OnBtnHistoryClick));
            EditorSizeScrollView.onDragFinished += DragFinish;
            //  ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("mm_hy_title");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void OnBtnAddNewSize() {
            if (null != ClickAddSize) {
                ClickAddSize();
            }
        }

        private void OnBtnHistoryClick() {
            if (null != ClickHistory)
                ClickHistory();
        }

        private void DragFinish() {
            
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        public void AddEditorSizePrefabs(string atid, List<SizeInfo> sizeInfos) {
            GameObject obj = NGUITools.AddChild(EditorSizeGrid.gameObject, EditorSizePrefabs);
            EditorSizePrefabs hsp = obj.GetComponent<EditorSizePrefabs>();
            hsp.AddListener();
            //TODO;
            hsp.Atid = atid;
            hsp.LstSizeInfo = sizeInfos;
            for (int i = 0; i < sizeInfos.Count; i++) {
                GameObject size = NGUITools.AddChild(hsp.SizeGrid.gameObject, SizePrefabs);
                SizePrefab sp = size.GetComponent<SizePrefab>();
                hsp.Ids.Add(sizeInfos[i].PropertiesCode);
                if (i >= 8) {
                    sp.LblName.text = "...";
                    break;
                }

                sp.LblName.text = sizeInfos[i].SizeName + ":";
                sp.LblValue.text = sizeInfos[i].SizeValue;
                sp.ChangeLangele();
            }
            hsp.SizeGrid.Reposition();
            hsp.SizeGrid.repositionNow = true;
            EditorSizeGrid.Reposition();
            EditorSizeGrid.repositionNow = true;
            hsp.ClickChoose += OnChooseSize;
            hsp.ClickDelete += OnDeleteSize;
            hsp.ClickEditor += OnEditorSize;
        }

        private void OnEditorSize(string atid, List<SizeInfo> sizeInfo) {
            if (null != ClickEditor)
                ClickEditor(atid, sizeInfo);
        }

        private void OnChooseSize(string atid, List<SizeInfo> sizeInfo) {
            if (null != ClickChoose)
                ClickChoose(atid, sizeInfo);
        }

        private void OnDeleteSize(string id) {
            if (null != ClickDelete)
                ClickDelete(id);
        }

        public void DeleteAllPrefabs() {          
            EditorSizeGrid.transform.DestroyChildren();
        }

        public void AdjustSizeItemDepth() {
            var depth = EditorSizeScrollView.GetComponent<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(EditorSizeGrid.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        public void DeletePrefabs(string atid) {
            for (var i = 0; i < EditorSizeGrid.transform.childCount; i++) {
                var sizePrefab = EditorSizeGrid.transform.GetChild(i).gameObject.GetComponent<EditorSizePrefabs>();
                if (sizePrefab.Atid == atid) {
                    Destroy(sizePrefab.gameObject);
                    EditorSizeGrid.repositionNow = true;
                    break;
                }
            }
            RepositionGrid();
        }

        private void RepositionGrid() {           
            EditorSizeGrid.Reposition(); 
            EditorSizeGrid.repositionNow = true;         
            NGUITools.SetDirty(EditorSizeGrid);
            var springPanel = EditorSizeScrollView.gameObject.GetComponent<SpringPanel>();
            springPanel.target = Vector3.zero;
            springPanel.enabled = true;
        }
    }

}

