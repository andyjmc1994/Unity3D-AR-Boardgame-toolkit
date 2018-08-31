using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {
	private string name = "";
	private Color color = Color.white;
	private bool removeB;
	public List<GamePiece> pieces = new List<GamePiece>();

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnGUI () {

	}

	public Color getColor(){

		return color;
	}

	public void setColor(Color newColor){
		color = newColor;
	}

	public string getName(){
		return name;
	}

	public void setName(string newName){
		name = newName;
	}

	public void addPiece(GamePiece piece){
		pieces.Add (piece);
	}

	public void clearPieces(){
		pieces.Clear();
	}


}
