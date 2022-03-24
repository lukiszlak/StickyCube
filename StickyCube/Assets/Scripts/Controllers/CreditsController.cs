using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject creditsMenu;

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
        creditsMenu.SetActive(true);
        rb.velocity = Vector2.up * 80;
    }

    public void CreditsStop()
    {
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        rb.velocity = new Vector2(0, 0);
        transform.position = new Vector2(transform.position.x, -478);
    }
}
