using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CaveProb", menuName = "Object/CaveProb", order = 0)]
public class CaveProb : ScriptableObject {
    public Prob[] probs;
    public bool shouldUpdate;
    public int totalPer;

    private void OnValidate() {
        if(shouldUpdate == true) {
            FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this))).GetFiles("*.asset");

            if(files.Length - 1 == 0) {
                probs = null;
                return;
            }
            else probs = new Prob[files.Length - 1];
            
            int percentage = 100 / files.Length - 1;

            int index = 0;
            for(int i = 0; i < files.Length; i++) {
                if(files[i].Name.Split('.')[0] == this.name) continue;

                probs[index] = new Prob();
                probs[index].objectName = files[i].Name.Split('.')[0];
                probs[index].probablity = percentage;
                index++;
            }

            if(percentage * (files.Length - 1)  != 100 && probs.Length > 0) 
                probs[0].probablity += 100 - (percentage * (files.Length - 1));
            
            shouldUpdate = false;
        }
        
        if(probs.Length == 0) return;

        totalPer = 0;

        foreach(Prob prob in probs) totalPer += prob.probablity;

        if(totalPer > 100 && probs.Length > 0) {
            probs[0].probablity -= totalPer - 100;
            totalPer = 100;
        }
    }

    public int GetProb(string objName) {
        foreach(Prob prob in probs) {
            if(prob.objectName == objName) return prob.probablity;
        }

        return 0;
    }

    [System.Serializable]
    public class Prob {
        public string objectName;
        [Range(0, 100)] public int probablity;
    }
}
