// Copyright (c) 2021-2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CreateScriptFoldersWithTests.Editor
{
    public class DoCreateScriptFoldersWithTestsTest
    {
        private const string RootFolderName = "CreateScriptFoldersWithTestsTest";
        private readonly string _rootFolderPath = Path.Combine("Assets", RootFolderName);

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AssetDatabase.DisallowAutoRefresh();

            var exist = AssetDatabase.IsValidFolder(_rootFolderPath);
            Assume.That(exist, Is.False, "Generated folder does not exist before test");

            var sut = ScriptableObject.CreateInstance<DoCreateScriptFoldersWithTests>();
            sut.Action(0, _rootFolderPath, null);
            // I understand that it shouldn't be exercise within OneTimeSetUp. I did not have a choice.
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            AssetDatabase.DeleteAsset(_rootFolderPath);
            AssetDatabase.AllowAutoRefresh();
        }

        [Test]
        public void Action_CreatedRuntimeFolderContainingAsmdef()
        {
            const string AssemblyName = RootFolderName + "";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Scripts", "Runtime", $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.True);
            Assert.That(asmdef.defineConstraints, Is.Empty);
            Assert.That(asmdef.includePlatforms, Is.Empty);
            Assert.That(asmdef.references, Is.Empty);
        }

        [Test]
        public void Action_CreatedEditorFolderContainingAsmdef()
        {
            const string AssemblyName = RootFolderName + ".Editor";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Scripts", "Editor", $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.True);
            Assert.That(asmdef.defineConstraints, Is.Empty);
            Assert.That(asmdef.includePlatforms, Does.Contain("Editor"));
            Assert.That(asmdef.references, Does.Contain(RootFolderName));
        }

        [Test]
        public void Action_CreatedRuntimeTestsFolderContainingAsmdef()
        {
            const string AssemblyName = RootFolderName + ".Tests";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Tests", "Runtime", $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.False);
            Assert.That(asmdef.defineConstraints, Does.Contain("UNITY_INCLUDE_TESTS"));
            Assert.That(asmdef.includePlatforms, Is.Empty);
            Assert.That(asmdef.references, Does.Contain(RootFolderName));
#if UNITY_2019_3_OR_NEWER
            Assert.That(asmdef.references, Does.Contain("UnityEngine.TestRunner"));
            Assert.That(asmdef.references, Does.Contain("UnityEditor.TestRunner"));
            Assert.That(asmdef.overrideReferences, Is.True);
            Assert.That(asmdef.precompiledReferences, Does.Contain("nunit.framework.dll"));
            Assert.That(asmdef.optionalUnityReferences, Is.Empty);
#else
            Assert.That(asmdef.optionalUnityReferences, Does.Contain("TestAssemblies"));
#endif
        }

        [Test]
        public void Action_CreatedEditorTestsFolderContainingAsmdef()
        {
            const string EditorAssemblyName = RootFolderName + ".Editor";
            const string AssemblyName = EditorAssemblyName + ".Tests";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Tests", "Editor", $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.False);
            Assert.That(asmdef.defineConstraints, Does.Contain("UNITY_INCLUDE_TESTS"));
            Assert.That(asmdef.includePlatforms, Does.Contain("Editor"));
            Assert.That(asmdef.references, Does.Contain(RootFolderName));
            Assert.That(asmdef.references, Does.Contain(EditorAssemblyName));
#if UNITY_2019_3_OR_NEWER
            Assert.That(asmdef.references, Does.Contain("UnityEngine.TestRunner"));
            Assert.That(asmdef.references, Does.Contain("UnityEditor.TestRunner"));
            Assert.That(asmdef.overrideReferences, Is.True);
            Assert.That(asmdef.precompiledReferences, Does.Contain("nunit.framework.dll"));
            Assert.That(asmdef.optionalUnityReferences, Is.Empty);
#else
            Assert.That(asmdef.optionalUnityReferences, Does.Contain("TestAssemblies"));
#endif
        }
    }
}
