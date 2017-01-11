using UnityEngine;
using Cloth3D.Data;
using Cloth3D.Interfaces;

namespace Cloth3D.Ui {
    public class MySizeViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UIButton BtnHeight;
        public UIButton BtnWeight;
        public UIButton BtnShoulder;
        public UIButton BtnBust;
        public UIButton BtnWaist;
        public UIButton BtnHipline;

        public UILabel LblHeight;
        public UILabel LblWeight;
        public UILabel LblShoulder;
        public UILabel LblBust;
        public UILabel LblWaist;
        public UILabel LblHipline;

        public UILabel LblTitle;
        public UILabel LblHeightName;
        public UILabel LblWeightName;
        public UILabel LblShoulderName;
        public UILabel LblBustName;
        public UILabel LblWaistName;
        public UILabel LblHiplineName;

        private float _currentSize;
        private int _minSize;
        private int _maxSize;

        public event MyAction ClickBack;
        public event MyAction<MysizeEntry> ClickSize;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("mys_title");
            LblHeightName.text = StrConfigProvider.Instance.GetStr("mys_height");
            LblWeightName.text = StrConfigProvider.Instance.GetStr("mys_weight");
            LblShoulderName.text = StrConfigProvider.Instance.GetStr("mys_shoulder");
            LblBustName.text = StrConfigProvider.Instance.GetStr("mys_bust");
            LblWaistName.text = StrConfigProvider.Instance.GetStr("mys_waistline");
            LblHiplineName.text = StrConfigProvider.Instance.GetStr("mys_hipline");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblHeightName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblWeightName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblShoulderName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblBustName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblWaistName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblHiplineName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            UIEventListener.Get(BtnHeight.gameObject).onClick += OnBtnSizeClick;
            UIEventListener.Get(BtnWeight.gameObject).onClick += OnBtnSizeClick;
            UIEventListener.Get(BtnShoulder.gameObject).onClick += OnBtnSizeClick;
            UIEventListener.Get(BtnBust.gameObject).onClick += OnBtnSizeClick;
            UIEventListener.Get(BtnWaist.gameObject).onClick += OnBtnSizeClick;
            UIEventListener.Get(BtnHipline.gameObject).onClick += OnBtnSizeClick;
        }
        private void OnBtnSizeClick(GameObject go) {
            MysizeEntry size = MysizeEntry.Height;
            if (go ==BtnHeight.gameObject) {
                size = MysizeEntry.Height;
            }else if(go ==BtnWeight.gameObject) {
                size = MysizeEntry.Weight;              
            }else if (go ==BtnShoulder.gameObject) {
                size = MysizeEntry.Shoulder;           
            }else if (go ==BtnBust.gameObject) {
                size = MysizeEntry.Bust;
            }else if (go ==BtnWaist.gameObject) {
                size = MysizeEntry.Waistline;
            }else if (go ==BtnHipline.gameObject) {
                size = MysizeEntry.Hipline;
            }
            if (ClickSize!= null) {
                ClickSize(size);
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        public void SetSizeLabel(UserSize userSize) {
            double tem;
            double.TryParse(userSize.Height, out tem);
            LblHeight.text = string.Format("{0:0.0}{1}", tem, "cm");
            double.TryParse(userSize.Weight, out tem);
            LblWeight.text = string.Format("{0:0.0}{1}", tem, "kg");
            double.TryParse(userSize.Shoulder, out tem);
            LblShoulder.text = string.Format("{0:0.0}{1}", tem, "cm");
            double.TryParse(userSize.Bust, out tem);
            LblBust.text = string.Format("{0:0.0}{1}", tem, "cm");
            double.TryParse(userSize.WaistLine, out tem);
            LblWaist.text = string.Format("{0:0.0}{1}", tem, "cm");
            double.TryParse(userSize.HipLine, out tem);
            LblHipline.text = string.Format("{0:0.0}{1}", tem, "cm");
        }
    }
}