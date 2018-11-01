//////////////////////////////////////////////////////////////////////////////
// //
// SUBMIT STATISTICS WITH //
// //
// Application.ExternalCall(“"kongregate.stats.submit”", "“MatchesMade”", 1); //
// //
//////////////////////////////////////////////////////////////////////////////

#if UNITY_WEBPLAYER

public var debugmode = true;

public var isKongregate = false;
public var userId = 0;
static var username = "Guest";
public var gameAuthToken = "";

public var stat = "stat";
public var statValue = 0;

function Awake()
{
	// This game object needs to survive multiple levels
	DontDestroyOnLoad(this);
	DontDestroyOnLoad(transform.gameObject);
	
	// Begin the API loading process if it is available
	Application.ExternalEval(
	"if(typeof(kongregateUnitySupport) != 'undefined'){" +
	" kongregateUnitySupport.initAPI('KongregateAPI(Clone)', 'OnKongregateAPILoaded');" +
	"}"
	);
}

function OnGUI()
{
	if(debugmode)
	{
		if(isKongregate)
		{
			GUILayout.Label("Connected!");
			GUILayout.Label("UserID: " + userId);
			GUILayout.Label("UserName: " + username);
			GUILayout.Label("gameAuthToken: " + gameAuthToken);
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
		
		GUILayout.BeginHorizontal();
		stat = GUILayout.TextField(stat, GUILayout.Width(100));
		if (GUILayout.Button("-"))	statValue = Mathf.Max(0, statValue - 1);
		GUILayout.Label(statValue.ToString());
		if(GUILayout.Button("+")) statValue++;
			
		if(GUILayout.Button("sumbit stat!"))
		{
			//Application.ExternalCall("kongregate.stats.submit","Stat",Random.Range(0,10));
			Application.ExternalCall("kongregate.stats.submit",stat,statValue);
		}
		GUILayout.EndHorizontal();
	}
}

function OnKongregateAPILoaded(userInfoString)
{
	// We now know we’re on Kongregate
	isKongregate = true;
	Debug.Log("ON KONG");
	// Split the user info up into tokens
	var params = userInfoString.Split("|"[0]);
	userId = parseInt(params[0]);
	username = params[1];
	gameAuthToken = params[2];
}

#endif