using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CaveBiom", menuName = "CaveBiom", order = 0)]
public class CaveBiom : ScriptableObject {
    public string biomName;
    public Eigenstate[] tiles;
    public Eigenstate[] objects;
}

[System.Serializable]
public class Eigenstate {
    public Object refer;
    public int probablity;
}