using UnityEngine;
using System.Collections;

public class ChildrenCheck: MonoBehaviour
{
	void Update()
	{
		if(transform.childCount <= 0)
			Destroy(gameObject);
	}
}