using UnityEngine;
using System.Collections.Generic;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class EditeAddressViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UIButton BtnBack;
        public UIButton BtnSave;

        public UIInput IpName;
        public UIInput IpPhone;
        public UIButton BtnProvince;
        public UIButton BtnCity;
        public UIButton BtnDistrict;
        public UIInput IpAddress;
        public UIInput IpPostalCode;

        public UILabel LblName;
        public UILabel LblPhone;
        public UILabel LblProvince;
        public UILabel LblCity;
        public UILabel LblDistrict;
        public UILabel LblAddress;
        public UILabel LblPostalcode;
        public UILabel LblSave;
        public UILabel LblIpName;
        public UILabel LblIpPhone;
        public UILabel LblIpAddress;
        public UILabel LblIpPostalCode;

        public GameObject ChosePanel;
        public UIScrollView AddresView;
        public UIGrid GridAddres;
        public GameObject AddressButtonPrefabs;

        public event MyAction ClickBack;
        public event MyAction<Address> ClickSave;
        public event MyAction ClickProvince;
        public event MyAction<string> ClickCity;
        public event MyAction<string> ClickArea;
        public Address Address { get; set; }

        protected override void AwakeUnityMsg() {
            Address = new Address();
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnSave.onClick.Add(new EventDelegate(OnBtnSaveClick));
            BtnProvince.onClick.Add(new EventDelegate(OnProvinceClick));
            BtnCity.onClick.Add(new EventDelegate(OnCityClick));
            BtnDistrict.onClick.Add(new EventDelegate(OnAreaClick));
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblName.text = StrConfigProvider.Instance.GetStr("addr_edit_name");
            LblPhone.text = StrConfigProvider.Instance.GetStr("addr_edit_phone");
            LblProvince.text = StrConfigProvider.Instance.GetStr("addr_edit_province");
            LblCity.text = StrConfigProvider.Instance.GetStr("addr_edit_city");
            LblDistrict.text = StrConfigProvider.Instance.GetStr("addr_edit_district");
            LblAddress.text = StrConfigProvider.Instance.GetStr("addr_edit_address");
            LblPostalcode.text = StrConfigProvider.Instance.GetStr("addr_edit_postalcode");
            LblSave.text = StrConfigProvider.Instance.GetStr("addr_edit_save");
            LblIpName.text = StrConfigProvider.Instance.GetStr("addr_edit_inputname");
            LblIpPhone.text = StrConfigProvider.Instance.GetStr("addr_edit_inputphone");
            BtnProvince.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_chooseprovince");
            BtnCity.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_choosecity");
            BtnDistrict.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_choosearea");
            LblIpAddress.text = StrConfigProvider.Instance.GetStr("addr_edit_inputaddress");
            LblIpPostalCode.text = StrConfigProvider.Instance.GetStr("addr_edit_inputpostalcode");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblProvince.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblDistrict.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblAddress.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPostalcode.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSave.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblIpName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblIpPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnProvince.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnCity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnDistrict.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblIpAddress.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblIpPostalCode.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }
        private void OnBtnSaveClick() {
                if (null != ClickSave) {
                    ClickSave(GetViewLabel());
                }         
        }
        private void OnProvinceClick() {
            GridAddres.transform.DestroyChildren();
           // ChosePanel.SetActive(true);
            if (null != ClickProvince) {
                ClickProvince ();
            }
        }

        private void OnCityClick() {
            GridAddres.transform.DestroyChildren();
            if (null != ClickCity)
                ClickCity(Address.ProvinceCode);
        }

        public void SetTitle(string str) {
            LblTitle.text = str;
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnAreaClick() {
            GridAddres.transform.DestroyChildren();
            if (null != ClickArea)
                ClickArea(Address.CityCode);
        }
       
        public void SetupAddressInfo(AddressLevel level, IDictionary<string,string> data) {
            ChosePanel.SetActive(true);
            MyAction<string, string> callBack = null;
            switch (level) {
                case AddressLevel.Province:
                    callBack = OnChoosProvinceClick;
                    break;
                case AddressLevel.City:
                    callBack = OnChooseCityClick;
                    break;
                case AddressLevel.Area:
                    callBack = OnChooseAreaClick;
                    break;
            }
            if (data.Count != 0) {
                foreach (var item in data) {
                    GameObject obj = NGUITools.AddChild(GridAddres.gameObject, AddressButtonPrefabs);
                    AddressButtonPrefabs abp = obj.GetComponent<AddressButtonPrefabs>();
                    abp.CodeName = item.Value;
                    abp.CodeValue = item.Key;
                    abp.ClickChoose += callBack;
                    GridAddres.Reposition();
                    GridAddres.repositionNow = true;
                }
                AddresView.ResetPosition();
            }
        }


        private void OnChoosProvinceClick(string provinceCode, string provinceName) {
            Address.ProvinceCode = provinceCode;
            Address.Province = provinceName;
            BtnProvince.GetComponent<UILabel>().text = Address.Province;
            BtnCity.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_city");
            BtnDistrict.GetComponent<UILabel>().text = LblDistrict.text = StrConfigProvider.Instance.GetStr("addr_edit_district");
            ChosePanel.SetActive(false);
        }

        private void OnChooseCityClick(string cityCode, string cityName) {
            Address.CityCode = cityCode;
            Address.City = cityName;
            BtnCity.GetComponent<UILabel>().text = Address.City;
            BtnDistrict.GetComponent<UILabel>().text = LblDistrict.text = StrConfigProvider.Instance.GetStr("addr_edit_district");
            ChosePanel.SetActive(false);
        }

        private void OnChooseAreaClick(string areaCode, string areaName ) {
            Address.AreaCode = areaCode;
            Address.Area = areaName;
            BtnDistrict.GetComponent<UILabel>().text = Address.Area;
            ChosePanel.SetActive(false);
        }

        private Address GetViewLabel() {
            Address address=new Address();
            address.Id = Address.Id;
            address.Contacts = IpName.value;
            address.Tel = IpPhone.value;
            address.Province = BtnProvince.GetComponent<UILabel>().text;
            address.ProvinceCode = Address.ProvinceCode;
            address.City = BtnCity.GetComponent<UILabel>().text;
            address.CityCode = Address.CityCode;
            address.Area = BtnDistrict.GetComponent<UILabel>().text;
            address.AreaCode = Address.AreaCode;
            address.Addr = IpAddress.value;
            address.PostCode = IpPostalCode.value;
            return address;
        }

        public void ShowEditeAddress(Address address,string addressId) {
            Address.Id = addressId;
            IpName.value = address.Contacts;
            IpPhone.value = address.Tel;
            BtnProvince.GetComponent<UILabel>().text = address.Province;
            Address.ProvinceCode = address.ProvinceCode;
            BtnCity.GetComponent<UILabel>().text = address.City;
            Address.CityCode = address.CityCode;
            BtnDistrict.GetComponent<UILabel>().text = address.Area;
            Address.AreaCode = address.AreaCode;
            IpAddress.value = address.Addr;
            IpPostalCode.value = address.PostCode;
        }

        public void ClearAddress() {
            Address.Id = string.Empty;
            Address.CityCode = string.Empty;
            Address.ProvinceCode = string.Empty;
        }

        public void WipeViewLabel() {
            IpName.value = string.Empty;
            IpPhone.value = string.Empty;
            BtnProvince.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_chooseprovince");
            BtnCity.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_choosecity");
            BtnDistrict.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("addr_edit_choosearea");
            IpAddress.value = string.Empty;
            IpPostalCode.value = string.Empty;
            LblIpName.text = StrConfigProvider.Instance.GetStr("addr_edit_inputname");
            LblIpPhone.text = StrConfigProvider.Instance.GetStr("addr_edit_inputphone");
            LblIpAddress.text = StrConfigProvider.Instance.GetStr("addr_edit_inputaddress");
            LblIpPostalCode.text = StrConfigProvider.Instance.GetStr("addr_edit_inputpostalcode");
        }

    }
}