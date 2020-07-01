﻿/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    public int[,] data
    {
        get; private set;
    }

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    private MazeDataGenerator dataGenerator;
	private MazeMeshGenerator meshGenerator;
	private readonly System.Random random = new System.Random();

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();

        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols,
        TriggerEventHandler startCallback=null, TriggerEventHandler goalCallback=null)
    {
		if (sizeRows % 2 == 0)
			sizeRows += 1;
		if (sizeCols % 2 == 0)
			sizeCols += 1;

        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);

		Pair start = GetRandomPosition ();
		startRow = start.First;
		startCol = start.Second;
		Pair goal = GetRandomPosition ();
		goalRow = goal.First;
		goalCol = goal.Second;

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();

        //PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] {mazeMat1, mazeMat2};
    }

    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects) {
            Destroy(go);
        }
    }

	/*
    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }
    */

	private Pair GetRandomPosition()
    {
        int[,] maze = data;
		List<Pair> possible_locations = new List<Pair>();
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // loop top to bottom, right to left
		for (int i = 0; i <= rMax; i++)
		{
			for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
					possible_locations.Add (new Pair (i, j));
                }
            }
        }
		Pair selection = possible_locations[random.Next (possible_locations.Count)];
		Debug.Log (string.Format("From {0} options, picked {1}", possible_locations.Count, selection.ToString()));
		return selection;
    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth);
        go.name = "Treasure";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    // top-down debug display
    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        // loop top to bottom, left to right
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }
}

class Pair {
	public int First { get; private set; }
	public int Second { get; private set; }
	public Pair(int first, int second) {
		this.First = first;
		this.Second = second;
	}

	public override bool Equals(object obj) {
		if(Object.ReferenceEquals(this, obj)) {
			return true;
		}
		Pair instance = obj as Pair;
		if(instance == null) {
			return false;
		}
		return this.First == instance.First && this.Second == instance.Second ||
			this.First == instance.Second && this.Second == instance.First;
	}

	public override int GetHashCode() {
		return this.First.GetHashCode() ^ this.Second.GetHashCode();
	}

	public override string ToString ()
	{
		return this.First.ToString () + "," + this.Second.ToString ();
	}
}
