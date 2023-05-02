using UnityEngine;

[CreateAssetMenu(fileName = "New Setting", menuName = "Setting", order = 2)]
public class Setting : ScriptableObject {
    public enum Quality {Low, Medium, High};
    public enum Switch {On, Off};
    public Quality grassDensity;
    public Switch shadow;
}
