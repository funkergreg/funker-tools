#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace FunkerTools.Editor {

    /// <summary>
    ///     Slices sprite sheets at 64x64
    /// </summary>
    public static class SpriteSlicer_64x64 {

        [MenuItem("Tools/Sprite Slicer 64x64")]
        static void UpdateSettings() {
            int sliceHeight = 64;
            int sliceWidth = 64;
            foreach(Texture2D obj in Selection.objects) {
                FunkerTools.SheetSlicers.SliceInPlace(obj, sliceWidth, sliceHeight);
            }
        }
    }
}

#endif
