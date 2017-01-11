using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class HomeViewPresenter : ViewPresenter,IWindowAnimation {
        public UIGrid HomeGrid;
        public GameObject AdvertisementPrefabs;

        public UIButton BtnInfo;
        public TweenPosition TweenerHome;

        public UIGrid RecommendGrid;
        public GameObject RecommendPrefab;
        public UIButton BtnScreen;

        public UIGrid DesignersRecommendGrid;
        public GameObject DRItem;

        public event MyAction ClickInfo;
        public event MyAction<string> ClickAd;
        public event MyAction<JsonData> ClickChooseRecommend;
        public event MyAction ClickScreen;
        public event MyAction<string> ClickMore;
        private Vector3 _from;
        private Vector3 _to;

        protected override void AwakeUnityMsg() {
            BtnsListener();
            _from = TweenerHome.from;
            _to = TweenerHome.to;
        }

        protected override void StartUnityMsg() {
        }

        protected override void OnDestroyUnityMsg() {
        }

        public void SetNoticeList(string[] url, Texture2D[] texturelist) {
            for (var i = 0; i < url.Length; i++) {
                var obj = NGUITools.AddChild(HomeGrid.gameObject, AdvertisementPrefabs);
                HomeGrid.Reposition();
                HomeGrid.repositionNow = true;
                obj.GetComponent<UITexture>().mainTexture = texturelist[i];

                var ap = obj.GetComponent<AdvertisementPrefabs>();
                ap.Url = url[i];
                UIEventListener.Get(obj).onClick += OnAdClick;
            }
        }

        private void OnAdClick(GameObject go) {
            var ad = go.GetComponent<AdvertisementPrefabs>();
            if (null!=ad && null!=ClickAd) {
                ClickAd(ad.Url);
            }
        }

        private void BtnsListener() {
            BtnInfo.onClick.Add(new EventDelegate(OnBtnInfoClick));
            BtnScreen.onClick.Add(new EventDelegate(OnBtnScreenClick));
        }

        private void OnBtnInfoClick() {
            if (null != ClickInfo) {
                ClickInfo();
            }
        }

        private void OnBtnScreenClick() {
            if (null != ClickScreen) {
                ClickScreen();
            }
        }

        public void DockAnimation(bool backWard=false) {
            if (backWard) {
                transform.localPosition = _to;
                TweenPosition.Begin(gameObject, TweenerHome.duration, _from);
            } else {
                transform.localPosition = _from;
                TweenPosition.Begin(gameObject, TweenerHome.duration, _to);
            }
        }
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        public void EnterAnimation(EventDelegate.Callback onComplete) {
        }
        public void QuitAnimation(EventDelegate.Callback onComplete) {
        }
        public void ResetAnimation() {
        }

        private void OnPrefabsClick(GameObject go) {
            var rp = go.GetComponent<RecommendPrefab>();
            if (null != rp && null != ClickChooseRecommend) {
                ClickChooseRecommend(rp.ProductJson);
            }
        }

        public void AddPlatformRecommendPrefabs(JsonData jdata, Texture2D[] texture2Ds) {
            for (var i = 0; i < jdata.Count; i++) {
                var jsonData = jdata[i];
                var item = NGUITools.AddChild(RecommendGrid.gameObject, RecommendPrefab);
                RecommendPrefab rp = item.GetComponent<RecommendPrefab>();
                rp.atid = Utils.GetJsonStr(jsonData["atid"]);
                rp.ProductJson = jsonData;
                rp.LblName.text = Utils.GetJsonStr(jsonData["name"]);
                var price = (Utils.GetJsonStr(jsonData["salesPrice"]) != string.Empty)
                    ? Utils.GetJsonStr(jsonData["salesPrice"])
                    : Utils.GetJsonStr(jsonData["originalPrice"]);
                rp.LblPrice.text = price;
                rp.TexRecommend.mainTexture = texture2Ds[i];
                UIEventListener.Get(item).onClick += OnPrefabsClick;

                RecommendGrid.Reposition();
                RecommendGrid.repositionNow = true;
                NGUITools.SetDirty(RecommendGrid);
            }
        }


        public void AddClassifyManageListPrefabs(JsonData jdata, Texture2D[] texture2Ds) {
            var obj = NGUITools.AddChild(DesignersRecommendGrid.gameObject, DRItem);
            DesignersRecommendGrid.Reposition();
            DesignersRecommendGrid.repositionNow = true;
            NGUITools.SetDirty(DesignersRecommendGrid);
            
            DesignersRecommendItemCtr drtc = obj.GetComponent<DesignersRecommendItemCtr>();
            for (var i = 0; i < jdata.Count; i++) {
                var jsonData = jdata[i];
                if(i==0)
                    drtc.LblTitle.text = drtc.DRType = Utils.GetJsonStr(jsonData["type"]);
                var item = NGUITools.AddChild(drtc.RecommendGrid.gameObject, RecommendPrefab);
                RecommendPrefab rp = item.GetComponent<RecommendPrefab>();
                rp.atid = Utils.GetJsonStr(jsonData["atid"]);
                rp.ProductJson = jsonData;
                rp.LblName.text = Utils.GetJsonStr(jsonData["name"]);
                var price = (Utils.GetJsonStr(jsonData["salesPrice"]) != string.Empty)
                    ? Utils.GetJsonStr(jsonData["salesPrice"])
                    : Utils.GetJsonStr(jsonData["originalPrice"]);
                rp.LblPrice.text = price;
                rp.TexRecommend.mainTexture = texture2Ds[i];
                UIEventListener.Get(item).onClick += OnPrefabsClick;

                drtc.RecommendGrid.Reposition();
                drtc.RecommendGrid.repositionNow = true;
                NGUITools.SetDirty(drtc.RecommendGrid);
            }
            drtc.ClickMore += OnDRMoreClick;
        }

        private void OnDRMoreClick(string drType) {
            if (null != ClickMore)
                ClickMore(drType);
        }

        public void DeleteAllDesignersRecommendItem() {
            RecommendGrid.transform.DestroyChildren();
            DesignersRecommendGrid.transform.DestroyChildren();
        }
    }
}