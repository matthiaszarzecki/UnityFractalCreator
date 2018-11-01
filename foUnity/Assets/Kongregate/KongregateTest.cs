using UnityEngine;
using System.Collections;

public class KongregateTest : MonoBehaviour 
{
	string stat = "Stat";
	string item = "Item";
	string items = "";
	int count;
	
	void OnGUI()
	{
		GUILayout.Label("Connected?: " + Kongregate.isConnected);
		GUILayout.Label("Username: " + Kongregate.username);
		GUILayout.Label("UserId: " + Kongregate.userId);
		GUILayout.Label("isGuest: " + Kongregate.isGuest);
		GUILayout.BeginHorizontal();
		stat = GUILayout.TextField(stat, GUILayout.Width(100));
		if (GUILayout.Button("-"))
			count = Mathf.Max(0, count - 1);
		GUILayout.Label(count.ToString());
		if (GUILayout.Button("+"))
			count++;
		if (GUILayout.Button("Submit Stat"))
			Kongregate.SubmitStat(stat, count);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(30);
		GUILayout.Label("Items: " + items);
		if (GUILayout.Button("Retrieve items"))
		{
			string[] updatedItems = Kongregate.items;
			items = string.Join(" ", updatedItems);
			items += string.Format(" ({0})", updatedItems.Length);
		}
		GUILayout.BeginHorizontal();
		item = GUILayout.TextField(item, GUILayout.Width(100));
		if (GUILayout.Button("Purchase Item"))
			Kongregate.PurchaseItem(item);
		GUILayout.EndHorizontal();
	}
}
