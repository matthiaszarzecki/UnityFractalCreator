using UnityEngine;
using System.Collections;

public class Kongregate : MonoBehaviour 
{	
	// Public API (Static Members)
	static public bool isConnected
	{
		get { return instance.jsAPILoaded; }
	}
	
	static bool _isGuest = false;
	static public bool isGuest
	{
		get 
		{
			if (!_isGuest && string.IsNullOrEmpty(_username))
				instance.CallJSFunction("isGuest()", "SetIsGuest"); 
			return _isGuest; 
		}
	}
	
	static string _username = "";
	static public string username
	{
		get
		{ 
			if (string.IsNullOrEmpty(_username))
				instance.CallJSFunction("getUsername()", "SetUsername");
			return _username;
		}
	}
	
	static string _userId = "";
	static public string userId
	{
		get 
		{ 
			if (string.IsNullOrEmpty(_userId))
				instance.CallJSFunction("getUserId()", "SetUserId"); 
			return _userId;
		}
	}
	
	static string _items = "";
	static public string[] items
	{
		get
		{
			instance.CallJSFunction("getUserItems()");
			if (string.IsNullOrEmpty(_items))
				return new string[0];
			else
				return _items.Split(',');
		}
	}
	
	static public void ShowSignIn()
	{
		instance.CallJSFunction("showSignInBox()");
	}
	
	static public void SubmitStat(string statistic, int value)
	{
		instance.CallJSFunction(string.Format("submitStat('{0}', {1})", statistic, value));
	}
	
	static public void PurchaseItem(string item)
	{
		Debug.Log("Attempting purchase of " + item);
		instance.CallJSFunction(string.Format("purchaseItem('{0}')", item));
	}
	
	static Kongregate _instance;
	static Kongregate instance
	{
		get
		{
			if (!_instance)
				new GameObject("Kongregate", typeof(Kongregate));
				
			return _instance;
		}
	}
	
	// Instance Members	
	bool jsAPILoaded;
	string jsReturnValue;
	
	void Awake()
	{
		// Only allow one instance of the API bridge
		if (_instance)
			Destroy(gameObject);
		
		_instance = this;
	}
	
	void Start() 
	{
		Application.ExternalEval(@"
			// Extern the JS Kongregate API
			var fileref=document.createElement('script');
			fileref.setAttribute('type','text/javascript');
			fileref.setAttribute('src','http://www.kongregate.com/javascripts/kongregate_api.js');
			document.getElementsByTagName('head')[0].appendChild(fileref);

			// Load the API
			var kongregate;
			var domReady = setTimeout('loadAPI()', 500);

			// Callback function
			function onComplete()
			{
				// Set the global kongregate API object
				kongregate = kongregateAPI.getAPI();
				kongregate.services.connect();
				SendUnityMessage('OnLoaded', '');
			}
			
			// Async check for extern JS load to be completed
			function loadAPI() 
			{	
				clearTimeout(domReady);
				if (typeof kongregateAPI != 'undefined')
					kongregateAPI.loadAPI(onComplete);
				else
					domReady = setTimeout('loadAPI()', 500);				
			}
			 
			function isGuest()
			{
				return kongregate.services.isGuest();
			}

			function getUsername()
			{
				return kongregate.services.getUsername();
			}

			function getUserId()
			{
				return kongregate.services.getUserId();
			}

			function showSignInBox()
			{
				if(kongregate.services.isGuest())
  					kongregate.services.showSignInBox();
			}
		
			function submitStat(statName, value)
			{
				kongregate.stats.submit(statName, value);
			}

			function getUserItems()
			{
				kongregate.mtx.requestUserItemList(null, onUserItems);
			}

			function onUserItems(result)
			{
				if (result.success)
				{
					var items = '';
					for (var i = 0; i < result.data.length; i++)
					{
						items += result.data[i].identifier;
						if (i < result.data.length - 1)
							items += ',';
					}
					SendUnityMessage('SetUserItems', items);
				}
			}

			function purchaseItem(item)
			{
				kongregate.mtx.purchaseItems([item], onPurchaseResult);
				SendUnityMessage('LogMessage', 'purchase sent....');				
			}

			function onPurchaseResult(result)
			{
				if (result.success)
				{
					SendUnityMessage('LogMessage', 'purchase complete...');
					getUserItems();
				}
			}

			// Utility function to send data back to Unity
			function SendUnityMessage(functionName, message)
			{
				var unity = GetUnity();
				unity.SendMessage('Kongregate', functionName, message);
			}
		");
		  
//		Application.ExternalCall("SendUnityMessage", "OnLoaded", "Message to myself");
//		Application.ExternalCall("loadAPI");
		DontDestroyOnLoad(gameObject);
	}
	
	void LogMessage(string message)
	{
		Debug.Log(message);
	}
	
	void OnLoaded(string error)
	{
		if (string.IsNullOrEmpty(error))
			jsAPILoaded = true;
		else
			Debug.LogError(error);
	}
	
	void SetIsGuest(object returnValue)
	{
		if (bool.TryParse(returnValue.ToString(), out _isGuest))
		{
			// Request username again if guest
			if (_isGuest)
				_username = "";
		}
	}
	
	void SetUsername(object returnValue)
	{
		_username = returnValue.ToString();
	}
	
	void SetUserId(object returnValue)
	{
		_userId = returnValue.ToString();
	}
	
	void SetUserItems(object returnValue)
	{
		_items = returnValue.ToString();
	}
	
	void CallJSFunction(string functionCall)
	{
		CallJSFunction(functionCall, null);
	}
		
	void CallJSFunction(string functionCall, string callback)
	{
		if (jsAPILoaded)
		{
			string scriptCode = functionCall;
			if (!string.IsNullOrEmpty(callback))
				scriptCode = string.Format("var value = {0};\nSendUnityMessage('{1}', String(value));", functionCall, callback);
			Application.ExternalEval(scriptCode);
		}
	}
}
