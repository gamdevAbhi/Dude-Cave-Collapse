using UnityEngine;

public class WaterManager : MonoBehaviour
{
    [Header("Referemce")]
    [SerializeField] private Renderer waterRenderer;
    [Header("Scale Properties")]
    [SerializeField] private float xScaleUnit = 1f;
    [SerializeField] private float yScaleUnit = 1f;
    [SerializeField] private float rippleSize = 0.5f;

    private Vector3 waterPosition = new Vector3(0.5f,0.1f,0.5f);

    public void ScaleWater(float x, float y) {
        transform.localScale = new Vector3(x * xScaleUnit, 1f, y * yScaleUnit);
        waterRenderer.material.SetFloat("_Size", Mathf.Max(x, y) * rippleSize);
    }
}
