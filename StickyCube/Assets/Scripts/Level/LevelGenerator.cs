using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int verticalGrid;
    public int horizontalGrid;
    public GameObject levelTrigger;
    public GameObject levelCube;

    private GameObject deathVolume;
    [SerializeField]
    public GameObject[,] checker;

    private void Awake()
    {
        checker = new GameObject[verticalGrid, horizontalGrid];
        deathVolume = GameObject.Find("Background/DeathVolume");
        deathVolume.SetActive(false);
    }

    void Start ()
    {

        // TODO Move it to function called creating grid or something like this
        for (int i = 0; i < verticalGrid; i++)
        {
            for (int j = 0; j < horizontalGrid; j++)
            {
                Vector3 moveFromEmpty = new Vector3((transform.position.x + i), transform.position.y, (transform.position.z + j));
                checker[i, j] = Instantiate(levelTrigger, GameObject.Find("Background/TriggerContainer").transform);
                checker[i, j].gameObject.name = (i + "_" + j);
                checker[i, j].transform.position = moveFromEmpty;
                checker[i, j].GetComponent<LevelCubesScript>().i = i;
                checker[i, j].GetComponent<LevelCubesScript>().j = j;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            deathVolume.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            deathVolume.SetActive(true);
        }
    }

    public void CreateCheckerArray(int x, int y)
    {
        if (GameObject.Find("Background/FloorContainer/" + x + "_" + y))
        {
            return;
        }
        Vector3 moveFromEmpty = new Vector3((transform.position.x + x), transform.position.y, (transform.position.z + y));
        GameObject temporaryContainer = Instantiate(levelCube, GameObject.Find("Background/FloorContainer").transform);
        temporaryContainer.transform.position = moveFromEmpty;
        temporaryContainer.name = x + "_" + y;
        temporaryContainer.transform.parent = GameObject.Find("FloorContainer").transform;
    }

    public void GenerateTheCube(int i,int j)
    {
        if (GameObject.Find("Background/FloorContainer/" + i + "_" + j))
        {
            return;
        }
        Vector3 posHolder = checker[i, j].transform.position;
        GameObject cubeHolder = Instantiate(levelCube, GameObject.Find("Background/FloorContainer").transform);
        cubeHolder.transform.Translate(posHolder);
        cubeHolder.gameObject.GetComponent<FloorValuesHolder>().SetXandY(i, j);
        cubeHolder.gameObject.name = i + "_" + j;
    }

}