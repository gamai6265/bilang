using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class ReferenceSizeCtr : IController{
        private ReferenceSizeViewPresenter _view;
        private INetworkMgr _networkMgr;
        private IUserCenter _userCenter;
        private readonly List<ClothSize> _lstClothSize=new List<ClothSize>();
        public bool Init() {
            _networkMgr=ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("network is null");
                return false;
            }

            _userCenter=ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter)as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("_userCenter is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<ReferenceSizeViewPresenter>(WindowId.ReferenceSize);
                _view.ClickBack += OnBackClick;
                _view.ClickSize += OnSizeClick;
                _view.ClickChoose += OnChooseClick;
            }
            UiManager.Instance.ShowWindow(WindowId.ReferenceSize);
            _networkMgr.QuerySender<IOrderSender>().SendGetReferenceSizeReq();
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
            UiManager.Instance.HideWindow(WindowId.ReferenceSize);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.ReferenceSize, "ui/ReferenceSize"),
            };
        }
        void OnBackClick() {
            OnBack(-1);
        }

        void OnSizeClick(string sizeName) {
            for (int i = 0; i < _lstClothSize.Count; i++) {
                if (_lstClothSize[i].SizeName == sizeName) {
                    ClothSize clothSize = _lstClothSize[i];
                    _view.SelectedItem(clothSize);
                    ChangeClothSize(clothSize);
                }
            }
        }

        private void ChangeClothSize(ClothSize clothSize) {
            _userCenter.ClothSize.SizeName = clothSize.SizeName;
            _userCenter.ClothSize.ClothName = clothSize.ClothName;
            _userCenter.ClothSize.Notation = clothSize.Notation;
            _userCenter.ClothSize.Postil = clothSize.Postil;
            _userCenter.ClothSize.SizeJianK = clothSize.SizeJianK;
            _userCenter.ClothSize.SizeJiaoW = clothSize.SizeJiaoW;
            _userCenter.ClothSize.SizeJiaQ = clothSize.SizeJiaQ;
            _userCenter.ClothSize.SizeLingW = clothSize.SizeLingW;
            _userCenter.ClothSize.SizeXiongW = clothSize.SizeXiongW;
            _userCenter.ClothSize.SizeXiuCLong = clothSize.SizeXiuCLong;
            _userCenter.ClothSize.SizeXiuCShort = clothSize.SizeXiuCShort;
            _userCenter.ClothSize.SizeXiuF = clothSize.SizeXiuF;
            _userCenter.ClothSize.SizeXiuKLong = clothSize.SizeXiuKLong;
            _userCenter.ClothSize.SizeXiuKShort = clothSize.SizeXiuKShort;
            _userCenter.ClothSize.SizeYaoW = clothSize.SizeYaoW;
            _userCenter.ClothSize.SizeYiC = clothSize.SizeYiC;
            _userCenter.ClothSize.UpdateTime = clothSize.UpdateTime;
        }

        void OnChooseClick(ClothSize clothSize) {
            EventCenter.Instance.Broadcast(EventId.ChooseSize);
            Hide();
            UiControllerMgr.Instance.GetController(WindowId.OrderConfirm).Show();
        }


        private void SetupReferenceSize(JsonData data) {
            _view.DeleteAllPrefab();
            for (int i = 0; i < data.Count; i++) {
                ClothSize size = new ClothSize();
                size.SizeName = Utils.GetJsonStr(data[i]["size_name"]);
                size.SizeJianK = Utils.GetJsonStr(data[i]["size_jian_k"]);
                size.SizeXiongW = Utils.GetJsonStr(data[i]["size_xiong_w"]);
                size.SizeYaoW = Utils.GetJsonStr(data[i]["size_yao_w"]);
                size.SizeYiC = Utils.GetJsonStr(data[i]["size_yi_c"]);
                size.SizeXiuCLong = Utils.GetJsonStr(data[i]["size_xiu_c_long"]);
                size.SizeLingW = Utils.GetJsonStr(data[i]["size_ling_w"]);
                size.SizeXiuKLong = Utils.GetJsonStr(data[i]["size_xiu_k_long"]);
                size.SizeXiuF = Utils.GetJsonStr(data[i]["size_xiu_f"]);
                _lstClothSize.Add(size);
                _view.AddSizeItem(size.SizeName);
            }
            _view.SelectedItem(_lstClothSize[0]);
        }


        /*void IOrderListener.OnGetReferenceSizeRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (msg["exit"].ToString() == "0") {
                    SetupReferenceSize(msg["data"]);
                } else {
                    LogSystem.Error("req return failed");
                }
            } else {
                   LogSystem.Error("network require failed");
            }
        }*/ //TODO 参考尺寸。
    }
}
