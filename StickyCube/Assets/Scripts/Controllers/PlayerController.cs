using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public AudioSource failSound;
    public float rotationTime;

    private Vector3 lastMove;
    private Vector3 currentDirection;
    private bool recentlyMoved = false;
    private bool reverted = false;
    private bool IsMoving = false;
    private int MovementIndex = 0;
    private int MaxMovementIndex = 90;
    private float rotationStartingTime;
    private float oldRotation;
    private Transform pivot_1; 
    private Transform pivot_2;
    private Transform background;
    private PuzzlesController puzzleController;
    private List<Transform> collidingObjects;

    private void Start()
    {
        collidingObjects = new List<Transform>();
        pivot_1 = gameObject.transform.Find("pivot1").gameObject.transform;
        pivot_2 = gameObject.transform.Find("pivot2").gameObject.transform;
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
        foreach (Transform collidingObject in collidingObjects)
        {
            if (collidingObject.tag == "GlueYellow" && collidingObject && IsMoving == false)
            {
                GameObject parentGameObject = collidingObject.parent.gameObject;
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

    }

    //Moves and rotates the position of the figure
    public void MoveToPosition(Vector3 direction)
    {
        if (IsMoving == true)
        {
            return;
        }
        else if (CanPlayerMoveToPosition(direction))
        {
            var currentRot = transform.rotation;
            currentDirection = direction;
            rotationStartingTime = Time.time;
            oldRotation = 0;
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

    private void RotatePlayer()
    {
        Vector3 ConvertedRotation = new Vector3(currentDirection.z, 0, -currentDirection.x);
        Vector3 currentPivotPosition;

        if (ConvertedRotation.x > 0 || ConvertedRotation.z < 0)
        {
            currentPivotPosition = pivot_1.position;
        }
        else
        {
            currentPivotPosition = pivot_2.position;
        }

        /// TODO refactor it, It was done just to work not to be perfect!
 
        float LerpIndex = (Time.time - rotationStartingTime) / rotationTime;
        float newRotation = Mathf.Lerp(0.0f, 90.0f, LerpIndex);
        float rotationStep = newRotation - oldRotation;
        oldRotation = newRotation;

        transform.RotateAround(currentPivotPosition, ConvertedRotation, rotationStep);

        //MovementIndex++;

        if (newRotation == 90)
        {
            CheckCollisionWithButton();
            MovementIndex = 0;
            recentlyMoved = true;
            IsMoving = false;
        }

        ///
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
        pivot_1.position = new Vector3(Mathf.Round(bounds.max.x), Mathf.Round(bounds.min.y), Mathf.Round(bounds.max.z));
        pivot_2.position = new Vector3(Mathf.Round(bounds.min.x), Mathf.Round(bounds.min.y), Mathf.Round(bounds.min.z));
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

    private bool CanPlayerMoveToPosition(Vector3 direction)
    {
        foreach (Transform child in transform)
        {
            int offsetMultiplier = 1;
            if (child.tag == "Player")
            {
                if (background.position.y + 1.5f < child.position.y)
                {
                    offsetMultiplier = 2;
                }

                Vector3 cubeNewPosition = child.position + (direction * offsetMultiplier) + Vector3.up;

                RaycastHit hit;

                if (Physics.Raycast(cubeNewPosition, Vector3.down, out hit, 4, 1 << 8))
                {
                    Debug.DrawRay(cubeNewPosition, Vector3.down, Color.red, 1.0f);
                    if (hit.collider.CompareTag("Respawn") || hit.transform.name == "GlueFigure" || (hit.transform.parent.name == "BluePuzzle" && !puzzleController.IsBlueDown()))
                    {
                        return false;
                    }
                    else if (hit.transform.parent.name == "BluePuzzle")
                    {

                    }
                }  
            }
        }
        
        return true;
    }

    ///// Debug Functions

    public void DetachGlueFigure()
    {
        if (IsPlayerHorizontall())
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
    }

    private bool IsPlayerHorizontall()
    {
        foreach (Transform child in transform)
        {
            if (background.position.y + 1.5f < child.position.y)
            {
                return false;
            }
        }
        return true;
    }

    /////

}
