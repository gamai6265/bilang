using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrderEvaluateViewPresenter : ViewPresenter {
        public UIButton BtnBack;

        public UIButton BtnCamera;
        public UIButton BtnPhoto;

        private GameObject[] _btnsQuality;
        private GameObject[] _btnsServe;
        private GameObject[] _btnsSize;

        public UIButton BtnSubmit;
        public UIInput IpEvaluate;
        public UILabel LblCode;
        public UILabel LblName;
        public UILabel LblQuantity;
        public UILabel LblCodeName;
        public UILabel LblQuantityName;
        public UILabel LblTitle;
        public UILabel LblEvaluate;
        public UILabel LblTakePhoto;
        public UILabel LblPhoto;
        public UILabel LblEvaluation;
        public UILabel LblServer;
        public UILabel LblSize;
        public UILabel LblQuality;
        public UILabel LblSubmit;

        private int _quality;
        private int _serve;
        private int _size;

        public UIButton SprQuality1;
        public UIButton SprQuality2;
        public UIButton SprQuality3;
        public UIButton SprQuality4;
        public UIButton SprQuality5;

        public UIButton SprServe1;
        public UIButton SprServe2;
        public UIButton SprServe3;
        public UIButton SprServe4;
        public UIButton SprServe5;

        public UIButton SprSize1;
        public UIButton SprSize2;
        public UIButton SprSize3;
        public UIButton SprSize4;
        public UIButton SprSize5;

        public UITexture TexClothes;
        public UITexture TexOne;
        public UITexture TexThree;
        public UITexture TexTwo;

        public event MyAction ClickBack;
        public event MyAction ClickCamera;
        public event MyAction ClickPhoto;
        public event MyAction<int> ClickQuality;
        public event MyAction<int> ClickServe;
        public event MyAction<int> ClickSize;
        public event MyAction<string> ClickSubmit;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("or_v_title");
            LblCodeName.text = StrConfigProvider.Instance.GetStr("or_v_code");
            LblQuantityName.text = StrConfigProvider.Instance.GetStr("or_v_quantity");
            LblEvaluate.text = StrConfigProvider.Instance.GetStr("or_v_evaluate");
            LblTakePhoto.text = StrConfigProvider.Instance.GetStr("or_v_takephoto");
            LblPhoto.text = StrConfigProvider.Instance.GetStr("or_v_photo");
            LblEvaluation.text = StrConfigProvider.Instance.GetStr("or_v_pe");
            LblServer.text = StrConfigProvider.Instance.GetStr("or_v_serve");
            LblSize.text = StrConfigProvider.Instance.GetStr("or_v_size");
            LblQuality.text = StrConfigProvider.Instance.GetStr("or_v_quality");
            LblSubmit.text = StrConfigProvider.Instance.GetStr("or_v_submit");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCodeName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblQuantityName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblEvaluation.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblTakePhoto.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPhoto.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblEvaluate.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblServer.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSize.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblQuality.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSubmit.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            _btnsQuality = new[] {
                SprQuality1.gameObject, SprQuality2.gameObject, SprQuality3.gameObject, SprQuality4.gameObject,
                SprQuality5.gameObject
            };

            _btnsServe = new[] {
                SprServe1.gameObject, SprServe2.gameObject, SprServe3.gameObject, SprServe4.gameObject,
                SprServe5.gameObject
            };

            _btnsSize = new[]
            {SprSize1.gameObject, SprSize2.gameObject, SprSize3.gameObject, SprSize4.gameObject, SprSize5.gameObject};

            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnCamera.onClick.Add(new EventDelegate(OnBtnCameraClick));
            BtnPhoto.onClick.Add(new EventDelegate(OnBtnPhotoClick));

            foreach (var b1 in _btnsQuality) {
                UIEventListener.Get(b1).onClick = OnQualityClick;
            }

            foreach (var b1 in _btnsServe) {
                UIEventListener.Get(b1).onClick = OnServeClick;
            }
            foreach (var b1 in _btnsSize) {
                UIEventListener.Get(b1).onClick = OnSizeClick;
            }

            BtnSubmit.onClick.Add(new EventDelegate(OnSubmit));
        }

        private void OnSubmit() {
            if (null != ClickSubmit) {
                ClickSubmit(IpEvaluate.value);
            }
        }

        private void OnQualityClick(GameObject go) {
            for (var i = 0; i < _btnsQuality.Length; i++) {
                if (go == _btnsQuality[i]) {
                    _quality = i + 1;
                    if (null != ClickQuality) {
                        ClickQuality(_quality);
                    }
                }
                _btnsQuality[i].GetComponentInChildren<UISprite>().spriteName = "Star1";
            }
            for (var k = 0; k < _quality; k++) {
                _btnsQuality[k].GetComponentInChildren<UISprite>().spriteName = "Star2";
            }
        }

        private void OnServeClick(GameObject go) {
            for (var i = 0; i < _btnsServe.Length; i++) {
                if (go == _btnsServe[i]) {
                    _serve = i + 1;
                    if (null != ClickServe) {
                        ClickServe(_serve);
                    }
                }
                _btnsServe[i].GetComponentInChildren<UISprite>().spriteName = "Star1";
            }
            for (var k = 0; k < _serve; k++) {
                _btnsServe[k].GetComponentInChildren<UISprite>().spriteName = "Star2";
            }
        }

        private void OnSizeClick(GameObject go) {
            for (var i = 0; i < _btnsSize.Length; i++) {
                if (go == _btnsSize[i]) {
                    _size = i + 1;
                    if (null != ClickSize) {
                        ClickSize(_size);
                    }
                }
                _btnsSize[i].GetComponentInChildren<UISprite>().spriteName = "Star1";
            }
            for (var k = 0; k < _size; k++) {
                _btnsSize[k].GetComponentInChildren<UISprite>().spriteName = "Star2";
            }
        }


        private void OnBtnPhotoClick() {
            if (null != ClickPhoto) {
                ClickPhoto();
            }
        }

        private void OnBtnCameraClick() {
            if (null != ClickCamera) {
                ClickCamera();
            }
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

        public void SetClothse(string id,string orderName,string orderCode,string number,Texture2D texture2D) {
            LblName.text = orderName;
            LblCode.text = orderCode;
            LblQuantity.text = number;
            TexClothes.mainTexture = texture2D;
            LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCode.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblQuantity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();

        }
        
        public void SetTexture(int index, Texture2D texture2D) {
            switch (index) {
                case 0:
                    TexOne.mainTexture = texture2D;
                    break;
                case 1:
                    TexTwo.mainTexture = texture2D;
                    break;
                case 2:
                    TexThree.mainTexture = texture2D;
                    break;
            }
        }

        public void ReSet() {
            LblName.text = String.Empty;
            LblCode.text = String.Empty;
            LblQuantity.text = String.Empty;
            TexClothes.mainTexture = null;
            TexOne.mainTexture = null;
            TexTwo.mainTexture = null;
            TexThree.mainTexture = null;
            IpEvaluate.value = string.Empty;
            for (var k = 0; k < _btnsQuality.Length; k++) {
                _btnsQuality[k].GetComponentInChildren<UISprite>().spriteName = "Star1";
                _btnsServe[k].GetComponentInChildren<UISprite>().spriteName = "Star1";
                _btnsSize[k].GetComponentInChildren<UISprite>().spriteName = "Star1";
            }
        }
    }
}