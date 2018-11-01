using UnityEngine;
using System.Collections;

public class Game: MonoBehaviour
{
	public static Game 		game;

	public 	GameObject 		prefabTriangle;
	public 	GameObject 		prefabTriangleHex;
	public 	GameObject 		prefabSquare;
	public 	GameObject 		prefabHexagon;
	public 	Transform 		spawnPointCenter;
	public 	Transform 		spawnPointTriangle;
	public 	Camera 			gameCamera;

	public 	Texture 		triangle;
	public 	Texture 		triangleHex;
	public 	Texture 		square;
	public 	Texture 		hexagon;
	public 	GUISkin 		skin;

	private GameObject[] 	currentObjects;
	private bool 			isVisible = true;
	private float 			hO = 0.35f;
	private float 			sO = 0.75f;
	private float 			bO = 0.75f;
	private float 			hB = 0;
	private float 			sB = 0;
	private float 			bB = 0.15f;
	private float 			randomOffset = 0.1f;
	private Transform 		fractalCollector;

	enum 					types {triangle, triangleHex, square, hexagon}
	private 				types CurrentType = types.triangleHex;

#if UNITY_WEBPLAYER
	private int 			triangleClicked = 0;
	private int 			triangleHexClicked = 0;
	private int 			squareClicked = 0;
	private int 			hexagonClicked = 0;
#endif

	void Awake()
	{
		game = gameObject.GetComponent<Game>();
		
		CreateCollector();
		FindObjects();
	}

	void Update()
	{
		AdaptObjectColor();
		AdaptBackgroundColor();
	}

	void OnGUI()
	{
		if(skin)
			GUI.skin = skin;

		if(isVisible)
		{
			//buttons to switch between types
			if(GUI.Button(RectAdapt(10, 10, 50, 50), triangle))
				SetType(types.triangle);
			
			if(GUI.Button(RectAdapt(65, 10, 50, 50), triangleHex))
				SetType(types.triangleHex);
			
			if(GUI.Button(RectAdapt(120, 10, 50, 50), square))
				SetType(types.square);
			
			if(GUI.Button(RectAdapt(175, 10, 50, 50), hexagon))
				SetType(types.hexagon);

			//sliders object-color
			GUI.Label(RectAdapt(10, 80, 215, 50), "Start Hue: " + hO.ToString("f2"));
			hO = GUI.HorizontalSlider(RectAdapt(10, 100, 215, 20), hO, 0, 1);

			GUI.Label(RectAdapt(10, 110, 215, 50), "Start Saturation: " + sO.ToString("f2"));
			sO = GUI.HorizontalSlider(RectAdapt(10, 130, 215, 20), sO, 0, 1);

			GUI.Label(RectAdapt(10, 140, 215, 50), "Start Brightness: " + bO.ToString("f2"));
			bO = GUI.HorizontalSlider(RectAdapt(10, 160, 215, 20), bO, 0, 1);
			
			//sliders background-color
			GUI.Label(RectAdapt(10, 200, 215, 50), "Background Hue: " + hB.ToString("f2"));
			hB = GUI.HorizontalSlider(RectAdapt(10, 220, 215, 20), hB, 0, 1);
			
			GUI.Label(RectAdapt(10, 230, 215, 50), "Background Saturation: " + sB.ToString("f2"));
			sB = GUI.HorizontalSlider(RectAdapt(10, 250, 215, 20), sB, 0, 1);
			
			GUI.Label(RectAdapt(10, 260, 215, 50), "Background Brightness: " + bB.ToString("f2"));
			bB = GUI.HorizontalSlider(RectAdapt(10, 280, 215, 20), bB, 0, 1);

			//slider for random color range
			GUI.Label(RectAdapt(10, 320, 215, 50), "Random Color Offset: " + randomOffset.ToString("f2"));
			randomOffset = GUI.HorizontalSlider(RectAdapt(10, 340, 215, 20), randomOffset, 0, 0.3f);

			//reset-button
			if(GUI.Button(RectAdapt(10, 380, 215, 50), "Reset"))
				Reset();

			//hide menu-button
			if(GUI.Button(RectAdapt(10, 435, 215, 50), "Hide Settings"))
			   isVisible = false;
		}
		else
		{
			if(GUI.Button(RectAdapt(10, 10, 215, 50), "Show Settings"))
			   isVisible = true;
		}

		GUI.Label (RectAdapt(10, 590, 215, 50), "Triangle Fractal Creator\nby Matthias Zarzecki");
	}

	public Transform FractalCollector()
	{
		if(fractalCollector)
			return fractalCollector;
		else
			return null;
	}

	public float RandomOffset()
	{
		return randomOffset;
	}

	private void FindObjects()
	{
		currentObjects = GameObject.FindGameObjectsWithTag("FractalObject");
	}

	private void AdaptObjectColor()
	{
		Color newColor = HSVToRGB(hO, sO, bO);

		foreach(GameObject g in currentObjects)
			if(g)
				g.GetComponent<Renderer>().material.SetColor("_Color", newColor);
	}

	private void AdaptBackgroundColor()
	{
		gameCamera.backgroundColor = HSVToRGB(hB, sB, bB);
	}

	private void CreateCollector()
	{
		GameObject tempFractalCollector = new GameObject("FractalCollector");
		fractalCollector = tempFractalCollector.transform;
	}

	private void SetType(types newType)
	{
		CurrentType = newType;
		Reset ();
	}
	
	private void Reset()
	{
		//delete all current objects
		Destroy(fractalCollector.gameObject);

		//create new collector
		CreateCollector();

		//instantiate new base object
		GameObject newObject;
		Transform newSpawnPoint = spawnPointCenter;
		if(CurrentType == types.triangle)
		{
			newObject = prefabTriangle;
			newSpawnPoint = spawnPointTriangle;
		}
		else if(CurrentType == types.triangleHex)
			newObject = prefabTriangleHex;
		else if(CurrentType == types.square)
			newObject = prefabSquare;
		else
			newObject = prefabHexagon;

		CheckClicks();

		Instantiate(newObject, newSpawnPoint.position, newSpawnPoint.rotation);

		//set object color to random color
		hO = Random.Range (0.0f, 1.1f);
		sO = Random.Range (0.3f, 1.1f);
		bO = Random.Range (0.4f, 1.1f);

		//assign new base object to current object slot
		FindObjects();
	}

	private void CheckClicks()
	{
#if UNITY_WEBPLAYER

		if(CurrentType == types.triangle && triangleClicked <= 0)
			triangleClicked = 1;
		else if(CurrentType == types.triangleHex && triangleHexClicked <= 0)
			triangleHexClicked = 1;
		else if(CurrentType == types.square && squareClicked <= 0)
			squareClicked = 1;
		else if(CurrentType == types.hexagon && hexagonClicked <= 0)
			hexagonClicked = 1;

		if(triangleClicked + triangleHexClicked + squareClicked + hexagonClicked >= 4)
			Application.ExternalCall("kongregate.stats.submit","AllTypes", 1);
#endif
	}

	private Rect RectAdapt(int x, int y, int width, int height)
	{
		return new Rect((x * Screen.width)/960, (y * Screen.height)/640, (width * Screen.width)/960, (height * Screen.height)/640);
	}

	private Color HSVToRGB(float h, float s, float v)
	{
		float r = 0;
		float g = 0;
		float b = 0;
		int i;
		float f;
		float p;
		float q;
		float t; 

		i = (int) Mathf.Floor(h * 6);

		f = h * 6 - i;
		p = v * (1 - s);
		q = v * (1 - f * s);
		t = v * (1 - (1 - f) * s);

		switch(i % 6)
		{
			case 0: r = v; g = t; b = p; break;
			case 1: r = q; g = v; b = p; break;
			case 2: r = p; g = v; b = t; break;
			case 3: r = p; g = q; b = v; break;
			case 4: r = t; g = p; b = v; break;
			case 5: r = v; g = p; b = q; break;
		}

		return new Color(r,g,b); 
	}
}