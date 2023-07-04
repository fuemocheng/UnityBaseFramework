using BaseFramework;
using System.IO;
using UnityEngine;
using UnityBaseFramework.Editor;
using UnityBaseFramework.Editor.ResourceTools;

namespace XGame.Editor
{
    public static class BaseFrameworkConfigs
    {
        //[BuildSettingsConfigPath]
        //public static string BuildSettingsConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameMain/Configs/BuildSettings.xml"));

        [ResourceCollectionConfigPath]
        public static string ResourceCollectionConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameMain/Configs/ResourceCollection.xml"));

        [ResourceEditorConfigPath]
        public static string ResourceEditorConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameMain/Configs/ResourceEditor.xml"));

        //[ResourceBuilderConfigPath]
        //public static string ResourceBuilderConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameMain/Configs/ResourceBuilder.xml"));
    }
}
