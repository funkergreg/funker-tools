#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FunkerTools {

    /// <summary>
    /// This Regex Sprite Slicer is designed to slice sprite sheets selected in the Unity 
    /// editor into individual sprites. For each file, if the string {width}x{height} can be
    /// found by searching the file name with a regular expression (regex), then the sprites 
    /// will be cut automatically. If dimensions are not found in the filename, then this 
    // script will create a pop-up for input values, allowing it to be cut by those dimensions.
    /// </summary>
    public class SpriteSlicer_Regex : EditorWindow {
        Texture2D gameObj;
        string objFilename;
        int sliceHeight;
        int sliceWidth;

        private void setObj(Texture2D obj) {
            gameObj = obj;
        }

        private void setFilename(string filename) {
            objFilename = filename;
        }

        private void setSliceWidth(int width) {
            sliceWidth = width;
        }

        private void setSliceHeight(int height) {
            sliceHeight = height;
        }

        private int getSliceHeight() {
            return sliceHeight;
        }

        private int getSliceWidth() {
            return sliceWidth;
        }

        private Texture2D getObj() {
            return gameObj;
        }

        private string getFilename() {
            return objFilename;
        }

        [MenuItem("Tools/Sprite Slicer REGEX")]
        static void Init() {
            foreach(Texture2D obj in Selection.objects) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string filename = Path.GetFileName(assetPath);
                int width = SheetSlicers.FindWidthPixelsInName(obj);
                int height = SheetSlicers.FindHeightPixelsInName(obj);
                // Ensure positive numbers above 0 pixels
                if(( height < 1 ) || ( width < 1 )) {
                    SpriteSlicer_Regex window = ScriptableObject.CreateInstance<SpriteSlicer_Regex>();
                    window.setObj(obj);
                    window.setFilename(filename);
                    window.setSliceWidth(width);
                    window.setSliceHeight(height);
                    window.position = new Rect(Screen.width / 2, Screen.height / 2, filename.Length * 11, 150);
                    window.ShowPopup();
                    // Debug.Log("Dimension Regex window init to mod height & width");
                } else {
                    // Slicing params populated by regex search
                    SheetSlicers.SliceInPlace(obj, width, height);
                    Debug.Log("Dimension Regex auto-slice complete!\n" +
                        filename + " sliced as (width x height) -> " + width + "x" + height);
                }
            }
        }

        void CreateGUI() {
            var label = new Label("Sprite Slicer for file: " + objFilename);
            rootVisualElement.Add(label);
            Debug.Log("Predicted Width: " + getObj().width + ", Height: " + getObj().height);

            var widthField = new IntegerField("Width", 4);
            widthField.value = getSliceWidth();
            rootVisualElement.Add(widthField);

            var heightField = new IntegerField("Height", 4);
            heightField.value = getSliceHeight();
            rootVisualElement.Add(heightField);

            var button = new UnityEngine.UIElements.Button();
            button.text = "Try to slice!";
            button.clicked += () => setSliceWidth(widthField.value);
            button.clicked += () => setSliceHeight(heightField.value);
            button.clicked += OnClick;
            rootVisualElement.Add(button);

            var button2 = new UnityEngine.UIElements.Button();
            button2.text = "Quit";
            button2.clicked += Button2Click;
            rootVisualElement.Add(button2);
        }

        void OnClick() {
            // Order here so as to not divide by zero on any check
            if(getSliceWidth() < 1) {
                Debug.LogWarning("Width not above 0 for: " + getFilename());
            } else if(getSliceHeight() < 1) {
                Debug.LogWarning("Height not above 0 for: " + getFilename());
            }
            // else if(getObj().width / getSliceWidth() != 0) {
            //     checkDims();
            //     Debug.LogWarning("Width not evenly slicable!  Check against total width: " + getObj().width);
            // } else if(getObj().height / getSliceHeight() != 0) {
            //     checkDims();
            //     Debug.LogWarning("Height not evenly slicable!  Check against total height: " + getObj().height);
            // }
            else {
                Debug.Log("On close, file: " + getFilename() + "\n" +
                "width: " + getSliceWidth() + ", height: " + getSliceHeight());
                SheetSlicers.SliceInPlace(getObj(), getSliceWidth(), getSliceHeight());
                this.Close();
            }
        }

        void Button2Click() {
            Debug.Log("No change on file: " + getFilename());
            this.Close();
        }

    }
}

#endif
