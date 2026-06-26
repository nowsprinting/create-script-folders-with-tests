// Copyright (c) 2021-2026 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace CreateScriptFoldersWithTests.Editor
{
    /// <summary>
    /// Create C# script folders and assemblies with tests via context menu.
    /// </summary>
    public static class CreateScriptFoldersWithTestsMenu
    {
        [MenuItem("Assets/Create/C# Script Folders and Assemblies with Tests")]
        private static void CreateFeatureFolderWithTestsMenuItem()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
#if UNITY_6000_4_OR_NEWER
                EntityId.None,
#else
                0,
#endif
                ScriptableObject.CreateInstance<DoCreateScriptFoldersWithTests>(),
                "New Feature",
                EditorGUIUtility.IconContent(EditorResources.folderIconName).image as Texture2D,
                (string)null);
        }
    }
}
