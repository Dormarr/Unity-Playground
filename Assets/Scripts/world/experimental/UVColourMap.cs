using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UVColourMap : MonoBehaviour
{
    public Texture2D colourMap;

    public Color GetColourFromUVMap(float temperature, float precipitation){
        temperature = Mathf.Clamp01(temperature);
        precipitation = Mathf.Clamp01(precipitation);

        Texture2D tex = colourMap;
        int texSize = 16;

        int u = Mathf.FloorToInt(temperature * texSize);
        int v = Mathf.FloorToInt(precipitation * texSize);

        Color pixelColour = tex.GetPixel(u, v);
        Debug.Log($"Grabbed UV Colour: {pixelColour.ToString()}");
        return pixelColour;
    }

    public void ApplyColourToTile(float temperature, float precipitation, Tilemap tilemap, Vector3Int tilePos){

        //use this colour value to determine the central colour for the tile to work outwards from.
        //The gradient and complimentary shades should start here.

        tilemap.SetTileFlags(tilePos, TileFlags.None);
        Color tintColour = GetColourFromUVMap(temperature, precipitation);
    }
}
