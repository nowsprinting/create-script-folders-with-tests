// Copyright (c) 2021-2023 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CreateScriptFoldersWithTests.Editor.Helpers
{
    /// <summary>
    /// Consecutive spaces and tabs shrink to a single space
    /// Ignore line breaks
    /// </summary>
    public class ShrinkSpaceComparer : IComparer<string>
    {
        /// <inheritdoc />
        public int Compare(string x, string y)
        {
            var compere = string.Compare(Shrink(x), Shrink(y), StringComparison.Ordinal);
            if (compere == 0)
            {
                return 0;
            }

            Debug.Log($"Expected: \"{Shrink(x)}\"");
            Debug.Log($"But was:  \"{Shrink(y)}\"");

            return compere;
        }

        private static string Shrink(string s)
        {
            return Regex.Replace(
                s.Replace(Environment.NewLine, string.Empty),
                "\\s+", " ");
        }
    }
}
