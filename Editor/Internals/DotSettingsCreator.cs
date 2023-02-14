// Copyright (c) 2021-2023 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CreateScriptFoldersWithTests.Editor.Internals
{
    internal class DotSettingsCreator
    {
        private readonly string _assemblyName;
        private readonly List<string> _namespaceFoldersToSkip;

        public DotSettingsCreator(string assemblyName)
        {
            _assemblyName = assemblyName;
            _namespaceFoldersToSkip = new List<string>();
        }

        public void AddNamespaceFoldersToSkip(string path)
        {
            var key = path
                .ToLower()
                .Replace("/", "_005C")
                .Replace(".", "_002E")
                .Replace("-", "_002D")
                .Replace(" ", "_0020")
                .Replace("(", "_0028")
                .Replace(")", "_0029");

            _namespaceFoldersToSkip.Add(
                $"<s:Boolean x:Key=\"/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/={key}/@EntryIndexedValue\">True</s:Boolean>");
        }

        public bool WasAddNamespaceFoldersToSkip()
        {
            return _namespaceFoldersToSkip.Any();
        }

        public void Flush()
        {
            using (var writer = new StreamWriter($"{_assemblyName}.csproj.DotSettings"))
            {
                writer.WriteLine(
                    "<wpf:ResourceDictionary xml:space=\"preserve\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:s=\"clr-namespace:System;assembly=mscorlib\" xmlns:ss=\"urn:shemas-jetbrains-com:settings-storage-xaml\" xmlns:wpf=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
                foreach (var s in _namespaceFoldersToSkip)
                {
                    writer.WriteLine(s);
                }

                writer.WriteLine("</wpf:ResourceDictionary>");
            }
        }
    }
}
