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
        private const string ModuleName = "CreateScriptFoldersWithTestsTarget";
        private const string DotSettingsSuffix = ".csproj.DotSettings";
        private readonly string _rootFolderPath = Path.Combine("Assets", ModuleName);
        private static readonly string s_rootEncoded = "assets_005C" + ModuleName.ToLower();
        private static readonly string s_foldersToSkipScripts = $"{s_rootEncoded}_005Cscripts/";
        private static readonly string s_foldersToSkipScriptsRuntime = $"{s_rootEncoded}_005Cscripts_005Cruntime/";
        private static readonly string s_foldersToSkipTests = $"{s_rootEncoded}_005Ctests/";
        private static readonly string s_foldersToSkipTestsRuntime = $"{s_rootEncoded}_005Ctests_005Cruntime/";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AssetDatabase.DisallowAutoRefresh();

            // Generated folder and files does not exist before test
            Assume.That(AssetDatabase.IsValidFolder(_rootFolderPath), Is.False);
            Assume.That(new FileInfo(ModuleName + DotSettingsSuffix), Does.Not.Exist);
            Assume.That(new FileInfo(ModuleName + ".Editor" + DotSettingsSuffix), Does.Not.Exist);
            Assume.That(new FileInfo(ModuleName + ".Tests" + DotSettingsSuffix), Does.Not.Exist);
            Assume.That(new FileInfo(ModuleName + ".Editor.Tests" + DotSettingsSuffix), Does.Not.Exist);

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
            const string AssemblyName = ModuleName + "";
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
            Assert.That(asmdef.rootNamespace, Is.Empty);

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipScripts));
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipScriptsRuntime));

            File.Delete(DotSettingsPath);
        }

        [Test]
        public void Action_CreatedEditorFolderContainingAsmdef()
        {
            const string AssemblyName = ModuleName + ".Editor";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Scripts", "Editor", $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.True);
            Assert.That(asmdef.defineConstraints, Is.Empty);
            Assert.That(asmdef.includePlatforms, Does.Contain("Editor"));
            Assert.That(asmdef.references, Does.Contain(ModuleName));
            Assert.That(asmdef.rootNamespace, Is.Empty);

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipScripts));

            File.Delete(DotSettingsPath);
        }

        [Test]
        public void Action_CreatedRuntimeTestsFolderContainingAsmdef()
        {
            const string AssemblyName = ModuleName + ".Tests";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Tests", "Runtime", $"{AssemblyName}.asmdef"));
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.False);
            Assert.That(asmdef.defineConstraints, Does.Contain("UNITY_INCLUDE_TESTS"));
            Assert.That(asmdef.includePlatforms, Is.Empty);
            Assert.That(asmdef.references, Does.Contain(ModuleName));
#if UNITY_2019_3_OR_NEWER
            Assert.That(asmdef.references, Does.Contain("UnityEngine.TestRunner"));
            Assert.That(asmdef.references, Does.Contain("UnityEditor.TestRunner"));
            Assert.That(asmdef.overrideReferences, Is.True);
            Assert.That(asmdef.precompiledReferences, Does.Contain("nunit.framework.dll"));
            Assert.That(asmdef.optionalUnityReferences, Is.Empty);
#else
            Assert.That(asmdef.optionalUnityReferences, Does.Contain("TestAssemblies"));
#endif
            Assert.That(asmdef.rootNamespace, Is.Empty);

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipTests));
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipTestsRuntime));

            File.Delete(DotSettingsPath);
        }

        [Test]
        public void Action_CreatedEditorTestsFolderContainingAsmdef()
        {
            const string EditorAssemblyName = ModuleName + ".Editor";
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
            Assert.That(asmdef.references, Does.Contain(ModuleName));
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
            Assert.That(asmdef.rootNamespace, Is.Empty);

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipTests));

            File.Delete(DotSettingsPath);
        }
    }
}
