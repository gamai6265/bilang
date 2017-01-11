using System;
using UnityEngine;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;

namespace Cloth3D.Design {
    internal class PartObject {
        public Transform Trans;
        public int PartId;
        public string FabricCode1 = string.Empty;
        public string FabricCode2 = string.Empty;
        public Texture2D FabricTexture1;
        public Texture2D FabricTexture2;
        public string PartWeight = string.Empty;
        public string MateId1Price = string.Empty;
        public string MateId2Price = string.Empty;
        public string MateId1SuppType = string.Empty;
        public string MateId2SuppType = string.Empty;
        public string Fabric1Rotation;
        public string Fabric2Rotation;
    }

    public class ModelView : MonoBehaviour {
        private Dictionary<Transform, PartObject> _dicTransform2PartObject;
        private Dictionary<int, PartObject> _dicModelPart2PartObject;
        private Dictionary<int, HotPoint> _dicModelPart2Hotpoints;
        private IDesignWorld _designWorld;
        private Transform _hotpointTrans;
        public int ModelId { get; set; }
        private List<int> _lstHideParts;
        public KeyValuePair<string, Texture2D> _mainTexutre = new KeyValuePair<string, Texture2D>(string.Empty, null);

        void Awake() {
            _lstHideParts = new List<int>();
            _dicModelPart2PartObject = new Dictionary<int, PartObject>();
            _dicTransform2PartObject = new Dictionary<Transform, PartObject>();
            _dicModelPart2Hotpoints = new Dictionary<int, HotPoint>();
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            var obj = new GameObject("Hotpoints");
            obj.transform.parent = transform;
            _hotpointTrans = obj.transform;
        }

        void Update() {
            //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward, Color.yellow);
            //foreach (var hotpoint in _dicModelPart2Hotpoints) {
            //    hotpoint.Value.DrawDebug();
            //}
        }

        public bool HandelTapEvent(Vector2 pos, GameObject obj) {
            bool handeld = false;
            foreach (var item in _dicModelPart2Hotpoints) {
                if (item.Value.MeshObj == obj) {
                    var hotPoint = item.Value;
                    var part = ModelPartProvider.Instance.GetModelPart(ModelId, hotPoint.PartId);
                    if (part != null && part.SubParts != null) {
                        for (int i = 0; i < part.SubParts.Count; i++) {
                            if (part.SubParts[i].Visible) {
                                _designWorld.DesignPartDetail(hotPoint.ModelId, part.SubParts[i].Id);
                                handeld = true;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            return handeld;
        }

        public void UpdateHotpointsScale(float scale) {
            foreach (var hotpoint in _dicModelPart2Hotpoints) {
                hotpoint.Value.UpdateScale(scale);
            }
        }
        public void UpdateHotpoints(bool show) {
            if (!show) {
                if (_hotpointTrans.gameObject.activeInHierarchy)
                    _hotpointTrans.gameObject.SetActive(false);
            } else {
                if (!_hotpointTrans.gameObject.activeInHierarchy) {
                    _hotpointTrans.gameObject.SetActive(true);
                    foreach (var hotpoint in _dicModelPart2Hotpoints) {
                        hotpoint.Value.Hide();
                    }
                }
                foreach (var hotpoint in _dicModelPart2Hotpoints) {
                    hotpoint.Value.UpdateOrientation();
                }
            }
        }

        public bool GetHotpointOriginalDir(ModelPart part, out Vector3 dir) {
            if (_dicModelPart2Hotpoints.ContainsKey(part.Id)) {
                var hotpoint = _dicModelPart2Hotpoints[part.Id];
                dir = hotpoint.GetOrignalDir();
                return true;
            }
            dir = Vector3.zero;
            return false;
        }

        private void GetPathParts(ModelPart part, List<ModelPart> lstParts) {
            if(!lstParts.Contains(part))
                lstParts.Add(part);
            if (part.ChildType == PartChildType.ChildOption) {
                if(part.OwnedParent!=null)
                    GetPathParts(part.OwnedParent, lstParts);
            } else {
                if(part.ParentPart!=null)
                    GetPathParts(part.ParentPart, lstParts);
            }
        }
        public void CreateModelFromData(List<PartInfo> lstPart) {
            var model = ModelPartProvider.Instance.GetModel(ModelId);
            if (model != null) {
                //restore data
                var lstModelpart =  new List<ModelPart>();
                for (int i = 0; i < lstPart.Count; i++) { 
                    var part  = ModelPartProvider.Instance.GetModelPart(ModelId, lstPart[i].PartId);
                    if (part != null) {
                        GetPathParts(part, lstModelpart);
                    }
                }

                lstModelpart.Sort((item1, item2) => {
                    return item1.Level - item2.Level;
                });
                for (int i = 0; i < lstModelpart.Count; i++) {
                    if (lstModelpart[i].ChildType == PartChildType.ChildFiexed) {
                        ModelPartProvider.Instance.RestorePart(lstModelpart[i], true, false, false);
                    } else {
                        ModelPartProvider.Instance.RestorePart(lstModelpart[i], true, true, false);
                    }
                }
                //create mesh
                _ChangeModelPart(model, transform);
            }
        }

        private Transform _CreateMesh(ModelPart part, Transform parent) {
            Transform ret;
            if (part.IsMountPoint) {
                var obj = new GameObject(part.PartName);
                obj.transform.parent = parent;
                _dicTransform2PartObject.Add(obj.transform, new PartObject() {
                    PartId = part.Id,
                    PartWeight = part.Weight,
                    Trans = obj.transform,
                });
                _dicModelPart2PartObject.Add(part.Id, new PartObject() {
                    PartId = part.Id,
                    PartWeight = part.Weight,
                    Trans = obj.transform
                });
                ret = obj.transform;
            } else {
                var model = ModelsConfigProvider.Instance.GetDataById(ModelId);
                string path = "Model/" + model.ModelName + "/" + part.PartName;
                var prefab = Resources.Load<GameObject>(path);
                if (prefab == null) {
                    LogSystem.Error("error. can't load mesh {0}/realPath:{1}", part.PartPath,path);
                    return null;
                }
                var go = Instantiate(prefab);
                go.transform.parent = parent;
                _dicModelPart2PartObject.Add(part.Id, new PartObject() {
                    PartId = part.Id,
                    PartWeight = part.Weight,
                    Trans = go.transform,
                });
                _dicTransform2PartObject.Add(go.transform, new PartObject() {
                    PartId = part.Id,
                    PartWeight = part.Weight,
                    Trans = go.transform
                });
                ret = go.transform;
            }
            return ret;
        }

        //loop & create mesh
        private void _ChangeModelPart(ModelPart part, Transform parentTrans) {
            //var aa = part.Id;
            if (part.Visible) {
                Transform currentTransform = null;
                if (_dicModelPart2PartObject.ContainsKey(part.Id))
                    currentTransform = _dicModelPart2PartObject[part.Id].Trans;
                else
                    currentTransform = _CreateMesh(part, parentTrans);
                currentTransform.gameObject.SetActive(true);
                if (part.SubParts != null) {
                    int styleCount = 0;
                    for (int i = 0; i < part.SubParts.Count; i++) {
                        if (part.SubParts[i].ChildType != PartChildType.ChildFiexed)
                            styleCount++;
                    }
                    //show hot points
                    if (styleCount > 1) {
                        HotPoint hotPoint = null;
                        if (!_dicModelPart2Hotpoints.ContainsKey(part.Id)) {
                            string path = "Prefabs/HotPoint"; 
                            var prefab = Resources.Load<GameObject>(path);
                            if (prefab == null) {
                                return;
                            }
                            var hotpointObj = Instantiate(prefab); //TODO danie 缓存
                            hotpointObj.name = "(Hotpoint:" + part.PartName + ")";
                            hotPoint = hotpointObj.GetComponent<HotPoint>();
                            hotPoint.Init(part.Id, part.HotPointPos, part.HotPointRotation, ModelId);
                            //parent hotpoint transform
                            hotpointObj.transform.parent = _hotpointTrans;
                            _dicModelPart2Hotpoints.Add(part.Id, hotPoint);
                        } else {
                            hotPoint = _dicModelPart2Hotpoints[part.Id];
                        }
                        hotPoint.gameObject.SetActive(true);
                    }
                    //sub parts
                    for (int i = 0; i < part.SubParts.Count; i++) {
                        _ChangeModelPart(part.SubParts[i], currentTransform);
                    }
                }
                //option parts
                if (part.LstOwnedOptions != null) {
                    for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                        _ChangeModelPart(part.LstOwnedOptions[i], currentTransform);
                    }
                }
                if (part.LstInfluences != null) {
                    for (int i = 0; i < part.LstInfluences.Count; i++) {
                        if (_dicModelPart2PartObject.ContainsKey(part.LstInfluences[i].Id)) {
                            var item = part.LstInfluences[i];
                            var itemObj = _dicModelPart2PartObject[item.Id];
                            if (item.Visible && item.LstOwnedOptions!=null) {
                                for (int j = 0; j < item.LstOwnedOptions.Count; j++) {
                                    _ChangeModelPart(item.LstOwnedOptions[j], itemObj.Trans);
                                }
                            }
                        }
                    }
                }
            } else {
                if (_dicModelPart2PartObject.ContainsKey(part.Id)) {
                    _dicModelPart2PartObject[part.Id].Trans.gameObject.SetActive(false);
                }
                _HideHotpoints(part);
            }
        }

        public void HideParts(List<ModelPart> lstParts) {
            for (int i = 0; i < lstParts.Count; i++) {
                if (_dicModelPart2PartObject.ContainsKey(lstParts[i].Id)) {
                    var trans = _dicModelPart2PartObject[lstParts[i].Id].Trans;
                    if (trans != null) {
                        trans.gameObject.SetActive(false);
                    }
                    _lstHideParts.Add(lstParts[i].Id);
                }
            }
        }

        public void RestoreHideParts() {
            for (int i = 0; i < _lstHideParts.Count; i++) {
                var objTran = _dicModelPart2PartObject[_lstHideParts[i]].Trans;
                if (objTran != null) {
                    objTran.gameObject.SetActive(true);
                }
            }
            _lstHideParts.Clear();
        }

        private void _HideHotpoints(ModelPart part) {
            if (_dicModelPart2Hotpoints.ContainsKey(part.Id)) {
                var hotpoint = _dicModelPart2Hotpoints[part.Id];
                hotpoint.gameObject.SetActive(false);
            }
            if (part.SubParts != null) {
                for (int i = 0; i < part.SubParts.Count; i++) {
                    _HideHotpoints(part.SubParts[i]);
                }
            }
        }

        public KeyValuePair<string, Texture2D> GetPartFabric(ModelPart part) {
            if (part.LstOwnedOptions != null) {
                for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                    if (part.LstOwnedOptions[i].Visible) {
                        return GetPartFabric(part.LstOwnedOptions[i]);
                    }
                }
                return new KeyValuePair<string, Texture2D>(string.Empty, null);
            }
            if (!part.IsMountPoint) {
                if (_dicModelPart2PartObject.ContainsKey(part.Id)) {
                    var foundPart = _dicModelPart2PartObject[part.Id];
                    return new KeyValuePair<string, Texture2D>(foundPart.FabricCode1, foundPart.FabricTexture1);
                }
                return new KeyValuePair<string, Texture2D>(string.Empty, null);
            }
            if (part.SubParts != null) {
                for (int i = 0; i < part.SubParts.Count; i++) {
                    if (part.SubParts[i].ChildType!=PartChildType.ChildFiexed&& part.SubParts[i].Visible) {
                        return GetPartFabric(part.SubParts[i]);
                    }
                }
                return new KeyValuePair<string, Texture2D>(string.Empty, null);
            }
            return new KeyValuePair<string, Texture2D>(string.Empty, null);
        }

        public bool ChangeModelPart(ModelPart part, out Dictionary<int, ModelPart> dicChangeParts) {
            dicChangeParts = null;
            bool ret = true;
            if (part.LstRejectParts != null) {
                for (int i = 0; i <part.LstRejectParts.Count; i++) {
                    if (_dicModelPart2PartObject.ContainsKey(part.LstRejectParts[i].Id) && ModelPartProvider.Instance.IsActivePart(part.LstRejectParts[i])) {
                        var rejectPart = _dicModelPart2PartObject[part.LstRejectParts[i].Id];
                        if (rejectPart.Trans.gameObject.activeSelf) {
                            Debug.Log("冲突部件"+part.LstRejectParts[i].PartName+":"+part.LstRejectParts[i].Id);
                            ret = false;
                            break;
                        }
                    }
                }
            }
            if (ret) {
                dicChangeParts = ModelPartProvider.Instance.RestorePart(part, true, true, true);
                var trs = _dicModelPart2PartObject[part.ParentPart.Id].Trans;
                _ChangeModelPart(part.ParentPart, trs);
            }
            return ret;
        }

        public void ChangePartFabric(ModelPart part, FabricInfo fabricInfo, Texture2D texture, float fabricSize,
            FabricSide side) {
            var dic = new Dictionary<int, int>();
            _ChangePartFabric(part, fabricInfo, texture, fabricSize, side, dic);
        }

        private void _ChangePartFabric(ModelPart part, FabricInfo fabricInfo, Texture2D texture, float fabricSize, FabricSide side, Dictionary<int, int> dic ) {
//            var aa = part.Id;
            if (!dic.ContainsKey(part.Id)) {
                dic.Add(part.Id, 0);
                if (_dicModelPart2PartObject.ContainsKey(part.Id)) {
                    if (!part.IsMountPoint && !part.IsDecorate) {
                        var partObj = _dicModelPart2PartObject[part.Id];
                        if (side != 0 && !part.PartName.StartsWith("niukou")) {//TODO 临时解决1.双面料功能.2.跟换主面料时纽扣也随之变化.后续优化
                            //if ((side & FabricSide.FabricUpside) != 0) {
                            if (SetMainTexture(partObj.Trans.gameObject, texture, part.MaterialName1, fabricSize)) {
                                partObj.FabricCode1 = fabricInfo.FabricCode;
                                partObj.MateId1Price = fabricInfo.FabricPrice;
                                partObj.MateId1SuppType = fabricInfo.SuppType;
                                partObj.FabricTexture1 = texture;
                            }
                        }
                        if (side != 0 && !part.PartName.StartsWith("niukou") && part.MaterialName2 != string.Empty) {
                            //if ((side & FabricSide.FabricFilpside) != 0) {
                            if (SetMainTexture(partObj.Trans.gameObject, texture, part.MaterialName2, fabricSize)) {
                                partObj.FabricCode2 = fabricInfo.FabricCode;
                                partObj.MateId2Price = fabricInfo.FabricPrice;
                                partObj.MateId2SuppType = fabricInfo.SuppType;
                                partObj.FabricTexture2 = texture;
                            }
                        }
                    }
                }
                //option parts
                if (part.LstOwnedOptions != null) {
                    for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                        _ChangePartFabric(part.LstOwnedOptions[i], fabricInfo, texture, fabricSize, side, dic);
                    }
                }
                //sub parts
                if (part.SubParts != null) {
                    for (int i = 0; i < part.SubParts.Count; i++) {
                        _ChangePartFabric(part.SubParts[i], fabricInfo, texture, fabricSize, side, dic);
                    }
                }
//               
                //influence other assoiate 
                if (part.LstAffectOtherAssociate != null) {
                    for (int i = 0; i < part.LstAffectOtherAssociate.Count; i++) {
                        _ChangePartFabric(part.LstAffectOtherAssociate[i], fabricInfo, texture, fabricSize, side, dic);
                    }
                }
            }
        }

        bool SetMainTexture(GameObject go, Texture2D texture, string material, float fabricSize) {
            Material[] lstMaterial = go.GetComponent<Renderer>().materials;
            if (lstMaterial == null) return false;
            bool ret = false;
            Material t1 = null;
            for (int i = 0; i < lstMaterial.Length; i++) {
                if (lstMaterial[i].name.StartsWith(material + " (Instance)")) {
                    t1 = lstMaterial[i];
                    break;
                }
            }
            if (t1 != null) {
                var size = new Vector2();
                size.x = size.y = fabricSize;
                t1.mainTexture = texture;
                t1.mainTextureScale = size;
                ret = true;
            }
            return ret;
        }
        public void ChangeGlobal(Texture2D texture, List<string> material) {
            //TODO 1.新添加的部分可能不会出现效果 ,2. 这里效率慢点 需要优化 3.纹理大小
            foreach (var item in _dicModelPart2PartObject) {
                var part = ModelPartProvider.Instance.GetModelPart(ModelId, item.Key);
                if (part != null && !part.IsMountPoint && !part.IsDecorate) {
                    for (int i = 0; i < material.Count; i++) {
                        SetMainTexture(item.Value.Trans.gameObject, texture, material[i], 1.0f);
                    }
                }
            }
        }


        public Dictionary<string, Texture2D> GetInUsedFabricList(ModelPart part) {
            Dictionary<string, Texture2D> dicFabricInfo = new Dictionary<string, Texture2D>();
            ModelPart.LoopVisibleParts(part, (item) => {
                if (_dicModelPart2PartObject.ContainsKey(item.Id)) {
                    var partObj = _dicModelPart2PartObject[item.Id].Trans.gameObject;
                    if (!item.IsMountPoint && !item.IsDecorate) {
                        Material[] fabricMaterial = partObj.GetComponent<Renderer>().materials;
                        if (fabricMaterial == null)
                            return;
                        Material t1 = null;
                        Material t2 = null;
                        for (int i = 0; i < fabricMaterial.Length; i++) {
                            if (item.MaterialName1 != string.Empty &&
                                fabricMaterial[i].name.StartsWith(item.MaterialName1) && !item.MaterialName1.StartsWith("niukou"))
                                t1 = fabricMaterial[i];
                            if (item.MaterialName2 != string.Empty &&
                                fabricMaterial[i].name.StartsWith(item.MaterialName2) && !item.MaterialName2.StartsWith("niukou"))
                                t2 = fabricMaterial[i];
                        }
                        string fabricCode;
                        Texture2D texture2D = null;
                        if (t1 != null && t1.mainTexture != null) {
                            fabricCode = _dicModelPart2PartObject[item.Id].FabricCode1;
                            texture2D = t1.mainTexture as Texture2D;
                            if (!dicFabricInfo.ContainsKey(fabricCode))
                                dicFabricInfo.Add(fabricCode, texture2D);
                        }
                        if (t2 != null && t2.mainTexture != null ) {
                            fabricCode = _dicModelPart2PartObject[item.Id].FabricCode2;
                            texture2D = t2.mainTexture as Texture2D;
                            if (!dicFabricInfo.ContainsKey(fabricCode))
                                dicFabricInfo.Add(fabricCode, texture2D);
                        }
                    }
                }
            });
            return dicFabricInfo;
        }

        public List<PartInfo> GetPartInfo() {
            List<PartInfo> partInfos=new List<PartInfo>();
            var part  = ModelPartProvider.Instance.GetModel(ModelId);
            if (part != null) {
                ModelPart.LoopVisibleParts(part, (item) => {
                    if (item != null && item.Visible && !item.IsDecorate && item.IsObsolete) {
                        if (_dicModelPart2PartObject.ContainsKey(item.Id)) {
                            var partObj = _dicModelPart2PartObject[item.Id];
                            var partInfo = new PartInfo();
                            partInfo.PartId = partObj.PartId;
                            partInfo.Partname = partObj.Trans.name;
                            partInfo.Fabric1 = partObj.FabricCode1;
                            partInfo.Fabric2 = partObj.FabricCode2;
                            partInfo.PartType = "1";
                            partInfo.PartWeight = partObj.PartWeight;
                            partInfo.MateId1Price = partObj.MateId1Price;
                            partInfo.MateId2Price = partObj.MateId2Price;
                            partInfo.MateId1SuppType = partObj.MateId1SuppType;
                            partInfo.MateId2SuppType = partObj.MateId2SuppType;
                            partInfo.Fabric1Rotation = partObj.Fabric1Rotation;
                            partInfo.Fabric2Rotation = partObj.Fabric2Rotation;
                            partInfos.Add(partInfo);
                        }
                    }
                });
            }
            return partInfos;
        }

        public void RotationFabirc(ModelPart part, string rotation, FabricSide side) {
            if (_dicModelPart2PartObject.ContainsKey(part.Id)) {
                if (!part.IsMountPoint && !part.IsDecorate) {
                    var partObj = _dicModelPart2PartObject[part.Id];
                    if ((side & FabricSide.FabricUpside) != 0) {
                        if (SetMainTextRotation(partObj.Trans.gameObject, part.MaterialName1, rotation)) {
//                            Debug.Log("-1-"+ part.MaterialName1+"/"+ rotation);
                            partObj.Fabric1Rotation = rotation;
                        }
                    }
                    if ((side & FabricSide.FabricFilpside) != 0) {
                        if (SetMainTextRotation(partObj.Trans.gameObject, part.MaterialName2, rotation)) {
//                            Debug.Log("-2-" + part.MaterialName2 + "/" + rotation);
                            partObj.Fabric2Rotation = rotation;
                        }
                    }
                }
            }
            //option parts
            if (part.LstOwnedOptions != null) {
                for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                    RotationFabirc(part.LstOwnedOptions[i], rotation, side);
                }
            }
            //sub parts
            if (part.SubParts != null) {
                for (int i = 0; i < part.SubParts.Count; i++) {
                    RotationFabirc(part.SubParts[i], rotation, side);
                }
            }
            
            //influence other assoiate 
            if (part.LstAffectOtherAssociate != null) {
                for (int i = 0; i < part.LstAffectOtherAssociate.Count; i++) {
                    RotationFabirc(part.LstAffectOtherAssociate[i], rotation, side);
                }
            }
        }

        private bool  SetMainTextRotation(GameObject go, string materialName, string rotation) {
            Material[] lstMaterial = go.GetComponent<Renderer>().materials;
            bool ret = false;
            if (lstMaterial == null) return false;
            Material t1 = null;
            for (int i = 0; i < lstMaterial.Length; i++) {
                if (lstMaterial[i].name.StartsWith(materialName + " (Instance)")) {
                    t1 = lstMaterial[i];
                    break;
                }
            }
            if (t1 != null) {
                float _rotation = 0f;
                switch (rotation) {
                    case "0":
                        _rotation = 0f;
                        break;
                    case "90":
                        _rotation = (float) Math.PI/2.0f;
                        break;
                    case "-45":
                        _rotation = (float) (135.0f/180*Math.PI);
                        break;
                    case "45":
                        _rotation = (float) (45.0f/180*Math.PI);
                        break;
                }
                t1.SetFloat("_Rotation", _rotation);
                ret = true;
            }
            return ret;
        }
    }
}
