using UnityEngine;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class RecommendViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UIButton BtnBack;
        public event MyAction ClickBack;
        public event MyAction<string> ClickChoose;

        public UIGrid RecommendGrid;
        public GameObject RecommendPrefabs;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            LblTitle.text = StrConfigProvider.Instance.GetStr("re_title");
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

        public void AddPrefabs(string templetCode,string atid, Texture2D _texture2D) {
            GameObject obj = NGUITools.AddChild(RecommendGrid.gameObject, RecommendPrefabs);

            RecommendPrefab rp = obj.GetComponent<RecommendPrefab>();
            rp.name = templetCode;
            rp.atid = atid;
            rp.TexRecommend.mainTexture = _texture2D;

            UIEventListener.Get(obj).onClick += OnRecommendPrefabsClick;

            RecommendGrid.Reposition();
            RecommendGrid.repositionNow = true;
            NGUITools.SetDirty(RecommendGrid);
        }

        private void OnRecommendPrefabsClick(GameObject go) {
            var rp = go.GetComponent<RecommendPrefab>();
            if (null != rp && null != ClickChoose) {
                ClickChoose(rp.atid);
            }
        }

        public void DeleteAll() {
            RecommendGrid.transform.DestroyChildren();
        }
    }
}