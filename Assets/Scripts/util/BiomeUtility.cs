using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;
using UnityEditor;

public static class BiomeUtility
{
    //Get chunk coordinates of a Vector2. Mostly used for managing chunks and debugging.

    public static Vector3Int GetVariableChunkPosition(Vector2 focus)
    {
        return new Vector3Int(Mathf.FloorToInt(focus.x / Config.chunkSize), Mathf.FloorToInt(focus.y / Config.chunkSize), 0);
    }

    public static TileBase ConvertTexture2DToTile(Texture2D texture){
        Sprite sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f), 12);

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;

        return tile;
    }

    public static Texture2D LoadTexture2DFromPath(string path){
        if(File.Exists(path)){
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2,2);
            if(texture.LoadImage(fileData)){
                return texture;
            }
        }
        throw new Exception($"File path invalid: '{path}'");
    }

    public static TileBase GetTileFromBiome(Biome biome)
    {
        TileBase tile;
        string tilePath = "Assets/textures/world/tiles/tilesObjects/" + biome.Name + ".asset";
        if(!File.Exists(tilePath)){
            string texturePath = "Assets/textures/world/tiles/" + biome.Name + ".png";
            Texture2D texture = LoadTexture2DFromPath(texturePath);
            tile = ConvertTexture2DToTile(texture);
            Debug.Log($"Generated tile from texture: {biome.Name}");
            //handle null or save the tile.
        }else{
            tile = AssetDatabase.LoadAssetAtPath<TileBase>(tilePath);
            Debug.Log($"Pulled tile: {biome.Name}");
        }
        
        if(tile == null){
            Debug.Log($"TileBase not found at path: {tilePath}");
        }

        return tile;
    }

    public static List<BiomeData> BiomeArrayToList(Biome[,] biomeArray){
        List<BiomeData> biomeList = new List<BiomeData>();

        for(int x = 0; x < biomeArray.GetLength(0); x++){
            for(int y = 0; y < biomeArray.GetLength(1); y++){
                Biome biome = biomeArray[x,y];
                biomeList.Add(new BiomeData(x, y, biome.Name));
            }
        }
        return biomeList;
    }

    public static Biome[,] ListToBiomeArray(List<BiomeData> biomeList, int width, int height){
        Biome[,] biomeArray = new Biome[width, height];

        foreach(BiomeData biomeData in biomeList){
            Biome biome = GetBiomeByName(biomeData.biomeName);
            biomeArray[biomeData.x, biomeData.y] = biome;
        }

        return biomeArray;
    }

    public static Biome GetBiomeByName(string biomeName){
        Registry<Biome> biomeRegistry = GlobalRegistry.categoryRegistry.GetCategoryRegistry<Biome>("biomes");
        Biome biome = biomeRegistry.Get(biomeName);
        Debug.Log($"Retrieved biome '{biome.Name}' by name.");
        return biome;
    }
}
