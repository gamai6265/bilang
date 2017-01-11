using System.Collections;
using Cloth3D.Data;
using UnityEngine;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Login {
    public class LoginMgr :ILoginMgr, ILoginListener {
        //private UserInfo _userInfo;
        private bool _isLogined = false;
        private INetworkMgr _networkMgr;
        private string _registerCode=string.Empty;
        public string Name { get; set; }
        public void Tick() {
        }

        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr==null) {
                LogSystem.Error("erro, network is null.");
                return false;
            }
            BindEvents();
            return true;
        }
        public event MyAction<string> OnLoginSucc;
        public event LoginFaildDelegate OnLoginFailed;
        public event LoginBoolDelegate OnGetRegisterCode;
        public event LoginBoolDelegate OnCheckRegisterCode;
        public event LoginBoolDelegate OnRegister;
        public event LoginBoolDelegate OnGetRestPwCode;
        public event LoginBoolDelegate OnResetPassword;
        public event LoginBoolDelegate OnLogout;

        private static string GetSession(JsonData jdata) {
            return jdata["set-cookie"].ToString();
        }
        public void Login(string username, string password) {
            var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendLoginReq(username, password);
            } else {
               LogSystem.Info("ILoginSender is null.");
            }
        }

        public void Logout() {
            _isLogined = false;
            if (OnLogout != null) {
                OnLogout(true);
                PlayerPrefs.DeleteAll();
            }
            /*var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendLogoutReq();
            } else {
               LogSystem.Warn("ILoginSender is null.");
            }*/
        }

        public void GetRegisterCode(string phone) {
            var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendGetRegisterCodeReq(phone);
            } else {
                LogSystem.Info("ILoginSender is null.");
            }
        }

        public void CheckRegisterCode(string tel, string code, string password) {
            _registerCode = code;
            var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendCheckRegisterCodeReq(tel, code, password);
            } else {
                LogSystem.Info("ILoginSender is null.");
            }
        }

        public void Register(string tel, string sex, string userAccount, string password, string realName) {
            var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendRegisterReq(_registerCode, tel, sex, userAccount, password, realName);
            } else {
                LogSystem.Info("ILoginSender is null.");
            }
        }

        public void GetResetPwCode(string tel) {
            var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendGetResetPwCodeReq(tel);
            } else {
                LogSystem.Info("ILoginSender is null.");
            }
        }

        public void RestPassword(string tel, string password, string code) {
            var sender = _networkMgr.QuerySender<ILoginSender>();
            if (sender != null) {
                sender.SendResetPasswordReq(tel, password, code);
            } else {
                LogSystem.Info("ILoginSender is null.");
            }
        }

        public bool IsLogined() {
            return _isLogined;
        }
        
        private void BindEvents() {
            var loginTrigger = _networkMgr.QueryTigger(TriggerNames.LoginTrigger);
            if (null != loginTrigger) {
                loginTrigger.AddListener(this);
            } else {
                LogSystem.Error("erro. login trigger is null.");
            }
        }


        public void OnLogoutRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (Utils.GetJsonStr(msg["exit"]) == "0") {
                    _isLogined = false;
                    if (OnLogout != null)
                        OnLogout(true);
                } else {
                    LogSystem.Info("logout req return failed");
                    if (OnLogout != null)
                        OnLogout(false);
                }
            } else {
                LogSystem.Info("net work require failed.");
                if (OnLogout != null)
                    OnLogout(false);
            }
        }

        public void OnLoginRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    var token = Utils.GetJsonStr(msg["token"]);
                    PlayerPrefs.DeleteKey(Constants.Token);
                    PlayerPrefs.SetString(Constants.Token, token);
                    var requestHeader = _networkMgr.QuerySender<IRequestHeader>();
                    if (requestHeader!=null) {
                        requestHeader.SetCookie(GetSession(msg));
                        requestHeader.SetToken(token);
                    }
                    _isLogined = true;
                    if (OnLoginSucc != null) {
                        OnLoginSucc(token);
                    }
                    LogSystem.Debug("login succ:");
                } else {
                    LogSystem.Info("login failed.{0}",Utils.GetJsonStr(msg["message"]));
                    if (null != OnLoginFailed) {
                        OnLoginFailed(LoginFailedType.PassWordWrong);
                    }
                }
            } else {
                LogSystem.Info("login network required failed.");
                if (null != OnLoginFailed) {
                    OnLoginFailed(LoginFailedType.NetworkWrong);
                }
            }
            
        }

        public void OnRegisterRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (Utils.GetJsonStr(msg["exit"]) == "0") {
                    if (null != OnRegister)
                        OnRegister(true);
                } else {
                    if (null != OnRegister)
                        OnRegister(false);
                }
            } else {
                if (null != OnRegister)
                    OnRegister(false);
            }
        }

        public void OnGetResetPwCodeRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    var cookie = _networkMgr.QuerySender<IRequestHeader>();
                    if (cookie != null) {
                        cookie.SetCookie(GetSession(msg));
                        LogSystem.Info("set Register session:{0}",GetSession(msg));
                    }
                    if (null != OnGetRestPwCode)
                        OnGetRestPwCode(true);
                } else {
                    if (null != OnGetRestPwCode)
                        OnGetRestPwCode(false);
                }
            } else {
                if (null != OnGetRestPwCode)
                    OnGetRestPwCode(false);
            }
        }

        public void OnResetPasswordRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    var cookie = _networkMgr.QuerySender<IRequestHeader>();
                    if (cookie != null) {
                        cookie.SetCookie("");
                        LogSystem.Info("set session:");
                    }
                    if (null != OnResetPassword)
                        OnResetPassword(true);
                } else {
                    if (null != OnResetPassword)
                        OnResetPassword(false);
                }
            } else {
                if (null != OnResetPassword)
                    OnResetPassword(false);
            }
        }

        public void OnCheckRegisterCodeRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    if (null != OnCheckRegisterCode)
                        OnCheckRegisterCode(true);
                } else {
                    if (null != OnCheckRegisterCode)
                        OnCheckRegisterCode(false);
                }
            } else {
                if (null != OnCheckRegisterCode)
                    OnCheckRegisterCode(false);
            }
        }

        public bool OnGetRegisterCodeRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    var cookie = _networkMgr.QuerySender<IRequestHeader>();
                    if (cookie != null) {
                        cookie.SetCookie(GetSession(msg));
                        LogSystem.Info("set Register session:{0}", GetSession(msg));
                    }
                    if (null != OnGetRegisterCode)
                        OnGetRegisterCode(true);
                } else {
                    if (null != OnGetRegisterCode)
                        OnGetRegisterCode(false);
                }
            } else {
                if (null != OnGetRegisterCode)
                    OnGetRegisterCode(false);
            }
            return true;
        }
    }
}
