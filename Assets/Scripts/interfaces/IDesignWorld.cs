using System;
using System.Collections.Generic;
using Cloth3D.Data;
using LitJson;
using UnityEngine;

namespace Cloth3D.Interfaces{
    public enum DesignState { //TODO danie delete
        StatePreview,
        StateDesign,
        StateStyle,
    }
    [Flags]
    public enum FabricSide {
        FabricUpside = 1 << 0,
        FabricFilpside = 1 << 1,
        //FabricAllside = 1 << 2 - 1,
    }
    public class SerialModelInfo { 
        public int ModelTypeId;
        public List<PartInfo> LstPartInfo;
    }
    
    interface IDesignWorld:IComponent {
        bool ChangePart(int partId);
        bool ChangeFabric(string fabricCode, Texture2D texture);
        void GetFabricStyleList(MyAction<bool, List<KeyValuePair<string, string>>> callback);
        void GetFabricInfo(string fabricStyleCode, int page, string queryType, MyAction<bool, List<FabricInfo>> callback);

        event MyAction<DesignState> OnChangeDesignStateEvent;
        void PreviewModel();
        void DesignModel();
        void StraightenModel();
        bool InitDesignModel(SerialModelInfo serialModelInfo);
        FabricSide ToggleFabricSide(FabricSide side);
        FabricSide FabricSide { get; }
        void DesignPartDetail(int modelId, int partId);
        int GetModelId();
        event MyAction<bool, string, string> OnCountPrice;
//        void GetModelSize(MyAction<bool,JsonData> callback);
        void GetUserSize(string modelId,MyAction<bool,JsonData> callback);
//        void GetUserSize(string modelId, string atid, MyAction<bool, JsonData> callback);
//        void GetModelSize(string id, MyAction<bool, JsonData> callback);
        void SaveModelSize(string modelId,string atid, string buyerPropertiesJson, MyAction<bool, JsonData> callback);
        void DeleteUserSize(string ids,MyAction<bool,JsonData> callback);
        List<PartInfo> GetPartInfo();
        Dictionary<string,Texture2D> GetInUsedFabricList();
        List<ModelPart> GetPartStyle();
        List<GlobalConfig> GetGlobalPartStyle();
        int CurrentPart { get; }
        int GlobalId { get; set; }
        ModelPart GetModelPart(int modelType, int partId);
        void GetFabricDetail(SerialModelInfo serialModelInfo, MyAction<bool> callback);
        void GetScreenShot(MyAction<Texture2D,Texture2D> callback);
        FabricInfo GetMainFabbric();
        string GetShirtName();
        void RotationFabirc(string rotation);
        Dictionary<string, string> GetGlobalStyle();
    }
}
