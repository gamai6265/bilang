using UnityEngine;
using System.Collections;
using System;
using Cloth3D.Comm;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class MeasuringSizeViewPresenter : ViewPresenter {
        public UITexture TexPartSize;
//        public UILabel LblExplain;
        public UILabel LblSize;
        public UIButton BtnBack;
        public UIButton BtnYes;
        public UISprite MeasureSprite;
//        public UITexture MeasurTexture;
        public UILabel LblTestMethod;
        public UILabel LblTitle;
        public UILabel LblMethod;

        public UIScrollView IntegerScrollView;
        public UIGrid IntegerGrid;
        public UIScrollView DecimalsScrollView;
        public UIGrid DecimalsGrid;

        public event MyAction ClickBack;
        public event MyAction<string,float> ClickYes;

        public GameObject SizeLabelPrefab;
        public GameObject PointInteger;
        public GameObject PointDecimals;

        private int _maxIntegerSize;
        private int _minIntegerSize;
        private int _integer;
        private float _decimals;
        private string _unit;
        private string _sizeCode;

        protected override void AwakeUnityMsg() {
            IntegerScrollView.onMomentumMove = OnIntegerDragFinished;
            DecimalsScrollView.onMomentumMove = OnDecimalsDragFinished;
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("ms_title");
            LblMethod.text = StrConfigProvider.Instance.GetStr("ms_method");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblMethod.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnIntegerDragFinished() {
            if (IntegerGrid.GetComponent<UICenterOnChild>().enabled == false)
                IntegerGrid.GetComponent<UICenterOnChild>().enabled = true;
            if (RayController(PointInteger) != null) {
                int tem;
                int.TryParse(RayController(PointInteger), out tem);
                _integer = tem;
                SetSize();
            }

        }

        private void OnDecimalsDragFinished() {
            if (DecimalsGrid.GetComponent<UICenterOnChild>().enabled == false)
                DecimalsGrid.GetComponent<UICenterOnChild>().enabled = true;
            if (RayController(PointDecimals) != null) {
                float tem;
                float.TryParse(RayController(PointDecimals), out tem);
                _decimals = tem;
                SetSize();
            }
        }

        void Update() {
            if (IntegerScrollView.isDragging) {
                OnIntegerDragFinished();
            }
            if (DecimalsScrollView.isDragging) {
                OnDecimalsDragFinished();
            }
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        protected override void OnDestroyUnityMsg() {
            IntegerScrollView.onDragFinished = null;
            DecimalsScrollView.onDragFinished = null;
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnYes.onClick.Add(new EventDelegate(OnBtnYesClick));
        }

        private void OnBtnYesClick() {
            if (null != ClickYes) {
                ClickYes(_sizeCode,_integer + _decimals);
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void SetSize() {               
            LblSize.text = string.Format("{0:0.0}{1}", _integer + _decimals, _unit);
        }

//        public void SetExplain(string str) {
//            LblExplain.text = str;
//        }
//
//        public void SetTexPartSize(Texture2D texture2D) {
//            TexPartSize.mainTexture = texture2D;
//        }

        private void AddSizeInteger() {
            for (int i = _minIntegerSize; i <= _maxIntegerSize; i++) {
                AddChild(IntegerScrollView, IntegerGrid.gameObject, SizeLabelPrefab, i.ToString());
            }

            var length = (_maxIntegerSize - _minIntegerSize)*IntegerGrid.cellWidth/2;
            var targetX = length - (_integer - _minIntegerSize)*IntegerGrid.cellWidth;
            var springPanel = IntegerScrollView.GetComponent<SpringPanel>();
            var pos = springPanel.transform.localPosition;
            SpringPanel.Begin(springPanel.gameObject, new Vector3(targetX, pos.y, pos.z), 8);
            IntegerGrid.GetComponent<UICenterOnChild>().enabled = false;
            springPanel.onFinished += () => {
                IntegerGrid.GetComponent<UICenterOnChild>().enabled = true;
                springPanel.onFinished = null;
            };
        }


        private void AddSizeDecimals() {
            for (float i = 0; i <= 9; i++) {
                AddChild(DecimalsScrollView, DecimalsGrid.gameObject, SizeLabelPrefab, "0." + i);
            }
            var length = 9*DecimalsGrid.cellWidth/2;
            var targetX = length - Convert.ToInt32(_decimals*10)*DecimalsGrid.cellWidth;
            var springPanel = DecimalsScrollView.GetComponent<SpringPanel>();
            var pos = springPanel.transform.localPosition;
            SpringPanel.Begin(springPanel.gameObject, new Vector3(targetX, pos.y, pos.z), 8);
            DecimalsGrid.GetComponent<UICenterOnChild>().enabled = false;
            springPanel.onFinished += () => {
                DecimalsGrid.GetComponent<UICenterOnChild>().enabled = true;
                springPanel.onFinished = null;

            };
        }

        public void ShowRuler() {
            AddSizeInteger();
            AddSizeDecimals();
        }

        private void AddChild(UIScrollView scrollView, GameObject grid, GameObject prefab, string size) {
            var obj = NGUITools.AddChild(grid, prefab);
            obj.name = size;
            var sizeNumber = obj.GetComponent<UILabel>();
            obj.GetComponent<UIDragScrollView>().scrollView = scrollView;
            sizeNumber.text = size;
            scrollView.ResetPosition();
            grid.GetComponent<UIGrid>().repositionNow = true;
        }

        private string RayController(GameObject point) {
            var position = UICamera.mainCamera.WorldToScreenPoint(point.transform.TransformPoint(Vector3.zero));
            var ray = UICamera.mainCamera.ScreenPointToRay(position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.GetComponent<UILabel>() != null)
                    return hit.collider.GetComponent<UILabel>().text;
            }
            return null;
        }

        public void ShowMySize(string spriteName) {
            LblTestMethod.text = "";
            MeasureSprite.spriteName = spriteName;
//            MeasurTexture.mainTexture = null;
            TexPartSize.mainTexture = null;
        }


        public void DeleteAllPrefabs() {
            _sizeCode = string.Empty;
            IntegerGrid.transform.DestroyChildren();
            DecimalsGrid.transform.DestroyChildren();
        }

        public void SetRulerSize(string sizeCode, float currenSize, SizeInfo sizeInfo) {
            _sizeCode = sizeCode;
            _integer = (int) currenSize;
            _decimals = currenSize - _integer;
            _minIntegerSize = sizeInfo.MinSize;
            _maxIntegerSize = sizeInfo.MaxSize;       
            LblTestMethod.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            _unit = sizeInfo.unit;
            SetSize();
        }

        public void SetMeasurementTexture(string measureMethod, Texture2D texture2D) {
            LblTestMethod.text = measureMethod;
            TexPartSize.mainTexture = texture2D;
            MeasureSprite.spriteName = "";
        }

        public void SetMySize(float currentSize,int minSize,int maxSize,string unit) {
            _sizeCode = null;
            _integer = (int)currentSize;
            _decimals = currentSize - _integer;
            _minIntegerSize = minSize;
            _maxIntegerSize = maxSize;          
            _unit = unit;
            SetSize();
        }
    }
}