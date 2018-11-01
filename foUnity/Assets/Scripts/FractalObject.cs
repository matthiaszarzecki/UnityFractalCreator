using UnityEngine;
using System.Collections;

public class FractalObject: MonoBehaviour
{
	public 	GameObject 	newSubObject;
	public 	Transform[] spawnPoints;
	public 	int 		newFraction = 2;

	private Game 		game;

	void Start()
	{
		game = Game.game;
		transform.parent = game.FractalCollector();
	}

	void OnMouseOver()
	{
		if(Input.GetKey(KeyCode.Mouse0))
			CreateFractal();
	}

	private void CreateFractal()
	{
		//create new objects at spawnPoints
		foreach(Transform t in spawnPoints)
		{
			GameObject newObject = Instantiate(newSubObject, t.transform.position, t.transform.rotation) as GameObject;

			//chain new Object to the same parent-object
			newObject.transform.parent = game.FractalCollector();

			//scale new object to 50% of current scale
			newObject.transform.localScale = Vector3.one * 1.0f/newFraction * transform.localScale.x;

			//give random color
			Color currentColor = newObject.GetComponent<Renderer>().material.color;
			float offset = game.RandomOffset();
			float r = Random.Range(currentColor.r - offset, currentColor.r + offset);
			float g = Random.Range(currentColor.g - offset, currentColor.g + offset);
			float b = Random.Range(currentColor.b - offset, currentColor.b + offset);
			Color newColor = new Color(r, g, b, 1f);
			newObject.GetComponent<Renderer>().material.color = newColor;
		}

		//delete self
		Destroy (gameObject);
	}
}