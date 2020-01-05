using System.Collections.Generic;
using UnityEngine;

public static class PathUtil
{
    private static Dictionary<string, string> _pathDict;
    
    public static string Get(string name)
    {
        if (_pathDict == null) FillPathDictionary();
        return _pathDict[name];
    }

    private static void FillPathDictionary()
    {
        _pathDict = new Dictionary<string, string>();
        string uiPrefabPath = "Prefabs/UI/";
        _pathDict.Add("ToolTipText", uiPrefabPath + "TextToolTip");
    }
}