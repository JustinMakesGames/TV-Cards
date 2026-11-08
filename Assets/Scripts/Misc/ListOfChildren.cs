using System.Collections.Generic;
using UnityEngine;

public static class ListOfChildren
{
    public static List<GameObject> ReturnListOfChildren(Transform parentObj)
    {
        List<GameObject> list = new List<GameObject>();

        for (int i = 0; i < parentObj.childCount; i++)
        {
            list.Add(parentObj.GetChild(i).gameObject);
        }

        return list;
    }
}
