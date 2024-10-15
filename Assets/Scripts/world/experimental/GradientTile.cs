using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEditor;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu(fileName = "New Gradient Tile", menuName = "Tiles/Gradient Tile")]
    public class GradientTile : TileBase
    {
        public int id;
        public Sprite mainTexture;
        public Texture2D mainTexture2D;
        public Texture2D colourMap;
        //I would like to add weighted texture variations.
        private Color[] originalColours;
        // public TextAsset csvFile;
        // public string[] csvStrings;
        // public Color[,] csvColourData;
        // private Color[] csvColourDataFlat;



        // public async void Initialize(Vector3Int position, Color[] inputColours, Sprite inputSprite){
        //     mainTexture = inputSprite;
        //     originalColours = ColourLibrary.grassUV;
        //     mainTexture = await Task.Run(() => ApplyGradientToTile(inputColours));
        // }

        public void Initialize(Vector3Int position, Color[] inputColours, Sprite inputSprite)
        {
            Debug.Log("Initialize: Starting initialization.");

            // Assign input sprite to mainTexture
            mainTexture = inputSprite;

            // csvColourData = colourData;

            
            // csvColourData = await UnityMainThreadDispatcher.Instance().EnqueueAsync(() => 
            // {
            //     return TextureUtility.LoadCSVAsColourArray(csvFile, 16, 16);
            // });

            // Ensure mainTexture and colours are valid

            if (mainTexture == null)
            {
                Debug.LogError("Initialize: inputSprite is null.");
                return;
            }

            if (inputColours == null || inputColours.Length == 0)
            {
                Debug.LogError("Initialize: inputColours are invalid.");
                return;
            }


            // Need to set on main thread. ----------------------------

            mainTexture = ApplyGradientToTile(inputColours);
            
            // --------------------------------------------------------

            // Check if the texture was successfully updated
            if (mainTexture == null)
            {
                Debug.LogError("Initialize: Failed to apply gradient to tile.");
                return;
            }

            Debug.Log("Initialize: Completed initialization successfully.");
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var iden = Matrix4x4.identity;
         
            tileData.sprite = mainTexture;
            tileData.color = Color.white; 
            tileData.transform = iden;

            Matrix4x4 transform = iden;
        }

        public void SetTileColours(Color[] newColours){
            originalColours = newColours;
        }

        public Sprite ApplyGradientToTile(Color[] inputColours)
        {
            Debug.Log("ApplyGradientToTile: Processing Started.");
            if (mainTexture == null)
            {
                Debug.LogError("Tile does not have a valid texture.");
                return null;
            }

            Texture2D updatedTexture = ReplaceColours(inputColours);

            Sprite updatedSprite = Sprite.Create(updatedTexture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            

            // mainTexture = updatedSprite;
            Debug.Log("Finished Rendering Tile.");
            return updatedSprite;
        }

        public Texture2D ReplaceColours(Color[] inputColours)
        {
            Debug.Log("ReplaceColours: Processing Started.");
            if (inputColours.Length != 16)
            {
                Debug.LogError("Input color array must have exactly 16 colors.");
                return null;
            }

            // Color[] pixels = new Color[256];

            // Debug.Log($"ReplaceColours: Colour Data Length: {csvColourData.GetLength(0)}, {csvColourData.GetLength(1)}");
            // for(int x = 0; x < csvColourData.GetLength(0); x++){
            //     for(int y = 0; y < csvColourData.GetLength(1); y++){
            //         for(int j = 0; j < inputColours.Length; j++){
                        // This calls nearly 17 million times. Let's trim that down eh?

                        //I'll migrate to a new csvData method, using a generated array in texture config.

            //             if(CompareColour(csvColourData[x,y], inputColours[j])){
            //                 pixels[x+y * csvColourData.GetLength(0)] = inputColours[j];
            //                 Debug.Log("ReplaceColours: Successfully Replaced Colour.");
            //             }
            //             Debug.Log("CompareColour Loop: No Match Found.");
            //         }
            //     }
            // }


            Color[] pixels = CycleColours(inputColours, Config.tileSize);

            Debug.Log("ReplaceColours: Started Creating NewTexture.");

            Texture2D newTexture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            newTexture.SetPixels(pixels);
            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.anisoLevel = 0;
            newTexture.Compress(false);
            newTexture.Apply();

            Debug.Log("ReplaceColours: Created NewTexture.");

            // System.GC.Collect();

            Debug.Log("ReplaceColours: Disposed of Temporary Data.");

            return newTexture;
        }

        bool CompareColour(Color colorA, Color colorB)
        {
            float sqrDistance = 
                Mathf.Pow(colorA.r - colorB.r, 2) + 
                Mathf.Pow(colorA.g - colorB.g, 2) + 
                Mathf.Pow(colorA.b - colorB.b, 2) +
                Mathf.Pow(colorA.a - colorB.a, 2);

            return sqrDistance < 0.0001f * 0.0001f;
        }

        public Color[] CycleColours(Color[] inputColours, int tileSize){// Pass in a name for different tile lookups.

            string name = "grass";
            Debug.Log($"CycleColours: Started generated colour array for {name}.");            


            GradientTile tile = BiomeUtility.GetGradientTileByName(name);
            int[] uvInts = TextureManager.GetTextureInts(name);
            Color[] colours = new Color[tileSize * tileSize];
            // Color[] uvColours = ColourLibrary.Get(name);

            for(int i = 0; i < tileSize * tileSize; i++){
                switch (uvInts[i]){
                    case 0:
                        colours[i] = inputColours[0];
                        break;
                    case 1:
                        colours[i] = inputColours[1];
                        break;
                    case 2:
                        colours[i] = inputColours[2];
                        break;
                    case 3:
                        colours[i] = inputColours[3];
                        break;
                    case 4:
                        colours[i] = inputColours[4];
                        break;
                    case 5:
                        colours[i] = inputColours[5];
                        break;
                    case 6:
                        colours[i] = inputColours[6];
                        break;
                    case 7:
                        colours[i] = inputColours[7];
                        break;
                    case 8:
                        colours[i] = inputColours[8];
                        break;
                    case 9:
                        colours[i] = inputColours[9];
                        break;
                    case 10:
                        colours[i] = inputColours[10];
                        break;
                    case 11:
                        colours[i] = inputColours[11];
                        break;
                    case 12:
                        colours[i] = inputColours[12];
                        break;
                    case 13:
                        colours[i] = inputColours[13];
                        break;
                    case 14:
                        colours[i] = inputColours[14];
                        break;
                    case 15:
                        colours[i] = inputColours[15];
                        break;
                    case -1:
                        colours[i] = Color.blue;
                        break;
                    default:
                        colours[i] = Color.red;
                        break;
                }

            }

            return colours;
        }
        
    }

}

[CustomEditor(typeof(GradientTile))]
public class GradientTileEditor : Editor{
    public override void OnInspectorGUI(){
        GradientTile gradientTile = (GradientTile)target;

        DrawDefaultInspector();

        // if(GUILayout.Button("Process CSV.")){
        //     gradientTile.csvColourData = TextureUtility.LoadCSVAsColourArray(gradientTile.csvFile, 16, 16);
        //     Debug.Log("Successfully processed CSV data to colour array.");

        //     gradientTile.csvStrings = TextureUtility.LoadPixelValues(gradientTile.csvFile, 16);

        // }
    }
}