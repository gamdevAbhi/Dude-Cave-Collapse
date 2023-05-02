using UnityEngine;

public class SpriteBillboard : MonoBehaviour {
    [Header("Reference")]
    public GameObject shadowPrefab;
    [Header("Billboard Properties")]
    public bool isAcute = false;
    public bool hasShadow = false;
    
    private Camera _mainCamera;
    private SettingManager settingManager;
    private GameObject shadow;
    private Vector3 shadowEuler = new Vector3(90f, 0f, 0f);

    private void Awake() {
        _mainCamera = Camera.main;

        if(hasShadow == true) {
            shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
            GameObject prefab = GameObject.Find("Shadow");
            if(prefab == null) prefab = new GameObject("Shadow");
            shadow.transform.parent = prefab.transform;
            shadow.transform.eulerAngles = shadowEuler;
        }

        settingManager = GameObject.FindGameObjectWithTag("Setting Manager").GetComponent<SettingManager>();
    }

    private void OnDisable() {
        if(shadow != null) shadow.gameObject.SetActive(false);
    }

    private void OnEnable() {
        if(settingManager._shadow == false && shadow != null) shadow.gameObject.SetActive(false);
        else if(shadow != null) shadow.gameObject.SetActive(true);
    }

    private void OnDestroy() {
        if(shadow != null) Destroy(shadow);    
    }

    public void LateUpdate() {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
        _mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        
        if(isAcute) transform.eulerAngles = new Vector3(45f, transform.eulerAngles.y, transform.eulerAngles.z);
        else transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);

        if(settingManager._shadow == false && shadow != null) shadow.gameObject.SetActive(false);
        else if(shadow != null) {
            shadow.gameObject.SetActive(true);
            shadow.transform.position = transform.position;
        }
    }
}
