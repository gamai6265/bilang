using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class TrendyDetailsViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UILabel LblTitle;
        public GameObject TrendDetailsPrefab;

        public UIScrollView TrendyDetailsScrollView;
        public UIGrid TrendyDetailsGrid;
        public event MyAction ClickBack;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            LblTitle.text = StrConfigProvider.Instance.GetStr("t_title");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        protected override void OnDestroyUnityMsg() {
        }

        public void AddPrefabs(string description, Texture2D texture2D) {
            var obj = NGUITools.AddChild(TrendyDetailsGrid.gameObject, TrendDetailsPrefab);

            var tdp = obj.GetComponent<TrendDetailsPrefab>();
            tdp.LblDetails.text = description;
            tdp.TrendDetailsPrefabs.mainTexture = texture2D;

            TrendyDetailsGrid.Reposition();
            TrendyDetailsGrid.repositionNow = true;
            NGUITools.SetDirty(TrendyDetailsGrid);
        }

        public void ResetScrollView() {
           TrendyDetailsScrollView.ResetPosition();
        }

        public void DeleteAllPrefabs() {
            TrendyDetailsGrid.transform.DestroyChildren();
        }
    }


}