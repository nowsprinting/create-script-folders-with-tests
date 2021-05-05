// Copyright (c) 2021 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CreateScriptFoldersWithTests.Editor
{
    /// <summary>
    /// <see cref="https://docs.unity3d.com/Manual/AssemblyDefinitionFileFormat.html"/>
    /// </summary>
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class AssemblyDefinition
    {
        public bool allowUnsafeCode = false; //Optional. Defaults to false.
        public bool autoReferenced = true; //Optional. Defaults to true.
        public string[] defineConstraints; // Optional. The symbols that serve as constraints. Can be empty.
        public string[] excludePlatforms; // Optional. The platform name strings to exclude or an empty array.
        public string[] includePlatforms; // Optional. The platform name strings to exclude or an empty array.
        public string name; // Required.
        public bool noEngineReferences = false; // Optional. Defaults to false.

        public string[] optionalUnityReferences;
        // Optional. In earlier versions of Unity, this field serialized the Unity References : Test Assemblies option used to designate the assembly as a test assembly

        public bool overrideReferences = false;
        // Optional. Set to true if precompiledReferences contains values. Defaults to false.

        public string[] precompiledReferences;
        // Optional. The file names of referenced DLL libraries including extension, but without other path elements. Can be empty.
        // This array is ignored unless you set overrideReferences to true.

        public string[]
            references; // Optional. References to other assemblies created with Assembly Definition assets.

        public object[] versionDefines; // Optional. Contains an object for each version define.

        public void SetForTestAssembly()
        {
            defineConstraints = new[] {"UNITY_INCLUDE_TESTS"};
#if UNITY_2019_3_OR_NEWER
            overrideReferences = true;
            precompiledReferences = new[] {"nunit.framework.dll"};
            AddReferences("UnityEngine.TestRunner");
            AddReferences("UnityEditor.TestRunner");
#else
            optionalUnityReferences = new[] {"TestAssemblies"};
#endif
        }

        public void AddReferences(string s)
        {
            if (references == null)
            {
                references = new string[] { };
            }

            references = references.Append(s).ToArray();
        }
    }
}
