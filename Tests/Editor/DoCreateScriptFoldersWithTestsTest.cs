// Copyright (c) 2021 Koji Hasegawa.
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
        private readonly string _path = Path.Combine("Assets", "_Test4CreateScriptFoldersWithTests");

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Assert.That(Directory.Exists(Path.GetFullPath(_path)), Is.False, "Folder does not exist before test");

            var sut = ScriptableObject.CreateInstance<DoCreateScriptFoldersWithTests>();
            sut.Action(0, _path, null);
            // I understand that it shouldn't be exercise within OneTimeSetUp. I did not have a choice.
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(Path.GetFullPath(_path), true);
        }

        [Test]
        public void Action_CreatedRuntimeFolderContainingAsmdef()
        {
            const string AssemblyName = "_Test4CreateScriptFoldersWithTests";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(Path.Combine(_path, "Scripts", "Runtime",
                $"{AssemblyName}.asmdef"));
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
            const string AssemblyName = "_Test4CreateScriptFoldersWithTests.Editor";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(Path.Combine(_path, "Scripts", "Editor",
                $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.True);
            Assert.That(asmdef.defineConstraints, Is.Empty);
            Assert.That(asmdef.includePlatforms, Does.Contain("Editor"));
            Assert.That(asmdef.references, Does.Contain("_Test4CreateScriptFoldersWithTests"));
        }

        [Test]
        public void Action_CreatedRuntimeTestsFolderContainingAsmdef()
        {
            const string AssemblyName = "_Test4CreateScriptFoldersWithTests.Tests";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(Path.Combine(_path, "Tests", "Runtime",
                $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.False);
            Assert.That(asmdef.defineConstraints, Does.Contain("UNITY_INCLUDE_TESTS"));
            Assert.That(asmdef.includePlatforms, Is.Empty);
            Assert.That(asmdef.references, Does.Contain("_Test4CreateScriptFoldersWithTests"));
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
            const string AssemblyName = "_Test4CreateScriptFoldersWithTests.Editor.Tests";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(Path.Combine(_path, "Tests", "Editor",
                $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.False);
            Assert.That(asmdef.defineConstraints, Does.Contain("UNITY_INCLUDE_TESTS"));
            Assert.That(asmdef.includePlatforms, Does.Contain("Editor"));
            Assert.That(asmdef.references, Does.Contain("_Test4CreateScriptFoldersWithTests.Editor"));
            Assert.That(asmdef.references, Does.Contain("_Test4CreateScriptFoldersWithTests"));
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
