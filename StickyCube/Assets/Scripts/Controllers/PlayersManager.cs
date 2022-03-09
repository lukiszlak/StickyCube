using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayersManager : MonoBehaviour {
   
    private PlayerController[] players;
    private Camera camera;

    private MenuController menuController;
    private PuzzlesController puzzleController;
    private int playerCount = 0; 
    private int currentPlayerNumber = 0;
    private float cameraTimeHolder; // TODO check if it does anything
    private bool cameraMoving = false;

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
        Setup();
        Time.timeScale = 1;
    }

    void Update()
    {
        Debug.DrawRay(transform.GetChild(currentPlayerNumber).transform.position, Vector3.down, Color.red);

        if (Input.GetKeyDown(KeyCode.I) /*&& figureBoxCount > 1*/)
        {
            DetachBox();
        }

        // TODO change it to something more sensible
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move("W");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move("S");
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move("A");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move("D");
        }
        //

        if (Input.anyKeyDown)
        {
            Move(Input.inputString);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangePlayer();
        }

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

    public void Move(string keyPressed)
    {
        if (keyPressed == "W" || keyPressed == "S" || keyPressed == "A" || keyPressed == "D")
        {
            players[currentPlayerNumber].MoveToPosition(keyPressed);
        }

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

    private void Setup()
    {
        //TODO Redo this shit it attaches player function to the buttons soo they can move
        //GameObject.Find("Right").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("D");});
        //GameObject.Find("Left").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("A");});
        //GameObject.Find("Down").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("S");});
        //GameObject.Find("Up").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("W");});
    }

    public void DetachBox()
    {
            GameObject tempBox = GameObject.Find("box");
            GameObject tempGlue = GameObject.Find("glue");
            var glueBoxHolder = new GameObject("GlueFigure");
            glueBoxHolder.gameObject.transform.position = tempBox.transform.position;
            tempBox.transform.SetParent(glueBoxHolder.transform);
            tempGlue.transform.SetParent(glueBoxHolder.transform);
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
                        case "Blue":
                            puzzleController.MoveBlue(true);
                            break;
                        case "Red":
                            ChangePlayer();
                            break;
                        case "Green":
                            puzzleController.MoveBlue(true);
                            break;
                    }
                }
            }
        }
    }
}
