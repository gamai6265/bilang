using System.Collections.Generic;
using System.Text;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class AddressCtr : IController{
        private AddressViewPresenter _view;
        private IUserCenter _userCenter;
        private List<Address> _addressesList;
        public bool Init() {
          //  EventCenter.Instance.AddListener(EventId.ShowAddress,ShowAddress);
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<AddressViewPresenter>(WindowId.Address);
                _view.ClickBack += OnBackClick;
                _view.ClickNew += OnNewAdressClick;
                _view.DeleteAddress += OnDeleteAddressClick;
                _view.EditAddress += OnEditAddressClick;
                _view.SetDefaultAddress += OnSetDefaultAddressClick;
                _view.ChooseAddress += OnChooseAddressClick;
            }
            _view.RemoveAllAddressItem();           
            UiManager.Instance.ShowWindow(WindowId.Address);
            _userCenter.QueryAddress(OnQueryAddress);
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
            UiManager.Instance.HideWindow(WindowId.Address);
            EventCenter.Instance.Broadcast(EventId.ShowAddress);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Address, "ui/Address"),
            };
        }

        private void OnQueryAddress(bool result, List<Address> lstAddress) {
            if (result) {
                _view.DeleteAll();
                _addressesList = lstAddress;
                for (int i = 0; i < lstAddress.Count; i++) {
                    _view.AddAddressPrefabs(lstAddress[i]);
                }
                _view.AdjustAddressItem();
                if (lstAddress.Count == 1 && lstAddress[0].IsDefault==false) {
                    _userCenter.SetDefaultAddress(lstAddress[0].Id, SetDefaultAddressCallBack);
                }
                
            }
        }

        private void SetDefaultAddressCallBack(bool result,string defaultId) {
            if (result) {
                _view.SetDefaultItem(defaultId);
            }
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnNewAdressClick() {
            Address add = new Address();
            Hide();
            EventCenter.Instance.Broadcast(EventId.EditeAddr, "Add", add, string.Empty);
        }

        private void OnDeleteAddressClick(bool isDefault,string addrId) {
            UiControllerMgr.Instance.ShowMessageBox(
               "", 
               StrConfigProvider.Instance.GetStr("mb_deleteaddress"), 
               StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    _OnDeleteAddressClick(isDefault, addrId);
                }
            );
        }

        private void _OnDeleteAddressClick(bool isDefault, string addrId) {
            MyAction<bool, string> deleteAction = null;
            deleteAction = (result, id) => {
                if (result) {
                    _view.DeleteItem(id);
                    if (isDefault) {
                            GetAddress();                                               
                    }
                                                      
                } else {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_deleteaddress"));
                }
            };
            _userCenter.DeleteAddress(addrId, deleteAction);
        }

        private void GetAddress() {
            MyAction<bool, List<Address>> defaultAction = null;
            defaultAction = (result, addressList) => {
                if (result) {
                    if (addressList.Count >= 1) {
                        SetDefaultAddress(addressList[0].Id);
                    } 
                } 
            };
            _userCenter.QueryAddress(defaultAction);
        }

        private void SetDefaultAddress(string defaultId) {
            MyAction<bool, string> defaultAction = null;
            defaultAction = (result, id) => {
                if (result) {
                    _view.SetDefaultItem(id);
                    foreach (var addressItem in _addressesList) {
                        if (addressItem.Id == id) {
                            EventCenter.Instance.Broadcast(EventId.ChangeOrderConfirmAddress, addressItem);
                            break;
                        }
                    }
                } else {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_deleteaddress"));
                }
            };
            _userCenter.SetDefaultAddress(defaultId,defaultAction);
        }


        void OnEditAddressClick(string addId) {
            for (int i = 0; i < _addressesList.Count; i++) {
                if (_addressesList[i].Id == addId) {
                    Hide();
                    EventCenter.Instance.Broadcast(EventId.EditeAddr, "Edit", _addressesList[i], addId);
                }
            }         
        }

        void OnSetDefaultAddressClick(string addrId) {
            MyAction<bool, string> setDefaultAction = null;
            setDefaultAction = (result, id) => {
                if (result) {
                    for (int i = 0; i < _addressesList.Count; i++) {
                        if (_addressesList[i].Id == id) {
                            _view.SetDefaultItem(addrId);
                        }
                    }                 
                } else {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_defaultaddress"));
                }
            };
            _userCenter.SetDefaultAddress(addrId, setDefaultAction);
        }

        void OnChooseAddressClick(string addrId) {
            Hide();
            EventCenter.Instance.Broadcast(EventId.ChooseAddress, addrId);
        }
    }
}