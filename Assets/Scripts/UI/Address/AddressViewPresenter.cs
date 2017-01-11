using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class AddressViewPresenter : ViewPresenter {
        public UIGrid AddressGrid;
        public GameObject AddressParticularsPrefabs;
        public UIButton BtnAdd;
        public UIButton BtnBack;
        public UIScrollView AddressScrollView;

        public UILabel LblTitle;
        public UILabel LblNewAddress;

        public event MyAction ClickBack;
        public event MyAction ClickNew;
        public event MyAction<bool,string> DeleteAddress;
        public event MyAction<string> EditAddress;
        public event MyAction<string> SetDefaultAddress;
        public event MyAction<string> ChooseAddress;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnAdd.onClick.Add(new EventDelegate(OnBtnAddClick));
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("addr_title");
            LblNewAddress.text = StrConfigProvider.Instance.GetStr("addr_newaddress");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblNewAddress.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void OnBtnAddClick() {
            if (null != ClickNew) {
                ClickNew();
            }
        }

        protected override void OnDestroyUnityMsg() {
            //Hide();
        }

        public void RemoveAllAddressItem() {
            AddressGrid.transform.DestroyChildren();
        }
        public void AddAddressPrefabs(Address addr) {
            var obj = NGUITools.AddChild(AddressGrid.gameObject, AddressParticularsPrefabs);
            var app = obj.GetComponent<AddressParticularsPrefabs>();         
            app.Id = addr.Id;
            app.NewAddress = "[382C2CFF]" + addr.Province + addr.City + addr.Area + addr.Addr + "[-]";
            app.LblAddress.text = "[382C2CFF]" + addr.Province + addr.City + addr.Area + addr.Addr + "[-]";            
            app.LblName.text = addr.Contacts;
            app.LblPhone.text = addr.Tel;
            app.isDefault = addr.IsDefault;
            app.LblAddress.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            app.LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            app.LblPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            app.SetDefaultState(addr.IsDefault);
            app.ClickEdit  += OnEditClick;
            app.ClickDelete += OnDeleteClick;
            app.ClickChoose += OnChooseClick;
            app.ClickDefault += OnSetDefaultClick;
            Reposition();
        }

        private void OnEditClick(string addrId) {
            if (null != EditAddress) {
                EditAddress(addrId);
            }
        }

        private void OnDeleteClick(bool isDefault,string addrId) {
            if (null != DeleteAddress) {
                DeleteAddress(isDefault, addrId);
            }
        }

        private void OnChooseClick(string addrId) {
            if (null != ChooseAddress) {
                ChooseAddress(addrId);
            }
        }

        private void OnSetDefaultClick(string addrId, bool isDefault) {
            if (null != SetDefaultAddress) {
                if(!isDefault)
                    SetDefaultAddress(addrId);
            }
        }

        public string GetAddressId() {
            return AddressGrid.GetChild(0).gameObject.GetComponent<AddressParticularsPrefabs>().Id;
        }

        public void AdjustAddressItem() {
            var depth = AddressScrollView.GetComponent<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(AddressGrid.gameObject, depth+1);
            GetSortedPanels(true);
        }

        public void DeleteItem(string addrId) {
            UiControllerMgr.Instance.ShowLodingTips(true);
            var children = AddressGrid.transform.GetComponentsInChildren<AddressParticularsPrefabs>(true);
            for (int i = 0; i < children.Length; i++) {
                if (children[i].Id==addrId) {
                    GameObject.Destroy(children[i].gameObject);                  
                    break;
                }
            }
            Reposition();
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        public void DeleteAll() {
            AddressGrid.transform.DestroyChildren();
        }

        public void SetDefaultItem(string addrId) {
            var children = AddressGrid.transform.GetComponentsInChildren<AddressParticularsPrefabs>(true);
            for (int i = 0; i < children.Length; i++) {
                if (children[i].Id != addrId) {                   
                    children[i].SetDefaultState(false);
                } else {
                    children[i].SetDefaultState(true);
                }
            }
        }

        public override void InitWindowData() {
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public void Reposition() {
            AddressGrid.Reposition();
            AddressGrid.repositionNow = true;
            NGUITools.SetDirty(AddressGrid);
            var springPanel = AddressScrollView.gameObject.GetComponent<SpringPanel>();
            springPanel.target = Vector3.zero;
            springPanel.enabled = true;
            AddressScrollView.MoveRelative(new Vector3(0, 1, 0));
        }

        public int GetCount() {
            return AddressGrid.GetChildList().Count;
        }

    }
}