using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Object", order = 1)]
public class Object : ScriptableObject {
    public enum Enviroment {Ground, Water}
    public GameObject objectRef;
    [Range(-5f, 5f)] public float increaseChance;
    public int[] depth = new int[2] {1, int.MaxValue};
    public Enviroment env;
    public bool walkable = true;
}
