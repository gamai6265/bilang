using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class TrendyViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UIButton BtnBack;
        public UIScrollView TrendyScrollView;
        public UIGrid TrendyGrid;
        public GameObject TrendyPrefabs;

        public event MyAction ClickBack;
        public event MyAction<string> ClickChoose;


        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            LblTitle.text = StrConfigProvider.Instance.GetStr("t_title");
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
            //HideWindow();
        }

        public void AddPrefabs(string id, Texture2D texture2D, string subjectDesc) {
            var obj = NGUITools.AddChild(TrendyGrid.gameObject, TrendyPrefabs);

            var tp = obj.GetComponent<TrendyPrefabs>();
            obj.name = tp.Id = id;
            tp.TexTrendy.mainTexture = texture2D;
            tp.LblTrendy.text = subjectDesc;

            UIEventListener.Get(obj).onClick += OnPrefabsClick;

            TrendyGrid.Reposition();
            TrendyGrid.repositionNow = true;
            NGUITools.SetDirty(TrendyGrid);
        }

        public void ResetScrollView() {
         TrendyScrollView.ResetPosition();
        }

        private void OnPrefabsClick(GameObject go) {
            var tp = go.GetComponent<TrendyPrefabs>();
            if (null != tp && null != ClickChoose) {
                ClickChoose(tp.Id);
            }
        }

        public void DeleteAll() {
            TrendyGrid.transform.DestroyChildren();
        }
    }
}