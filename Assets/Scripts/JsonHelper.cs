using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Json
{
	public class JsonHelper
	{
		/***
		 * Function to load top-level json array
		 ***/
		public static T[] getJsonArray<T>(string json)
		{
			string newJson = "{ \"array\": " + json + "}";
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
			return wrapper.array;
		}

		[Serializable]
		private class Wrapper<T>
		{
			public T[] array;
		}

		/**
		 * Function load json file in resource path
		 **/
		public static string LoadJsonResource(string path) {
			string filePath = "json/" + path.Replace (".json", "");
			TextAsset targetFile = Resources.Load<TextAsset> (filePath);
			return targetFile.text;
		}

		public static T loadJsonObject<T>(string jsonFileName) {
			//Json Top Level Array Type
			string jsonString = JsonHelper.LoadJsonResource (jsonFileName);
			T jsonObject  = JsonUtility.FromJson<T> (jsonString);
			return jsonObject;

		}

		public static T[] loadJsonArray<T>(string jsonFileName) {
			//Json Top Level Array Type
			string jsonString = JsonHelper.LoadJsonResource (jsonFileName);
			T[] jsonArray  = JsonHelper.getJsonArray<T> (jsonString);
			return jsonArray;
		}

		public static T[] FromJson<T>(string nodesJson)
		{
			throw new NotImplementedException();
		}

		public static string ToJson(ScenarioNodeData[] nodesToSave, bool p1)
		{
			throw new NotImplementedException();
		}

		public static string ToJson(ConnectionData[] connectionsToSave, bool p1)
		{
			throw new NotImplementedException();
		}
	}
}