﻿using System;
using System.Collections.Generic;
using System.Text;
using Cloth3D;
using UnityEngine;

//TODO danie namespace
public class HardWareQuality {
    // Fields
    private static int mCpuQuality = 1;
    //private static int mGpuQuality = 1;
    private static QualityInfo mHighQuality = new QualityInfo(); //高端机器信息
    private static Vector2 mHighResolution = new Vector2(1536f, 1152f);
    public static bool mIsFirst;
    public static bool mIsFirstLaunch;
    private static QualityInfo mLowQuality = new QualityInfo(); //低端机器信息
    private static Vector2 mLowResolution = new Vector2(960f, 540f);
    private static QualityInfo mMediumQuality = new QualityInfo(); //中端机器信息
    private static Vector2 mMediumResolution = new Vector2(1280f, 720f);
    private static int mMemoryQuality = 1;
    private static int mQualityLevel; //由cpu和memory最差者决定
    public static bool mSplitSceneLoaded = false;
    private static QualityInfo mVeryLowQuality = new QualityInfo();
    private static Vector2 mVeryLowResolution = new Vector2(800f, 480f);

    // Methods
    private HardWareQuality() {
        Clear();
    }

    public static void Clear() {
        mIsFirst = true;
    }

    public static void ComputeHardwarePerformance() {
        InitQalityInfo();
        if (SystemInfo.processorCount <= 1) {
            mCpuQuality = 3;
        } else if (SystemInfo.processorCount <= 2) {
            mCpuQuality = 2;
        } else if (SystemInfo.processorCount <= 4) {
            mCpuQuality = 1;
        } else {
            mCpuQuality = 0;
        }
        if (SystemInfo.systemMemorySize < 0) {
            mMemoryQuality = 3;
        } else if (SystemInfo.systemMemorySize < 700) {
            mMemoryQuality = 2;
        } else if (SystemInfo.systemMemorySize < 0x5dc) {
            mMemoryQuality = 1;
        } else {
            mMemoryQuality = 0;
        }
        SetQualityLevel(Math.Max(mCpuQuality, mMemoryQuality));
        QualitySettings.SetQualityLevel(GetQualityLevel());
    }

    public static QualityInfo GetQualityInfo() {
        if (mQualityLevel == (int)QualityLevelType.High) {
            return mHighQuality;
        }
        if (mQualityLevel == (int)QualityLevelType.Medium) {
            return mMediumQuality;
        }
        if (mQualityLevel == (int)QualityLevelType.Low) {
            return mLowQuality;
        }
        return mVeryLowQuality;
    }

    public static int GetQualityLevel() {
        return mQualityLevel;
    }

    public static void InitQalityInfo() {
        mHighQuality.mTerrainPixelError = 90;
        mHighQuality.mTerrainDetailObjectDraw = true;
        mHighQuality.mTerrainDetailDensity = 1f;
        mHighQuality.mTerrainDetailDistance = 0x23;
        mHighQuality.mTerrainTreeDistance = 0x23;
        mHighQuality.mCameraClipFar = 35f;
        mHighQuality.mTerrainLayerCullDistance = 70f;
        mHighQuality.mObjectLayerClipDistance = 50f;
        mHighQuality.mMonsterLayerCullDistance = 50f;
        mHighQuality.mNPCLayerCullDistance = 50f;
        mHighQuality.mStaticEffectLayerCullDistance = 50f;
        mHighQuality.mVegetationLayerCullDistance = 50f;
        mHighQuality.mWaterLayerCullDistance = 50f;
        mHighQuality.mAnimationLayerCullDistance = 50f;
        mHighQuality.mParticleBalanceCount = 0x100;
        mHighQuality.mParticleSceneShow = true;
        mHighQuality.mParticleSyncShow = true;
        mHighQuality.mShadowQuality = 1;
        mHighQuality.mAsyncCoroutineMax = 15;

        mMediumQuality.mTerrainPixelError = 100;
        mMediumQuality.mTerrainDetailObjectDraw = true;
        mMediumQuality.mTerrainDetailDensity = 0.6f;
        mMediumQuality.mTerrainDetailDistance = 30;
        mMediumQuality.mTerrainTreeDistance = 30;
        mMediumQuality.mCameraClipFar = 35f;
        mMediumQuality.mTerrainLayerCullDistance = 60f;
        mMediumQuality.mObjectLayerClipDistance = 40f;
        mMediumQuality.mMonsterLayerCullDistance = 40f;
        mMediumQuality.mNPCLayerCullDistance = 40f;
        mMediumQuality.mStaticEffectLayerCullDistance = 40f;
        mMediumQuality.mVegetationLayerCullDistance = 40f;
        mMediumQuality.mWaterLayerCullDistance = 40f;
        mMediumQuality.mAnimationLayerCullDistance = 40f;
        mMediumQuality.mParticleBalanceCount = 0x80;
        mMediumQuality.mParticleSceneShow = true;
        mMediumQuality.mParticleSyncShow = true;
        mMediumQuality.mShadowQuality = 1;
        mMediumQuality.mAsyncCoroutineMax = 12;

        mLowQuality.mTerrainPixelError = 110;
        mLowQuality.mTerrainDetailObjectDraw = false;
        mLowQuality.mTerrainDetailDensity = 0f;
        mLowQuality.mTerrainDetailDistance = 0;
        mLowQuality.mTerrainTreeDistance = 0;
        mLowQuality.mCameraClipFar = 35f;
        mLowQuality.mTerrainLayerCullDistance = 60f;
        mLowQuality.mObjectLayerClipDistance = 35f;
        mLowQuality.mMonsterLayerCullDistance = 35f;
        mLowQuality.mNPCLayerCullDistance = 35f;
        mLowQuality.mStaticEffectLayerCullDistance = 35f;
        mLowQuality.mVegetationLayerCullDistance = 35f;
        mLowQuality.mWaterLayerCullDistance = 30f;
        mLowQuality.mAnimationLayerCullDistance = 30f;
        mLowQuality.mParticleBalanceCount = 0x40;
        mLowQuality.mParticleSceneShow = true;
        mLowQuality.mParticleSyncShow = true;
        mLowQuality.mShadowQuality = 1;
        mLowQuality.mAsyncCoroutineMax = 8;

        mVeryLowQuality.mTerrainPixelError = 0x69;
        mVeryLowQuality.mTerrainDetailObjectDraw = false;
        mVeryLowQuality.mTerrainDetailDensity = 0f;
        mVeryLowQuality.mTerrainDetailDistance = 0;
        mVeryLowQuality.mTerrainTreeDistance = 0;
        mVeryLowQuality.mCameraClipFar = 32f;
        mVeryLowQuality.mTerrainLayerCullDistance = 50f;
        mVeryLowQuality.mObjectLayerClipDistance = 25f;
        mVeryLowQuality.mMonsterLayerCullDistance = 25f;
        mVeryLowQuality.mNPCLayerCullDistance = 25f;
        mVeryLowQuality.mStaticEffectLayerCullDistance = 25f;
        mVeryLowQuality.mVegetationLayerCullDistance = 25f;
        mVeryLowQuality.mWaterLayerCullDistance = 20f;
        mVeryLowQuality.mAnimationLayerCullDistance = 25f;
        mVeryLowQuality.mParticleBalanceCount = 0x30;
        mVeryLowQuality.mParticleSceneShow = false;
        mVeryLowQuality.mParticleSyncShow = false;
        mVeryLowQuality.mShadowQuality = 0;
        mVeryLowQuality.mAsyncCoroutineMax = 5;

        string str = SystemInfo.processorType.ToLower();
        if (SystemInfo.deviceModel.ToLower().Contains("sm-t211")) {
            mSplitSceneLoaded = true;
        }
    }

    public static bool IsCPUVeryLow() {
        return (mCpuQuality == (int)QualityLevelType.VeryLow);
    }

    public static bool IsLowQuality() {
        return (mQualityLevel == (int)QualityLevelType.Low);
    }

    public static bool IsVeryLowQuality() {
        return (mQualityLevel == (int)QualityLevelType.VeryLow);
    }

    public static void SetCameraQuality() {
        QualityInfo qualityInfo = GetQualityInfo();
        Camera main = Camera.main;
        if (main != null) {
            main.farClipPlane = qualityInfo.mCameraClipFar;
            //int index = LayerMask.NameToLayer("Terrain");
            //int num2 = LayerMask.NameToLayer("Effect");
            //int num3 = LayerMask.NameToLayer("Monster");
            //int num4 = LayerMask.NameToLayer("NPC");
            //int num5 = LayerMask.NameToLayer("Vegetation");
            //int num6 = LayerMask.NameToLayer("Water");
            //if (index >= 0) {
            //  main.layerCullDistances[index] = qualityInfo.mTerrainLayerCullDistance;
            //}
            //if (num2 >= 0) {
            //  main.layerCullDistances[num2] = qualityInfo.mStaticEffectLayerCullDistance;
            //}
            //if (num3 >= 0) {
            //  main.layerCullDistances[num3] = qualityInfo.mMonsterLayerCullDistance;
            //}
            //if (num4 >= 0) {
            //  main.layerCullDistances[num4] = qualityInfo.mNPCLayerCullDistance;
            //}
            //if (num5 >= 0) {
            //  main.layerCullDistances[num5] = qualityInfo.mVegetationLayerCullDistance;
            //}
            //if (num6 >= 0) {
            //  main.layerCullDistances[num6] = qualityInfo.mWaterLayerCullDistance;
            //}
        }
    }

    public static void SetDisplaySetting() {
    }

    public static void SetParticleFxQuality(bool off) {
        //if (mQualityLevel == 3) {
        //  if (off) {
        //    Singleton<FxManager>.GetInstance().SwitchSyncFx(false);
        //  } else {
        //    Singleton<FxManager>.GetInstance().SwitchSyncFx(true);
        //  }
        //  Singleton<FxManager>.GetInstance().SwitchUseAbsSyncFx(true);
        //  Singleton<FxManager>.GetInstance().SwitchSceneFxs(false);
        //} else {
        //  if (off) {
        //    Singleton<FxManager>.GetInstance().SwitchUseAbsSyncFx(true);
        //    Singleton<FxManager>.GetInstance().SwitchSceneFxs(false);
        //  } else {
        //    Singleton<FxManager>.GetInstance().SwitchUseAbsSyncFx(false);
        //    Singleton<FxManager>.GetInstance().SwitchSceneFxs(true);
        //  }
        //  Singleton<FxManager>.GetInstance().SwitchSyncFx(true);
        //}
    }

    public static void SetQualityAll() {
        if (mIsFirst) {
            SetTerrainQuality();
            SetParticleFxQuality(false);
            SetCameraQuality();
            SetShadowQuality();
            SetDisplaySetting();
            SetAsyncCoroutineMax();
            mIsFirst = false;
        }
    }

    public static void SetQualityLevel(int level) {
        mQualityLevel = level;
    }

    public static void SetResolution() {
        int width = Screen.width;
        int height = Screen.height;
        float num3 = 0f;
        float num4 = 0f;
        float num5 = 0f;
        Vector2 zero = Vector2.zero;
        if (mQualityLevel == (int)QualityLevelType.High) {
            zero = mHighResolution;
        } else if (mQualityLevel == (int)QualityLevelType.Medium) {
            zero = mMediumResolution;
        } else if (mQualityLevel == (int)QualityLevelType.Low) {
            zero = mLowResolution;
        } else if (mQualityLevel == (int)QualityLevelType.VeryLow) {
            zero = mVeryLowResolution;
        }
        if ((Screen.currentResolution.width > zero.x) && (zero.x != 0f)) {
            num3 = ((float)width) / zero.x;
        }
        if ((Screen.currentResolution.height > zero.y) && (zero.y != 0f)) {
            num4 = ((float)height) / zero.y;
        }
        num5 = Math.Max(num3, num4);
        if (num5 > 0f) {
            width = Mathf.FloorToInt(((float)width) / num5);
            height = Mathf.FloorToInt(((float)height) / num5);
            Screen.SetResolution(width, height, true);
        }
        //if ((width != Screen.width) && (height != Screen.height)) {
        //  Singleton<CustomManager>.GetInstance().CustomJoystickInput.UpdateScreen(width, height);
        //}
    }

    public static void SetShadowQuality() {
        //TODO:Shadow enable or disable
    }

    public static void SetTerrainQuality() {
        //QualityInfo qualityInfo = GetQualityInfo();
        //Terrain activeTerrain = Terrain.activeTerrain;
        //if (activeTerrain != null) {
        //  activeTerrain.heightmapPixelError = qualityInfo.mTerrainPixelError;
        //  activeTerrain.detailObjectDensity = qualityInfo.mTerrainDetailDensity;
        //  activeTerrain.detailObjectDistance = qualityInfo.mTerrainDetailDistance;
        //  activeTerrain.treeDistance = qualityInfo.mTerrainTreeDistance;
        //}
    }

    public static void SetAsyncCoroutineMax() {
        QualityInfo qualityInfo = GetQualityInfo();
        // TODO danie ResUpdateControler.AsyncMaxCoroutine = qualityInfo.mAsyncCoroutineMax;
    }

    public static bool IsFirstLaunch {
        get {
            if (PlayerPrefs.HasKey("FirstLaunch")) {
                return PlayerPrefs.GetInt("FirstLaunch") > 0;
            } else {
                return false;
            }
        }
        set {
            if (value) {
                PlayerPrefs.SetInt("FirstLaunch", 1);
            } else {
                PlayerPrefs.SetInt("FirstLaunch", 0);
            }
        }
    }

    // Nested Types
    public class QualityInfo {
        // Fields
        public float mAnimationLayerCullDistance;
        public float mCameraClipFar;
        public float mMonsterLayerCullDistance;
        public float mNPCLayerCullDistance;
        public float mObjectLayerClipDistance;
        public short mParticleBalanceCount;
        public bool mParticleSceneShow;
        public bool mParticleSyncShow;
        public byte mShadowQuality;
        public float mStaticEffectLayerCullDistance;
        public float mTerrainDetailDensity;
        public byte mTerrainDetailDistance;
        public bool mTerrainDetailObjectDraw;
        public float mTerrainLayerCullDistance;
        public byte mTerrainPixelError;
        public byte mTerrainTreeDistance;
        public float mVegetationLayerCullDistance;
        public float mWaterLayerCullDistance;
        public int mAsyncCoroutineMax;
    }

    public enum QualityLevelType {
        High,
        Medium,
        Low,
        VeryLow
    }
}
