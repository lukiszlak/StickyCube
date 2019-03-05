using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits_Controller : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject creditsObject;
    //public GameObject creditsSong;

    public Rigidbody2D rb;

    void Awake ()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.y >= 6400)
        {
            CreditsStop();
        }
    }

    public void CreditsStart()
    {
        mainMenu.SetActive(false);
        //creditsSong.SetActive(true);
        creditsObject.SetActive(true);
        rb.velocity = Vector2.up * 80;
    }

    public void CreditsStop()
    {
        mainMenu.SetActive(true);
        //creditsSong.SetActive(false);
        creditsObject.SetActive(false);
        rb.velocity = new Vector2(0, 0);
        transform.position = new Vector2(transform.position.x, -478);
    }
}
