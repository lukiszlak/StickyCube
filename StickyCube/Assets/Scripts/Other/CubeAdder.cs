using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAdder : MonoBehaviour
{

	private int add = 0;
	private GameObject[] moveChildren;

	private void Start()
	{
		moveChildren = new GameObject[transform.childCount];
		foreach (Transform child in transform)
		{
			moveChildren[add] = child.gameObject;
			add++;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("GlueYellow"))
		{
			foreach (Transform child in transform)
			{
				if (child.CompareTag("Background"))
				{
					child.tag = "Player";
					child.GetComponent<Collider>().isTrigger = false;
					other.transform.parent.gameObject.GetComponentInParent<CubeController>().cubesAmmount += 1;
				}
			}

			for (int i = 0; i < moveChildren.Length; i++)
			{
				moveChildren[i].transform.parent = other.transform.parent;
			}
            Destroy(gameObject);
		}
	}
}
