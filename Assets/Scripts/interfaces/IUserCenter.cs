using System.Collections.Generic;
using Cloth3D.Data;
using LitJson;
using UnityEngine;

namespace Cloth3D.Interfaces {
    public enum MysizeEntry {
        Height,
        Weight,
        Shoulder,
        Bust,
        Waistline,
        Hipline,
    }
    interface IUserCenter : IComponent {
        UserInfo UserInfo { get; }
        ClothSize ClothSize { get; }
        Dictionary<string, Dictionary<string,string>> DicCityList { get; }
        Dictionary<string, Dictionary<string,string>> DicAreaList { get; }
        
        event MyAction<UserInfo> OnUserInfoUpdate;
        event MyAction<bool, string> OnSetDefaultAddress;
        void QueryAddress(MyAction<bool, List<Address>> callback);
        void DeleteAddress(string addrId, MyAction<bool, string> callback);
        void SetDefaultAddress(string addrId,MyAction<bool,string> callback);
        void NewAddress(string addr, MyAction<bool,JsonData> callback);
        void ModifyUserSize(MysizeEntry entry, string value, MyAction<bool> callback);
        void ShareUrl();
        void GetCityList(string provinceCode, MyAction<bool, Dictionary<string, string>> callback);
        void GetAreaList(string cityCode, MyAction<bool, Dictionary<string, string>> callback);
        Address GetDefaultAddress();
        void AddOrUpdateModelSize(string modelId,string modelSizeJson, MyAction<bool,JsonData> callback);
        void ModifyUserInfo(string userName, IList<KeyValuePair<string, byte[]>> files, MyAction<bool, JsonData> callback);

        void GetModelSizeList(string id, string modelid, string type, string height, string weight,
            MyAction<bool, JsonData> callback);
    }
}
