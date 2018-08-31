using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ARTC : EditorWindow {
	private static int guiLevel = 1;
	static int scaleX = 1;
	static int scaleY = 1;
	static float masterScale = 0;
	static bool boardPreview = false;
	static int boardType = 1;
	static bool showBtn = false;
	static bool allSelect = false;
	static bool reColourInd = false;
	SomeData gData;
	static List<Vector3[]> rectsToDraw = new List<Vector3[]>();
	GameObject boardImage;
	Material backgroundImage;
	GameObject mtFolder;
	GameObject pieceFolder;
	static List<int> selected = new List<int>();
	static List<GameObject> squares = new List<GameObject>();
	public static List<Team> teams = new List<Team>();
	public static List<Piece> pieces = new List<Piece>();
	static readonly string[] Columns = new[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","AA","AB","AC","AD","AE","AF","AG","AH","AI","AJ","AK","AL","AM","AN","AO","AP","AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ","BA","BB","BC","BD","BE","BF","BG","BH"};
	int multiplier = 1;
	GameObject board;
	GameObject cameraAR;
	Vector2 scrollPos;
	string ssel = "";
	string psel = "";
	int square = 0;
	GameObject imageTarget;
	GameObject imageTarget2;
	GameObject gameObject;
	Editor gameObjectEditor;
	static int select = 20;
	int choiceIndex = 0;
	int choiceIndex2 = 0;
	int choiceIndexSym = 0;
	static string[] symbols = new string[]{"■","□","◀","▲","▣","▤","◆","◉","◌","▮"};
	string ibsName = "";
	string itpName = "";
	int yOffset = 210;
	static bool placer = false;
	static int pkount = 0;
	static GamePiece editing;
	static int thing = 0;
	Move newMove;

	public ARTC ()
	{
	}
		
	[MenuItem ("Window/ARTC")]
	public static void Launch ()
	{
		GetWindow (typeof (ARTC)).Show ();
		GetWindow (typeof(ARTC)).minSize = new Vector2 (200, 200);
		allSelect = false;
		rectsToDraw.Clear ();

	}

	public void OnGUI ()
	{
		if (guiLevel == 1) {
			greetingScreen ();
		} else if (guiLevel == 2) {
			addImageTarget ();
		} else if (guiLevel == 3) {
			updatePreview (boardType, scaleX, scaleY);
			drawRectangs (false);
			boardScreen ();
		} else if (guiLevel == 4) {
			teamScreen ();
		} else if (guiLevel == 5) {
			arPieceScreen ();
		} else if (guiLevel == 6) {
			rulePage ();
		} 
		else if (guiLevel == 7) {
			//drawPiecePreview();
			//drawRectangs (true);
			startStateScreen ();
		}
		else if (guiLevel == 8) {
			//drawPiecePreview();
			drawRectangs (true);
			thePlacer ();
		} else if (guiLevel == 9) {
			trackerInst ();
		}
		else if (guiLevel == 10) {
			drawMove ();
		}

	}

	void updatePreview(int shape, int sizeX, int sizeY ) {
		Handles.BeginGUI();
		Handles.color = Color.red;
		if (shape == 1) {
			drawRects (sizeX, sizeY);
		} else if (shape == 2) {

		}
		Handles.EndGUI ();


	}

	void drawRectangs(bool label) {
		int c = selected.Count;
		if (reColourInd == true && rectsToDraw.Count > c) {
			selected.Clear ();
			reColourInd = false;
		} else if (rectsToDraw.Count < c) {
			selected.Clear ();
			reColourInd = false;

		} else {
			int mover = 0;

			for (int x = 0; x < c; x++) {
				Handles.DrawSolidRectangleWithOutline (rectsToDraw [selected [x]], new Color (1, 1, 1, 0.2f), new Color (0, 0, 0, 1));
				if (label) {
					labeler (rectsToDraw [selected [x]] [0], (int)(rectsToDraw [selected [x]] [0].x - 20) / multiplier, (int)(rectsToDraw [selected [x]] [0].y - yOffset) / multiplier);
				}				
				if (guiLevel == 7) {
					for (int s = 0; s < teams.Count; s++) {
						foreach (GamePiece piece in teams[s].pieces) {
							if (piece.getSquare () == selected [x]) {
								GUIStyle style = new GUIStyle ();
								style.normal.textColor = teams [s].getColor ();
								style.fontSize = 25;
								Handles.Label (new Vector3 (rectsToDraw [selected [x]] [0].x + mover, rectsToDraw [selected [x]] [0].y), symbols [piece.getSymb ()], style);
								mover = mover + 10;
							}
						}
					}
				}
				HandleUtility.Repaint ();
				mover = 0;
			}

		}
	}

	void drawRects(int sizeX, int sizeY){
		float xFit = 400/sizeX;
		float yFit = 400/sizeY;
		int xCount = 0;
		int yCount = 0;
		multiplier = (int)Mathf.Min(xFit, yFit);

		for (int x=0; x<(sizeX*multiplier); x=x+multiplier) {
			for (int y=0; y<(sizeY*multiplier); y=y+multiplier) {
				Handles.DrawPolyLine(new Vector3(20+x, yOffset+y),new Vector3(20+x+multiplier, yOffset+y),new Vector3(20+x+multiplier, yOffset+y+multiplier),new Vector3(20+x, yOffset+y+multiplier), new Vector3(20+x, yOffset+y));
				yCount++;
			}
			xCount++; 
		}
		populate (sizeX, sizeY);
	}
		
	void populate(int sizeX, int sizeY){
		rectsToDraw.Clear ();
		float xFit = 400 / sizeX;
		float yFit = 400 / sizeY;
		int xCount = 0;
		int yCount = 0;
		multiplier = (int)Mathf.Min (xFit, yFit);

		for (int x = 0; x < (sizeX * multiplier); x = x + multiplier) {
			for (int y = 0; y < (sizeY * multiplier); y = y + multiplier) {

				Vector3[] verts = new Vector3[] {
					new Vector3 (20 + x, yOffset + y),
					new Vector3 (20 + x + multiplier, yOffset + y),
					new Vector3 (20 + x + multiplier, yOffset + y + multiplier),
					new Vector3 (20 + x, yOffset + y + multiplier)
				};

				rectsToDraw.Add (verts);
				labeler (new Vector3 (20 + x, yOffset + y), xCount, yCount);
				yCount++;
			}
			yCount = 0;
			xCount++;
		}
	}
		
	public void labeler (Vector3 xxyy, int xCount, int yCount){

		Handles.Label (xxyy, IndexToColumn (xCount, yCount));

	}

	public static string IndexToColumn(int indexX, int indexY)
	{
		return Columns[indexX] + (scaleY - (indexY)).ToString();
	}

	void selecter(int sizeX, int sizeY){
		populate (sizeX, sizeY);
		int all = rectsToDraw.Count;
		if (reColourInd) { 
			selected.Clear ();
			for (int q = 0; q < all; q++) {
				selected.Add (q);
			} 
		}
	}

	void colourInderviduals(Vector2 mousePos, int xSize, int ySize){
		int square = workOutSquare (mousePos,xSize,ySize);
		if (selected.Contains (square)) {
			reColourInd = false;
			selected.Remove (square);
			drawRectangs (false);
		} else {
			selected.Add (square);
			drawRectangs (false);
		}
	}

	int workOutSquare(Vector2 mousePos, int xSize, int ySize){
		int mouseX = (int)mousePos.x;
		int mouseY = (int)mousePos.y;
		int modX = (mouseX-20) / multiplier;
		int modY = (mouseY-yOffset) / multiplier;
		int square = ((modX * ySize) + (modY));
		return square;
	}
		
	void placePreviewPiece(int square, int pieceType, int team){
		if (!pieceFolder) {
			GameObject imTar = GameObject.Find("ImageTarget");
			pieceFolder = new GameObject();
			pieceFolder.name = "PieceFolder";
			pieceFolder.transform.parent = imTar.transform;
		}
		GameObject x;
		if (pieces[pieceType].getModel () == null) {
			x = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/counter.prefab", typeof(GameObject));
		} else {
			x = editing.getModel ();
		}
		x = Instantiate (x);
		GamePiece component = x.AddComponent<GamePiece> ();
		component.setName (teams[team].pieces.Count.ToString() +"_" +pieces[pieceType].getTName());
		component.availableMoves = pieces[pieceType].availableMoves;
		component.setSquare (square);
		teams[team].addPiece(component);
		x.name = pieces[pieceType].getTName () + "_" + symbols [pieces[pieceType].getSymb ()];
		scll (x, 0.3F);
		x.transform.parent = pieceFolder.transform;
		if (square != 256) {
			x.transform.position = squares [square].transform.position;
			x.AddComponent<SmoothMove> ().destination = squares [square].transform.position;
		}
	}
		
	void unityTheBoard (){
		GameObject boardPrefab;
		squares.Clear();
		GameObject imTar = GameObject.Find("ImageTarget");
		if (mtFolder) {
			DestroyImmediate (mtFolder);
		}
		mtFolder = new GameObject();
		mtFolder.name = "SquareFolder";
		mtFolder.transform.parent = imTar.transform;
		string path = "Assets/Prefabs/sb.prefab";
		boardPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
		int c = selected.Count;
		int r = rectsToDraw.Count;
		float sqX = 0;
		float sqY = 0;
		double offsetX = ((scaleX-1) * 0.15);
		double offsetY = ((scaleY-1) * 0.15);
		for (int x = 0; x < r; x++) {
			if (selected.Contains (x)) {
				board = Instantiate (boardPrefab, new Vector3 ((float)((sqX*0.3)-offsetX), 0, (float)((-sqY*0.3)+offsetY)), Quaternion.Euler (0, 0, 0));
				squares.Add (board);
				//board.name = "board" + selected [x];
				board.transform.parent = mtFolder.transform;
			}
			sqY++;
			if (scaleY == sqY) {
				sqX++;
				sqY = 0;
			}
		}
	}

	void fullboardImage (){
		if (boardImage) {
			DestroyImmediate (boardImage);
		}
		string path = "Assets/Prefabs/bi.prefab";
		boardImage = (GameObject) AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
		boardImage = Instantiate (boardImage, new Vector3 (0, (float)0.02, 0), Quaternion.Euler (0, 0, 0)) as GameObject;
		boardImage.transform.localScale = new Vector3((float)(scaleX*0.03), (float)0.1, (float)(scaleY*0.03));
		boardImage.GetComponent<Renderer> ().sharedMaterial = backgroundImage;
		boardImage.name = "bimage";
		boardImage.transform.parent = mtFolder.transform;
	}
		
	void scaler(){
		float smallest = Mathf.Min (scaleX, scaleY);
		smallest = smallest * 0.3F;
		float goal = masterScale * 1;
		float mult = goal / smallest;
		Vector3 temp = mtFolder.transform.localScale;
		mtFolder.transform.localScale = new Vector3 (temp.x * mult, temp.y, temp.z * mult);


	}
		
	public static int Tabs(string[] options, int selected)
	{
		const float DarkGray = 0.4f;
		const float LightGray = 0.76078431372f;
		const float StartSpace = 10;

		GUILayout.Space(StartSpace);
		Color storeColor = GUI.backgroundColor;
		Color highlightCol = new Color(LightGray, LightGray, LightGray);
		Color bgCol = new Color(DarkGray, DarkGray, DarkGray);

		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		buttonStyle.padding.bottom = 8;

		GUILayout.BeginHorizontal();
		{   //Create a row of buttons
			for (int i = 0; i < options.Length; ++i)
			{
				GUI.backgroundColor = i == selected ? highlightCol : bgCol;
				if (GUILayout.Button(options[i], buttonStyle))
				{
					selected = i; //Tab click
					//guiLevel = i+2;
					select = 20;
				}
			}
		} GUILayout.EndHorizontal();
		//Restore color
		GUI.backgroundColor = storeColor;
		//Draw a line over the bottom part of the buttons (ugly haxx)
		var texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, highlightCol);
		texture.Apply();
		GUI.DrawTexture(new Rect(0, buttonStyle.lineHeight + buttonStyle.border.top + buttonStyle.margin.top + StartSpace,  Screen.width, 4),texture);

		return selected;
	}
		
	void greetingScreen() {
		GUILayout.BeginArea(new Rect(10, 10, 400, 300));
		GUILayout.Label ("ARTCBG", EditorStyles.boldLabel);
		GUILayout.Label ("Welcome to the Augmented Reality ToolKit for Board Games");
		GUILayout.Space(20);
		var continueButton = GUILayout.Button ("Continue", GUILayout.Width(100));
		GUILayout.EndArea();

		if (continueButton == true) {
			GameObject imTar = GameObject.Find("ARCamara");
			if (imTar) {
				DestroyImmediate(imTar);
			}
			GameObject cameraAR = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Vuforia/Prefabs/ARCamera.prefab", typeof(GameObject));
			cameraAR = Instantiate (cameraAR);
			cameraAR.AddComponent<GameLogic> ();
			cameraAR.name = "ARCamera";
			placer = false;
			pkount = 0;
			ssel = "";
			psel = "";
			ibsName = "";
			itpName = "";
			guiLevel = 2;
			imageTarget = null;
			imTar = GameObject.Find("ImageTarget");
			if (imTar) {
				DestroyImmediate(imTar);
			}

			gData = cameraAR.AddComponent<SomeData>();
			GameObject inputT = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/BasicUI.prefab", typeof(GameObject));
			Instantiate (inputT);
		}
	}
		
	void addImageTarget() {
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 0) + 2;
		GUILayout.BeginArea(new Rect(10, 10, 400, 700));
		GUILayout.Space(25);
		GUILayout.Label ("Setup Target image", EditorStyles.boldLabel);
		GUILayout.Space(10);
		bool pitBtn = GUILayout.Button("New Target",GUILayout.Width(100));
		GUILayout.Label ("Instructions...");
		GUILayout.Space(10);
		GUILayout.Label ("Target Image selected : "+ibsName, EditorStyles.boldLabel);
		GUILayout.Space(10);
		Texture boarImage = (Texture) AssetDatabase.LoadAssetAtPath("Assets/Editor/QCAR/ImageTargetTextures/toolkit/"+ ibsName + "_scaled.jpg", typeof(Texture));
		if (boarImage) {
			GUILayout.Box (boarImage,GUILayout.Width(300),GUILayout.Height(300));
		}
		GUILayout.EndArea();

		if (pitBtn) {
			GameObject imTar = GameObject.Find("ImageTarget");
			if (imTar) {
				DestroyImmediate(imTar);
			}
			imageTarget = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Vuforia/Prefabs/ImageTarget.prefab", typeof(GameObject));
			imageTarget = Instantiate (imageTarget);
			imageTarget.name = "ImageTarget";


		}
		if (imageTarget) {
			Vuforia.ImageTargetBehaviour ibs = imageTarget.GetComponent<Vuforia.ImageTargetBehaviour> ();
			ibsName = ibs.TrackableName;
		}
	}
		
	void boardScreen() {
		boardPreview = true; 
		boardType = 1;
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 1) + 2;
		GUILayout.BeginArea(new Rect(10, 10, 400, 700));
		GUILayout.Space(25);
		GUILayout.Label ("Board Designer", EditorStyles.boldLabel);
		GUILayout.Space(5);
		GUILayout.Label ("Board Dimensions");
		GUILayout.BeginHorizontal();
		GUILayout.Label ("X",EditorStyles.boldLabel);
		scaleX = EditorGUILayout.IntSlider(scaleX,1, 16,GUILayout.Width(350));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Y",EditorStyles.boldLabel);
		scaleY = EditorGUILayout.IntSlider(scaleY,1, 16,GUILayout.Width(350));
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
		GUILayout.Label ("On the preview, select which squares you want active");
		showBtn = GUILayout.Button("(De)Select All",GUILayout.Width(100));
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		backgroundImage = (Material) EditorGUILayout.ObjectField("Overlay Image", backgroundImage, typeof (Material), false); 
		//Texture boarImage = (Texture) AssetDatabase.LoadAssetAtPath("Assets/Prefabs/tag.png", typeof(Texture));
		if (backgroundImage) {
			//	GUILayout.Box (backgroundImage.mainTexture);
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(430);
		GUILayout.Label ("Scale compared to Target Image");
		masterScale = EditorGUILayout.Slider(masterScale,1.0F, 10.0F, GUILayout.Width(350));
		GUILayout.BeginHorizontal();
		//GUILayout.Space(290);
		var updatePB = GUILayout.Button ("Apply", GUILayout.Width(100));
		//var doneB = GUILayout.Button ("Continue", GUILayout.Width(100));
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		if (updatePB == true) {
			DestroyImmediate (pieceFolder);
			unityTheBoard ();
			fullboardImage ();
			gData.scaleX = scaleX;
			gData.scaleY = scaleY;
			gData.squares = squares;
			scaler ();
		}

		if (showBtn == true) {
			reColourInd = !reColourInd;
			if (reColourInd) {
				selecter (scaleX, scaleY);
			} else {
				selected.Clear ();
			}
		} 
		if (Event.current.type == EventType.MouseDown && Event.current.mousePosition.y > yOffset) {
			colourInderviduals(Event.current.mousePosition,scaleX,scaleY);
		}
	}
		
	void teamScreen () {
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 2) + 2;
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Space(25);
		GUILayout.Label ("Team Setup", EditorStyles.boldLabel);
		GUILayout.Label ("Configure the teams/players for your board game", EditorStyles.boldLabel);
		var addBtn = GUILayout.Button("Add Team",GUILayout.Width(100));
		for (int x=0;x<teams.Count;x++){
			GUILayout.BeginHorizontal();
			teams[x].setName(EditorGUILayout.TextField("Team name: ", teams[x].getName()));
			Color matColor = teams[x].getColor ();
			matColor = EditorGUILayout.ColorField("Team Colour:", matColor);
			teams[x].setColor (matColor);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			var removeB = GUILayout.Button ("Remove", GUILayout.Width(100));
			GUILayout.EndHorizontal();
			if (removeB) {
				teams.RemoveAt(x);
			}
			GUILayout.Space(50);

		}
		GUILayout.EndArea();
		if (addBtn == true) {
			if (teams.Count < 10) {
				GameObject teamsFolder = GameObject.Find("TeamFolder");
				if (teamsFolder == null) {
					teamsFolder = new GameObject();
					teamsFolder.name = "TeamFolder";
					teamsFolder.transform.parent = imageTarget.transform;
				}
				GameObject teamHolder = new GameObject();
				teamHolder.name = ("Team " + (teams.Count +1));
				teamHolder.transform.parent = teamsFolder.transform;
				Team newTeam = teamHolder.AddComponent<Team> ();
				newTeam.setName ("Team " + (teams.Count +1));
				newTeam.setColor (new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F)));
				teams.Add (newTeam);
			}

		}

	}

	void arPieceScreen () {
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 3) + 2;
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Space(25);
		GUILayout.Label ("Add AR Pieces", EditorStyles.boldLabel);
		GUILayout.Label ("Configure AR pieces for your game");

		var addBtn = GUILayout.Button("Add Team",GUILayout.Width(100));
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
		for (int x=0;x<pieces.Count;x++){
			GUILayout.BeginHorizontal();
			pieces[x].setTName(EditorGUILayout.TextField("Piece name: ", pieces[x].getTName()));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			var removeB = GUILayout.Button ("Remove", GUILayout.Width(100));
			GUILayout.EndHorizontal();
			GameObject model = pieces[x].getModel();
			model = (GameObject) EditorGUILayout.ObjectField("Piece Object", model, typeof (GameObject), false);
			choiceIndexSym = pieces [x].getSymb ();
			choiceIndexSym = EditorGUILayout.Popup(choiceIndexSym,symbols, GUILayout.Width(200));
			pieces[x].setSymb(choiceIndexSym);
			pieces [x].setModel (model);
			if (removeB) {
				pieces.RemoveAt(x);
			}
			GUILayout.Space(50);
		}
		EditorGUILayout.EndScrollView();

		if (addBtn == true) {
			if (pieces.Count < 10) {
				Piece newPiece = new Piece ();
				newPiece.setTName ("Piece " + (pieces.Count +1));
				pieces.Add (newPiece);
				newPiece.setSymb (pieces.Count - 1);
			}

		}
		GUILayout.EndArea();

	}

	string[][] dropdowns(){
		string[] tn = new string[teams.Count];
		string[] pn = new string[pieces.Count];
		for (int y = 0; y < teams.Count; y++) {
			tn [y] = teams [y].getName ();
		}
		for (int w = 0; w < pieces.Count; w++) {
			pn [w] = pieces[w].getTName () + " " + symbols[pieces[w].getSymb ()] ;
		}
		string[][] temp = { tn, pn };
		return temp;
	}
		
	void startStateScreen () {
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 4) + 2;
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Space(25);
		guiLevel = guiLevel + Tabs (new string[2] { "Piece View", "Board View" }, 0);

		GUILayout.Space(10);
		string[][] temp = dropdowns ();
		string[] teamnames = temp [0];
		string[] piecenames = temp [1];
		Color c = GUI.backgroundColor;
		GUILayout.BeginHorizontal();
		if (teams.Count > 0) {
			GUI.backgroundColor = teams [choiceIndex].getColor ();
		}

		choiceIndex = EditorGUILayout.Popup(choiceIndex, teamnames, GUILayout.Width(100));
		choiceIndex2 = EditorGUILayout.Popup(choiceIndex2, piecenames, GUILayout.Width(100));
		GUI.backgroundColor = c;

		var addB = GUILayout.Button ("Add", GUILayout.Width(100));
		GUILayout.Space(25);

		if (addB) {
			pkount++;
			placePreviewPiece (256, choiceIndex2, choiceIndex);
		}
		GUI.backgroundColor = c;
		EditorGUILayout.EndHorizontal();

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,GUILayout.Width(400), GUILayout.Height(600));
		for (int s = 0; s < teams.Count; s++) {
			foreach (GamePiece piece in teams[s].pieces) {
				piece.setName(EditorGUILayout.TextField("Name: ", piece.getName()));
				GUILayout.BeginHorizontal();
				Color y = GUI.backgroundColor;
				if (piece.getSquare() != 256) {
					GUI.backgroundColor = Color.red;
				}
				var placeB = GUILayout.Button ("Place", GUILayout.Width(100));
				if (piece.isTrack ()) {
					GUI.backgroundColor = Color.red;
				} else
					
					GUI.backgroundColor = y;
				var trackerB = GUILayout.Button ("Tracker", GUILayout.Width(100));
				GUI.backgroundColor = y;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();

				GUILayout.Label (teams[s].getName() + " | " + piece.getTName() + " " + symbols[piece.getSymb()] + " | " + typeForLabel(piece) , EditorStyles.boldLabel);
				Texture boarImage = (Texture) AssetDatabase.LoadAssetAtPath("Assets/Editor/QCAR/ImageTargetTextures/toolkit/"+ piece.getTI() + "_scaled.jpg", typeof(Texture));
				if (boarImage) {
					GUILayout.Box (boarImage,GUILayout.Width(60),GUILayout.Height(60));
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label ("Size : ");
				piece.setSize(EditorGUILayout.IntSlider(piece.getSize(),1, 100,GUILayout.Width(300)));
				scll (piece.getgp(), (float)piece.getSize()/100);
				GUILayout.EndHorizontal();
				GUILayout.Space(25);
				if (placeB) {
					editing = piece;
					GameObject imTar = GameObject.Find (editing.getName () + "_" + symbols [editing.getSymb ()] + "_IT");
					if (imTar) {
						DestroyImmediate (imTar);
					}
					editing.unTrackify ();
					placer = true;
					guiLevel = 8;
				} else if (trackerB) {
					editing = piece;
					editing.setSquare (256);
					GameObject imTar = GameObject.Find (editing.getName () + "_" + symbols [editing.getSymb ()] + "_IT");
					if (imTar) {
						DestroyImmediate (imTar);
					}
					editing.unTrackify ();
					itpName = "";
					editing = piece;
					DestroyImmediate (editing.getgp ());
					guiLevel = 9;
				} 
			}
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea ();
	}

	string typeForLabel(GamePiece x){
		if (x.isTrack ()) {
			return "AR Tracked";
		} else if (x.getSquare() != 256) {
			return "Placed - "+IndexToColumn (x.getSquare()/scaleX, x.getSquare()%scaleX);
		}
		else
			return "";
	}
		
	void thePlacer (){
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 5) + 2;
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Space(25);
		guiLevel = guiLevel + Tabs (new string[2] { "Piece View", "Board View" }, 1);

		GUILayout.Space(10);
		string[][] temp = dropdowns ();
		string[] teamnames = temp [0];
		string[] piecenames = temp [1];
		Color c = GUI.backgroundColor;
		GUILayout.BeginHorizontal();
		if (teams.Count > 0) {
			GUI.backgroundColor = teams [choiceIndex].getColor ();
		}
		choiceIndex = EditorGUILayout.Popup(choiceIndex, teamnames, GUILayout.Width(100));
		choiceIndex2 = EditorGUILayout.Popup(choiceIndex2, piecenames, GUILayout.Width(100));
		if (placer) {
			GUI.backgroundColor = Color.red;
		} else {
			GUI.backgroundColor = c;
		}
		var placeB = GUILayout.Button ("Place", GUILayout.Width(100));
		GUI.backgroundColor = c;

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label (ssel + " - ", EditorStyles.boldLabel);
		GUILayout.Label (psel);
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
		GUILayout.Space (600);
		var clearSqB = GUILayout.Button ("Clear Square", GUILayout.Width(100));
		var clearB = GUILayout.Button ("Clear All", GUILayout.Width(100));

		if (Event.current.type == EventType.MouseDown && Event.current.mousePosition.y > yOffset) {
			square = workOutSquare (Event.current.mousePosition,scaleX,scaleY);
			if (editing != null) {
				editing.setSquare (square);
				editing.getgp ().transform.position = squares[square].transform.position;
				editing = null;
				guiLevel = 6;
			} else if (placer) {
				placePreviewPiece (square, choiceIndex2, choiceIndex);
			}
			ssel = IndexToColumn (square/scaleX, square%scaleX);
			psel = "";
			int counnt = 0;
			for (int s = 0; s < teams.Count; s++) {
				foreach (GamePiece piece in teams[s].pieces) {
					if (piece.getSquare () == square) {
						counnt++;
						psel = psel + "Team " + s + ": " + piece.getTName () + "  ";
						if (counnt == 3) {
							psel = psel + "\n";
							counnt = 0;
						}
					}
				}
			}

		}
		if (placeB) {
			placer = !placer;
		}
		if (clearB) {
			foreach (Team team in teams) {
				team.clearPieces ();
			}
			psel = "";
			DestroyImmediate (pieceFolder);
		}
		if (clearSqB) {
			for (int i = 0; i < teams.Count; i++) {
				for (int r = teams[i].pieces.Count -1; r >= 0; r--) {
					if (teams[i].pieces[r].getSquare() == square) {
						teams [i].pieces.Remove (teams [i].pieces [r]);
					}
				}
			}
			psel = "";
		}

	}

	void trackerInst (){
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Label ("Setup Target image", EditorStyles.boldLabel);
		GUILayout.Space(10);
		bool pitBtn = GUILayout.Button("Add Target",GUILayout.Width(100));
		GUILayout.Label ("Instructions...");
		GUILayout.Space(10);
		GUILayout.Label ("Target Image selected : "+itpName, EditorStyles.boldLabel);
		GUILayout.Space(10);
		Texture boarImage = (Texture) AssetDatabase.LoadAssetAtPath("Assets/Editor/QCAR/ImageTargetTextures/toolkit/"+ itpName + "_scaled.jpg", typeof(Texture));
		if (boarImage) {
			editing.trackify ();
			GUILayout.Box (boarImage,GUILayout.Width(300),GUILayout.Height(300));
		}
		var backB = GUILayout.Button ("Done", GUILayout.Width(100));
		if (backB) {
			guiLevel = 7;
			if (!boarImage) {
				GameObject imTar = GameObject.Find(editing.getName() + "_" + symbols[editing.getSymb()] + "_IT");
				if (imTar) {
					DestroyImmediate(imTar);
				}
				editing.unTrackify ();

			}
		}

		GUILayout.EndArea();

		if (pitBtn) {
			GameObject imTar = GameObject.Find(editing.getName() + "_" + symbols[editing.getSymb()] + "_IT");
			if (imTar) {
				DestroyImmediate(imTar);
			}
			imageTarget2 = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Vuforia/Prefabs/ImageTarget.prefab", typeof(GameObject));
			imageTarget2 = Instantiate (imageTarget2);
			imageTarget2.transform.position =  new Vector3(0, 0, 1);
			imageTarget2.name = editing.getName() + "_" + symbols[editing.getSymb()] + "_IT";
			GameObject x;
			if (editing.getModel () == null) {
				x = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/counter.prefab", typeof(GameObject));
			} else {
				x = editing.getModel ();
			}
			x = Instantiate (x);
			x.name = editing.getName () + "_" + symbols [editing.getSymb ()];
			scll (x, 2);
			//GamePiece component = x.AddComponent<GamePiece> ();
			editing.setgp (x);

			x.transform.parent = imageTarget2.transform;
			//x.transform.localScale = imageTarget2.transform.localScale;
			//x.transform.lossyScale = new Vector3 (6, 6, 6);
			x.transform.transform.position = imageTarget2.transform.position;


		}
		if (imageTarget2) {
			Vuforia.ImageTargetBehaviour itp = imageTarget2.GetComponent<Vuforia.ImageTargetBehaviour> ();
			itpName = itp.TrackableName;
			editing.setTI (itpName);

		}

	}
		
	void scll(GameObject x, float sizeS){
		if (x != null) {
			Bounds a = x.GetComponent<MeshRenderer> ().bounds;
			Vector3 obj1_size = a.max - a.min;
			x.transform.localScale = x.transform.localScale * (componentMax (new Vector3 (sizeS, sizeS, sizeS)) / componentMax (obj1_size));
		}
	}
		
	float componentMax(Vector3 a)
	{
		return Mathf.Max(Mathf.Max(a.x, a.y), a.z);
	}

	Vector3 div(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
	}
		
	void rulePage (){
		guiLevel = Tabs (new string[6] {"Target Image","Board Designer","Teams","Pieces","Moves","startState"}, 4) + 2;
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Space(20);
		GUILayout.Label ("Rules", EditorStyles.boldLabel);
		string[][] temp = dropdowns ();
		string[] piecenames = temp [1];
		choiceIndex2 = EditorGUILayout.Popup(choiceIndex2,piecenames, GUILayout.Width(200));
		GUILayout.Space(20);
		bool moveBut = GUILayout.Button("Add Moves",GUILayout.Width(100));
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,GUILayout.Width(400), GUILayout.Height(600));
		for(int q= 0;q<pieces[choiceIndex2].availableMoves.Count;q++){
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (pieces[choiceIndex2].availableMoves[q].name, EditorStyles.boldLabel);
			bool editB = GUILayout.Button("Edit",GUILayout.Width(100));
			bool removeB = GUILayout.Button("Remove",GUILayout.Width(100));
			if (removeB) {
				pieces [choiceIndex2].availableMoves.RemoveAt(q);
			}
			if (editB) {
				newMove = pieces [choiceIndex2].availableMoves [q];
				pieces [choiceIndex2].availableMoves.RemoveAt(q);
				guiLevel = 10;
			}
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.EndScrollView ();
		GUILayout.EndArea();

		if (moveBut) {
			guiLevel = 10;
			newMove = CreateInstance<Move> ();
			newMove.name = pieces [choiceIndex2].getTName() + "_Move_" + pieces [choiceIndex2].availableMoves.Count.ToString ();

		}

	}

	void drawMove () {
		dMGrid ();
		GUILayout.BeginArea (new Rect (10, 10, 400, 700));
		GUILayout.Space(20);
		GUILayout.Label ("Rules", EditorStyles.boldLabel);
		bool moveBut = GUILayout.Button("Done",GUILayout.Width(100));
		GUILayout.Space(20);
		newMove.name = EditorGUILayout.TextField("Piece name: ", newMove.name);

		GUILayout.BeginHorizontal();
		Color y = GUI.backgroundColor;
		if (thing == 1) {
			GUI.backgroundColor = Color.red;
		}
		bool addMove = GUILayout.Button("Move",GUILayout.Width(100));
		GUI.backgroundColor = y;
		if (thing == 2) {
			GUI.backgroundColor = Color.red;
		}
		bool addEmpty = GUILayout.Button("Empty",GUILayout.Width(100));
		GUI.backgroundColor = y;
		if (thing == 3) {
			GUI.backgroundColor = Color.red;
		}
		bool addOpponant = GUILayout.Button("Opponant",GUILayout.Width(100));
		GUI.backgroundColor = y;
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		if (moveBut) {
			guiLevel = 6;
			thing = 0;
			pieces [choiceIndex2].availableMoves.Add (newMove);
		}
		if (addMove) {
			thing = 1;
		}
		else if (addEmpty) {
			thing = 2;
		}
		else if (addOpponant) {
			thing = 3;
		}

		if (Event.current.type == EventType.MouseDown && Event.current.mousePosition.y > yOffset) {
			square = workOutSquare (Event.current.mousePosition,7,7);
			if (thing == 1) {
				if (newMove.destinations.Contains(square)){
					newMove.destinations.Remove (square);
				} else {
					newMove.destinations.Add (square);
				}
			}
			if (thing == 2) {
				if (newMove.emptyConditions.Contains(square)){
					newMove.emptyConditions.Remove (square);
				} else {
					newMove.emptyConditions.Add (square);
				}
			}
			if (thing == 3) {
				if (newMove.enemConditions.Contains(square)){
					newMove.enemConditions.Remove (square);
				} else {
					newMove.enemConditions.Add (square);
				}
			}
		}
	}

	void dMGrid(){
		int xCount = 0;
		int yCount = 0;
		GUIStyle style = new GUIStyle ();
		style.fontSize = 55;
		for (int x = 0; x < (399); x = x + 57) {
			for (int y = 0; y < (399); y = y + 57) {
				if (((xCount == 0 ||xCount == 3|| xCount == 6)&&(yCount == 0 ||yCount == 3|| yCount == 6))||(xCount != 0 && xCount != 6 && yCount != 0 && yCount != 6)){
					Vector3[] verts = new Vector3[] {
						new Vector3 (20 + x, 210 + y),
						new Vector3 (77 + x, 210 + y),
						new Vector3 (77 + x, 267 + y),
						new Vector3 (20 + x, 267 + y)
					};

					Color z = Handles.color;
					if (newMove.destinations.Contains ((xCount * 7) + yCount)) {
						Handles.color = Color.red;
					}
					if (newMove.emptyConditions.Contains ((xCount * 7) + yCount)) {
						Handles.Label (new Vector3 (25 + x, 210 + y), "Empty");
					}
					if (newMove.enemConditions.Contains ((xCount * 7) + yCount)) {
						Color h = style.normal.textColor;
						style.normal.textColor = Color.red;
						Handles.Label (new Vector3 (25 + x, 210 + y), "◌", style);
						style.normal.textColor = h;
					}
					Handles.DrawSolidRectangleWithOutline (verts, new Color (1, 1, 1, 0.2f), new Color (0, 0, 0, 1));

					Handles.color = z;
					if (xCount == 0 || xCount == 6 || yCount == 0 || yCount == 6) {
						Handles.Label (new Vector3 (25 + x, 210 + y), "∞");
					} else if (xCount == 3 && yCount == 3) {
						Handles.Label (new Vector3 (30 + x, 200 + y), symbols[pieces[choiceIndex2].getSymb()], style);
					}
				}
				yCount++;
			}
			yCount = 0;
			xCount++;
		}
		HandleUtility.Repaint ();
	}
}