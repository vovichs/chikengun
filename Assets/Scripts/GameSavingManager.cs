using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSavingManager : MonoBehaviour
{
	public static void SaveCurrentGameWithName(string gameTitle)
	{
		string str = "{ \"scene_name\": \"" + SceneManager.GetActiveScene().name + "\",";
		str = str + "\"custom_game_title\": \"" + gameTitle + "\",";
		str += "\"objects\": [";
		InventoryObject[] array = UnityEngine.Object.FindObjectsOfType<InventoryObject>();
		InventoryObject[] array2 = array;
		foreach (InventoryObject inventoryObject in array2)
		{
			str += "{";
			string text = str;
			str = text + "\"catId\": " + inventoryObject.categoryId + ",";
			text = str;
			str = text + "\"id\": " + inventoryObject.id + ",";
			string prefabName = inventoryObject.prefabName;
			str = str + "\"prefab\": \"" + prefabName + "\",";
			text = str;
			object[] obj = new object[4]
			{
				text,
				"\"px\": ",
				null,
				null
			};
			Vector3 position = inventoryObject.transform.position;
			obj[2] = position.x;
			obj[3] = ",";
			str = string.Concat(obj);
			text = str;
			object[] obj2 = new object[4]
			{
				text,
				"\"py\": ",
				null,
				null
			};
			Vector3 position2 = inventoryObject.transform.position;
			obj2[2] = position2.y;
			obj2[3] = ",";
			str = string.Concat(obj2);
			text = str;
			object[] obj3 = new object[4]
			{
				text,
				"\"pz\": ",
				null,
				null
			};
			Vector3 position3 = inventoryObject.transform.position;
			obj3[2] = position3.z;
			obj3[3] = ",";
			str = string.Concat(obj3);
			text = str;
			object[] obj4 = new object[4]
			{
				text,
				"\"rx\": ",
				null,
				null
			};
			Vector3 eulerAngles = inventoryObject.transform.eulerAngles;
			obj4[2] = eulerAngles.x;
			obj4[3] = ",";
			str = string.Concat(obj4);
			text = str;
			object[] obj5 = new object[4]
			{
				text,
				"\"ry\": ",
				null,
				null
			};
			Vector3 eulerAngles2 = inventoryObject.transform.eulerAngles;
			obj5[2] = eulerAngles2.y;
			obj5[3] = ",";
			str = string.Concat(obj5);
			string arg = str;
			Vector3 eulerAngles3 = inventoryObject.transform.eulerAngles;
			str = arg + "\"rz\": " + eulerAngles3.z;
			str += "},";
		}
		str += "]}";
		MonoBehaviour.print(str);
		AddNewGameTitle(gameTitle);
		SaveGameInPrefs(gameTitle, str);
		GetSavedGame(gameTitle);
	}

	private static void AddNewGameTitle(string title)
	{
		string @string = PlayerPrefs.GetString("all_game_titles");
		@string = ((!(PlayerPrefs.GetString("all_game_titles", string.Empty) == string.Empty)) ? (@string + "|||" + title) : (@string + title));
		PlayerPrefs.SetString("all_game_titles", @string);
		PlayerPrefs.Save();
		MonoBehaviour.print("list0 = " + PlayerPrefs.GetString("all_game_titles"));
	}

	private static void SaveGameInPrefs(string gameTitle, string json)
	{
		PlayerPrefs.SetString(gameTitle, json);
	}

	public static void RemoveGame(string gameTitle)
	{
		string @string = PlayerPrefs.GetString("all_game_titles");
		MonoBehaviour.print("list = " + @string);
		string[] array = @string.Split(new string[1]
		{
			"|||"
		}, StringSplitOptions.RemoveEmptyEntries);
		array = Array.FindAll(array, (string element) => element != gameTitle);
		string.Join("|||", array);
		PlayerPrefs.SetString("all_game_titles", string.Join("|||", array));
		PlayerPrefs.Save();
		MonoBehaviour.print("list0 = " + PlayerPrefs.GetString("all_game_titles"));
		PlayerPrefs.DeleteKey(gameTitle);
	}

	public static string[] GetSavedGamesList()
	{
		string @string = PlayerPrefs.GetString("all_game_titles");
		return @string.Split(new string[1]
		{
			"|||"
		}, StringSplitOptions.RemoveEmptyEntries);
	}

	public static SavedGameInfo GetSavedGame(string gameTitle)
	{
		SavedGameInfo result = default(SavedGameInfo);
		string @string = PlayerPrefs.GetString(gameTitle);
		JSONNode jSONNode = JSON.Parse(@string);
		string sceneName = jSONNode["scene_name"];
		string gameTitle2 = jSONNode["custom_game_title"];
		result.sceneName = sceneName;
		result.gameTitle = gameTitle2;
		JSONNode jSONNode2 = jSONNode["objects"];
		InventoryItem[] array = new InventoryItem[jSONNode2.Count];
		for (int i = 0; i < array.Length; i++)
		{
			JSONNode jSONNode3 = jSONNode2[i];
			MonoBehaviour.print(jSONNode3["prefab"]);
			InventoryItem inventoryItem = new InventoryItem();
			inventoryItem.id = (byte)jSONNode3["id"].AsInt;
			inventoryItem.categoryId = (byte)jSONNode3["catId"].AsInt;
			inventoryItem.prefabName = jSONNode3["prefab"];
			inventoryItem.pos.x = jSONNode3["px"].AsFloat;
			inventoryItem.pos.y = jSONNode3["py"].AsFloat;
			inventoryItem.pos.z = jSONNode3["pz"].AsFloat;
			inventoryItem.rot.x = jSONNode3["rx"].AsFloat;
			inventoryItem.rot.y = jSONNode3["ry"].AsFloat;
			inventoryItem.rot.z = jSONNode3["rz"].AsFloat;
			array[i] = inventoryItem;
		}
		result.items = array;
		return result;
	}
}
