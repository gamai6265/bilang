using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D.Data {
    public class FabricInfo {
        public string FabricCode = string.Empty; //面料id
        public string FabricImage = string.Empty; //面料图片
        public string Currency = string.Empty; //面料货币类型
        public string FabricPrice = string.Empty; //面料价格
//        public string BarginPrice = string.Empty;
        public string FabricColor = string.Empty; //面料色系
        public string FabricFigure = string.Empty; //面料花纹
//        public string Discount = string.Empty;
        public float ActualWidth = 0;
        public string FabricTexture = string.Empty; //面料材质
        public string FabricPly = string.Empty; //面料厚度
        public string FabricOrigin = string.Empty; //面料产地
        public string Brand = string.Empty; //面料品牌
        public string YarnCount = string.Empty; //面料纱支代码
        public string FabricColorName = string.Empty; //面料色系(FabricColor)对应的代码名称
        public string FabricTextureName = string.Empty; //面料材质(FabricTexture)对应的代码名称
        public string FabricFigureName = string.Empty; //面料花纹(FabricFigure)对应的代码名称
        public string SuppType = string.Empty;
    }
}
