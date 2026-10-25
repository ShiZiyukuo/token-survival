#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace TokenSurvival.EditorTools {
 public static class MobileBuild {
  public static void BuildAndroid(){
   RebuildProjectBuilder.BuildAll();
   Directory.CreateDirectory("Build/Android");
   PlayerSettings.productName="Token Survival - 养一只大肥鱼";PlayerSettings.companyName="Indie Rebuild";PlayerSettings.applicationIdentifier="com.tokensurvival.whaleroom";PlayerSettings.bundleVersion="0.2.1";
   PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.allowedAutorotateToPortrait=false;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;PlayerSettings.allowedAutorotateToLandscapeRight=true;PlayerSettings.allowedAutorotateToLandscapeLeft=true;
   PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevel35;PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64|AndroidArchitecture.ARMv7;PlayerSettings.Android.useCustomKeystore=false;
   EditorUserBuildSettings.buildAppBundle=false;EditorUserBuildSettings.androidBuildSystem=AndroidBuildSystem.Gradle;EditorUserBuildSettings.androidETC2Fallback=AndroidETC2Fallback.Quality32Bit;
   var report=BuildPipeline.BuildPlayer(new[]{"Assets/TokenSurvival/Scenes/MainRoom.unity"},"Build/Android/TokenSurvival.apk",BuildTarget.Android,BuildOptions.Development);Debug.Log("[TokenSurvival] ANDROID_BUILD="+report.summary.result+" errors="+report.summary.totalErrors+" warnings="+report.summary.totalWarnings);if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Android APK build failed");
  }
 }
}
#endif
