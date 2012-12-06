using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Creatable))]
public class CreatableInspector : Editor {
	
    public override void OnInspectorGUI() {
        Creatable creatable = target.GetComponent<Creatable>();
		
        if(creatable.CompareTag("Building")) {
            creatable.buildProgressObject = EditorGUILayout.ObjectField("Build Progress Object", typeof(BuildProgress));
        }
	}
}
