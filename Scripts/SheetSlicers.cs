#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace FunkerTools {
    public static class SheetSlicers {

        /// <summary>
        /// Only use on single resources.
        /// Uses same params as <see cref="MakeSlices"/>
        /// </summary>
        public static SpriteRect[] GetSlicedSpriteArray(Texture2D obj, int sliceWidth, int sliceHeight) {
            List<SpriteRect> spriteRects = MakeSlices(obj, sliceWidth, sliceHeight);
            return spriteRects.ToArray(); // returns SpriteRect[]
        }

        /// <summary>
        /// Use on single or multiple resources (does not return the resources to caller).
        /// Uses same params as <see cref="MakeSlices"/>
        /// </summary>
        public static void SliceInPlace(Texture2D obj, int sliceWidth, int sliceHeight) {
            MakeSlices(obj, sliceWidth, sliceHeight);
        }

        public static int FindWidthPixelsInName(Texture2D obj) {
            int width = 0;
            Regex widthx = new Regex("[0-9]+x");
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(obj);
            string filename = Path.GetFileName(assetPath);
            Match found = widthx.Match(filename);
            if(widthx.IsMatch(filename)) {
                // TODO not sure below is actually checking for length 1
                if(found.Groups.Count != 1) {
                    Debug.LogWarning(filename + " cannot be parsed for width");
                } else {
                    // Debug.Log(filename + " contains width " + found.Groups[0]);
                    string result = found.Groups[0].ToString();
                    try {
                        // ignore x at end
                        string subs = result.Substring(0, result.Length - 1);
                        // Debug.Log("substring width (int): " + subs);
                        // parse int
                        width = Int32.Parse(subs);
                    } catch(FormatException) {
                        Debug.LogError(filename + " parse error on: " + result);
                    }
                }
            }
            // else {
            //     Debug.Log("width expression not found");
            // }
            return width;
        }

        public static int FindHeightPixelsInName(Texture2D obj) {
            int height = 0;
            Regex heightx = new Regex("x[0-9]+");
            string assetPath = AssetDatabase.GetAssetPath(obj);
            string filename = Path.GetFileName(assetPath);
            Match found = heightx.Match(filename);
            if(heightx.IsMatch(filename)) {
                // TODO not sure below is actually checking for length 1
                if(found.Groups.Count != 1) {
                    Debug.LogWarning(filename + " cannot be parsed for height");
                } else {
                    // Debug.Log(filename + " contains height " + found.Groups[0]);
                    string result = found.Groups[0].ToString();
                    try {
                        // ignore x at front
                        string subs = result.Substring(1, result.Length - 1);
                        // Debug.Log("substring height (int): " + subs);
                        // parse int
                        height = Int32.Parse(subs);
                    } catch(FormatException) {
                        Debug.LogError(filename + " parse error on: " + result);
                    }
                }
            }
            // else {
            //     Debug.Log("height expression not found");
            // }
            return height;
        }

        /// <summary>
        ///   Can be used on any/multiple assets.
        ///   Script is based partly on these references, but with modified asset management for Unity 2022.3.31f1:
        ///   https://docs.unity3d.com/Manual/Sprite-data-provider-api.html
        ///   https://forum.unity.com/threads/sprite-editor-automatic-slicing-by-script.320776/#post-9756150
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sliceWidth"></param>
        /// <param name="sliceHeight"></param>
        /// <returns> a <c>List<SpriteRect></c> - may return an empty list </returns>
        static List<SpriteRect> MakeSlices(Texture2D obj, int sliceWidth, int sliceHeight) {
            // Init the return object
            List<SpriteRect> spriteRects = new();

            string assetPath = AssetDatabase.GetAssetPath(obj);
            string filename = Path.GetFileName(assetPath);
            // Mod the Asset (the full sprite sheet)
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if(textureImporter == null) {
                Debug.LogError("Failed to get TextureImporter for texture");
            } else {
                // Set type to sprite
                textureImporter.textureType = TextureImporterType.Sprite;
                // Set to 'multiple' mode
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                // Ensure no compression
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                // TODO verify sprite pixels per unit?
                textureImporter.spritePixelsPerUnit = 64;
                // Reimport the texture with updated settings
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            // Slice full sheet into individual sprites
            if(obj is Texture2D) {
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
                dataProvider.InitSpriteEditorDataProvider();
                // Below only for Unity 2021.2 and newer
                var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
                // Containers
                var nameFileIdPairs = spriteNameFileIdDataProvider.GetNameFileIdPairs().ToList();
                spriteRects = dataProvider.GetSpriteRects().ToList();
                // Clear containers (so Unity does not add elements to current)
                nameFileIdPairs.Clear();
                spriteRects.Clear();
                // Like an index, uniqueName is used to create a unique name for each entry in the sprite sheet...
                int uniqueName = 0;
                // ... with three digits, including any leading zeroes
                string leadingZeroesFormat = "000";
                // NOTE: Some sprite system base sheets have cell ids numbered 0-255, so using three digits is sufficient for these;
                // NOTE: More zeroes could be added to handle sprite sheets with more than 1000 individual sprite elements
                // Slice sprites as Row then Column
                for(int y = obj.height; y > 0; y -= sliceHeight) { // Row (starts at top)
                    for(int x = 0; x < obj.width; x += sliceWidth) { // Column (starts at left)
                        SpriteRect spriteRect = new SpriteRect() {
                            rect = new Rect(x, y - sliceHeight, sliceWidth, sliceHeight),
                            // Place uniqueName as prefix and concat filename with extension removed
                            // NOTE: Since the names may be 4 or 5 ID elements long, placing the sprite's unique cell id
                            //   (place in the sheet) at the front allows identification by parsing the name
                            //   such as when creating prefabs with other scripts
                            name = $"{uniqueName.ToString(leadingZeroesFormat)}_{filename.Replace(".png", "")}",
                            pivot = new Vector2(0.5f, 0f),
                            alignment = SpriteAlignment.BottomCenter,
                            border = new Vector4(0, 0, 0, 0),
                            spriteID = GUID.Generate()
                        };
                        spriteRects.Add(spriteRect);
                        // Below only for Unity 2021.2 and newer
                        // Register the new Sprite Rect's name and GUID with the ISpriteNameFileIdDataProvider
                        nameFileIdPairs.Add(new SpriteNameFileIdPair(spriteRect.name, spriteRect.spriteID));
                        // Update index (for naming only)
                        uniqueName++;
                    }
                }
                // Set sprites (and ID provider details) to data provider
                dataProvider.SetSpriteRects(spriteRects.ToArray());
                spriteNameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);
                // Apply the changes made to the data provider
                dataProvider.Apply();
                // Reimport the asset to have the changes applied
                var assetImporter = dataProvider.targetObject as AssetImporter;
                assetImporter.SaveAndReimport();
            } else {
                Debug.LogError($"Asset obj at {assetPath} is not Texture2D type");
            }
            // Note no checks are being performed that there is anything in the list
            return spriteRects;
        }
    }
}

#endif
