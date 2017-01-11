using LitJson;

namespace Cloth3D.Data {
    public class Address {
        public string Id = string.Empty;
        public string Contacts = string.Empty;
        public string Tel = string.Empty;
        public string Province = string.Empty;
        public string ProvinceCode = string.Empty;
        public string City = string.Empty;
        public string CityCode = string.Empty;
        public string Area = string.Empty;
        public string AreaCode = string.Empty;
        public string Addr = string.Empty;
        public string PostCode = string.Empty;
        public string DefaultIdentifier = string.Empty;
        public bool IsDefault = false;

        public string ToJson(string userId) {
            JsonData addrJson = new JsonData();
            addrJson["post_code"] = PostCode;
            addrJson["linkman"] = Contacts;
            addrJson["area_code"] = AreaCode;
            addrJson["province"] = Province;
            addrJson["area"] = Area;
            addrJson["user_id"] = userId;
            addrJson["addr"] = Addr;
            addrJson["default_flag"] = IsDefault;
            addrJson["city"] = City;
            addrJson["city_code"] = CityCode;
            addrJson["link_tel"] = Tel;
            return addrJson.ToJson();
        }
    }
}