using UnityEngine;
using System.Collections;
using System;

public class GameQualityManager : Manager
{
    private static int mQualityLevel;
    private static int mMemoryQuality = 1;
    private static int mCpuQuality = 1;
    private static QualityInfo mHighQuality = new QualityInfo();
    private static Vector2 mHighResolution = new Vector2(1920f, 1080f);
    private static QualityInfo mMediumQuality = new QualityInfo();
    private static Vector2 mMediumResolution = new Vector2(1280f, 720f);
    private static QualityInfo mLowQuality = new QualityInfo();
    private static Vector2 mLowResolution = new Vector2(960f, 540f);
    private static QualityInfo mVeryLowQuality = new QualityInfo();
    private static Vector2 mVeryLowResolution = new Vector2(800f, 480f);

    public static void InitQalityInfo()
    {
        mHighQuality.mParticleSceneShow = true;
        mHighQuality.mShadowQuality = 1;
        mHighQuality.mAsyncCoroutineMax = 15;
        mHighQuality.mShaderLevel = 1;

        mMediumQuality.mParticleSceneShow = true;
        mMediumQuality.mShadowQuality = 1;
        mMediumQuality.mAsyncCoroutineMax = 12;
        mMediumQuality.mShaderLevel = 0;

        mLowQuality.mParticleSceneShow = false;
        mLowQuality.mShadowQuality = 1;
        mLowQuality.mAsyncCoroutineMax = 8;
        mLowQuality.mShaderLevel = 0;

        mVeryLowQuality.mParticleSceneShow = false;
        mVeryLowQuality.mShadowQuality = 0;
        mVeryLowQuality.mAsyncCoroutineMax = 5;
        mVeryLowQuality.mShaderLevel = 0;

        //  string str = SystemInfo.processorType.ToLower();

        //  Log(string.Format("cpuName = {0}, deviceModel = {1}", str, SystemInfo.deviceModel));
    }
    public enum QualityLevelType
    {
        High,
        Medium,
        Low,
        VeryLow
    }

    public static void InitGameQuality()
    {
        InitQalityInfo();
        if (SystemInfo.processorCount <= 1)
        {
            mCpuQuality = 3;
        }
        else if (SystemInfo.processorCount <= 2)
        {
            mCpuQuality = 2;
        }
        else if (SystemInfo.processorCount <= 4)
        {
            mCpuQuality = 1;
        }
        else
        {
            mCpuQuality = 0;
        }
        if (SystemInfo.systemMemorySize < 0)
        {
            mMemoryQuality = 3;
        }
        else if (SystemInfo.systemMemorySize < 700)
        {
            mMemoryQuality = 2;
        }
        else if (SystemInfo.systemMemorySize < 0x5dc)
        {
            mMemoryQuality = 1;
        }
        else
        {
            mMemoryQuality = 0;
        }
        SetQualityLevel(Math.Max(mCpuQuality, mMemoryQuality));
        QualitySettings.SetQualityLevel(GetQualityLevel());

    }

    public static int GetQualityLevel()
    {
        return mQualityLevel;
    }

    public static void SetQualityLevel(int level)
    {
        mQualityLevel = level;
		QualitySettings.SetQualityLevel(mQualityLevel,true);
    }


    public static QualityInfo GetQualityInfo()
    {
        if (mQualityLevel == (int)QualityLevelType.High)
        {
            return mHighQuality;
        }
        if (mQualityLevel == (int)QualityLevelType.Medium)
        {
            return mMediumQuality;
        }
        if (mQualityLevel == (int)QualityLevelType.Low)
        {
            return mLowQuality;
        }
        return mVeryLowQuality;
    }


    public static void SetShaderLevel()
    {
        QualityInfo qualityInfo = GetQualityInfo();
        if(qualityInfo.mShaderLevel > 0)
           Shader.EnableKeyword("SHADER_AdvancedEffect");
        else
           Shader.DisableKeyword("SHADER_AdvancedEffect");
    }

    public class QualityInfo
    {
        public bool mParticleSceneShow;
        public byte mShadowQuality;
        public int mAsyncCoroutineMax;
        public int mShaderLevel;
    }




}
