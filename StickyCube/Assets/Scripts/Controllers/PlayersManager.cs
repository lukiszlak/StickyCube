using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour {
   
    private PlayerController[] players;
    private Camera camera;

    private MenuController menuController;
    private PuzzlesController puzzleController;
    private int playerCount = 0; 
    private int currentPlayerNumber = 0;
    private float cameraTimeHolder;
    private bool cameraMoving = false;

    public enum Direction {Forward, Back, Left, Right };

    private void Awake()
    {
        camera = Camera.main;

        players = new PlayerController[transform.childCount];
        playerCount = transform.childCount;
        puzzleController = GameObject.Find("Background").GetComponent<PuzzlesController>();
        menuController = GameObject.Find("MenuController").GetComponent<MenuController>();

        for (int i = 0; i < transform.childCount; i++)
        {
            players[i] = transform.GetChild(i).transform.GetComponent<PlayerController>();
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        Debug.DrawRay(transform.GetChild(currentPlayerNumber).transform.position, Vector3.down, Color.red);

        //if (Input.GetKeyDown(KeyCode.I) /*&& figureBoxCount > 1*/)
        //{
        //    DetachGlueFigure();
        //}

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    ChangePlayer();
        //}

        if (cameraMoving)
        {
            float cameraTimeToMove = cameraTimeHolder - Time.time;
            if (cameraTimeToMove <= 0)
            {
                cameraMoving = false;
                return;
            }
            Vector3 cameraPositionHolder = camera.transform.position; 
            camera.transform.position = new Vector3(Mathf.Lerp(cameraPositionHolder.x, players[currentPlayerNumber].transform.position.x + 6, cameraTimeToMove), cameraPositionHolder.y, cameraPositionHolder.z);
        }
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            switch (context.action.name)
            {
                case "Forward":
                    Move(Vector3.forward);
                    break;
                case "Back":
                    Move(Vector3.back);
                    break;
                case "Left":
                    Move(Vector3.left);
                    break;
                case "Right":
                    Move(Vector3.right);
                    break;
                default:
                    break;
            } 
        }
    }

    private void Move(Vector3 direction)
    {
            players[currentPlayerNumber].MoveToPosition(direction);
    }

    public void ChangePlayer()
    {
        if (currentPlayerNumber < playerCount -1)
        {
            currentPlayerNumber++;
        }
        else
        {
            currentPlayerNumber = 0;
        }
        cameraMoving = true;
        cameraTimeHolder = Time.time + 30;
    }

    public void ButtonPush()
    {
        int x = 0;

        bool isTouchingButton = false;

        foreach (Transform child in players[currentPlayerNumber].transform)
        {
            RaycastHit hit;
            isTouchingButton = Physics.Raycast(child.position, Vector3.down, out hit, 1, 1 << 8);
            if (child.CompareTag("Player"))
            {
                if (isTouchingButton &&
                    (hit.collider.CompareTag("Finish")
                    || hit.collider.CompareTag("Blue")
                    || hit.collider.CompareTag("Red")
                    || hit.collider.CompareTag("Green")))
                {
                    x++;
                }
                if (isTouchingButton &&
                   (hit.transform.GetComponent<FloorValuesHolder>() && x == hit.transform.GetComponent<FloorValuesHolder>().x))
                {
                    switch (hit.transform.tag)
                    {
                        case "Finish":
                            menuController.LevelEnd();
                            break;
                        case "Red":
                            ChangePlayer();
                            break;
                        case "Green":
                            puzzleController.AddPlayersCollidingWithButtons(1);
                            break;
                    }
                }
            }
        }
    }

    ///// Debug Functions

    public void DetachGlueFigure()
    {
        players[currentPlayerNumber].DetachGlueFigure();
    }

    /////
}
