using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UVColourMap : MonoBehaviour
{
    public Sprite colorMap;

    public Color GetColourFromUVMap(float temperature, float precipitation){
        temperature = Mathf.Clamp01(temperature);
        precipitation = Mathf.Clamp01(precipitation);

        Texture2D tex = colorMap.texture;
        int texSize = tex.width;

        int u = Mathf.FloorToInt(temperature * texSize);
        int v = Mathf.FloorToInt(precipitation * texSize);


        Color pixelColour = tex.GetPixel(u, v);
        return pixelColour;
    }

    public void ApplyColourToTile(float temperature, float precipitation, Tilemap tilemap, Vector3Int tilePos){
        tilemap.SetTileFlags(tilePos, TileFlags.None);
        Color tintColour = GetColourFromUVMap(temperature, precipitation);
        tilemap.SetColor(tilePos, tintColour);
        Debug.Log($"Colour Applied to Tile: {tintColour}");
    }
}
