using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CaveBiom", menuName = "CaveBiom", order = 0)]
public class CaveBiom : ScriptableObject {
    public string biomName;
    public Eigenstate[] tiles;
    public int tilePer;
    public Eigenstate[] objects;
    [Range(0,100)] public int objectProbablity;
    public int objectPer;

    public int GetPer(Eigenstate[] states) {
        int per = 0;

        foreach(Eigenstate state in states) per += state.probablity;

        return per;
    }

    public void OnValidate() {
        tilePer = GetPer(tiles);
        objectPer = GetPer(objects);
    }
}

[System.Serializable]
public class Eigenstate {
    public Object refer;
    [Range(0, 100)] public int probablity;
}