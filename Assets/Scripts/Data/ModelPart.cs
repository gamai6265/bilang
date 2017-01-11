using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cloth3D.Data {
    public enum PartChildType {
        ChildDefault = 0,
        ChildFiexed = 1,
        ChildRejectAll = 2,
        ChildOption = 3,
    }
    public class ModelPart : IData {
        public int Id;
        public int ModelId;
        public bool Visible = false;
        public string PartName = string.Empty;
        public PartChildType ChildType = PartChildType.ChildDefault;
        public string DisplayName = string.Empty;
        public string PartPath = string.Empty;
        public string ParentPath = string.Empty;
        //options 
        public List<ModelPart> LstOwnedOptions = null;
        public ModelPart OwnedParent = null;
        public string ChildValue= string.Empty;
        //influences
        public List<ModelPart> LstInfluences = null;
        public List<ModelPart> LstInfluenceFactors = null;
        public string InfluenceFactors = string.Empty;
        public string ExpectValues = string.Empty;
        public List<List<string>> LstExpectValues= null;
        //reject & associate
        public string RejectPartsStr = string.Empty;
        public ModelPart AssociateFabricPart = null;
        public List<ModelPart> LstAffectOtherAssociate = null; //被关联的节点，名字好戳
        public int AssociateFabricPartId = -1;
        public List<ModelPart> LstRejectParts = null;
        //others
        public bool IsDoubleSide = false;
        public bool IsDefault = false;
        public string MaterialName1 = string.Empty;
        public string MaterialName2 = string.Empty;
        public bool IsGlobalMetarial = false;
        public bool IsMountPoint = false;
        public bool IsDecorate = false;
        public bool IsValid = false;
        public string SampleImage = string.Empty;
        public Vector3 HotPointPos = Vector3.zero;
        public Vector3 HotPointRotation = Vector3.zero;
        public string HotPointHidesStr = string.Empty;
        public Vector3 ViewPoint = Vector3.zero;
        public Vector3 ViewPointRotation = Vector3.zero;
        public List<ModelPart> LstHotPointHibdes = null;
        public string Weight = string.Empty;
        public string Instruction = string.Empty;
        public bool IsObsolete = false;
        public List<ModelPart> SubParts = null;
        public ModelPart ParentPart = null;
        public List<string> LstModelSizes = null;
        public int Level = 0;
        public ModelPart LastVisibleStylePart = null;
        public ModelPart LastOptionPart = null;

        public string ExtWeight = string.Empty;
        public string Cost = string.Empty;

        public bool CollectDataFromDbc(DbcRow node) {
            Id = DbcUtils.ExtractNumeric(node, "Id", 0);
            PartPath = DbcUtils.ExtractString(node, "PartPath", string.Empty);
            var seperatorIndex = PartPath.LastIndexOf("/", StringComparison.Ordinal);
            if (seperatorIndex == -1) {
                return false;
            }
            ParentPath = PartPath.Substring(0, seperatorIndex);
            if (PartPath.Length > 1)
                PartName = PartPath.Substring(seperatorIndex + 1, PartPath.Length - seperatorIndex - 1);
            var childTypeNum = DbcUtils.ExtractNumeric<int>(node, "ChildType", 0);
            try {
                ChildType = (PartChildType) childTypeNum;
            } catch (Exception) {
                LogSystem.Error("option type is wrong."+PartPath);
                ChildType = PartChildType.ChildDefault;
            }
            ChildValue = DbcUtils.ExtractString(node, "ChildValue", string.Empty).Trim();
            InfluenceFactors = DbcUtils.ExtractString(node, "InfluenceFactors", string.Empty);
            ExpectValues = DbcUtils.ExtractString(node, "ExpectValues", string.Empty);
            if (ExpectValues != string.Empty) {
                var lstFiled= Utils.SplitAsList(ExpectValues, DbcUtils.FieldStringSeperator);
                LstExpectValues = new List<List<string>>();
                for (int i = 0; i < lstFiled.Count; i++) {
                    var lstNums = Utils.SplitAsList(lstFiled[i], new string[] { ";" });
                    LstExpectValues.Add(lstNums);
                }
            }

            DisplayName = DbcUtils.ExtractString(node, "DisplayName", string.Empty);
            RejectPartsStr = DbcUtils.ExtractString(node, "RejectParts", string.Empty);
            AssociateFabricPartId = DbcUtils.ExtractNumeric(node, "AssociateFabricPart", -1);
            IsDoubleSide = DbcUtils.ExtractBool(node, "IsDoubleSide", false);
            MaterialName1 = DbcUtils.ExtractString(node, "MetarialName1", string.Empty);
            MaterialName2 = DbcUtils.ExtractString(node, "MetarialName2", string.Empty);
            IsGlobalMetarial = DbcUtils.ExtractBool(node, "IsGlobalMetarial", false);
            IsMountPoint = DbcUtils.ExtractBool(node, "IsMountPoint", false);
            IsDecorate = DbcUtils.ExtractBool(node, "IsDecorate", false);
            IsValid = DbcUtils.ExtractBool(node, "IsValid", false);
            IsObsolete = DbcUtils.ExtractBool(node, "IsObsolete", false);
            SampleImage = DbcUtils.ExtractString(node, "SampleImage", string.Empty);
            var lstPos = DbcUtils.ExtractNumericList<float>(node, "HotPoint", 0);
            if (lstPos != null && lstPos.Count == 3) {
                HotPointPos = new Vector3() {
                    x = lstPos[0],
                    y = lstPos[1],
                    z = lstPos[2]
                };
            }
            var lstRot = DbcUtils.ExtractNumericList<float>(node, "HotPointRotation", 0);
            if (lstRot != null && lstRot.Count == 3) {
                HotPointRotation = new Vector3() {
                    x = lstRot[0],
                    y = lstRot[1],
                    z = lstRot[2]
                };
            }
            HotPointHidesStr = DbcUtils.ExtractString(node, "HotPointHides", string.Empty);
            lstPos = DbcUtils.ExtractNumericList<float>(node, "ViewPoint", 0);
            if (lstPos != null && lstPos.Count == 3) {
                ViewPoint = new Vector3() {
                    x = lstPos[0],
                    y = lstPos[1],
                    z = lstPos[2]
                };
            }
            lstRot = DbcUtils.ExtractNumericList<float>(node, "ViewPointRotation", 0);
            if (lstRot != null && lstRot.Count == 3) {
                ViewPointRotation = new Vector3() {
                    x = lstRot[0],
                    y = lstRot[1],
                    z = lstRot[2]
                };
            }
            Weight = DbcUtils.ExtractString(node, "Weight", string.Empty);
            Instruction = DbcUtils.ExtractString(node, "Instruction", string.Empty);
            IsDefault = DbcUtils.ExtractBool(node, "IsDefault", false);
            var modelSizeStr = DbcUtils.ExtractString(node, "ModelSize", string.Empty);
            if (modelSizeStr != string.Empty) {
                var sizeItem = Utils.SplitAsList(modelSizeStr, new[] { "|" });
                LstModelSizes = new List<string>();
                for (var i = 0; i < sizeItem.Count; i++) {
                    if (!LstModelSizes.Contains(sizeItem[i])) {
                        LstModelSizes.Add(sizeItem[i]);
                    }
                }
            }

            return true;
        }

        public int GetId() {
            return Id;
        }

        public static void LoopVisibleParts(ModelPart part, MyAction<ModelPart> callback) {
            if (part.Visible) {
                callback(part);
                if (part.LstOwnedOptions != null) {
                    for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                        LoopVisibleParts(part.LstOwnedOptions[i], callback);
                    }
                }
                if (part.SubParts != null) {
                    for (int i = 0; i < part.SubParts.Count; i++) {
                        LoopVisibleParts(part.SubParts[i], callback);
                    }
                }
            }
        }
        public static ModelPart FindVisibleMeshPart(ModelPart part) {
            if (part.LstOwnedOptions != null) {
                for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                    if (part.LstOwnedOptions[i].Visible)
                        return part.LstOwnedOptions[i];
                }
            } else if(part.SubParts!=null){
                for (int i = 0; i < part.SubParts.Count; i++) {
                    if (part.SubParts[i].Visible && part.SubParts[i].ChildType != PartChildType.ChildFiexed) // except fixed type
                        return part.SubParts[i];
                }
            }
            return null;
        }
    }

    public class ModelPartProvider : Singleton<ModelPartProvider> {
        protected readonly Dictionary<int, Dictionary<int, ModelPart>> DicData =
            new Dictionary<int, Dictionary<int, ModelPart>>();

        private Dictionary<int, ModelPart> _dicModels = new Dictionary<int, ModelPart>();
        private Dictionary<int, List<ModelPart>> _dicGlobalParts = new Dictionary<int, List<ModelPart>>();

        public bool Load(int modelId, string file) {
            var result = true;

            var dbc = new Dbc();
            if (!dbc.Load(file)) {
                return false;
            }
            SortedDictionary<string, ModelPart> sortedDicModel = null;
            Dictionary<int, ModelPart> dicModel = null;
            if (!DicData.ContainsKey(modelId)) {
                sortedDicModel = new SortedDictionary<string, ModelPart>();
                dicModel = new Dictionary<int, ModelPart>();
                DicData.Add(modelId, dicModel);
            } else {
                LogSystem.Debug("conatin the same model id{0}",modelId);
                return false;
            }
            for (var index = 0; index < dbc.RowNum; index++) {
                var row = dbc.GetRowByIndex(index);
                if (row != null) {
                    var data = new ModelPart();
                    var ret = data.CollectDataFromDbc(row);
                    if (ret) {
                        if (!dicModel.ContainsKey(data.GetId())) {
                            dicModel.Add(data.GetId(), data);
                            sortedDicModel.Add(data.PartPath, data);
                        } else {
                            LogSystem.Warn("erro, has the same key.{0}",file + data.PartPath);
                        }
                    } else {
                        LogSystem.Warn("DataMgr.CollectDataFromDbc collectData Row:{0} failed!", index);
                        result = false;
                    }
                } else {
                    LogSystem.Warn("dbc GetRowByIndex is null. index:{0}", index);
                    result = false;
                }
            }
            result &= BuildUpModel(modelId, sortedDicModel, dicModel);
            return result;
        }

        public void Clear() {
            DicData.Clear();
        }

        private void AddChild(Stack<ModelPart> stackParts, ModelPart modelPart) {
            if (modelPart.ChildType == PartChildType.ChildOption) {
                if (stackParts.First().LstOwnedOptions == null)
                    stackParts.First().LstOwnedOptions = new List<ModelPart>();
                stackParts.First().LstOwnedOptions.Add(modelPart);
                modelPart.OwnedParent = stackParts.First();
            } else {
                if (stackParts.First().SubParts == null)
                    stackParts.First().SubParts = new List<ModelPart>();
                //sub parts
                stackParts.First().SubParts.Add(modelPart);
                //parent part
                modelPart.ParentPart = stackParts.First();
            }
        }
        private bool BuildUpModel(int modelId, SortedDictionary<string, ModelPart> sortedDicModel,
            Dictionary<int, ModelPart> dicModel) {
            if (dicModel.Count <= 0)
                return false;
            ModelPart firstPart = sortedDicModel.First().Value;
            //buildup model tree
            Stack<ModelPart> stackParts = new Stack<ModelPart>();
            foreach (var item in sortedDicModel) {
                ModelPart modelPart = item.Value;
                modelPart.ModelId = modelId;
                if (modelPart.IsGlobalMetarial) {
                    if (!_dicGlobalParts.ContainsKey(modelId)) {
                        _dicGlobalParts.Add(modelId, new List<ModelPart>());
                    }
                    var lstGlobalParts = _dicGlobalParts[modelId];
                    lstGlobalParts.Add(modelPart);
                    continue;
                }
                if (stackParts.Count > 0 && modelPart.PartPath == stackParts.First().PartPath) {
                    AddChild(stackParts, modelPart);
                } else {
                    while (stackParts.Count > 0 && stackParts.First().PartPath != modelPart.ParentPath) {
                        stackParts.Pop();
                    }
                    if (modelPart != firstPart) {
                        modelPart.Level = stackParts.Count;
                        if (stackParts.Count <= 0) {
                            LogSystem.Warn("error, model part can't find parent!{0}",modelPart.PartPath);
                            return false;
                        }
                        AddChild(stackParts, modelPart);
                    }
                }
                stackParts.Push(modelPart);
                //buildup influencesFactors
                var influencesFactorRet = BuildUpInfluenceFactor(modelPart, dicModel);
                if (!influencesFactorRet)
                    return false;
                //buildup RejectParts info
                bool rejectRet = BuildUpPartRejectModels(modelPart, dicModel);
                if (!rejectRet)
                    return false;
                //buildup AssociateFabricPart
                if (modelPart.AssociateFabricPartId != -1 && dicModel.ContainsKey(modelPart.AssociateFabricPartId)) {
                    var assPart = dicModel[modelPart.AssociateFabricPartId];
                    modelPart.AssociateFabricPart = assPart;
                    if(assPart.LstAffectOtherAssociate==null)
                        assPart.LstAffectOtherAssociate = new List<ModelPart>();
                    if(!assPart.LstAffectOtherAssociate.Contains(modelPart))
                        assPart.LstAffectOtherAssociate.Add(modelPart);
                }
                //buildup HotPointHides
                bool hideRet = BuildUpHotPointHides(modelPart, dicModel);
                if (!hideRet)
                    return false;
            }

            _dicModels.Add(modelId, firstPart);

            //setup default model
            RestorePart(firstPart, true, true, true);
           
            return true;
        }
        
        private bool BuildUpInfluenceFactor(ModelPart part, Dictionary<int, ModelPart> dicModel) {
            var vecNums = Utils.SplitAsNumericList<int>(part.InfluenceFactors, DbcUtils.FieldNumSeperator);
            if (part.LstInfluenceFactors== null && vecNums.Count > 0)
                part.LstInfluenceFactors = new List<ModelPart>();
            for (int i = 0; i < vecNums.Count; i++) {
                if (!dicModel.ContainsKey(vecNums[i])) {
                    LogSystem.Warn("error. Influence part not found{0}", vecNums[i]);
                    return false;
                }
                var influenceFactorPart = dicModel[vecNums[i]];
                part.LstInfluenceFactors.Add(influenceFactorPart);
                if(influenceFactorPart.LstInfluences==null)
                    influenceFactorPart.LstInfluences = new List<ModelPart>();
                if(!influenceFactorPart.LstInfluences.Contains(part))
                    influenceFactorPart.LstInfluences.Add(part);
            }
            return true;
        }
        private bool BuildUpPartRejectModels(ModelPart part, Dictionary<int, ModelPart> dicModel) {
            var vecNums = Utils.SplitAsNumericList<int>(part.RejectPartsStr, DbcUtils.FieldNumSeperator);
            if (part.LstRejectParts == null && vecNums.Count > 0)
                part.LstRejectParts = new List<ModelPart>();
            for (int i = 0; i < vecNums.Count; i++) {
                if (!dicModel.ContainsKey(vecNums[i])) {
                    LogSystem.Warn("error. {1}'s reject part not found{0}", vecNums[i], part.PartName+part.Id);
                    return false;
                }
                var rejectPart = dicModel[vecNums[i]];
                part.LstRejectParts.Add(rejectPart);
            }
            return true;
        }

        private bool BuildUpHotPointHides(ModelPart part, Dictionary<int, ModelPart> dicModel) {
            var vecNums = Utils.SplitAsNumericList<int>(part.HotPointHidesStr, DbcUtils.FieldNumSeperator);
            if (part.LstHotPointHibdes == null && vecNums.Count > 0)
                part.LstHotPointHibdes = new List<ModelPart>();
            for (int i = 0; i < vecNums.Count; i++) {
                if (!dicModel.ContainsKey(vecNums[i])) {
                    LogSystem.Warn("error. hotpoint hide part not found{0}", vecNums[i]);
                    return false;
                }
                var hidePart = dicModel[vecNums[i]];
                part.LstHotPointHibdes.Add(hidePart);
            }
            return true;
        }

        public ModelPart GetModelPart(int modelId, int partId) {
            if (DicData.ContainsKey(modelId)) {
                var dicModel = DicData[modelId];
                if (dicModel.ContainsKey(partId)) {
                    return dicModel[partId];
                }
            }
            return null;
        }

        public ModelPart GetModel(int modelId) {
            if (_dicModels.ContainsKey(modelId))
                return _dicModels[modelId];
            return null;
        }

        public List<ModelPart> GetGlobalModelParts(int modelId) {
            if (_dicGlobalParts.ContainsKey(modelId)) {
                return _dicGlobalParts[modelId];
            }
            return null;
        }

        private ModelPart FindOption(ModelPart part) {
            if (part.LstOwnedOptions == null || part.LstInfluenceFactors == null)
                return null;
            List<string> lst = new List<string>();
            for (int i = 0; i < part.LstInfluenceFactors.Count; i++) {
                lst.Add(GetChildValue(part.LstInfluenceFactors[i]));
            }
            ModelPart foundOption = null;
            for (int i = 0; i < part.LstOwnedOptions.Count; i++) {
                var found = true;
                for (int valueIndex = 0; valueIndex < lst.Count; valueIndex++) {
                    if(lst[valueIndex]==string.Empty) continue;
                    if (!part.LstOwnedOptions[i].LstExpectValues[valueIndex].Contains(lst[valueIndex])) {
                        found = false;
                        break;
                    }
                }
                if (found) {
                    foundOption = part.LstOwnedOptions[i];
                    break;
                }
            }
            return foundOption;
        }

        private string GetChildValue(ModelPart part) {
            string ret = string.Empty;
            if (part.SubParts != null) {
                for (int i = 0; i < part.SubParts.Count; i++) {
                    var child = part.SubParts[i];
                    if (child.ChildType!= PartChildType.ChildOption && child.Visible) {
                        ret = child.ChildValue;
                        break;
                    }
                }
            }
            return ret;
        }

        public Dictionary<int, ModelPart> RestorePart(ModelPart part, bool includedChild, bool includeSibiling, bool includeParent) {
            Dictionary<int, ModelPart> dic = new Dictionary<int, ModelPart>();
            _RestorePart(part, includedChild, includeSibiling, includeParent, dic);
            return dic;
        }
        public void _RestorePart(ModelPart part, bool includedChild, bool includeSibiling, bool includeParent, Dictionary<int, ModelPart> dicVisited)  {
            var aa = part.Id;
            part.Visible = true;

            if (!dicVisited.ContainsKey(part.Id)) {
                dicVisited.Add(part.Id, part);
                //parent part 
                if (includeParent && part.ParentPart != null)
                    _RestorePart(part.ParentPart, false, false, true, dicVisited);

                //sibiling parts
                if (includeSibiling && part.ParentPart != null) {
                    if (part.ChildType == PartChildType.ChildRejectAll) {
                        for (int i = 0; i < part.ParentPart.SubParts.Count; i++) {
                            var item = part.ParentPart.SubParts[i];
                            if (item != part) {
                                if (item.Visible && item.ChildType!=PartChildType.ChildFiexed && item.ParentPart!=null) {
                                    item.ParentPart.LastVisibleStylePart = item;
                                }
                                item.Visible = false;
                            }
                        }
                    } else if (part.ChildType == PartChildType.ChildDefault) {
                        for (int i = 0; i < part.ParentPart.SubParts.Count; i++) {
                            var item = part.ParentPart.SubParts[i];
                            if (item != part) {
                                if (item.ChildType == PartChildType.ChildFiexed) {
                                    //item.Visible = true;
                                    _RestorePart(item, true, false, includeParent, dicVisited);
                                } else {
                                    if (item.Visible && item.ParentPart != null) {
                                        item.ParentPart.LastVisibleStylePart = item;
                                    }
                                    item.Visible = false;
                                }
                            }
                        }
                    } else if (part.ChildType == PartChildType.ChildFiexed) {
                        var defaultSibiling = GetDefaultSubPart(part.ParentPart.SubParts);
                        if (defaultSibiling != null)
                            _RestorePart(defaultSibiling, true, false, includeParent, dicVisited);
                    }
                }
                //option parts
                if (part.ChildType == PartChildType.ChildOption) {
                    if (part.OwnedParent != null && part.OwnedParent.LstOwnedOptions != null) {
                        for (int i = 0; i < part.OwnedParent.LstOwnedOptions.Count; i++) {
                            var item = part.OwnedParent.LstOwnedOptions[i];
                            if (item != part) {
                                if (item.Visible && item.OwnedParent != null)
                                    item.OwnedParent.LastOptionPart = item;
                                item.Visible = false;
                            }
                        }
                    }
                }

                //recursion sub parts
                if (includedChild && part.SubParts != null) {
                    var defaultSubPart = GetDefaultSubPart(part.SubParts);
                    if (defaultSubPart != null) {
                        _RestorePart(defaultSubPart, true, true, includeParent, dicVisited);
                        if (defaultSubPart.ChildType != PartChildType.ChildRejectAll) {
                            for (int i = 0; i < part.SubParts.Count; i++) {
                                if (part.SubParts[i].ChildType == PartChildType.ChildFiexed)
                                    _RestorePart(part.SubParts[i], true, true, includeParent, dicVisited);
                            }
                        }
                    } else {
                        for (int i = 0; i < part.SubParts.Count; i++) {
                            if (part.SubParts[i].ChildType == PartChildType.ChildFiexed)
                                _RestorePart(part.SubParts[i], true, true, includeParent, dicVisited);
                        }
                    }
                }
                // at last, process the reject part.
                /*
                if (part.LstRejectParts != null) {
                    for (int i = 0; i < part.LstRejectParts.Count; i++) {
                        part.LstRejectParts[i].Visible = false;
                    }
                }*/
            }
            
            //influences parts
            if (part.ParentPart != null && part.ParentPart.LstInfluences != null) {
                for (int i = 0; i < part.ParentPart.LstInfluences.Count; i++) {
                    var item = part.ParentPart.LstInfluences[i];
                    if (item.Visible) {
                        var option = FindOption(item);
                        if (option != null) {
                            for (int optionIndex = 0; optionIndex < item.LstOwnedOptions.Count; optionIndex++) {
                                if (item.LstOwnedOptions[optionIndex] != option)
                                    item.LstOwnedOptions[optionIndex].Visible = false;
                            }
                            _RestorePart(option, true, false, includeParent, dicVisited);
                        }
                    }
                }
            }
            if (part.LstOwnedOptions!=null) {
                var option = FindOption(part);
                if (option != null) {
                    for (int optionIndex = 0; optionIndex < part.LstOwnedOptions.Count; optionIndex++) {
                        if (part.LstOwnedOptions[optionIndex] != option)
                            part.LstOwnedOptions[optionIndex].Visible = false;
                    }
                    _RestorePart(option, true, false, includeParent, dicVisited);
                }
            }
        }

        private ModelPart GetDefaultSubPart(List<ModelPart> lstParts) {
            ModelPart ret = null;
            for (int i = 0; i < lstParts.Count; i++) {
                var part = lstParts[i];
                if (part.ChildType != PartChildType.ChildFiexed) {
                    if(ret == null)
                        ret = lstParts[i];
                    if (lstParts[i].IsDefault) {
                        ret = lstParts[i];
                        break;
                    }
                } 
            }
            return ret;
        }

        public bool IsActivePart(ModelPart part) {
            var loopPart = part;
            var root = GetModel(part.ModelId);
            if (root == null)
                return false;
            while (loopPart.Visible) {
                if (loopPart.ParentPart != null) {
                    loopPart = loopPart.ParentPart;
                } else if (loopPart.ChildType == PartChildType.ChildOption && loopPart.OwnedParent != null) {
                    loopPart = loopPart.OwnedParent;
                } else {
                    break;
                }
            }
            if (loopPart.Visible && loopPart==root)
                return true;
            return false;
        }
    }
}
