using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int MaxMovementIndex = 20;
    public AudioSource failSound;

    private string lastMove;
    private string currentDirection;
    private bool recentlyMoved = false;
    private bool reverted = false;
    private bool IsMoving = false;
    private int MovementIndex = -1;
    private GameObject pivot_1; 
    private GameObject pivot_2;
    private Transform background;
    private PuzzlesController puzzleController;
    private List<Transform> collidingObjects;

    private void Start()
    {
        collidingObjects = new List<Transform>();
        pivot_1 = gameObject.transform.Find("pivot1").gameObject;
        pivot_2 = gameObject.transform.Find("pivot2").gameObject;
        background = GameObject.Find("Background").transform;
        puzzleController = background.GetComponent<PuzzlesController>();
        BoundsGenerate();
    }

    private void Update()
    {
        if (IsMoving)
        {
            RotatePlayer();
        }
        else if (collidingObjects.Count > 0 && recentlyMoved)
        {
            AddCubes();
            recentlyMoved = false;
        }
        else
        {
            collidingObjects.Clear(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        collidingObjects.Add(other.transform);

        if (other.transform.tag == "Blue" && reverted == false)
        {
            puzzleController.AddPlayersCollidingWithButtons(1);
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Blue")
        {
            puzzleController.AddPlayersCollidingWithButtons(-1);
        }
    }

    private void AddCubes()
    {
        bool isCollidingWithCube = false;
        bool isCollidingWithGlue = false;
        Transform collidingObjectTransform = null;

        foreach (Transform collidingObject in collidingObjects)
        {
            if (collidingObject.tag == "Background")
            {
                isCollidingWithCube = true;
                break;
            }
            else if (collidingObject.tag == "GlueYellow")
            {
                collidingObjectTransform = collidingObject;
                isCollidingWithGlue = true;
                //break;
            }
        }

        if (isCollidingWithCube)
        {
            MoveRevert();
        }
        else if (isCollidingWithGlue && collidingObjectTransform && IsMoving == false)
        {
            GameObject parentGameObject = collidingObjectTransform.parent.gameObject;
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
    }

    //Moves and rotates the position of the figure
    public void MoveToPosition(string direction)
    {
        if (IsMoving == true)
        {
            return;
        }
        else if (CanPlayerMoveToPosition(direction))
        {
            var currentRot = transform.rotation;
            currentDirection = direction;
            lastMove = direction;
            reverted = false;
            IsMoving = true;
            BoundsGenerate();
        }
        else
        {
            failSound.Play();
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
        reverted = true;
        IsMoving = true;
        failSound.Play();
    }

    private void RotatePlayer()
    {
        switch (currentDirection)
        {
            //Checks which direction to move
            case "W":
                transform.RotateAround(pivot_1.transform.position, Vector3.right, 90 / MaxMovementIndex);
                break;
            case "S":
                transform.RotateAround(pivot_2.transform.position, Vector3.left, 90 / MaxMovementIndex);
                break;
            case "A":
                transform.RotateAround(pivot_2.transform.position, Vector3.forward, 90 / MaxMovementIndex);
                break;
            case "D":
                transform.RotateAround(pivot_1.transform.position, Vector3.back, 90 / MaxMovementIndex);
                break;
        }

        MovementIndex++;

        if (MovementIndex >= MaxMovementIndex)
        {
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
            MovementIndex = 0;
            recentlyMoved = true;
            IsMoving = false;
        }
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

    ///// Debug Functions

    private bool CanPlayerMoveToPosition(string direction) // TODO change name of destination to offset or something like that, change direction to enum
    {
        Vector3 movementOffset = Vector3.zero;

        switch (direction)
        {
            case "W":
                movementOffset = Vector3.forward;
                break;
            case "A":
                movementOffset = Vector3.left;
                break;
            case "S":
                movementOffset = Vector3.back;
                break;
            case "D":
                movementOffset = Vector3.right;
                break;

        }

        foreach (Transform child in transform)
        {
            if (child.tag == "Player")
            {
                if (background.position.y + 1.5f < child.position.y)
                {
                    movementOffset += movementOffset;
                }

                RaycastHit hit;
                if (Physics.Raycast(child.position + movementOffset, Vector3.down * 2, out hit, 4, 1 << 8))
                {
                    if (hit.collider.CompareTag("Respawn"))
                    {
                        return false;
                    }
                }  
            }
        }
        
        return true;
    }

    public void DetachGlueFigure()
    {
        Transform tempBox = transform.Find("GlueBox");
        Transform tempGlue = transform.Find("GlueGlue");
        if (tempBox && tempGlue)
        {
            Transform glueBoxHolder = new GameObject("GlueFigure").transform;
            glueBoxHolder.position = tempBox.position;
            tempBox.parent = glueBoxHolder;
            tempGlue.parent = glueBoxHolder;
        }
        else
        {
            print("We couldn't detach sticky cube because We couldn't find its children");
            failSound.Play();
        }
    }
    
    /////

}
