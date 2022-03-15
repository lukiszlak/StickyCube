using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCubesScript : MonoBehaviour
{
    public int i;
    public int j;
    public LevelGenerator parent;

    enum State { EditMode, DeleteMode, PlayMode };
    State myState = State.EditMode;
    private bool isGenerated = false; // TODO check if we need this when generating levels

    void Start()
    {
        parent = GameObject.Find("FloorGenerator").GetComponent<LevelGenerator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            myState = State.PlayMode;
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            myState = State.EditMode;
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            myState = State.DeleteMode;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && myState == State.EditMode && isGenerated == false)
        {
            parent.GenerateTheCube(i, j);
            isGenerated = true;
        }
        else if (other.CompareTag("Player") && myState == State.DeleteMode && isGenerated == true)
        {
            Destroy(GameObject.Find(i + "_" + j));
            isGenerated = false;
        }
    }
}