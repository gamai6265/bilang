using Cloth3D.Data;
using LitJson;

namespace Cloth3D.Interfaces {
    internal interface ILoginEvents {
    }

    public enum LoginFailedType {
        NetworkWrong,
        PassWordWrong
    }

    public delegate void LoginVoidDelegate();

    public delegate void LoginFaildDelegate(LoginFailedType type);

    public delegate void LgoinJsonDelegate(JsonData json);

    public delegate void LoginBoolDelegate(bool result);

    //public delegate void LoginSuccDelegate(string userId, string tel, string userName);

    internal interface ILoginMgr : IComponent {
        event MyAction<string> OnLoginSucc;
        event LoginFaildDelegate OnLoginFailed;
        event LoginBoolDelegate OnGetRegisterCode;
        event LoginBoolDelegate OnCheckRegisterCode;
        event LoginBoolDelegate OnRegister;
        event LoginBoolDelegate OnGetRestPwCode;
        event LoginBoolDelegate OnResetPassword;
        event LoginBoolDelegate OnLogout;
        void Login(string username, string password);
        void Logout();
        void GetRegisterCode(string phone);
        void CheckRegisterCode(string tel, string code,string password);
        void Register(string tel, string sex, string userAccount, string password, string realName);
        void GetResetPwCode(string tel);
        void RestPassword(string tel, string password, string code);
        bool IsLogined();
    }
}