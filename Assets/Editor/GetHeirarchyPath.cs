using UnityEngine;
using System.Collections;
using UnityEditor;

public class GetHeirarchyPath
{



    [MenuItem("Terahard/Get Heirarchy Path")]
    public static void GetHeirarchyMenuItem()
    {
        PrintHeirarchy();
    }




    public static void PrintHeirarchy()
    {
        GameObject[] go = Selection.gameObjects;
        for (int i = 0; i < go.Length; i++)
        {
            Transform t = go[i].transform;

            string path = t.name;

            while(t.parent!= null)
            {
                t = t.parent;
                path = string.IsNullOrEmpty(path) ? t.name : t.name + "/" + path;
            }
            Debug.Log(go[i].name + "\n\"" + path + "\"");
        }
    }
}