using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float moveTime;
    public AudioSource failSound;

    private string lastMove;
    private GameObject pivot_1; 
    private GameObject pivot_2; 
    private PuzzlesController puzzleController;
    private List<Transform> collidingObjects;

    private void Start()
    {
        collidingObjects = new List<Transform>();
        pivot_1 = gameObject.transform.Find("pivot1").gameObject;
        pivot_2 = gameObject.transform.Find("pivot2").gameObject;
        puzzleController = GameObject.Find("Background").GetComponent<PuzzlesController>();
        BoundsGenerate();
    }

    private void Update()
    {
        if (collidingObjects.Count > 0)
        {
            AddCubes();
        }
    }

    public void MoveRevert()
    {
        if (lastMove == "W")
        {
            MoveToPosition("S");
        }
        else if (lastMove == "S")
        {
            MoveToPosition("W");
        }
        else if (lastMove == "A")
        {
            MoveToPosition("D");
        }
        else if (lastMove == "D")
        {
            MoveToPosition("A");
        }
        //TODO Dodaj prawidłowy dźwięk przy anulowaniu
        failSound.Play();
    }

    public void CheckCollisionWithButton()
    {
        bool isTouchingButton = false;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                RaycastHit hit;
                isTouchingButton = Physics.Raycast(child.position, Vector3.down, out hit, 1, 1 << 8);
                if (isTouchingButton == true &&
                   (hit.collider.CompareTag("Finish")
                   || hit.collider.CompareTag("Blue")
                   || hit.collider.CompareTag("Red")
                   || hit.collider.CompareTag("Green")))
                {
                    transform.parent.GetComponent<PlayersManager>().ButtonPush();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        collidingObjects.Add(other.transform);
    }

    private void AddCubes()
    {
        bool isCollidingWithCube = false;
        bool isCollidingWithGlue = false;
        foreach (Transform collidingObject in collidingObjects)
        {
            if (collidingObject.tag == "Background")
            {
                isCollidingWithCube = true;
                break;
            }
            else if (collidingObject.tag == "GlueYellow")
            {
                isCollidingWithGlue = true;
            }
        }

        if (isCollidingWithCube)
        {
            MoveRevert();
        }
        else if (isCollidingWithGlue)
        {
            GameObject parentGameObject = collidingObjects[0].parent.gameObject;
            int childCount = parentGameObject.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform currentChild = parentGameObject.transform.GetChild(0);

                if (currentChild.CompareTag("Background"))
                {
                    currentChild.tag = "Player";
                    currentChild.GetComponent<Collider>().isTrigger = false;
                }

                currentChild.transform.parent = transform;
            }

            Destroy(parentGameObject);
        }
        collidingObjects.Clear();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Blue" && GameObject.Find("BluePuzzle").GetComponent<Animator>().GetBool("BlueDown"))
        {
            puzzleController.MoveBlue(false);
        }
    }

    //Moves and rotates the position of the figure
    public void MoveToPosition(string direction)
    { 
        var currentRot = transform.rotation;
        lastMove = direction;

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
                    RaycastHit hit;
                    if (Physics.Raycast(child.position, Vector3.down * 2, out hit, 2, 1 << 8))
                    {
                        if (hit.collider.CompareTag("Respawn"))
                        { 
                            MoveRevert();
                            return;
                        }
                    }
                }
            }
        CheckCollisionWithButton();
    }

    // Uncomment when we will need Debug gizmos
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(bounds.center, bounds.size);
    //}

    public void BoundsGenerate()
    {
        Bounds bounds = new Bounds(gameObject.transform.position, new Vector3(1, 1, 1));
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
