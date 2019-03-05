using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesController : MonoBehaviour {

    private Animator animator;
    private GameObject bluePuzzle;

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

    public void MoveBlue(bool isDown)
    {
        animator.SetBool("BlueDown", isDown);
    }
}