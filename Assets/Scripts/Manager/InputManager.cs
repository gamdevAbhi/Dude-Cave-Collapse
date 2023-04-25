using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    [SerializeField] private List<KeyInput> inputList;
    public Dictionary<string, KeyInput> inputs;

    public void Awake() {
        inputs = new Dictionary<string, KeyInput>();

        foreach(KeyInput key in inputList) {
            inputs.Add(key.action, key);
        }
    }

    public void Update() {
        foreach(KeyInput key in inputList) {
            key.CheckInputs();
        }
    }
}

[Serializable]
public class KeyInput {
    public string action;
    public KeyCode primaryKey;
    public KeyCode secondaryKey;
    public Action onDown;
    public Action onPress;
    public Action onRelease;

    public void CheckInputs() {
        if(primaryKey != KeyCode.None
        && Input.GetKeyDown(primaryKey)
        || secondaryKey != KeyCode.None
        && Input.GetKeyDown(secondaryKey)) {
            if(onDown != null) onDown.Invoke();
        }
        else if(primaryKey != KeyCode.None
        && Input.GetKeyUp(primaryKey)
        || secondaryKey != KeyCode.None
        && Input.GetKeyUp(secondaryKey)) {
            if(onRelease != null) onRelease.Invoke();
        }

        if(primaryKey != KeyCode.None
        && Input.GetKey(primaryKey)
        || secondaryKey != KeyCode.None
        && Input.GetKey(secondaryKey)) {
            if(onPress != null) onPress.Invoke();
        }
    }
}
