using System;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public enum AddressLevel {
        Province,
        City,
        Area
    }
    public class EditeAddressCtr : IController {
        private EditeAddressViewPresenter _view;
        private INetworkMgr _networkMgr;
        private IUserCenter _userCenter;
        private string _type;

        public bool Init() {
            EventCenter.Instance.AddListener<string, Address, string>(EventId.EditeAddr, ShowEditeAddress);
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Warn("network mgr is null.");
                return false;
            }
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Warn("user center is null.");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<EditeAddressViewPresenter>(WindowId.EditeAddress);
                _view.ClickBack += OnBackClick;
                _view.ClickSave += OnSaveClick;
                _view.ClickProvince += OnProvinceClick;
                _view.ClickCity += OnCityClick;
                _view.ClickArea += OnAreaClick;
            }
            UiManager.Instance.ShowWindow(WindowId.EditeAddress);
            _view.ClearAddress();
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.EditeAddress);
            UiControllerMgr.Instance.GetController(WindowId.Address).Show();
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.EditeAddress, "ui/EditeAddress"),
            };
        }
        
        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnSaveClick(Address addr) {
            if (String.IsNullOrEmpty(addr.Contacts)) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_consignee"));
            } else if (String.IsNullOrEmpty(addr.Tel)) {                                            
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_phone"));
            } else if (addr.Tel.Length < 11) {
                UiControllerMgr.Instance.ShowTips("请填写正确的手机号");     
            } else if (string.IsNullOrEmpty(addr.ProvinceCode)) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_province"));
            } else if (string.IsNullOrEmpty(addr.CityCode)) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_city"));
            } else if (string.IsNullOrEmpty(addr.AreaCode)) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_area"));
            } else if (String.IsNullOrEmpty(addr.Addr)) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_address"));
            } else if (String.IsNullOrEmpty(addr.PostCode)) {                   
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_postalcode"));
            }else if (addr.PostCode.Length < 6) {
                UiControllerMgr.Instance.ShowTips("请填写正确的邮政编码");
            }else {
                JsonData data = new JsonData();
                data["contacts"] = addr.Contacts;
                data["telphone"] = addr.Tel;
                data["provinceCode"] = addr.ProvinceCode;
                data["province"] = addr.Province;
                data["cityCode"] = addr.CityCode;
                data["city"] = addr.City;
                data["areaCode"] = addr.AreaCode;
                data["area"] = addr.Area;
                data["detailedAddress"] = addr.Addr;
                data["postCode"] = addr.PostCode;
                data["atid"] = addr.Id;
                _userCenter.NewAddress(data.ToJson(), NewAddressCallBack);
            }
        }

        private void NewAddressCallBack(bool result,JsonData msg) {
            if (result) {
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.Address).Show();
            }
        }

        private void OnProvinceClick() {
            CodeSetProvider.Instance.QueryCodeset("PROVINCE", OnCodeSet);
        }
        private void OnCodeSet(CodeSet codeset) {
            _view.SetupAddressInfo(AddressLevel.Province, codeset.GetData());
        }

        private void OnCityClick(string provinceCode) {
            if (provinceCode != string.Empty) {
                if (_userCenter.DicCityList.ContainsKey(provinceCode)) {
                    _view.SetupAddressInfo(AddressLevel.City, _userCenter.DicCityList[provinceCode]);
                } else {
                    _userCenter.GetCityList(provinceCode, OnGetCityListRsp);
                }
            }
        }

        private void OnAreaClick(string cityCode) {
            if (cityCode != string.Empty) {
                if (_userCenter.DicAreaList.ContainsKey(cityCode)) {
                    _view.SetupAddressInfo(AddressLevel.Area, _userCenter.DicAreaList[cityCode]);
                } else {
                    _userCenter.GetAreaList(cityCode, OnGetAreaRsp);
                }
            }
        }

        private void ShowEditeAddress(string type, Address address, string id) {
            Show();
            _type = type;
            if (type == "Add") {
                _view.SetTitle(StrConfigProvider.Instance.GetStr("addr_edit_title_newaddress"));
                _view.WipeViewLabel();
            } else if (type == "Edit") {
                _view.SetTitle(StrConfigProvider.Instance.GetStr("addr_edit_title_editaddress"));
                _view.ShowEditeAddress(address, id);
            }
        }

        private void OnGetCityListRsp(bool result,IDictionary<string,string> lstCity ) {
            if (result) {
                _view.SetupAddressInfo(AddressLevel.City, lstCity);
            }
        }

        private void OnGetAreaRsp(bool result, IDictionary<string, string> lstArea) {
            if (result) {
                _view.SetupAddressInfo(AddressLevel.Area, lstArea);
            }
        }
    }
}
