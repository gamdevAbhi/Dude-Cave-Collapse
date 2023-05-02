using UnityEngine;

public class QualitySupport : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject high;
    [SerializeField] private GameObject medium;
    [SerializeField] private GameObject low;
    [SerializeField] private VisiblityDetector detector;
    private GameObject current;

    public void SetQuality(Setting.Quality quality) {
        high.SetActive(quality == Setting.Quality.High? true : false);
        medium.SetActive(quality == Setting.Quality.Medium? true : false);
        low.SetActive(quality == Setting.Quality.Low? true : false);

        current = (quality == Setting.Quality.Low)? low : (quality == Setting.Quality.Medium)? medium : high;
    }

    public void LateUpdate() {
        if(detector._isVisble) current.gameObject.SetActive(true);
        else current.gameObject.SetActive(false);
    }
}
