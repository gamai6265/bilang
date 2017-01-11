using UnityEngine;
using System.Collections;
using LitJson;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cloth3D;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class HistorySizeViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UILabel LblTitle;

        public UIScrollView HistoryScrollView;
        public UIGrid HistorySizeGrid;
        public GameObject HistorySizePrefabs;
        public GameObject SizePrefabs;

        public event MyAction ClickBack;
        public event MyAction<List<SizeInfo>> ClickChoose;
        public event MyAction<List<SizeInfo>> ClickSave;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            ChangeLanguage();
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

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        public void AddHistorySizePrefabs(List<SizeInfo> sizeInfos) {
            GameObject obj = NGUITools.AddChild(HistorySizeGrid.gameObject, HistorySizePrefabs);
            HistorySizePrefabs hip = obj.GetComponent<HistorySizePrefabs>();
            hip.LstSizeInfo = sizeInfos;
            for (int i = 0; i < sizeInfos.Count; i++) {
                GameObject size = NGUITools.AddChild(hip.SizeGrid.gameObject, SizePrefabs);
                SizePrefab sp = size.GetComponent<SizePrefab>();
                if (i >= 8) {
                    sp.LblName.text = "...";
                    break;
                }
                sp.LblName.text = sizeInfos[i].SizeName + ":";
                sp.LblValue.text = sizeInfos[i].SizeValue;
//                sp.ChangeLangele();
            }
            hip.SizeGrid.Reposition();
            hip.SizeGrid.repositionNow = true;
            HistorySizeGrid.Reposition();
            HistorySizeGrid.repositionNow = true;
            hip.ClickChoose += OnChooseSize;
            hip.ClickSave += OnSave;
        }

        private void OnChooseSize(List<SizeInfo> sizeInfos) {
            if(null!=ClickChoose)
                ClickChoose(sizeInfos);
        }

        private void OnSave(List<SizeInfo> sizeInfos) {
            if (null != ClickSave)
                ClickSave(sizeInfos);
        }

        public void DeleteAllPrefabs() {
            HistorySizeGrid.transform.DestroyChildren();
        }

        public void AdjustSizeItemDepth() {
            var depth = HistoryScrollView.GetComponent<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(HistorySizeGrid.gameObject, depth+1);
            GetSortedPanels(true);
        }
    }
}