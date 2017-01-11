using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class CollectViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UIButton BtnBack;

        public UIScrollView CollectScrollView;
        public UIGrid CollectGrid;
        public GameObject CollectPrefabs;
        
        public event MyAction ClickBack;
        public event MyAction<string> ClickSure;

        public event MyAction<string> ClickChoose;
        private string _templetCode;

        private Vector3 _startPosition = Vector3.zero;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            LblTitle.text = StrConfigProvider.Instance.GetStr("c_title");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            CollectScrollView.onDragStarted+=CollectScrollViewOnDragStart;
            CollectScrollView.onDragFinished += CollectScrollViewOnDragFinished;
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
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public void AddPrefabs(string code, string collectName, string price, string time, Texture2D texture2D) {
            var obj = NGUITools.AddChild(CollectGrid.gameObject, CollectPrefabs);

            var cp = obj.GetComponent<CollectPrefabs>();
            obj.name = cp.TempletCode = code;
            cp.TexCollect.mainTexture = texture2D;
            cp.LblName.text = collectName;
            cp.LblPrice.text = price;
            cp.LblDate.text = time;

            UIEventListener.Get(obj).onClick += OnClickChoose;
            cp.ClickDelete += OnClickDelete;

            RepositionGrid();
        }

        private void CollectScrollViewOnDragStart() {
            _startPosition = CollectScrollView.transform.localPosition;
        }

        private void CollectScrollViewOnDragFinished() {
            if (!CollectScrollView.shouldMoveVertically) {
                SpringPanel.Begin(CollectScrollView.GetComponent<UIPanel>().gameObject, _startPosition, 13f)
                    .strength = 8f;
           }
        }

        private void RepositionGrid() {
            CollectGrid.Reposition();
            CollectGrid.repositionNow = true;
            NGUITools.SetDirty(CollectGrid);
            var springPanel = CollectScrollView.gameObject.GetComponent<SpringPanel>();
            springPanel.target = Vector3.zero;
            springPanel.enabled = true;
            CollectScrollView.MoveRelative(new Vector3(0,1,0));
        }

        private void OnClickChoose(GameObject go) {
            var cp = go.GetComponent<CollectPrefabs>();
            if (null != cp && null != ClickChoose) {
                ClickChoose(cp.TempletCode);
            }
        }

        private void OnClickDelete(string templetCode) {
            _templetCode = templetCode;
            if (null != ClickSure) {
                ClickSure(_templetCode);
            }
        }

        public void DeleteAllPrefabs() {
             CollectGrid.transform.DestroyChildren();
        }

        public void DeletePrefabs(string templetCode) {
            for (var i = 0; i < CollectGrid.transform.childCount; i++) {
                var str = CollectGrid.transform.GetChild(i).gameObject.name;
                if (templetCode == str) {
                    Destroy(CollectGrid.transform.GetChild(i).gameObject);
                    CollectGrid.repositionNow = true;
                    break;
                }
            }           
            RepositionGrid();
        }
        public void AdjustItem() {
            var depth = CollectScrollView.GetComponent<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(CollectScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }



    }
}