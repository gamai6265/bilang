using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cloth3D.Gfx {
    public delegate void BeforeLoadSceneDelegation(string curName, string targetName, int targetSceneId);

    public delegate void AfterLoadSceneDelegation(string targetName, int targetSceneId);

    public sealed partial class GfxModule {

        private readonly ActionQueue _actionQueue = new ActionQueue();


        private readonly float c_MinTerrainHeight = 120.0f;
        private readonly Dictionary<GameObject, int> _gameObjectIds = new Dictionary<GameObject, int>();

        private readonly LinkedListDictionary<int, GameObjectInfo> _gameObjects =
            new LinkedListDictionary<int, GameObjectInfo>();

        private MyAction m_LevelLoadedCallback; //加载场景完成回调
        private ResAsyncInfo m_LoadCacheResInfo;
        private AsyncOperation m_LoadingBarAsyncOperation; //加载loading场景异步信息对象

        private AsyncOperation _loadingSceneAsyncOperation; //加载场景异步信息对象

        private readonly object m_SyncLock = new object();

        //Gfx线程执行的函数，供逻辑线程异步调用
        private void LoadSceneImpl(string name, MyAction onFinish) {
            _loadingSceneAsyncOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        }

        private void CreateGameObjectImpl(int id, string resource, SharedGameObjectInfo info) {
            if (null != info) {
                try {
                    var pos = new Vector3(info.X, info.Y, info.Z);
                    if (!info.IsFloat)
                        pos.y = SampleTerrainHeight(pos.x, pos.z);
                    var q = Quaternion.Euler(0, RadianToDegree(info.FaceDir), 0);
                    var obj = new GameObject();//TODo danie ResourceManager.Instance.NewObject(resource) as GameObject;
                    if (null != obj) {
                        if (null != obj.transform) {
                            obj.transform.position = pos;
                            obj.transform.localRotation = q;
                            obj.transform.localScale = new Vector3(info.Sx, info.Sy, info.Sz);
                        }
                        RememberGameObject(id, obj, info);
                        obj.SetActive(true);
                    }
                } catch (Exception ex) {
                    //CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
                }
            }
        }

        private void CreateGameObjectImpl(int id, string resource, float x, float y, float z, float rx, float ry,
            float rz, bool attachTerrain) {
            try {
                if (attachTerrain)
                    y = SampleTerrainHeight(x, z);
                var pos = new Vector3(x, y, z);
                var q = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry), RadianToDegree(rz));
                var obj = new GameObject();//TODO danie ResourceManager.Instance.NewObject(resource) as GameObject;
                if (null != obj) {
                    obj.transform.position = pos;
                    obj.transform.localRotation = q;
                    RememberGameObject(id, obj);
                    obj.SetActive(true);
                }
            } catch (Exception ex) {
                //CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }

        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<int> triangles, uint color,
            string mat, bool attachTerrain) {
            if (vertices.Count >= 3) {
                var uvs = new List<float>();
                var count = vertices.Count/3;
                for (var i = 0; i < count; ++i) {
                    var ix = i%4;
                    switch (ix) {
                        case 0:
                            uvs.Add(0);
                            uvs.Add(0);
                            break;
                        case 1:
                            uvs.Add(0);
                            uvs.Add(1);
                            break;
                        case 2:
                            uvs.Add(1);
                            uvs.Add(0);
                            break;
                        case 3:
                            uvs.Add(1);
                            uvs.Add(1);
                            break;
                    }
                }
                CreateGameObjectWithMeshDataImpl(id, vertices, uvs, triangles, color, mat, attachTerrain);
            }
        }

        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<float> uvs, List<int> triangles,
            uint color, string mat, bool attachTerrain) {
            var a = (byte) ((color & 0xff000000) >> 24);
            var r = (byte) ((color & 0x0ff0000) >> 16);
            var g = (byte) ((color & 0x0ff00) >> 8);
            var b = (byte) (color & 0x0ff);
            var c = new Color32(r, g, b, a);

            Material material = null;
            var shader = Shader.Find(mat);
            if (null != shader) {
                material = new Material(shader);
                material.color = c;
            } else {
                material = new Material(mat);
                material.color = c;
            }

            CreateGameObjectWithMeshDataHelper(id, vertices, uvs, triangles, material, attachTerrain);
        }

        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<int> triangles, string matRes,
            bool attachTerrain) {
            if (vertices.Count >= 3) {
                var uvs = new List<float>();
                var count = vertices.Count/3;
                for (var i = 0; i < count; ++i) {
                    var ix = i%4;
                    switch (ix) {
                        case 0:
                            uvs.Add(0);
                            uvs.Add(0);
                            break;
                        case 1:
                            uvs.Add(0);
                            uvs.Add(1);
                            break;
                        case 2:
                            uvs.Add(1);
                            uvs.Add(0);
                            break;
                        case 3:
                            uvs.Add(1);
                            uvs.Add(1);
                            break;
                    }
                }
                CreateGameObjectWithMeshDataImpl(id, vertices, uvs, triangles, matRes, attachTerrain);
            }
        }

        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<float> uvs, List<int> triangles,
            string matRes, bool attachTerrain) {
            var matObj = new Material("xx");//TODo danie ResourceManager.Instance.LoadPrefab(matRes);
            var material = matObj as Material;
            if (null != material) {
                CreateGameObjectWithMeshDataHelper(id, vertices, uvs, triangles, material, attachTerrain);
            }
        }

        private void CreateGameObjectForAttachImpl(int id, string resource) {
            try {
                var obj = new GameObject();//TODO danie ResourceManager.Instance.NewObject(resource) as GameObject;
                if (null != obj) {
                    RememberGameObject(id, obj);
                    obj.SetActive(true);
                }
            } catch (Exception ex) {
                //CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }

        private void CreateAndAttachGameObjectImpl(string resource, int parentId, string path, float recycleTime) {
            try {
                var obj = new GameObject();//TODO danie ResourceManager.Instance.NewObject(resource, recycleTime) as GameObject;
                var parent = GetGameObject(parentId);
                if (null != obj) {
                    obj.SetActive(true);
                    if (null != obj.transform && null != parent && null != parent.transform) {
                        var t = parent.transform;
                        if (!string.IsNullOrEmpty(path)) {
                            t = FindChildRecursive(parent.transform, path);
                        }
                        if (null != t) {
                            obj.transform.parent = t;
                            obj.transform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
            } catch (Exception ex) {
                //CallGfxErrorLog("CreateAndAttachGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }

        private void DestroyGameObjectImpl(int id) {
            try {
                var obj = GetGameObject(id);
                if (null != obj) {
                    ForgetGameObject(id, obj);
                    obj.SetActive(false);
                    //TODO danie
                    /*if (!ResourceManager.Instance.RecycleObject(obj)) {
                        Object.Destroy(obj);
                    }*/
                }
            } catch (Exception ex) {
                //CallGfxErrorLog(string.Format("DestroyGameObject:{0} failed:{1}\n{2}", id, ex.Message, ex.StackTrace));
            }
        }

        private void UpdateGameObjectLocalPositionImpl(int id, float x, float y, float z) {
            var obj = GetGameObject(id);
            if (null != obj) {
                obj.transform.localPosition = new Vector3(x, y, z);
            }
        }

        private void UpdateGameObjectLocalPosition2DImpl(int id, float x, float z, bool attachTerrain) {
            var obj = GetGameObject(id);
            if (null != obj) {
                float y = 0;
                if (attachTerrain)
                    y = SampleTerrainHeight(x, z);
                else
                    y = obj.transform.localPosition.y;
                obj.transform.localPosition = new Vector3(x, y, z);
            }
        }

        private void UpdateGameObjectLocalRotateImpl(int id, float rx, float ry, float rz) {
            var obj = GetGameObject(id);
            if (null != obj) {
                obj.transform.localRotation = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry),
                    RadianToDegree(rz));
            }
        }

        private void UpdateGameObjectLocalRotateYImpl(int id, float ry) {
            var obj = GetGameObject(id);
            if (null != obj) {
                var rx = obj.transform.localRotation.eulerAngles.x;
                var rz = obj.transform.localRotation.eulerAngles.z;
                obj.transform.localRotation = Quaternion.Euler(rx, RadianToDegree(ry), rz);
            }
        }

        private void UpdateGameObjectLocalScaleImpl(int id, float sx, float sy, float sz) {
            var obj = GetGameObject(id);
            if (null != obj) {
                obj.transform.localScale = new Vector3(sx, sy, sz);
            }
        }

        private void AttachGameObjectImpl(int id, int parentId, float x, float y, float z, float rx, float ry, float rz) {
            var obj = GetGameObject(id);
            var parent = GetGameObject(parentId);
            if (null != obj && null != obj.transform && null != parent && null != parent.transform) {
                obj.transform.parent = parent.transform;
                obj.transform.localPosition = new Vector3(x, y, z);
                obj.transform.localRotation = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry),
                    RadianToDegree(rz));
            }
        }

        private void AttachGameObjectImpl(int id, int parentId, string path, float x, float y, float z, float rx,
            float ry, float rz) {
            var obj = GetGameObject(id);
            var parent = GetGameObject(parentId);
            if (null != obj && null != obj.transform && null != parent && null != parent.transform) {
                var t = FindChildRecursive(parent.transform, path);
                if (null != t) {
                    obj.transform.parent = t;
                    obj.transform.localPosition = new Vector3(x, y, z);
                    obj.transform.localRotation = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry),
                        RadianToDegree(rz));
                }
            }
        }

        private void DetachGameObjectImpl(int id) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.transform) {
                obj.transform.parent = null;
            }
        }

        private void SetGameObjectVisibleImpl(int id, bool visible) {
            var obj = GetGameObject(id);
            if (null != obj) {
                var renderers = obj.GetComponentsInChildren<Renderer>();
                for (var i = 0; i < renderers.Length; ++i) {
                    renderers[i].enabled = visible;
                }
            }
        }

        //TODO danie 动画改用新的
        private void PlayAnimationImpl(int id, bool isStopAll) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    obj.GetComponent<Animation>().Play(isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                } catch {
                }
            }
        }

        private void PlayAnimationImpl(int id, string animationName, bool isStopAll) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    if (null != obj.GetComponent<Animation>()[animationName]) {
                        obj.GetComponent<Animation>()
                            .Play(animationName, isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} PlayerAnimation {1} clipcount {2}", id, animationName, obj.animation.GetClipCount());
                    }
                } catch {
                }
            }
        }

        private void StopAnimationImpl(int id, string animationName) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    if (null != obj.GetComponent<Animation>()[animationName]) {
                        obj.GetComponent<Animation>().Stop(animationName);
                        //CallLogicLog("Obj {0} StopAnimation {1} clipcount {2}", id, animationName, obj.animation.GetClipCount());
                    }
                } catch {
                }
            }
        }

        private void StopAnimationImpl(int id) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    obj.GetComponent<Animation>().Stop();
                } catch {
                }
            }
        }

        private void BlendAnimationImpl(int id, string animationName, float weight, float fadeLength) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        obj.GetComponent<Animation>().Blend(animationName, weight, fadeLength);
                        //CallLogicLog("Obj {0} BlendAnimation {1} clipcount {2}", id, animationName, obj.animation.GetClipCount());
                    }
                } catch {
                }
            }
        }

        private void CrossFadeAnimationImpl(int id, string animationName, float fadeLength, bool isStopAll) {
            var obj = GetGameObject(id);
            var obj_info = GetSharedGameObjectInfo(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    if (null != obj.GetComponent<Animation>()[animationName] && obj_info != null &&
                        !obj_info.IsGfxAnimation) {
                        obj.GetComponent<Animation>()
                            .CrossFade(animationName, fadeLength, isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} CrossFadeAnimation {1} clipcount {2}", id, animationName, obj.animation.GetClipCount());
                    } else {
                        if (null == obj.GetComponent<Animation>()[animationName]) {
                            //CallLogicErrorLog("Obj {0} CrossFadeAnimation {1} AnimationState is null, clipcount {2}", id, animationName, obj.GetComponent<Animation>().GetClipCount());
                        }
                        if (null == obj_info) {
                            //CallLogicErrorLog("Obj {0} CrossFadeAnimation {1} obj_info is null, obj name {2}", id, animationName, obj.name);
                        }
                    }
                } catch {
                }
            }
        }

        private void PlayQueuedAnimationImpl(int id, string animationName, bool isPlayNow, bool isStopAll) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    if (null != obj.GetComponent<Animation>()[animationName]) {
                        obj.GetComponent<Animation>()
                            .PlayQueued(animationName, isPlayNow ? QueueMode.PlayNow : QueueMode.CompleteOthers,
                                isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} PlayQueuedAnimation {1} clipcount {2}", id, animationName, obj.animation.GetClipCount());
                    }
                } catch {
                }
            }
        }

        private void CrossFadeQueuedAnimationImpl(int id, string animationName, float fadeLength, bool isPlayNow,
            bool isStopAll) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    if (null != obj.GetComponent<Animation>()[animationName]) {
                        obj.GetComponent<Animation>()
                            .CrossFadeQueued(animationName, fadeLength,
                                isPlayNow ? QueueMode.PlayNow : QueueMode.CompleteOthers,
                                isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} CrossFadeQueuedAnimation {1} clipcount {2}", id, animationName, obj.animation.GetClipCount());
                    }
                } catch {
                }
            }
        }

        private void RewindAnimationImpl(int id, string animationName) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    if (null != obj.GetComponent<Animation>()[animationName]) {
                        obj.GetComponent<Animation>().Rewind(animationName);
                    }
                } catch {
                }
            }
        }

        private void RewindAnimationImpl(int id) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    obj.GetComponent<Animation>().Rewind();
                } catch {
                }
            }
        }

        private void SetAnimationSpeedImpl(int id, string animationName, float speed) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        state.speed = speed;
                    }
                } catch {
                }
            }
        }

        private void SetAnimationSpeedByTimeImpl(int id, string animationName, float time) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        state.speed = state.length/state.time;
                    }
                } catch {
                }
            }
        }

        private void SetAnimationWeightImpl(int id, string animationName, float weight) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        state.weight = weight;
                    }
                } catch {
                }
            }
        }

        private void SetAnimationLayerImpl(int id, string animationName, int layer) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        state.layer = layer;
                    }
                } catch {
                }
            }
        }

        private void SetAnimationBlendModeImpl(int id, string animationName, int blendMode) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>()) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        state.blendMode = (AnimationBlendMode) blendMode;
                    }
                } catch {
                }
            }
        }

        private void AddMixingTransformAnimationImpl(int id, string animationName, string path, bool recursive) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>() && null != obj.transform) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        var t = obj.transform.Find(path);
                        if (null != t) {
                            state.AddMixingTransform(t, recursive);
                        }
                    }
                } catch {
                }
            }
        }

        private void RemoveMixingTransformAnimationImpl(int id, string animationName, string path) {
            var obj = GetGameObject(id);
            if (null != obj && null != obj.GetComponent<Animation>() && null != obj.transform) {
                try {
                    var state = obj.GetComponent<Animation>()[animationName];
                    if (null != state) {
                        var t = obj.transform.Find(path);
                        if (null != t) {
                            state.RemoveMixingTransform(t);
                        }
                    }
                } catch {
                }
            }
        }

        private AudioSource GetAudioSource(GameObject obj, string source_obj_name) {
            if (obj == null) {
                return null;
            }
            var audiosources = obj.GetComponentsInChildren<AudioSource>();
            foreach (var audio in audiosources) {
                if (audio.gameObject.name.Equals(source_obj_name)) {
                    return audio;
                }
            }
            return null;
        }

        private void PlaySoundImpl(int id, string audiosource, float pitch) {
            var obj = GetGameObject(id);
            var target_audio_source = GetAudioSource(obj, audiosource);
            if (target_audio_source == null) {
                //CallLogicErrorLog("id={0} obj can't find audiosource {1}! can't play sound!", id, audiosource);
                return;
            }
            target_audio_source.pitch = pitch;
            target_audio_source.Play();
        }

        private void SetAudioSourcePitchImpl(int id, string audiosource, float pitch) {
            var obj = GetGameObject(id);
            var target_audio_source = GetAudioSource(obj, audiosource);
            if (target_audio_source == null) {
                //CallLogicErrorLog("id={0} obj can't find audiosource {1}! can't set sound pitch!", id, audiosource);
                return;
            }
            target_audio_source.pitch = pitch;
        }

        private void StopSoundImpl(int id, string audiosource) {
            var obj = GetGameObject(id);
            var target_audio_source = GetAudioSource(obj, audiosource);
            if (target_audio_source == null) {
                //CallLogicErrorLog("id={0} obj can't find audiosource {1}! can't stop sound!", id, audiosource);
                return;
            }
            target_audio_source.Stop();
        }

        private void SetShaderImpl(int id, string shaderPath) {
            var obj = GetGameObject(id);
            if (null == obj) {
                return;
            }
            var shader = Shader.Find(shaderPath);
            if (null == shader) {
                //CallLogicErrorLog("id={0} obj can't find shader {1}!", id, shaderPath);
                return;
            }
            var renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (var i = 0; i < renderers.Length; ++i) {
                if (renderers[i].material.shader != shader) {
                    renderers[i].material.shader = shader;
                }
            }
        }

        private void SetBlockedShaderImpl(int id, uint rimColor, float rimPower, float cutValue) {
            var objInfo = GetGameObjectInfo(id);
            if (null == objInfo || null == objInfo.ObjectInstance || null == objInfo.SharedInfo) {
                return;
            }
            var needChange = false;
            var skinnedRenderers = objInfo.ObjectInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedRenderers) {
                foreach (var mat in renderer.materials) {
                    var name = mat.shader.name;
                    if (0 != name.CompareTo("DFM/Blocked") && 0 != name.CompareTo("DFM/NotBlocked")) {
                        needChange = true;
                    }
                }
            }
            var meshRenderers = objInfo.ObjectInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in meshRenderers) {
                foreach (var mat in renderer.materials) {
                    var name = mat.shader.name;
                    if (0 != name.CompareTo("DFM/Blocked") && 0 != name.CompareTo("DFM/NotBlocked")) {
                        needChange = true;
                    }
                }
            }
            if (needChange) {
                var rb = (byte) ((rimColor & 0xFF000000) >> 24);
                var gb = (byte) ((rimColor & 0x00FF0000) >> 16);
                var bb = (byte) ((rimColor & 0x0000FF00) >> 8);
                var ab = (byte) (rimColor & 0x000000FF);
                var r = rb/255.0f;
                var g = gb/255.0f;
                var b = bb/255.0f;
                var a = ab/255.0f;
                var c = new Color(r, g, b, a);

                var blocked = Shader.Find("DFM/Blocked");
                var notBlocked = Shader.Find("DFM/NotBlocked");
                if (null == blocked) {
                    //CallLogicLog("id={0} obj can't find shader DFM/Blocked !", id);
                    return;
                }
                if (null == notBlocked) {
                    //CallLogicLog("id={0} obj can't find shader DFM/NotBlocked !", id);
                    return;
                }
                foreach (var renderer in skinnedRenderers) {
                    objInfo.SharedInfo.m_SkinedMaterialChanged = true;
                    var texture = renderer.material.mainTexture;

                    var blockedMat = new Material(blocked);
                    var notBlockedMat = new Material(notBlocked);
                    Material[] mats = {
                        notBlockedMat,
                        blockedMat
                    };
                    blockedMat.SetColor("_RimColor", c);
                    blockedMat.SetFloat("_RimPower", rimPower);
                    blockedMat.SetFloat("_CutValue", cutValue);
                    notBlockedMat.SetTexture("_MainTex", texture);

                    renderer.materials = mats;
                }
                foreach (var renderer in meshRenderers) {
                    objInfo.SharedInfo.m_MeshMaterialChanged = true;
                    var texture = renderer.material.mainTexture;

                    var blockedMat = new Material(blocked);
                    var notBlockedMat = new Material(notBlocked);
                    Material[] mats = {
                        notBlockedMat,
                        blockedMat
                    };
                    blockedMat.SetColor("_RimColor", c);
                    blockedMat.SetFloat("_RimPower", rimPower);
                    blockedMat.SetFloat("_CutValue", cutValue);
                    notBlockedMat.SetTexture("_MainTex", texture);

                    renderer.materials = mats;
                }
            }
        }

        private void RestoreMaterialImpl(int id) {
            var objInfo = GetGameObjectInfo(id);
            if (null == objInfo) {
                return;
            }
            var obj = objInfo.ObjectInstance;
            var info = objInfo.SharedInfo;
            if (null != obj && null != info) {
                if (info.m_SkinedMaterialChanged) {
                    var renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    var ix = 0;
                    var ct = info.m_SkinedOriginalMaterials.Count;
                    foreach (var renderer in renderers) {
                        if (ix < ct) {
                            renderer.materials = info.m_SkinedOriginalMaterials[ix] as Material[];
                            ++ix;
                        }
                    }
                    info.m_SkinedMaterialChanged = false;
                }
                if (info.m_MeshMaterialChanged) {
                    var renderers = obj.GetComponentsInChildren<MeshRenderer>();
                    var ix = 0;
                    var ct = info.m_MeshOriginalMaterials.Count;
                    foreach (var renderer in renderers) {
                        if (ix < ct) {
                            renderer.materials = info.m_MeshOriginalMaterials[ix] as Material[];
                            ++ix;
                        }
                    }
                    info.m_MeshMaterialChanged = false;
                }
            }
        }

        private void SetTimeScaleImpl(float scale) {
            Time.timeScale = scale;
        }

        //Gfx线程执行的函数，对逻辑线程的异步调用由这里发起
        internal float SampleTerrainHeight(float x, float z) {
            var y = c_MinTerrainHeight;
            if (null != Terrain.activeTerrain) {
                y = Terrain.activeTerrain.SampleHeight(new Vector3(x, c_MinTerrainHeight, z));
            } else {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, c_MinTerrainHeight*2, z), Vector3.down, out hit, c_MinTerrainHeight*2,
                    1 << LayerMask.NameToLayer("Terrains"))) {
                    y = hit.point.y;
                }
            }
            return y;
        }

        internal GameObject GetGameObject(int id) {
            GameObject ret = null;
            if (_gameObjects.Contains(id))
                ret = _gameObjects[id].ObjectInstance;
            return ret;
        }

        internal SharedGameObjectInfo GetSharedGameObjectInfo(int id) {
            SharedGameObjectInfo ret = null;
            if (_gameObjects.Contains(id))
                ret = _gameObjects[id].SharedInfo;
            return ret;
        }

        internal SharedGameObjectInfo GetSharedGameObjectInfo(GameObject obj) {
            var id = GetGameObjectId(obj);
            return GetSharedGameObjectInfo(id);
        }

        internal bool ExistGameObject(GameObject obj) {
            var id = GetGameObjectId(obj);
            return id > 0;
        }

        internal float RadianToDegree(float dir) {
            return (float) (dir*180/Math.PI);
        }

        internal Transform FindChildRecursive(Transform parent, string bonePath) {
            var t = parent.Find(bonePath);
            if (null != t) {
                return t;
            }
            var ct = parent.childCount;
            for (var i = 0; i < ct; ++i) {
                t = FindChildRecursive(parent.GetChild(i), bonePath);
                if (null != t) {
                    return t;
                }
            }
            return null;
        }

        internal void VisitGameObject(MyAction<GameObject, SharedGameObjectInfo> visitor) {
            if (Monitor.TryEnter(m_SyncLock)) {
                try {
                    for (var node = _gameObjects.FirstValue; null != node; node = node.Next) {
                        var info = node.Value;
                        if (null != info && null != info.ObjectInstance) {
                            visitor(info.ObjectInstance, info.SharedInfo);
                        }
                    }
                } finally {
                    Monitor.Exit(m_SyncLock);
                }
            }
        }

        private void HandleSceneProgress() {
                if (_loadingSceneAsyncOperation!=null && _loadingSceneAsyncOperation.isDone) {
                    Resources.UnloadUnusedAssets();
            }
        }

        private GameObjectInfo GetGameObjectInfo(int id) {
            GameObjectInfo ret = null;
            if (_gameObjects.Contains(id))
                ret = _gameObjects[id];
            return ret;
        }

        private int GetGameObjectId(GameObject obj) {
            var ret = 0;
            if (_gameObjectIds.ContainsKey(obj)) {
                ret = _gameObjectIds[obj];
            }
            return ret;
        }

        private void RememberGameObject(int id, GameObject obj) {
            RememberGameObject(id, obj, null);
        }

        private void RememberGameObject(int id, GameObject obj, SharedGameObjectInfo info) {
            if (_gameObjects.Contains(id)) {
                var oldObj = _gameObjects[id].ObjectInstance;
                oldObj.SetActive(false);
                _gameObjectIds.Remove(oldObj);
                Object.Destroy(oldObj);
                _gameObjects[id] = new GameObjectInfo(obj, info);
            } else {
                _gameObjects.AddLast(id, new GameObjectInfo(obj, info));
            }
            if (null != info) {
                if (!info.m_SkinedMaterialChanged) {
                    var renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var renderer in renderers) {
                        info.m_SkinedOriginalMaterials.Add(renderer.materials);
                    }
                }
                if (!info.m_MeshMaterialChanged) {
                    var renderers = obj.GetComponentsInChildren<MeshRenderer>();
                    foreach (var renderer in renderers) {
                        info.m_MeshOriginalMaterials.Add(renderer.materials);
                    }
                }
            }
            _gameObjectIds.Add(obj, id);
        }

        private void ForgetGameObject(int id, GameObject obj) {
            var info = GetSharedGameObjectInfo(id);
            if (null != info) {
                RestoreMaterialImpl(id);
                info.m_SkinedOriginalMaterials.Clear();
                info.m_MeshOriginalMaterials.Clear();
            }
            _gameObjects.Remove(id);
            _gameObjectIds.Remove(obj);
        }

        private void CreateGameObjectWithMeshDataHelper(int id, List<float> vertices, List<float> uvs,
            List<int> triangles, Material mat, bool attachTerrain) {
            var obj = new GameObject();
            var meshFilter = obj.AddComponent<MeshFilter>();
            var renderer = obj.AddComponent<MeshRenderer>();
            var mesh = new Mesh();

            var _vertices = new Vector3[vertices.Count/3];
            for (var i = 0; i < _vertices.Length; ++i) {
                var x = vertices[i*3];
                var y = vertices[i*3 + 1];
                var z = vertices[i*3 + 2];
                if (attachTerrain)
                    y = SampleTerrainHeight(x, z) + 0.01f;
                _vertices[i] = new Vector3(x, y, z);
            }
            var _uvs = new Vector2[uvs.Count/2];
            for (var i = 0; i < _uvs.Length; ++i) {
                var u = uvs[i*2];
                var v = uvs[i*2 + 1];
                _uvs[i] = new Vector2(u, v);
            }

            mesh.vertices = _vertices;
            mesh.uv = _uvs;
            mesh.triangles = triangles.ToArray();
            mesh.Optimize();

            meshFilter.mesh = mesh;
            renderer.material = mat;

            RememberGameObject(id, obj);
            obj.SetActive(true);
        }

        private class GameObjectInfo {
            public readonly GameObject ObjectInstance;
            public readonly SharedGameObjectInfo SharedInfo;

            public GameObjectInfo(GameObject o, SharedGameObjectInfo i) {
                ObjectInstance = o;
                SharedInfo = i;
            }
        }

        public GfxModule() {
        }
    }
}