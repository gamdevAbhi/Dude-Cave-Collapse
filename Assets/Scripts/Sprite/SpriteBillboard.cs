using UnityEngine;

public class SpriteBillboard : MonoBehaviour {
    private Camera _mainCamera;
    private GameObject shadow;
    private Vector3 shadowEuler = new Vector3(90f, 0f, 0f);

    public GameObject shadowPrefab;
    public bool isStatic = false;
    public bool isAcute = false;
    public bool hasShadow = false;

    private void Start() {
        _mainCamera = Camera.main;
        if(hasShadow == true) {
            shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
            shadow.transform.eulerAngles = shadowEuler;
        }
    }

    public void LateUpdate() {
        if(isStatic) transform.rotation = _mainCamera.transform.rotation;
        else transform.LookAt(_mainCamera.transform, Vector3.up);

        if(isAcute) transform.eulerAngles = new Vector3(45f, transform.eulerAngles.y, 0f);
        else transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        if(hasShadow == false && shadow != null) {
            Destroy(shadow);
            shadow = null;
        }
        else if(hasShadow == true && shadow == null) {
            shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
            shadow.transform.eulerAngles = shadowEuler;
        }

        if(hasShadow == true) {
            shadow.transform.position = transform.position;
            shadow.transform.eulerAngles = shadowEuler;
        }
    }
}
