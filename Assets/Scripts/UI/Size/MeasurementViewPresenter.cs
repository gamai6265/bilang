using UnityEngine;
using System.Collections;
using Cloth3D.Comm;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class MeasurementViewPresenter : ViewPresenter {

        public UILabel LblTitle;
        public UIButton BtnBack;
        public UIButton BtnAdaptation;
        public UIButton BtnService;
        public UIButton BtnSave;
        public UIInput IpHight;
        public UIInput IpWeight;
        public UITexture TexHead;
        public UILabel LblUserName;

        public GameObject MeasurePrefab;
        public UIGrid MeasurementGrid;
        
        public event MyAction ClickBack;
        public event MyAction<string, string> ClickAdaptation;
        public event MyAction ClickService;
        public event MyAction ClickSave;
        public event MyAction<string> OnClickChooseSize; 

        protected override void AwakeUnityMsg() {
             ChangeLanguage();
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

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("mm_title");
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnSave.onClick.Add(new EventDelegate(OnBtnSaveClick));
            BtnAdaptation.onClick.Add(new EventDelegate(OnBtnAdaptationClick));
            BtnService.onClick.Add(new EventDelegate(OnBtnServiceClick));
        }

        private void OnBtnServiceClick() {
            if (null != ClickService) {
                ClickService();
            }
        }

        private void OnBtnAdaptationClick() {
            if (null != ClickAdaptation) {
                ClickAdaptation(IpHight.value, IpWeight.value);
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }
        private void OnBtnSaveClick() {
            if (null != ClickSave) {
                ClickSave();
            }
        }
       
        public void AddPrefabs(SizeInfo sizeInfo) {
            GameObject obj = NGUITools.AddChild(MeasurementGrid.gameObject, MeasurePrefab);
            MeasurementPrefabs mps = obj.GetComponent<MeasurementPrefabs>();
            mps.SetLabel(sizeInfo);
            mps.OnChooseClick += OnChooseClickSize;
            MeasurementGrid.Reposition();
            MeasurementGrid.repositionNow = true;
        }

        public void SetHightAndWeight(string hight, string weight) {
            IpHight.value = hight;
            IpWeight.value = weight;
        }

        public void SetUserInfo(string userName, string photo, Texture2D texture2D) {
            LblUserName.text = userName;
            if (texture2D == null) {
                CoroutineMgr.Instance.StartCoroutine(GetPhone(photo));
            }else {
                TexHead.mainTexture = texture2D;
            }
        }

        private IEnumerator GetPhone(string path) {
            var www = new WWW(path);
            yield return www;
            if (www.error == null) {
                if (www.isDone) {
                    var texture = www.texture;
                    TexHead.mainTexture = texture;
                }
            }
            else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                TexHead.mainTexture = www.texture;
            }
            www.Dispose();
        }

        public void DeleteAll() {
            MeasurementGrid.transform.DestroyChildren();
        }

        private void OnChooseClickSize(string propertiesCode) {
            if (null != OnClickChooseSize) {
                OnClickChooseSize(propertiesCode);
            }
        }

        public void DeleteAllPrefabs() {
          MeasurementGrid.transform.DestroyChildren();
        }
    }
}
