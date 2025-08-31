// Copyright (c) 2021-2025 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CreateScriptFoldersWithTests.Editor
{
    public class DoCreateScriptFoldersWithTestsTestUnderPackages
    {
        private const string ModuleName = "CreateScriptFoldersWithTestsTarget";
        private const string DotSettingsSuffix = ".csproj.DotSettings";

        private readonly string _rootFolderPath =
            Path.Combine("Packages", "com.nowsprinting.create-script-folders-with-tests", ModuleName);

        private static readonly string s_rootEncoded =
            "packages_005Ccom_002Enowsprinting_002Ecreate_002Dscript_002Dfolders_002Dwith_002Dtests_005C" +
            ModuleName.ToLower();

        private static readonly string s_foldersToSkipRuntime = $"{s_rootEncoded}_005Cruntime/";
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
                Path.Combine(_rootFolderPath, "Runtime", $"{AssemblyName}.asmdef"));
            // Not create Scripts directory when under Packages
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.True);
            Assert.That(asmdef.defineConstraints, Is.Empty);
            Assert.That(asmdef.includePlatforms, Is.Empty);
            Assert.That(asmdef.references, Is.Empty);
            Assert.That(asmdef.rootNamespace, Is.EqualTo(ModuleName));

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipRuntime));

            File.Delete(DotSettingsPath);
        }

        [Test]
        public void Action_CreatedEditorFolderContainingAsmdef()
        {
            const string AssemblyName = ModuleName + ".Editor";
            var file = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(
                Path.Combine(_rootFolderPath, "Editor", $"{AssemblyName}.asmdef"));
            // Not create Scripts directory when under Packages
            Assert.That(file, Is.Not.Null);

            var asmdef = new AssemblyDefinition();
            EditorJsonUtility.FromJsonOverwrite(file.ToString(), asmdef);
            Assert.That(asmdef.name, Is.EqualTo(AssemblyName));
            Assert.That(asmdef.autoReferenced, Is.True);
            Assert.That(asmdef.defineConstraints, Is.Empty);
            Assert.That(asmdef.includePlatforms, Does.Contain("Editor"));
            Assert.That(asmdef.references, Does.Contain(ModuleName));
            Assert.That(asmdef.rootNamespace, Is.EqualTo(ModuleName));

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Not.Exist);
            // Does not create DotSettings only Packages/Editor case
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
            Assert.That(asmdef.rootNamespace, Is.EqualTo(ModuleName));

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
            const string RuntimeTestAssemblyName = ModuleName + ".Tests";
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
            Assert.That(asmdef.references, Does.Contain(RuntimeTestAssemblyName));
#if UNITY_2019_3_OR_NEWER
            Assert.That(asmdef.references, Does.Contain("UnityEngine.TestRunner"));
            Assert.That(asmdef.references, Does.Contain("UnityEditor.TestRunner"));
            Assert.That(asmdef.overrideReferences, Is.True);
            Assert.That(asmdef.precompiledReferences, Does.Contain("nunit.framework.dll"));
            Assert.That(asmdef.optionalUnityReferences, Is.Empty);
#else
            Assert.That(asmdef.optionalUnityReferences, Does.Contain("TestAssemblies"));
#endif
            Assert.That(asmdef.rootNamespace, Is.EqualTo(ModuleName));

            const string DotSettingsPath = AssemblyName + DotSettingsSuffix;
            Assume.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Does.Contain(s_foldersToSkipTests));

            File.Delete(DotSettingsPath);
        }

        [Test]
        public void Action_PackagesRuntimeAssemblyInfoPath()
        {
            var assemblyInfoPath = Path.Combine(_rootFolderPath, "Runtime", "AssemblyInfo.cs");
            Assert.That(File.Exists(assemblyInfoPath), Is.True);
        }

        [Test]
        public void Action_PackagesEditorAssemblyInfoPath()
        {
            var assemblyInfoPath = Path.Combine(_rootFolderPath, "Editor", "AssemblyInfo.cs");
            Assert.That(File.Exists(assemblyInfoPath), Is.True);
        }

        [Test]
        public void Action_PackagesTestsRuntimeAssemblyInfoPath()
        {
            var assemblyInfoPath = Path.Combine(_rootFolderPath, "Tests", "Runtime", "AssemblyInfo.cs");
            Assert.That(File.Exists(assemblyInfoPath), Is.True);
        }

        [Test]
        public void Action_PackagesTestsEditorAssemblyInfo()
        {
            var assemblyInfoPath = Path.Combine(_rootFolderPath, "Tests", "Editor", "AssemblyInfo.cs");
            Assert.That(File.Exists(assemblyInfoPath), Is.True);
        }
    }
}
