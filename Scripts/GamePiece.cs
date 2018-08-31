using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {

	private string typeName = "";
	private GameObject model;
	private int symbolID = 0;
	private int size = 30;
	public int curruntSquare = 256;
	public string name = "";
	private string targetName = "";
	private bool tracked;
	private GameObject gamePiece = null;
	public List<Move> availableMoves = new List<Move>();




	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
	}
	public string getTName(){
		return typeName;
	}

	public void setTName(string newName){
		typeName = newName;
	}

	public string getName(){
		return name;
	}

	public void setName(string newName){
		name = newName;
	}

	public void setModel(GameObject mod){
		model = mod;
	}

	public GameObject getModel(){
		return model;
	}
	public void setgp(GameObject gp){
		gamePiece = gp;
	}

	public GameObject getgp(){
		if (isTrack()) {
			return gamePiece; 
		} else {
			return this.gameObject;
		}
	}


	public void setSymb(int sym){
		symbolID = sym;
	}

	public int getSymb(){
		return symbolID;
	}
	public void setSize(int sizei){
		size = sizei;
	}

	public int getSize(){
		return size;
	}

	public int getSquare(){
		return curruntSquare;
	}

	public void setSquare(int square){
		curruntSquare = square;
	}

	public bool isTrack(){
		return tracked;
	}

	public void trackify(){
		tracked = true;
	}

	public void unTrackify(){
		tracked = false;
	}
	public void setTI(string ti){
		targetName = ti;
	}

	public string getTI(){
		return targetName;
	}


}
