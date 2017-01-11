using System;
using System.Collections.Generic;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class UserInfoViewPresenter : ViewPresenter {
        public GameObject AddressButtonPrefabs;
        public GameObject AddressChosePanel;
        public GameObject BirthdayChosePanel;

        public UILabel LblTitle;
        public UILabel LblSave;
        public UILabel LblName;
        public UILabel LblNickName;
        public UILabel LblPhone;
        public UILabel LblCellPhoneLabel;
        public UILabel LblHead;

        public UIScrollView ScYears;
        public UIScrollView ScMonth;
        public UIScrollView ScDay;
        public UIButton BtnAddress;

        public UIButton BtnBack;
        public UIButton BtnBirthday;
        public UIButton BtnSave;
        public UIButton BtnSex;
        public UIButton BtnSure;
        public UIButton BtnLogout;
        public UIGrid GridAddres;
        public UIGrid GridSex;

        public UIGrid YearsGrid;
        public UIGrid MonthsGrid;
        public UIGrid DaysGrid;

        public GameObject PointYear;
        public GameObject PointMonth;
        public GameObject PointDay;


        public UIInput IpNickName;
        public UIInput IpPhone;
        public GameObject SexButtonPrefabs;

        public GameObject SexChosePanel;

        public UITexture TexHead;
        public UIButton BtnHead;
        public GameObject DateNumber;

        public UILabel NickNameLabel;
        public UILabel CellPhoneLabel;


        private string _province;
        private string _provinceCode;
        private string _city;
        private string _cityCode;
        private string _area;
        private string _areaCode;
        public string SexCode;
        public string SexName;

        public event MyAction ClickBack;
        public event MyAction<string> ClickSave;
        public event MyAction ClickAddress;
        public event MyAction<string> ChooseProvince;
        public event MyAction<string> ChooseCity;
        public event MyAction ClickHead;
        public event MyAction ClickLogout;

        private bool _initDate = false;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("ui_title");
            LblSave.text = StrConfigProvider.Instance.GetStr("ui_save");
            LblName.text = StrConfigProvider.Instance.GetStr("ui_name");
            LblNickName.text = StrConfigProvider.Instance.GetStr("ui_nickname");
            LblPhone.text = StrConfigProvider.Instance.GetStr("ui_phone");
            LblCellPhoneLabel.text = StrConfigProvider.Instance.GetStr("ui_cellphone");
            LblHead.text = StrConfigProvider.Instance.GetStr("ui_head");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSave.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblNickName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCellPhoneLabel.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblHead.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnSave.onClick.Add(new EventDelegate(OnBtnSaveClick));
            BtnBirthday.onClick.Add(new EventDelegate(OnBtnBirthdayClick));
            BtnAddress.onClick.Add(new EventDelegate(OnBtnAddressClick));
            BtnSex.onClick.Add(new EventDelegate(OnBtnSexClick));
            BtnSure.onClick.Add(new EventDelegate(OnBtnSureClick));
            BtnHead.onClick.Add(new EventDelegate(OnBtnHeadClick));
            BtnLogout.onClick.Add(new EventDelegate(OnBtnLogoutClick));
        }

        private void OnBtnSureClick() {
            int year = Convert.ToInt32(RayControler(PointYear));
            int month = Convert.ToInt32(RayControler(PointMonth));
            int day = Convert.ToInt32(RayControler(PointDay));
            var isTrue = CheckBirthdayDate(year, month, day);
            if (isTrue) {
                BtnBirthday.GetComponent<UILabel>().text = year + "-" + month + "-" + day;
                BirthdayChosePanel.SetActive(false);
            } else {
                LogSystem.Warn("The date is false!");
            }
        }

        private void OnBtnSexClick() {
            if (SexChosePanel.gameObject.activeSelf) {
                SexChosePanel.SetActive(false);
            } else {
                SexChosePanel.SetActive(true);
            }

        }

        private void OnBtnAddressClick() {
            if (AddressChosePanel.gameObject.activeSelf) {
                AddressChosePanel.SetActive(false);
            } else {
                AddressChosePanel.SetActive(true);
                if (null != ClickAddress) {
                    ClickAddress();
                }
            }
        }

        private void OnBtnBirthdayClick() {
            InitDateItems();
            if (BirthdayChosePanel.gameObject.activeSelf) {
                BirthdayChosePanel.SetActive(false);
            } else {
                BirthdayChosePanel.SetActive(true);
            }
        }

        private void OnBtnSaveClick() {
            if (null != ClickSave) {
                ClickSave(NickNameLabel.text);
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void OnBtnHeadClick() {
            if (null != ClickHead)
                ClickHead();
        }
        private void OnBtnLogoutClick() {
            if (null != ClickLogout) {
                ClickLogout();
            }
        }

        private void InitDateItems() {
            if (_initDate) return;
            //year
            int year = System.DateTime.Now.Year;
            for (int i = 0; i < 100; i++) {
                AddDateNumber(ScYears, YearsGrid, DateNumber, year.ToString());
                year--;
            }
            //month
            for (int i = 12; i > 0; i--) {
                AddDateNumber(ScMonth, MonthsGrid, DateNumber, i.ToString());
            }
            //day
            for (int i = 31; i > 0; i--) {
                AddDateNumber(ScDay, DaysGrid, DateNumber, i.ToString());
            }
            _initDate = true;
        }

        private void AddDateNumber(UIScrollView scrollView, UIGrid grid, GameObject prefab, string number) {
            var obj = NGUITools.AddChild(grid.gameObject, prefab);
            var numberLabel = obj.GetComponent<UILabel>();
            numberLabel.text = number;
            //numberLabel.GetComponent<UIDragScrollView>().scrollView = scrollView;
            grid.Reposition();
            grid.repositionNow = true;
            NGUITools.SetDirty(grid);
        }

        //射线检测日期
        private string RayControler(GameObject point) {
            var pt = UICamera.mainCamera.WorldToScreenPoint(point.transform.TransformPoint(Vector3.zero));
            var ray = UICamera.mainCamera.ScreenPointToRay(pt);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {

            }
            return hit.collider.GetComponent<UILabel>().text;
        }

        private bool CheckBirthdayDate(int year, int month, int day) {
            int iday = System.DateTime.DaysInMonth(year, month);
            if (day > iday)
                return false;
            return true;
        }

        public void AddSexPrefas(string codeValue, string codeName) {
            var obj = NGUITools.AddChild(GridSex.gameObject, SexButtonPrefabs);
            var sexButtonPrefab = obj.GetComponent<SexButtonPrefabs>();
            sexButtonPrefab.ChooseClick += SetSexChoose;
            sexButtonPrefab.codeValue = codeValue;
            sexButtonPrefab.codeName = codeName;
            sexButtonPrefab.NameLabel.text = codeName;
            GridSex.Reposition();
            GridSex.repositionNow = true;
            NGUITools.SetDirty(GridSex);
        }

        private void SetSexChoose(string sexName,string sexValue) {
            SexChosePanel.SetActive(false);
            BtnSex.GetComponent<UILabel>().text =sexName;
            SexCode = sexValue;
        }

        public void DeleteAllGridPrefabs(UIGrid grid) {
            grid.transform.DestroyChildren();
        }

        public void SetUpAddress(AddressLevel level, IDictionary<string, string> data) {
            DeleteAllGridPrefabs(GridAddres);
            MyAction<string, string> callbacks = null;
            switch (level) {
                case AddressLevel.Province:
                    callbacks = OnChooseProvince;
                    break;
                case AddressLevel.City:
                    callbacks = OnChooseCity;
                    break;
                case AddressLevel.Area:
                    callbacks = OnChooseArea;
                    break;
            }
            foreach (var item in data) {
                var obj = NGUITools.AddChild(GridAddres.gameObject, AddressButtonPrefabs);
                var abp = obj.GetComponent<AddressButtonPrefabs>();
                abp.CodeName = item.Value;
                abp.CodeValue = item.Key;
                abp.ClickChoose += callbacks;
                GridAddres.Reposition();
                GridAddres.repositionNow = true;
            }
        }

        private void OnChooseProvince(string provinceCode, string provinceName) {
            _city = null;
            _area = null;
            _province = provinceName;
            _provinceCode = provinceCode;
            BtnAddress.GetComponent<UILabel>().text = _province + _city + _area;
            DeleteAllGridPrefabs(GridAddres);
            if (null != ChooseProvince) {
                ChooseProvince(provinceCode);
            }
        }

        private void OnChooseCity(string cityCode, string cityName) {
            _city = cityName;
            _cityCode = cityCode;
            BtnAddress.GetComponent<UILabel>().text = _province + _city + _area;
            if (null != ChooseCity) {
                ChooseCity(cityCode);
            }
        }

        private void OnChooseArea(string areaCode, string areaName) {
            _area = areaName;
            _areaCode = areaCode;
            BtnAddress.GetComponent<UILabel>().text = _province + _city + _area;
            AddressChosePanel.SetActive(false);
        }

        public void SetUserInfos(string name, string tel, Texture2D head) {
            NickNameLabel.text = name;
            CellPhoneLabel.text = tel;
            TexHead.mainTexture = head;
            //            CoroutineMgr.Instance.StartCoroutine(GetPhone(userInfo.Photo));
        }
//        private IEnumerator GetPhone(string path) {
//            var www = new WWW(path);
//            yield return www;
//            if (www.isDone) {
//                var texture = www.texture;
//                TexHead.mainTexture = texture;
//                www.Dispose();
//            }
//        }

        public void SetUserSex() {
            BtnSex.GetComponent<UILabel>().text = SexName;
        }

        public void SetHead(Texture2D texture2D) {
            TexHead.mainTexture = texture2D;
        }

//        private UserInfo GetUserInfo() {
//            UserInfo userInfo=new UserInfo();
//            //TODO 微商城修改用户信息
//            userInfo.UserName = NickNameLabel.text;
//            userInfo.Tel = CellPhoneLabel.text;
////            userInfo.Brithday = BtnBirthday.GetComponent<UILabel>().text;
////            userInfo.Addr = BtnAddress.GetComponent<UILabel>().text;
////            userInfo.SexCode = SexCode;
////            userInfo.ProviceCode = _provinceCode;
////            userInfo.Province = _province;
////            userInfo.CityCode = _cityCode;
////            userInfo.City = _city;
////            userInfo.AreaCode = _areaCode;
////            userInfo.Area = _area;
//            return userInfo;
//        }
    }
}