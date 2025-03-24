#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace FunkerTools.Editor {

    /// <summary>
    ///     Slices sprite sheets at 16x16
    /// </summary>
    public static class SpriteSlicer_16x16 {

        [MenuItem("Tools/Sprite Slicer 16x16")]
        static void UpdateSettings() {
            int sliceHeight = 16;
            int sliceWidth = 16;
            foreach(Texture2D obj in Selection.objects) {
                FunkerTools.SheetSlicers.SliceInPlace(obj, sliceWidth, sliceHeight);
            }
        }
    }
}

#endif
