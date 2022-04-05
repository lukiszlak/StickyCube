using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesController : MonoBehaviour {

    private Animator animator;
    private GameObject bluePuzzle;
    public int PlayersCollidingWithButtons;
    public bool ButtonOnPush;

    void Start()
    {
        bluePuzzle = GameObject.Find("Background/BluePuzzle").gameObject;
        animator = bluePuzzle.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetBool("BlueDown", !animator.GetBool("BlueDown"));
        }
    }

    private void MoveBlue(bool isDown)
    {
        animator.SetBool("BlueDown", isDown);
    }

    public void AddPlayersCollidingWithButtons(int ammount)
    {
        // TODO move it to 2 functions for SOLID
        int OldPlayersCollidingWithButton = PlayersCollidingWithButtons;
        if (ButtonOnPush == true || (ammount > 0 && PlayersCollidingWithButtons == 0 ))
        {
            PlayersCollidingWithButtons += ammount;
        }
        else if (ButtonOnPush == false && ammount > 0 && PlayersCollidingWithButtons > 0)
        {
            PlayersCollidingWithButtons -= ammount;
        }

        if (PlayersCollidingWithButtons == 0 && OldPlayersCollidingWithButton != 0)
        {
            MoveBlue(false);
        }
        else if (PlayersCollidingWithButtons != 0 && OldPlayersCollidingWithButton == 0)
        {
            MoveBlue(true);
        }
    }

    public bool IsBlueDown()
    {
        return animator.GetBool("BlueDown");
    }
}