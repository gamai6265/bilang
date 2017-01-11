using System;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ButtonViewPresenter : ViewPresenter {
        public UIButton BtnYes;
        public UIGrid ButtonGrid;
        public GameObject ButtonItemPrefabs;

        public UIScrollView ButtonScrollView;

        public event MyAction ClickYes;
        public event MyAction SvDragFinished;
        public event MyAction<string, Texture2D> ClickButtonItem;

        protected override void AwakeUnityMsg() {
            ButtonScrollView.onDragFinished += OnDragFinished;
            BtnYes.onClick.Add(new EventDelegate(OnBtnYesClick));
        }

        protected override void StartUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void OnBtnYesClick() {
            if (null != ClickYes) {
                ClickYes();
            }
        }

        private void OnDragFinished() {
            var constraint = ButtonScrollView.panel.CalculateConstrainOffset(ButtonScrollView.bounds.min,
                ButtonScrollView.bounds.max);
            if (constraint.x > 1f) {
                if (null != SvDragFinished) {
                    SvDragFinished();
                }
            }
        }

        public void AddPrefabs(string pname, Texture2D texture) {
            var obj = NGUITools.AddChild(ButtonGrid.gameObject, ButtonItemPrefabs);

            var bip = obj.GetComponent<ButtonItemPrefabs>();
            obj.name = bip.LblName.text = pname;
            bip.texture.mainTexture = texture;

            UIEventListener.Get(obj).onClick += OnButtonPrefabsClick;

            ButtonGrid.Reposition();
            ButtonGrid.repositionNow = true;
            NGUITools.SetDirty(ButtonGrid);
        }

        private void OnButtonPrefabsClick(GameObject go) {
            var bip = go.GetComponent<ButtonItemPrefabs>();
            if (null != bip && null != ClickButtonItem) {
                ClickButtonItem(bip.name, (Texture2D)bip.texture.mainTexture);
            }
        }

        public void DeleteAllPrefabs() {
            ButtonGrid.transform.DestroyChildren();
        }
    }
}