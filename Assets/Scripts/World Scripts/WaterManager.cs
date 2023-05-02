using UnityEngine;

public class WaterManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Renderer waterRenderer;
    [Header("Water Properties")]
    [SerializeField] private float xScaleUnit = 1f;
    [SerializeField] private float yScaleUnit = 1f;
    [SerializeField] private float rippleSize = 0.5f;

    private Vector3 waterPosition = new Vector3(0.3f,0.1f,0.3f);
    
    private void Start() {
        waterRenderer.gameObject.SetActive(true);
        transform.position = waterPosition;
    }
    public void ScaleWater(float x, float y) {
        transform.localScale = new Vector3(x * xScaleUnit + waterPosition.x * 2, 1f, y * yScaleUnit + waterPosition.z * 2);
        waterRenderer.material.SetFloat("_Size", Mathf.Min(x, y) * rippleSize);
    }
}
