using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Object", order = 1)]
public class Object : ScriptableObject {
    public enum Enviroment {Tile, Water}
    public GameObject objectRef;
    [Range(-100,100)] public int matchChange;
    public int[] depth = new int[2] {1, int.MaxValue};
    public Enviroment env;
}
