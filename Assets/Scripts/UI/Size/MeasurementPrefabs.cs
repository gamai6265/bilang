using UnityEngine;
using System.Collections;
using Cloth3D.Comm;
using Cloth3D.Data;

namespace  Cloth3D.Ui {
    public class MeasurementPrefabs : ViewPresenter {
        public UILabel LblName;
        public UILabel LblValue;
        public UIButton BtnChoose;
        public string propertiesCode;
        public UITexture Texture;

        public event MyAction<string> OnChooseClick;

        public override void InitWindowData() {
        }

        public void SetLabel(SizeInfo sizeInfo) {
            propertiesCode = sizeInfo.PropertiesCode;
            if (sizeInfo.ShowValue == "-" || float.Parse(sizeInfo.SizeValue) < sizeInfo.MinSize ||
                float.Parse(sizeInfo.SizeValue) > sizeInfo.MaxSize) {
                LblName.text = "[FF0000FF]" + sizeInfo.SizeName + "[-]";
                LblValue.text = "[FF0000FF]" + sizeInfo.ShowValue + " " + sizeInfo.unit + "[-]";
            } else {
                LblName.text = sizeInfo.SizeName;
                LblValue.text = sizeInfo.ShowValue + " " + sizeInfo.unit;
            }
            CoroutineMgr.Instance.StartCoroutine(GetImage(sizeInfo.SizePicture));
            AdjustSize();
        }

        private IEnumerator GetImage(string picture) {
            var www = new WWW(Constants.SizeImage + picture);
            yield return www;
            if (www.error == null) {
                if (www.isDone) {
                    Texture.mainTexture = www.texture;
                }
            }
            else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                Texture.mainTexture = www.texture;
            }
            www.Dispose();
        }

        private void AdjustSize() {
            LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblValue.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        protected override void AwakeUnityMsg() {
           BtnChoose.onClick.Add(new EventDelegate(Onclick));
        }

        private void Onclick() {
            if (null != OnChooseClick) {
                OnChooseClick(propertiesCode);
            }
        }
    }
}

