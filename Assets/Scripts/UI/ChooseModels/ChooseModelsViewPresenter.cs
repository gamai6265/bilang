using System.Collections.Generic;
using UnityEngine;

namespace Cloth3D.Ui{
    public class ChooseModelsViewPresenter : ViewPresenter{
        public UILabel LblTitle;
        public UIButton BtnBack;
        public UIGrid ChooseModelsGrid;
        public GameObject ChooseModelsObj;
        public event MyAction ClickBack;
        public event MyAction<int> ClickChoose;
        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBack));
        }

        public override void InitWindowData(){
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }
        protected override void OnDestroyUnityMsg() {
        }

        private void OnBtnBack() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        public void AddPrefabs(int id, string typeName, Texture2D sampleImage) {
            var obj = NGUITools.AddChild(ChooseModelsGrid.gameObject, ChooseModelsObj);
            var cmp = obj.GetComponent<ChooseModelsPrefab>();
            cmp.Lbl.text = typeName;
            cmp.Tex.mainTexture = sampleImage;
            cmp.Id = id;
            UIEventListener.Get(obj).onClick += OnClickChooseModels;

            ChooseModelsGrid.Reposition();
            ChooseModelsGrid.repositionNow = true;
            NGUITools.SetDirty(ChooseModelsGrid);
        }

        private void OnClickChooseModels(GameObject go) {
            var cmp = go.GetComponent<ChooseModelsPrefab>();
            if (null != cmp && null != ClickChoose) {
                ClickChoose(cmp.Id);
            }
        }

        public void DeleteAll() {
            ChooseModelsGrid.transform.DestroyChildren();
        }
    }
}