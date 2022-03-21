using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesController : MonoBehaviour {

    private Animator animator;
    private GameObject bluePuzzle;
    public int PlayersCollidingWithButton;

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

    public void AddPlayerCollidingWithButton(int ammount)
    {
        // TODO move it to 2 functions for SOLID
        int OldPlayersCollidingWithButton = PlayersCollidingWithButton;
        PlayersCollidingWithButton += ammount;

        if (PlayersCollidingWithButton == 0 && OldPlayersCollidingWithButton != 0)
        {
            MoveBlue(false);
        }
        else if (PlayersCollidingWithButton != 0 && OldPlayersCollidingWithButton == 0)
        {
            MoveBlue(true);
        }
    }
}