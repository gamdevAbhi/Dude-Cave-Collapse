using UnityEngine;

public class SpriteBillboard : MonoBehaviour {
    [Header("Reference")]
    public GameObject shadowPrefab;
    [Header("Billboard Properties")]
    public bool isStatic = false;
    public bool isAcute = false;
    public bool hasShadow = false;
    
    private GameObject shadow;
    private Vector3 shadowEuler = new Vector3(90f, 0f, 0f);
    private Camera _mainCamera;

    private void Start() {
        _mainCamera = Camera.main;

        if(hasShadow == true) {
            shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
            shadow.transform.eulerAngles = shadowEuler;
            GameObject prefab = GameObject.Find("Shadows");

            if(prefab == null) prefab = new GameObject("Shadows");
            
            shadow.transform.parent = prefab.transform;
        }
    }

    public void LateUpdate() {
        if(isStatic) transform.rotation = _mainCamera.transform.rotation;
        else transform.LookAt(_mainCamera.transform, Vector3.up);

        if(isAcute) transform.eulerAngles = new Vector3(45f, transform.eulerAngles.y, 0f);
        else transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        if(hasShadow == true) {
            shadow.transform.position = transform.position;
        }
    }

    private void OnDestroy() {
        if(shadow != null) Destroy(shadow);    
    }
}
