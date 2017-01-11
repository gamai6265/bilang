using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class UserInfoCtr : IController{
        private UserInfoViewPresenter _view;
        private INetworkMgr _networkMgr;
        private ILoginMgr _loginMgr;
        private IUserCenter _userCenter;
        private List<string> _sexMessageValueList=new List<string>();
        private List<string> _sexMessageNameList = new List<string>();
        private Texture2D _head = null;
        private string _userName = string.Empty;

        public bool Init() {    
            _userCenter=ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("network is null");
                return false;
            }
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (null == _loginMgr) {
                LogSystem.Error("login mgr is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<UserInfoViewPresenter>(WindowId.UserInfo);
                _view.ClickBack += OnBackClick;
                _view.ClickSave += OnSaveClick;
                _view.ClickAddress += OnClickAddress;
                _view.ChooseProvince += OnClickProvince;
                _view.ChooseCity += OnClickCity;
                _view.ClickHead += OnClickHead;
                _view.ClickLogout += OnLogoutClick;
            }
            UiManager.Instance.ShowWindow(WindowId.UserInfo);
            GetSexCodeSet();
            _view.SetUserInfos(_userCenter.UserInfo.UserName,_userCenter.UserInfo.Tel,_userCenter.UserInfo.Head);
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
            _head = null;
            _userName = string.Empty;
            UiManager.Instance.HideWindow(WindowId.UserInfo);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>> {
                new KeyValuePair<WindowId, string>(WindowId.UserInfo, "ui/UserInfo")
            };
        }

        private void OnSaveClick(string userName) {
            _userName = userName;
            IList<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            if (_head != null) {
                byte[] bytes = _head.EncodeToJPG();
                files.Add(new KeyValuePair<string, byte[]>("image", bytes));
            }
            else {
                _head = _userCenter.UserInfo.Head;
                byte[] bytes = _userCenter.UserInfo.Head.EncodeToJPG();
                files.Add(new KeyValuePair<string, byte[]>("image", bytes));
            }
            _userCenter.ModifyUserInfo(_userName, files, ModifyUserInfo);
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnGetCodesets(CodeSet codeset) {
            var dic = codeset.GetData();
            foreach (var s in dic) {
                _sexMessageValueList.Add(s.Key);
                _sexMessageNameList.Add(s.Value);
            }
            for (int i = 0; i < dic.Count; i++) {
                _view.AddSexPrefas(_sexMessageValueList[i],_sexMessageNameList[i]);
                //TODO 微商城修改用户信息
//                if (_sexMessageValueList[i] == _userCenter.UserInfo.SexCode) {
//                    _view.SexCode = _sexMessageValueList[i];
//                    _view.SexName = _sexMessageNameList[i];
//                    _view.SetUserSex();
//                }
            }
        }

        private void GetSexCodeSet() {
            _view.DeleteAllGridPrefabs(_view.GridSex);
            CodeSetProvider.Instance.QueryCodeset("GENDER", OnGetCodesets);      
        }

        private void OnClickAddress() {
            CodeSetProvider.Instance.QueryCodeset("PROVINCE",OnCodeSet);
        }

        private void OnClickHead() {
            SDKContorl.Instance.OpenPhoto("LocalPhoto", PhotoComplete);
        }

        private void PhotoComplete(string base64) {
#if UNITY_ANDROID
            CoroutineMgr.Instance.StartCoroutine(LoadTexture(base64));
#endif

#if UNITY_IPHONE
            Texture2D tex = new Texture2D (4, 4, TextureFormat.ARGB32, false);
            byte[] bytes = System.Convert.FromBase64String(base64);
            tex.LoadImage(bytes);
            _view.SetHead(tex);
            _head = tex;
#endif
        }

        IEnumerator LoadTexture(string name) {
            string path = "file://" + "/mnt/sdcard/Android/data/com.bilang.tailor/image/" + name;
            WWW www = new WWW(path);
            yield return www;
            if (www.isDone) {
                Texture2D texture2D = www.texture;
                _view.SetHead(texture2D);
                _head = texture2D;
                www.Dispose();
            }
        }

        private void OnCodeSet(CodeSet codeSet) {
            _view.SetUpAddress(AddressLevel.Province,codeSet.GetData());
        }

        private void OnClickProvince(string provinceCode) {
            if (_userCenter.DicCityList.ContainsKey(provinceCode)) {
                _view.SetUpAddress(AddressLevel.City, _userCenter.DicCityList[provinceCode]);
            } else {
                _userCenter.GetCityList(provinceCode, OnGetCityListRsp);
            }
        }

        private void OnClickCity(string cityCode) {
            if (_userCenter.DicAreaList.ContainsKey(cityCode)) {
                _view.SetUpAddress(AddressLevel.Area, _userCenter.DicAreaList[cityCode]);
            } else {
                _userCenter.GetAreaList(cityCode, OnGetAreaListRsp);
            }
        }

        private void OnGetCityListRsp(bool result, Dictionary<string, string> lstCity) {
            if (result) {
                _view.SetUpAddress(AddressLevel.City, lstCity);
            }
        }

        private void OnGetAreaListRsp(bool result, Dictionary<string, string> lstArea) {
            if (result) {
                _view.SetUpAddress(AddressLevel.Area, lstArea);
            }
        }

        private void ModifyUserInfo(bool result, JsonData msg) {
            if (result) {
                _userCenter.UserInfo.Head = _head;
                _userCenter.UserInfo.UserName = _userName;
                EventCenter.Instance.Broadcast(EventId.ModifyUserInfo, _head, _userName);
                Hide();
            }
        }
        private void OnLogoutClick() {
            LoginBoolDelegate logoutAction = null;
            logoutAction = (result) => {
                if (result) {
                    EventCenter.Instance.Broadcast(EventId.ClosePersonalCenter);
                    Hide();
                } else {
                    UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_logout"));
                }
            };
            _loginMgr.OnLogout += logoutAction;
            _loginMgr.Logout();
        }
    }
}