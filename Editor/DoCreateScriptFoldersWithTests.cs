// Copyright (c) 2021-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using System.Text;
using CreateScriptFoldersWithTests.Editor.Internals;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#if !UNITY_6000_0_OR_NEWER
using System.Reflection;
using UnityEngine;
#endif

namespace CreateScriptFoldersWithTests.Editor
{
    /// <summary>
    /// Create C# script folders and assemblies with tests.
    /// </summary>
    public class DoCreateScriptFoldersWithTests : EndNameEditAction
    {
        private const string Scripts = "Scripts";
        private const string Tests = "Tests";
        private const string Runtime = "Runtime";
        private const string Editor = "Editor";

        /// <inheritdoc/>
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var moduleName = Path.GetFileName(pathName);
            AssetDatabase.CreateFolder(Path.GetDirectoryName(pathName), moduleName);
            CreateFirstLayer(pathName, IsUnderAssets(pathName) ? Scripts : null);
            CreateFirstLayer(pathName, Tests);
        }

        private static void CreateFirstLayer(string pathName, string firstLayerName)
        {
            if (firstLayerName != null)
            {
                AssetDatabase.CreateFolder(pathName, firstLayerName);
            }

            CreateSecondLayer(pathName, firstLayerName, Runtime);
            CreateSecondLayer(pathName, firstLayerName, Editor);
        }

        private static void CreateSecondLayer(string pathName, string firstLayerName, string secondLayerName)
        {
            AssetDatabase.CreateFolder(PathCombineAllowNull(pathName, firstLayerName), secondLayerName);
            CreateAssemblyDefinitionFile(pathName, firstLayerName, secondLayerName);
            CreateDotSettingsFile(pathName, firstLayerName, secondLayerName);
        }

        private static void CreateAssemblyDefinitionFile(string pathName, string firstLayerName, string secondLayerName)
        {
            var moduleName = Path.GetFileName(pathName);
            var assemblyName = AssemblyName(moduleName, firstLayerName, secondLayerName);
            var asmdef = new AssemblyDefinition { name = assemblyName };

            if (firstLayerName is Tests)
            {
                asmdef.autoReferenced = false;
                asmdef.SetForTestAssembly();
                if (secondLayerName == Editor)
                {
                    asmdef.AddReferences($"{moduleName}.{Editor}");
                    asmdef.AddReferences($"{moduleName}.{Tests}");
                }
                else
                {
                    asmdef.AddReferences(moduleName);
                }
            }

            if (secondLayerName == Editor)
            {
                asmdef.includePlatforms = new[] { "Editor" };
                asmdef.AddReferences(moduleName);
            }

            if (IsUnderPackages(pathName))
            {
                asmdef.rootNamespace = moduleName;
            }

            var path = Path.Combine(
                PathCombineAllowNull(pathName, firstLayerName), secondLayerName, $"{assemblyName}.asmdef");
            CreateScriptAssetWithContent(path, EditorJsonUtility.ToJson(asmdef));
        }

        private static void CreateDotSettingsFile(string pathName, string firstLayerName, string secondLayerName)
        {
            var moduleName = Path.GetFileName(pathName);
            var assemblyName = AssemblyName(moduleName, firstLayerName, secondLayerName);
            var dotSettingsCreator = new DotSettingsCreator(assemblyName);

            if (firstLayerName is Scripts || firstLayerName is Tests)
            {
                dotSettingsCreator.AddNamespaceFoldersToSkip(Path.Combine(pathName, firstLayerName));
            }

            if (secondLayerName == Runtime)
            {
                dotSettingsCreator.AddNamespaceFoldersToSkip(
                    Path.Combine(PathCombineAllowNull(pathName, firstLayerName), secondLayerName));
            }

            if (dotSettingsCreator.WasAddNamespaceFoldersToSkip())
            {
                dotSettingsCreator.Flush();
            }
        }

        private static string AssemblyName(string moduleName, string firstLayerName, string secondLayerName)
        {
            var assemblyName = new StringBuilder(moduleName);
            if (secondLayerName == Editor)
            {
                assemblyName.Append($".{Editor}");
            }

            if (firstLayerName is Tests)
            {
                assemblyName.Append($".{Tests}");
            }

            return assemblyName.ToString();
        }

        private static void CreateScriptAssetWithContent(string path, string content)
        {
#if UNITY_6000_0_OR_NEWER
            ProjectWindowUtil.CreateScriptAssetWithContent(path, content);
#else
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var createScriptAssetWithContentMethod = projectWindowUtilType.GetMethod("CreateScriptAssetWithContent",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (createScriptAssetWithContentMethod == null)
            {
                Debug.LogError("Can not invoke UnityEditor.ProjectWindowUtil.CreateScriptAssetWithContent()");
                return;
            }

            createScriptAssetWithContentMethod.Invoke(null, new object[] { path, content });
#endif
        }

        private static bool IsUnderAssets(string pathName)
        {
            return pathName.StartsWith("Assets/");
        }

        private static bool IsUnderPackages(string pathName)
        {
            return !IsUnderAssets(pathName);
        }

        private static string PathCombineAllowNull(string pathName, string firstLayerName)
        {
            return firstLayerName != null ? Path.Combine(pathName, firstLayerName) : pathName;
        }
    }
}
