	using System;
	using System.Collections;
	using System.Collections.Generic;
	using MoonSharp.Interpreter;
	using NaughtyAttributes;
	using UnityEngine;
	using UnityEngine.Assertions;

	public class RougeCraftApi : MonoBehaviour
	{
		public MyClass debug;
		private void Start()
		{
			MyClass myObject = new MyClass();
			myObject.level = 1;
			myObject.timeElapsed = 47.5f;
			myObject.playerName = "Dr Charles Francis";
			
			string json = JsonUtility.ToJson(myObject);
			
			myObject = JsonUtility.FromJson<MyClass>(json);

			debug = myObject;
		}
	}
	[Serializable]
	public class MyClass
	{
		public int level;
		public float timeElapsed;
		public string playerName;
	}