using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Setting setting;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private bool updateSetting;

    public bool _shadow {
        get {return setting.shadow == Setting.Switch.On;}
    }
    
    public void SetSetting() {
        foreach(Tile tile in caveManager._currentRoom.grid) {
            if(tile._objectRef == null) continue;
            QualitySupport qualitySupport = tile._objectRef.GetComponent<QualitySupport>();
            if(qualitySupport == null) continue;
            if(tile._objectRef.layer == LayerMask.NameToLayer("Grass")) qualitySupport.SetQuality(setting.grassDensity);
        }
    }

    private void LateUpdate() {
        if(updateSetting) {
            SetSetting();
            updateSetting = false;
        } 
    }
}
