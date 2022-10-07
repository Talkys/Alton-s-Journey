using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Map
{
    public static string[] map;

    public static void debugMap(string[] mapa)
    {
        string log = "";
        for(int i=0; i<mapa.Length; i++)
            log += (mapa[i]+"\n");

        Debug.Log(log);
    }

    public static List<string> toList(string[] mapa)
    {
        List<string> newMap = new List<string>();

        for(int i=map.Length-1; i>=0; i--)
            newMap.Add(mapa[i]);

        return newMap;
    } 

    public static void resetMap()
    {
        Map.map =  new string[10];
            for(int i=0;i<10;i++)
                Map.map[i] = "#        #";

        Map.map[0] = "##########";
        Map.map[9] = "##########";
    }
    
}
