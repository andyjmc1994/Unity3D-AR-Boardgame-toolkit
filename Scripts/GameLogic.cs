using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {

	Team[] teams;
	Dropdown dd1;
	Dropdown dd2;
	Button playB;
	Text label;
	public List<int> ocupied = new List<int>();
	public List<int> enLoc = new List<int>();
	public List<Move> movlst = new List<Move>();
	SomeData data;
	int scaleX;

	void Start () {
		GameObject x =  GameObject.Find("BasicUILabel");
		label = x.GetComponent<Text> ();
		x =  GameObject.Find("BasicUIDrop1");
		dd1 = x.GetComponent<Dropdown> ();
		x =  GameObject.Find("BasicUIDrop2");
		dd2 = x.GetComponent<Dropdown> ();
		x =  GameObject.Find("BasicUIButton");
		playB = x.GetComponent<Button> ();
		playB.onClick.AddListener(TaskOnClick);
		GameObject teamsFolder = GameObject.Find("TeamFolder");
		teams = teamsFolder.GetComponentsInChildren<Team>();
		GameObject arcam = GameObject.Find("ARCamera");
		data = arcam.GetComponentInChildren<SomeData>();
		scaleX = data.scaleX;
		dd1.value = 0;
		play ();
	}
	
	void Update () {
	}
	void play(){
		turn ();
	}
	void turn(){
		dd1.options.Clear ();
		foreach (GamePiece piece in teams[0].pieces) {
			Dropdown.OptionData list = new Dropdown.OptionData(piece.name);
			dd1.options.Add(list);
		}
		dd2.options.Clear ();
		bool good = true;
		foreach (Move mv in teams[0].pieces[dd1.value].availableMoves) {
			foreach (int empty in mv.emptyConditions) {
				if(ocupied.Contains(getActualSquare(empty,teams[0].pieces[dd1.value].getSquare()))){
					good = false;
					break;
				}
			}
			if (good == false) {
				break;
			}
			foreach (int enemy in mv.enemConditions) {
				if(enLoc.Contains(getActualSquare(enemy,teams[0].pieces[dd1.value].getSquare()))){
					good = false;
					break;
				}
			}
			if (good == true) {
				movlst.Add (mv);
				Dropdown.OptionData list = new Dropdown.OptionData(mv.name);
				dd2.options.Add(list);

			}
		}
	}

	void badLocs(){
		ocupied.Clear ();
		enLoc.Clear ();
		for(int y=0; y< teams.Length -1;y++){
			foreach (GamePiece piece in teams[y].pieces) {
				if (!ocupied.Contains (piece.getSquare ())) {
					ocupied.Add (piece.getSquare ());
					if (y != 0) {
						enLoc.Add(piece.getSquare ());
					}
				}
			}
		}
	}

	void TaskOnClick()
	{
		teams [0].pieces [dd1.value].GetComponent<SmoothMove> ().destination = data.squares [getActualSquare(teams[0].pieces[dd1.value].availableMoves[dd2.value].destinations[0],teams[0].pieces[dd1.value].getSquare())].transform.position;
		teams [0].pieces [dd1.value].setSquare (getActualSquare (teams [0].pieces [dd1.value].availableMoves [dd2.value].destinations [0], teams [0].pieces [dd1.value].getSquare ()));
	}
		
	int getActualSquare(int mapped, int currunt){
		int x = 0;
		int y = 0;
		if (mapped < 24) {
			while(true){
				if (mapped >= 21 && mapped <=27) {
					break;
				}
				x++;
				mapped = mapped + 7;
			}
			y = 24 - mapped;
		}
		else if (mapped > 24) {
			while(true){
				if (mapped >= 21 && mapped <=27) {
					break;
				}
				x++;
				mapped = mapped - 7;
			}
			y = 24 - mapped;
			x = -x;
		}
		int newS = currunt - (x * scaleX) - y;
		return newS;
	}
}
