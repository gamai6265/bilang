using System.Collections.Generic;
using UnityEngine;

namespace Cloth3D.Data{
    public class ShopItems {
        public string ModelId = string.Empty;
        public string Id=string.Empty;
        public string Num=string.Empty;
        public string GoodsName = string.Empty;
        public Texture2D GoodPic = null;
        public List<PartInfo> lstPartInfo;
        public List<FabricInfo> lstFabricInfo ;
    }
}
