// Copyright (c) 2021 Koji Hasegawa.
// This software is released under the MIT License.

using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace CreateScriptFoldersWithTests.Editor
{
    public static class CreateScriptFoldersWithTestsMenu
    {
        [MenuItem("Assets/Create/C# Script Folders and Assemblies with Tests")]
        private static void CreateFeatureFolderWithTestsMenuItem()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateScriptFoldersWithTests>(),
                "New Feature",
                EditorGUIUtility.IconContent(EditorResources.folderIconName).image as Texture2D,
                (string)null);
        }
    }
}
