using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSave))]
public class FloorLoadingInEditor : Editor
{
    private GameObject objectHolder;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSave mySaveScript = (LevelSave)target;

        if (GUILayout.Button("Load Saved Scene"))
        {
            mySaveScript.Load();
        }

        if (GUILayout.Button("Clear Scene"))
        {
            objectHolder = GameObject.Find("FloorContainer");
            for (int i = objectHolder.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(objectHolder.transform.GetChild(i).gameObject);
            }
        }
    }
}
