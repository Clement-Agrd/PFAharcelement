using System;
using System.Collections.Generic;

[Serializable]
public class StatTreeSaveData
{
    public int              totalXP       = 0;
    public int              spentXP       = 0;
    public List<NodeSave>   nodes         = new List<NodeSave>();
}

[Serializable]
public class NodeSave
{
    public string statTypeName;
    public int    currentLevel;
}