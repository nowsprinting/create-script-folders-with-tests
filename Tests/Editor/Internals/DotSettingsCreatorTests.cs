// Copyright (c) 2021-2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.IO;
using CreateScriptFoldersWithTests.Editor.Helpers;
using NUnit.Framework;

namespace CreateScriptFoldersWithTests.Editor.Internals
{
    public class DotSettingsCreatorTests
    {
        private const string ModuleName = "CreateScriptFoldersWithTestsTarget";
        private const string DotSettingsSuffix = ".csproj.DotSettings";
        private const string DotSettingsPath = ModuleName + DotSettingsSuffix;

        [SetUp]
        public void SetUp()
        {
            Assume.That(new FileInfo(DotSettingsPath), Does.Not.Exist);
        }

        [Test]
        public void DotSettingsCreator_CreatedDotSettingsFile()
        {
            var sut = new DotSettingsCreator(ModuleName);
            sut.Flush();

            Assert.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Is.EqualTo(@"<wpf:ResourceDictionary
    xml:space=""preserve""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:s=""clr-namespace:System;assembly=mscorlib""
    xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml""
    xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
</wpf:ResourceDictionary>")
                .Using(new ShrinkSpaceComparer()));

            File.Delete(DotSettingsPath);
        }

        [Test]
        public void AddNamespaceFoldersToSkip_DotSettingsContainNamespaceFoldersToSkipElement()
        {
            var sut = new DotSettingsCreator(ModuleName);
            sut.AddNamespaceFoldersToSkip("Packages/com.nowsprinting.create-script-folders-with-tests/Foo");
            sut.AddNamespaceFoldersToSkip("Packages/com.nowsprinting.create-script-folders-with-tests/Foo/Bar");
            sut.AddNamespaceFoldersToSkip("Packages/com.nowsprinting.create-script-folders-with-tests/New Feature (1)");
            sut.Flush();

            Assert.That(new FileInfo(DotSettingsPath), Does.Exist);
            Assert.That(File.ReadAllText(DotSettingsPath), Is.EqualTo(@"<wpf:ResourceDictionary
    xml:space=""preserve""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:s=""clr-namespace:System;assembly=mscorlib""
    xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml""
    xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
<s:Boolean x:Key=""/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=packages_005Ccom_002Enowsprinting_002Ecreate_002Dscript_002Dfolders_002Dwith_002Dtests_005Cfoo/@EntryIndexedValue"">True</s:Boolean>
<s:Boolean x:Key=""/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=packages_005Ccom_002Enowsprinting_002Ecreate_002Dscript_002Dfolders_002Dwith_002Dtests_005Cfoo_005Cbar/@EntryIndexedValue"">True</s:Boolean>
<s:Boolean x:Key=""/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=packages_005Ccom_002Enowsprinting_002Ecreate_002Dscript_002Dfolders_002Dwith_002Dtests_005Cnew_0020feature_0020_00281_0029/@EntryIndexedValue"">True</s:Boolean>
</wpf:ResourceDictionary>")
                .Using(new ShrinkSpaceComparer()));

            File.Delete(DotSettingsPath);
        }
    }
}
