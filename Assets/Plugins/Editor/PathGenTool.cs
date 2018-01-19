using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathGenTool : EditorWindow {

	GameObject obj;
	[MenuItem ("GameObject/Path Gen. Tool")] //Place the Set Pivot menu item in the GameObject menu
    static void Init () {
        PathGenTool window = (PathGenTool)EditorWindow.GetWindow (typeof (PathGenTool));
        window.Show ();
    }

	int group=0;
	int X=0;
	int Z=0;
	int NodesCount=0;
	void OnGUI() {
		
		 X = EditorGUILayout.IntField("X",X);
		 Z = EditorGUILayout.IntField("Z",Z);
		 NodesCount = EditorGUILayout.IntField("Number of Nodes",NodesCount);
		//GameObject g = new GameObject();
		GameObject g = (GameObject)EditorGUILayout.ObjectField("Object to Spawn",obj,typeof(GameObject),true);
		if (GUILayout.Button("Generate"))
		{
			group=0;
			GameObject rootPath = new GameObject("rootPath");
			int step = NodesCount;///(Width+depth);
			
			GenerateNodesAlongPath(rootPath,new Vector2(0,0),new Vector2(X,0),step);
			GenerateNodesAlongPath(rootPath,new Vector2(X,0),new Vector2(X,Z),step);
			GenerateNodesAlongPath(rootPath,new Vector2(X,Z),new Vector2(0,Z),step);
			GenerateNodesAlongPath(rootPath,new Vector2(0,Z),new Vector2(0,0),step);
			
			
			
			
		}
	}
	// Use this for initialization
	void GeneratePath (int Width,int Depth, int NodesCount, GameObject g) {
		
		
	

	}
	
	// Update is called once per frame
	void GenerateNodesAlongPath(GameObject root, Vector2 start,Vector2 end, int step) {
		string Axis = start.x == end.x ? "Z":"X";
		int limit =0;
		group++;
		switch (Axis)
		{
			case "X":
				float StartX=0f;
				float EndX=0f;
				float dir=1;
				if (start.x > end.x)
				{
 					StartX = start.x;
					EndX   = end.x;
					dir = -1;
				}
				else
				{
					 StartX = Mathf.Min(start.x, end.x);
					 EndX   = Mathf.Max(start.x, end.x);
				}
				
				while (StartX != EndX)
				{
					//Add Node(StartX,start.y);
					GameObject n = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					n.name = ("NodeX"+group.ToString());
					n.transform.SetParent(root.transform);
					n.transform.position = new Vector3(StartX,0,start.y);
					SphereCollider sc = n.GetComponent<SphereCollider>();
					sc.transform.localScale = new Vector3(2f,2f,2f);
					if (sc)
					{
						DestroyImmediate(sc);
					}
					StartX =StartX + (step*dir);
					if(limit++ >10000)
					{
						Debug.Log("Crash:X"+StartX+":"+step);
						break;
					}
				}

			break;
			
			case "Z":
				float StartZ=0f;
				float EndZ=0f;
				float dirZ=1;
				if (start.y > end.y)
				{
					StartZ = start.y;
					EndZ   = end.y;
					dirZ = -1;
				}
				else
				{
					StartZ = Mathf.Min(start.y, end.y);
					EndZ   = Mathf.Max(start.y, end.y);
				}
				
					while (StartZ != EndZ)
					{
						//Add Node(StartX,start.y);
						GameObject n =  GameObject.CreatePrimitive(PrimitiveType.Sphere);
						n.name = ("NodeX"+group.ToString());
						n.transform.SetParent(root.transform);
						n.transform.position = new Vector3(start.x,0,StartZ);
						SphereCollider sc = n.GetComponent<SphereCollider>();
						sc.transform.localScale = new Vector3(2f,2f,2f);
						if (sc)
						{
							DestroyImmediate(sc);
						}
						StartZ =StartZ + (step*dirZ);
						if(limit++ >10000)
						{
							Debug.Log("CrashZ:"+StartZ+":"+step);
							break;
						}
					}
			break;
		}

	}
}
