using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : ScriptableObject {

	public List<int> destinations = new List<int>();
	public List<int> emptyConditions = new List<int>();
	public List<int> enemConditions = new List<int>();
	public string name = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
