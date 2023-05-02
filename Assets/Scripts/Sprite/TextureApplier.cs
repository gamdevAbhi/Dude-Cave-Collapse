using UnityEngine;

public class TextureApplier : MonoBehaviour
{
    [SerializeField] private Texture2D mainTex;
    
    private void Awake() {
        if(GetComponent<MeshRenderer>() != null) {
            GetComponent<Renderer>().material.SetTexture("_BaseMap", mainTex);
        }

        Destroy(this);
    }
}
