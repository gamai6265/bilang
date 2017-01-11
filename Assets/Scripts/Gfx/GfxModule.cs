using System.Collections.Generic;
using Cloth3D.Interfaces;

namespace Cloth3D.Gfx {
    public partial class GfxModule : IComponent {
        public void Tick() {
            /*long curTime = TimeUtility.GetLocalMilliseconds();
            if (m_LastLogTime + 10000 < curTime) {
                m_LastLogTime = curTime;

                if (_actionQueue.CurActionNum > 10) {
                    //CallGfxLog("GfxModule.Tick actionNum:{0}", _actionQueue.CurActionNum);
                }

                _actionQueue.DebugPoolCount((string msg) => {
                    //CallGfxLog("GfxActionQueue {0}", msg);
                });
            }*/
            //HandleSync();
            //HandleInput();
            HandleSceneProgress();
            _actionQueue.HandleActions(4096);
        }

        public bool Init() {
            return true;
        }

        public IActionQueue Dispatcher { get { return _actionQueue; } }

        public string Name { get; set; }

        public void LoadScene(string sceneName, int chapter, int sceneId, HashSet<int> limitList, MyAction onFinish) {
            //QueueGfxAction(s_Instance.LoadSceneImpl, sceneName, chapter, sceneId, limitList, onFinish);
        }

        public void CreateGameObject(int id, string resource, SharedGameObjectInfo info) {
            //QueueGfxAction(s_Instance.CreateGameObjectImpl, id, resource, info);
        }

        //public void CreateGameObject(int id, string resource, float x, float y, float z, float rx, float ry, float rz, bool attachTerrain) {
        //    QueueGfxAction(s_Instance.CreateGameObjectImpl, id, resource, x, y, z, rx, ry, rz, attachTerrain);
        //}
        public void CreateGameObjectWithMeshData(int id, List<float> vertices, List<int> triangles, uint color,
            string matRes, bool attachTerrain) {
            ///QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, triangles, color, matRes, attachTerrain);
        }

        public void CreateGameObjectWithMeshData(int id, List<float> vertices, List<float> uvs, List<int> triangles,
            uint color, string mat, bool attachTerrain) {
            //QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, uvs, triangles, color, mat, attachTerrain);
        }

        public void CreateGameObjectWithMeshData(int id, List<float> vertices, List<int> triangles, string matRes,
            bool attachTerrain) {
            //QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, triangles, matRes, attachTerrain);
        }

        public void CreateGameObjectWithMeshData(int id, List<float> vertices, List<float> uvs, List<int> triangles,
            string matRes, bool attachTerrain) {
            //QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, uvs, triangles, matRes, attachTerrain);
        }

        public void CreateAndAttachGameObject(string resource, int parentId, string path, float recycleTime = -1) {
            //QueueGfxAction(s_Instance.CreateAndAttachGameObjectImpl, resource, parentId, path, recycleTime);
        }

        public void CreateGameObjectForAttach(int id, string resource) {
            //QueueGfxAction(s_Instance.CreateGameObjectForAttachImpl, id, resource);
        }

        public void DestroyGameObject(int id) {
            //QueueGfxAction(s_Instance.DestroyGameObjectImpl, id);
        }

        //public static object SyncLock {
        //    get {
        //return s_Instance.m_SyncLock;
        //    }
        //}
        public void UpdateGameObjectLocalPosition(int id, float x, float y, float z) {
            //QueueGfxAction(s_Instance.UpdateGameObjectLocalPositionImpl, id, x, y, z);
        }

        public void UpdateGameObjectLocalPosition2D(int id, float x, float z) {
            //UpdateGameObjectLocalPosition2D(id, x, z, true);
        }

        public void UpdateGameObjectLocalPosition2D(int id, float x, float z, bool attachTerrain) {
            //QueueGfxAction(s_Instance.UpdateGameObjectLocalPosition2DImpl, id, x, z, attachTerrain);
        }

        public void UpdateGameObjectLocalRotate(int id, float rx, float ry, float rz) {
            //QueueGfxAction(s_Instance.UpdateGameObjectLocalRotateImpl, id, rx, ry, rz);
        }

        public void UpdateGameObjectLocalRotateY(int id, float ry) {
            //QueueGfxAction(s_Instance.UpdateGameObjectLocalRotateYImpl, id, ry);
        }

        public void UpdateGameObjectLocalScale(int id, float sx, float sy, float sz) {
            //QueueGfxAction(s_Instance.UpdateGameObjectLocalScaleImpl, id, sx, sy, sz);
        }

        public void AttachGameObject(int id, int parentId) {
            //AttachGameObject(id, parentId, 0, 0, 0, 0, 0, 0);
        }

        public void AttachGameObject(int id, int parentId, float x, float y, float z, float rx, float ry, float rz) {
            //QueueGfxAction(s_Instance.AttachGameObjectImpl, id, parentId, x, y, z, rx, ry, rz);
        }

        public void AttachGameObject(int id, int parentId, string path) {
            //AttachGameObject(id, parentId, path, 0, 0, 0, 0, 0, 0);
        }

        public void AttachGameObject(int id, int parentId, string path, float x, float y, float z, float rx, float ry,
            float rz) {
            //QueueGfxAction(s_Instance.AttachGameObjectImpl, id, parentId, path, x, y, z, rx, ry, rz);
        }

        public void DetachGameObject(int id) {
            //QueueGfxAction(s_Instance.DetachGameObjectImpl, id);
        }

        public void SetGameObjectVisible(int id, bool visible) {
            //QueueGfxAction(s_Instance.SetGameObjectVisibleImpl, id, visible);
        }

        public void PlayAnimation(int id) {
            //PlayAnimation(id, false);
        }

        public void PlayAnimation(int id, bool isStopAll) {
            //QueueGfxAction(s_Instance.PlayAnimationImpl, id, isStopAll);
        }

        public void PlayAnimation(int id, string animationName) {
            //PlayAnimation(id, animationName, false);
        }

        public void PlayAnimation(int id, string animationName, bool isStopAll) {
            //QueueGfxAction(s_Instance.PlayAnimationImpl, id, animationName, isStopAll);
        }

        public void StopAnimation(int id, string animationName) {
            //QueueGfxAction(s_Instance.StopAnimationImpl, id, animationName);
        }

        public void StopAnimation(int id) {
            //QueueGfxAction(s_Instance.StopAnimationImpl, id);
        }

        public void BlendAnimation(int id, string animationName) {
            //BlendAnimation(id, animationName, 1, 0.3f);
        }

        public void BlendAnimation(int id, string animationName, float weight) {
            //BlendAnimation(id, animationName, weight, 0.3f);
        }

        public void BlendAnimation(int id, string animationName, float weight, float fadeLength) {
            //QueueGfxAction(s_Instance.BlendAnimationImpl, id, animationName, weight, fadeLength);
        }

        public void CrossFadeAnimation(int id, string animationName) {
            //CrossFadeAnimation(id, animationName, 0.3f, false);
        }

        public void CrossFadeAnimation(int id, string animationName, float fadeLength) {
            //CrossFadeAnimation(id, animationName, fadeLength, false);
        }

        public void CrossFadeAnimation(int id, string animationName, float fadeLength, bool isStopAll) {
            //QueueGfxAction(s_Instance.CrossFadeAnimationImpl, id, animationName, fadeLength, isStopAll);
        }

        public void PlayQueuedAnimation(int id, string animationName) {
            //PlayQueuedAnimation(id, animationName, false, false);
        }

        public void PlayQueuedAnimation(int id, string animationName, bool isPlayNow) {
            //PlayQueuedAnimation(id, animationName, isPlayNow, false);
        }

        public void PlayQueuedAnimation(int id, string animationName, bool isPlayNow, bool isStopAll) {
            //QueueGfxAction(s_Instance.PlayQueuedAnimationImpl, id, animationName, isPlayNow, isStopAll);
        }

        public void CrossFadeQueuedAnimation(int id, string animationName) {
            //CrossFadeQueuedAnimation(id, animationName, 0.3f, false, false);
        }

        public void CrossFadeQueuedAnimation(int id, string animationName, float fadeLength) {
            //CrossFadeQueuedAnimation(id, animationName, fadeLength, false, false);
        }

        public void CrossFadeQueuedAnimation(int id, string animationName, float fadeLength, bool isPlayNow) {
            //CrossFadeQueuedAnimation(id, animationName, fadeLength, isPlayNow, false);
        }

        public void CrossFadeQueuedAnimation(int id, string animationName, float fadeLength, bool isPlayNow,
            bool isStopAll) {
            //QueueGfxAction(s_Instance.CrossFadeQueuedAnimationImpl, id, animationName, fadeLength, isPlayNow, isStopAll);
        }

        public void RewindAnimation(int id, string animationName) {
            //QueueGfxAction(s_Instance.RewindAnimationImpl, id, animationName);
        }

        public void RewindAnimation(int id) {
            //QueueGfxAction(s_Instance.RewindAnimationImpl, id);
        }

        public void SetAnimationSpeed(int id, string animationName, float speed) {
            //QueueGfxAction(s_Instance.SetAnimationSpeedImpl, id, animationName, speed);
        }

        public void SetAnimationSpeedByTime(int id, string animationName, float time) {
            //QueueGfxAction(s_Instance.SetAnimationSpeedByTimeImpl, id, animationName, time);
        }

        public void SetAnimationWeight(int id, string animationName, float weight) {
            //QueueGfxAction(s_Instance.SetAnimationWeightImpl, id, animationName, weight);
        }

        public void SetAnimationLayer(int id, string animationName, int layer) {
            //QueueGfxAction(s_Instance.SetAnimationLayerImpl, id, animationName, layer);
        }

        public void SetAnimationBlendMode(int id, string animationName, int blendMode) {
            //QueueGfxAction(s_Instance.SetAnimationBlendModeImpl, id, animationName, blendMode);
        }

        public void AddMixingTransformAnimation(int id, string animationName, string path) {
            //AddMixingTransformAnimation(id, animationName, path, true);
        }

        public void AddMixingTransformAnimation(int id, string animationName, string path, bool recursive) {
            //QueueGfxAction(s_Instance.AddMixingTransformAnimationImpl, id, animationName, path, recursive);
        }

        public void RemoveMixingTransformAnimation(int id, string animationName, string path) {
            //QueueGfxAction(s_Instance.RemoveMixingTransformAnimationImpl, id, animationName, path);
        }

        // sound
        public void PlaySound(int id, string audio_source_obj_name, float pitch) {
            //QueueGfxAction(s_Instance.PlaySoundImpl, id, audio_source_obj_name, pitch);
        }

        public void SetSoundPitch(int id, string audio_source_obj_name, float pitch) {
            //QueueGfxAction(s_Instance.SetAudioSourcePitchImpl, id, audio_source_obj_name, pitch);
        }

        public void StopSound(int id, string audio_source_obj_name) {
            //QueueGfxAction(s_Instance.StopSoundImpl, id, audio_source_obj_name);
        }

        public void SetShader(int id, string shaderPath) {
            //QueueGfxAction(s_Instance.SetShaderImpl, id, shaderPath);
        }

        public void SetBlockedShader(int id, uint rimColor, float rimPower, float cutValue) {
            //QueueGfxAction(s_Instance.SetBlockedShaderImpl, id, rimColor, rimPower, cutValue);
        }

        public void RestoreMaterial(int id) {
            //QueueGfxAction(s_Instance.RestoreMaterialImpl, id);
        }

        public void SetTimeScale(float scale) {
            //QueueGfxAction(s_Instance.SetTimeScaleImpl, scale);
        }
    }
}