using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubeController : MonoBehaviour {


    public int showMeDatFinishRaycastCount;

    public int cubesAmmount;
    public int cubesToFinish;
    public int cubesHittingFinish;
    public int playersAmmount;
    public bool secondPlayerControlled = false;
    
    public GameObject[] players;
    public Camera camera;

    private GameObject levelEndScreen;
    private GameObject controlButtons;
    private GameObject restartButtons;
    private GameObject puzzleController;
    public Vector3 cameraPlacement;
    private RaycastHit hit;
    public int playerNumber = 0; //
    public int currentPlayerNumber = 0;
    private float cameraTimeHolder;
    private string lastMove;
    private string activeScene;
    private bool cameraMoving = false;

    private void Awake()
    {
        camera = Camera.main;

        players = new GameObject[transform.childCount];
        playerNumber = transform.childCount;
        puzzleController = GameObject.Find("Background");

        for (int i = 0; i < transform.childCount; i++)
        {
            players[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        Setup();
        levelEndScreen = GameObject.Find("LevelEnd");
        controlButtons = GameObject.Find("Controls");
        restartButtons = GameObject.Find("PauseButtons");
        levelEndScreen.SetActive(false);
        Time.timeScale = 1;
        activeScene = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        Debug.DrawRay(players[currentPlayerNumber].gameObject.transform.position, Vector3.down, Color.red);

        if (Input.GetKeyDown(KeyCode.I) /*&& figureBoxCount > 1*/)
        {
            DetachBox();
        }

        // TODO change it to something more sensible
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("W");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("S");
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("A");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("D");
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
            camera.transform.position = new Vector3(Mathf.Lerp(camera.transform.position.x, players[currentPlayerNumber].transform.position.x + 6, cameraTimeToMove), cameraPlacement.y, cameraPlacement.z);
        }
    }

    public void Move(string keyPressed)
    {
        if (keyPressed == "W" || keyPressed == "S" || keyPressed == "A" || keyPressed == "D")
        {
            players[currentPlayerNumber].gameObject.transform.GetComponent<PlayerController>().MoveToPosition(keyPressed);
        }

    }

    public void ChangePlayer()
    {
        if (currentPlayerNumber < playerNumber -1)
        {
            currentPlayerNumber++;
        }
        else
        {
            currentPlayerNumber = 0;
        }
        cameraPlacement = camera.transform.position;
        cameraMoving = true;
        cameraTimeHolder = Time.time + 30;
    }

    private void Setup()
    {
        //TODO Redo this shit it attaches player function to the buttons soo they can move
        GameObject.Find("Right").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("D");});
        GameObject.Find("Left").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("A");});
        GameObject.Find("Down").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("S");});
        GameObject.Find("Up").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("PlayerContainer").GetComponent<CubeController>().Move("W");});
    }

    public void LevelEnd()
    {
        levelEndScreen.SetActive(true);
        restartButtons.SetActive(false);
        controlButtons.SetActive(false);
        Time.timeScale = 0;
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
                            LevelEnd();
                            break;
                        case "Blue":
                            puzzleController.GetComponent<PuzzlesController>().MoveBlue(true);
                            break;
                        case "Red":
                            ChangePlayer();
                            break;
                        case "Green":
                            puzzleController.GetComponent<PuzzlesController>().MoveBlue(true);
                            break;
                    }
                }
            }
        }
    }
}
