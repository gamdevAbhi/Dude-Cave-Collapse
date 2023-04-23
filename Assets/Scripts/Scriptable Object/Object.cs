using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Object/CreateObject", order = 0)]
public class Object : ScriptableObject {
    public GameObject objectRef;
    [Range(-100,100)] public int matchChange;
    public int[] depth = new int[2] {1, int.MaxValue};
}
