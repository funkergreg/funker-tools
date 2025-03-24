#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace FunkerTools.Editor {

    /// <summary>
    ///     Slices sprite sheets at 32x32
    /// </summary>
    public static class SpriteSlicer_32x32 {

        [MenuItem("Tools/Sprite Slicer 32x32")]
        static void UpdateSettings() {
            int sliceHeight = 32;
            int sliceWidth = 32;
            foreach(Texture2D obj in Selection.objects) {
                FunkerTools.SheetSlicers.SliceInPlace(obj, sliceWidth, sliceHeight);
            }
        }
    }
}

#endif
