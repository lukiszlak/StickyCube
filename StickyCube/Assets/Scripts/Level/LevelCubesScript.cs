using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCubesScript : MonoBehaviour
{
    // TODO check if we can just move this function inside LevelGenerator
    public int i;
    public int j;
    public LevelGenerator parent;

    enum State { EditMode, DeleteMode, PlayMode };
    State myState = State.EditMode;
    private bool isGenerated = false; // TODO check if we need this when generating levels
    private Text GameModeText;

    void Start()
    {
        parent = GameObject.Find("FloorGenerator").GetComponent<LevelGenerator>();
        GameModeText = GameObject.Find("GameMode").GetComponent<Text>();
        GameModeText.text = "Edit Mode";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            myState = State.PlayMode;
            GameModeText.text = "Play Mode";
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            myState = State.EditMode;
            GameModeText.text = "Edit Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            myState = State.DeleteMode;
            GameModeText.text = "Delete Mode";
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
            GameObject objectToDelete = GameObject.Find(string.Format("Background/FloorContainer/{0}_{1}", i, j));

            if (objectToDelete)
            {
                Destroy(objectToDelete);
                isGenerated = false;
            }
        }
        else if (other.CompareTag("Finish"))
        {
            Destroy(gameObject);
        }
    }
}