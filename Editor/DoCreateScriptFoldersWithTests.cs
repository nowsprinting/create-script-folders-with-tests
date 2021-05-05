// Copyright (c) 2021 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace CreateScriptFoldersWithTests.Editor
{
    /// <inheritdoc/>
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
            CreateFirstLayer(pathName, Scripts);
            CreateFirstLayer(pathName, Tests);
        }

        private static void CreateFirstLayer(string pathName, string firstLayerName)
        {
            AssetDatabase.CreateFolder(pathName, firstLayerName);
            CreateSecondLayer(pathName, firstLayerName, Runtime);
            CreateSecondLayer(pathName, firstLayerName, Editor);
        }

        private static void CreateSecondLayer(string pathName, string firstLayerName, string secondLayerName)
        {
            AssetDatabase.CreateFolder(Path.Combine(pathName, firstLayerName), secondLayerName);
            CreateAssemblyDefinitionFile(pathName, firstLayerName, secondLayerName);
        }

        private static void CreateAssemblyDefinitionFile(string pathName, string firstLayerName,
            string secondLayerName)
        {
            var moduleName = Path.GetFileName(pathName);
            var assemblyName = AssemblyName(moduleName, firstLayerName, secondLayerName);
            var asmdef = new AssemblyDefinition {name = assemblyName};

            if (firstLayerName == Tests)
            {
                asmdef.autoReferenced = false;
                asmdef.SetForTestAssembly();
                asmdef.AddReferences(secondLayerName == Editor ? $"{moduleName}.{Editor}" : moduleName);
            }

            if (secondLayerName == Editor)
            {
                asmdef.includePlatforms = new[] {"Editor"};
                asmdef.AddReferences(moduleName);
            }

            var path = Path.Combine(pathName, firstLayerName, secondLayerName, $"{assemblyName}.asmdef");
            CreateScriptAssetWithContent(path, EditorJsonUtility.ToJson(asmdef));
        }

        private static string AssemblyName(string moduleName, string firstLayerName, string secondLayerName)
        {
            var assemblyName = new StringBuilder(moduleName);
            if (secondLayerName == Editor)
            {
                assemblyName.Append($".{Editor}");
            }

            if (firstLayerName == Tests)
            {
                assemblyName.Append($".{Tests}");
            }

            return assemblyName.ToString();
        }

        private static void CreateScriptAssetWithContent(string path, string content)
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var createScriptAssetWithContentMethod = projectWindowUtilType.GetMethod("CreateScriptAssetWithContent",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (createScriptAssetWithContentMethod == null)
            {
                Debug.LogError("Can not invoke UnityEditor.ProjectWindowUtil.CreateScriptAssetWithContent()");
                return;
            }

            createScriptAssetWithContentMethod.Invoke(null, new object[] {path, content});
        }
    }
}
