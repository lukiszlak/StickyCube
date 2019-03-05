using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float moveTime;


    private float currentTime;
    private bool revert = false;
    private string lastMove;
    private GameObject pivot_1; 
    private GameObject pivot_2; 
    private GameObject puzzleController;
    public AudioSource failSound; //
    private RaycastHit hit;
    private Bounds bounds;

    private void Start()
    {
        pivot_1 = gameObject.transform.Find("pivot1").gameObject;
        pivot_2 = gameObject.transform.Find("pivot2").gameObject;
        puzzleController = GameObject.Find("Background");
        BoundsGenerate();
    }

    public void MoveRevert()
    {
        if (lastMove == "W")
        {
            MoveToPosition("S");
            Debug.Log("COFAM");
        }
        else if (lastMove == "S")
        {
            MoveToPosition("W");
            Debug.Log("COFAM");
        }
        else if (lastMove == "A")
        {
            MoveToPosition("D");
            Debug.Log("COFAM");
        }
        else if (lastMove == "D")
        {
            MoveToPosition("A");
            Debug.Log("COFAM");
        }
        //TODO Dodaj prawidłowy dźwięk przy anulowaniu
        failSound.Play();
    }



    public void ButtonSearch()
    {
        bool isTouchingButton = false;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                isTouchingButton = Physics.Raycast(child.position, Vector3.down, out hit, 1, 1 << 8);
                if (isTouchingButton == true &&
                   (hit.collider.CompareTag("Finish")
                   || hit.collider.CompareTag("Blue")
                   || hit.collider.CompareTag("Red")
                   || hit.collider.CompareTag("Green")))
                {
                    transform.parent.GetComponent<CubeController>().ButtonPush();
                }
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Blue" && GameObject.Find("BluePuzzle").GetComponent<Animator>().GetBool("BlueDown"))
        {
            puzzleController.GetComponent<PuzzlesController>().MoveBlue(false);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MoveRevert();
        }
    }

    //Moves and rotates the position of the figure
    public void MoveToPosition(string direction)
    { 
        var currentRot = transform.rotation;
        lastMove = direction;

        revert = false;
        BoundsGenerate();

            switch (direction)
            {
                //Checks which direction to move
                case "W":
                    transform.RotateAround(pivot_1.transform.position, Vector3.right, 90);
                    break;
                case "S":
                    transform.RotateAround(pivot_2.transform.position, Vector3.left, 90);
                    break;
                case "A":
                    transform.RotateAround(pivot_2.transform.position, Vector3.forward, 90);
                    break;
                case "D":
                    transform.RotateAround(pivot_1.transform.position, Vector3.back, 90);
                    break;
            }

            BoundsGenerate();

            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player"))
                {
                    if (Physics.Raycast(child.position, Vector3.down * 2, out hit, 2, 1 << 8))
                    {
                        if (hit.collider.CompareTag("Respawn"))
                        { 
                            MoveRevert();
                            Debug.Log("Powinienem spaść");
                            return;
                        }
                    }
                }
            }
        ButtonSearch();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    public void BoundsGenerate()
    {
        bounds = new Bounds(gameObject.transform.position, new Vector3(1, 1, 1));
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                bounds.Encapsulate(child.GetComponent<Renderer>().bounds);
            }

        }
        pivot_1.transform.position = new Vector3(Mathf.Round(bounds.max.x), Mathf.Round(bounds.min.y), Mathf.Round(bounds.max.z));
        pivot_2.transform.position = new Vector3(Mathf.Round(bounds.min.x), Mathf.Round(bounds.min.y), Mathf.Round(bounds.min.z));
    }
}
