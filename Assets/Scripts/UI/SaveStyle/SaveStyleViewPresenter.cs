using UnityEngine;
using System.Collections;
using System;
using Cloth3D;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class SaveStyleViewPresenter : ViewPresenter {
        public UIButton BtnBack;

        public UILabel LbShirtName;
        public UIInput IpIdea;
        public UIInput IpTag;

        public UIButton BtnSeasons;
        public UILabel LblSeasons;
        public UIButton BtnStyle;
        public UILabel LblStyle;

        public UILabel LblSaveStyle1;
        public UILabel LblSaveStyle2;
        public UILabel LblStyleName;
        public UILabel LblDesignIdea;
        public UILabel LblTag;
        public UILabel LblSeason;
        public UILabel LblStyles;
        public UILabel LblSave;

        public UILabel LbExplain;
        public UIGrid PrefabsGrid;
        public GameObject SaveStylePrefabs;

        public UIButton BtnSave;

        public UITweener Panel2;

        public event MyAction ClickBack;
        public event MyAction ClickSeasons;
        public event MyAction ClickStyle;
        public event MyAction ClickSave;
        public event MyAction<string> ClickStyleBtn;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void ChangeLanguage() {
            LblSaveStyle1.text = StrConfigProvider.Instance.GetStr("ss_savestyle");
            LblSaveStyle2.text = StrConfigProvider.Instance.GetStr("ss_savestyle");
            LblStyleName.text = StrConfigProvider.Instance.GetStr("ss_stylename");
            LblDesignIdea.text = StrConfigProvider.Instance.GetStr("ss_designidea");
            LblTag.text = StrConfigProvider.Instance.GetStr("ss_tag");
            LblSeason.text = StrConfigProvider.Instance.GetStr("ss_season");
            LblStyles.text = StrConfigProvider.Instance.GetStr("ss_style");
            LblSave.text = StrConfigProvider.Instance.GetStr("ss_save");
            LblSeasons.text = StrConfigProvider.Instance.GetStr("ss_unlimited");
            LblStyle.text = StrConfigProvider.Instance.GetStr("ss_unlimited");
            LbShirtName.text = StrConfigProvider.Instance.GetStr("ss_stylename");
            LblSaveStyle1.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSaveStyle2.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblStyleName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblDesignIdea.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblTag.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSeason.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblStyles.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSave.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSeasons.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblStyle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LbShirtName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnSeasons.onClick.Add(new EventDelegate(OnSeasonsClick));
            BtnStyle.onClick.Add(new EventDelegate(OnStyleClick));
            BtnSave.onClick.Add(new EventDelegate(OnBtnSaveClick));
        }
        private void OnBtnSaveClick() {
            if (null != ClickSave) {
                ClickSave();
            }
        }

        private void OnStyleClick() {
            if (null != ClickStyle) {
                ClickStyle();
            }
        }

        private void OnSeasonsClick() {
            if (null != ClickSeasons) {
                ClickSeasons();
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        protected override void OnDestroyUnityMsg() {
        }

        public void PlayTweener(bool tof) {
            if (tof) {
                Panel2.ResetToBeginning();
                Panel2.PlayForward();
            } else {
                Panel2.PlayReverse();
            }
        }

        public void SetShirtName(string _str) {
            LbShirtName.text = _str;
        }

        public void SetSeasonsLabel(string _str) {
            LblSeasons.text = _str;
        }

        public void SetStyleLabel(string _str) {
            LblStyle.text = _str;
        }

        public void SetLbExplain(string _str) {
            LbExplain.text = _str;
        }

//        public string[] GetLabel() {
//            var idea = IpIdea.value.ToString();
//            var tag = IpTag.value.ToString();
//            var strings = new string[2] { idea, tag };
//            return strings;
//        }

        public void AddPrefabs(string name, string code) {
            GameObject obj = NGUITools.AddChild(PrefabsGrid.gameObject, SaveStylePrefabs);

            SaveStylePrefabs ssp = obj.GetComponent<SaveStylePrefabs>();
            ssp.LblText.text = ssp.codeName = name;
            ssp.codeValue = code;

            UIEventListener.Get(obj).onClick += OnSaveStylePrefabsClick;

            PrefabsGrid.Reposition();
            PrefabsGrid.repositionNow = true;
            NGUITools.SetDirty(PrefabsGrid);
        }

        private void OnSaveStylePrefabsClick(GameObject go) {
            var ssp = go.GetComponent<SaveStylePrefabs>();
            if (null != ssp && null != ClickStyleBtn) {
                ClickStyleBtn(ssp.codeName);
            }
        }

        public void DeleteAll() {
            PrefabsGrid.transform.DestroyChildren();
        }

//        public string GetSeason() {
//            return LblSeasons.text;
//        }
//
//        public string GetStyle() {
//            return LblStyle.text;
//        }

        public string GetTags() {
            return IpTag.value.Trim();
        }

        public string GetName() {
            return LbShirtName.text;
        }

        public string GetMemo() {
            return LblDesignIdea.text;
        }

        public void ClearData() {
            LblStyle.text = null;
            LblSeasons.text = null;
            IpIdea.value = "不限";
            IpTag.value = "不限";
        }
    }
}