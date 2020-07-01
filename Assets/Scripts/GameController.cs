/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    [SerializeField] private FpsMovement player;
    [SerializeField] private Text timeLabel;
    [SerializeField] private Text scoreLabel;

    private MazeConstructor generator;

    private DateTime startTime;
    private int timeLimit;

    private int score;
    private bool goalReached;

	private int sizeRows;
	private int sizeCols;

	private readonly System.Random random = new System.Random();

    // Use this for initialization
    void Start() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

    private void StartNewGame()
    {
        timeLimit = 0;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
    }

    private void StartNewMaze()
    {
		sizeRows = random.Next(11, 31);
		sizeCols = random.Next(11, 31);
		Debug.Log ("Rows: " + sizeRows.ToString() + "  Cols: " + sizeCols.ToString());
		generator.GenerateNewMaze(sizeRows, sizeCols, OnStartTrigger, OnGoalTrigger);

        float x = generator.startCol * generator.hallWidth;
        float y = 1;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);

        goalReached = false;
        player.enabled = true;

        // restart timer
        //startTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey (KeyCode.Escape)) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Debug.Log (Application.platform);
			if (Application.platform != RuntimePlatform.WebGLPlayer) {
				Application.Quit ();
			}
		}
		if (Input.GetMouseButtonDown (0) || Input.GetKeyDown(KeyCode.Space)) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
        if (!player.enabled)
        {
            return;
        }

        int timeUsed = (int)(DateTime.Now - startTime).TotalSeconds;
        int timeLeft = timeLimit + timeUsed;

        timeLabel.text = timeLeft.ToString();
    }

    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;

        score += 1;
        scoreLabel.text = score.ToString();

		Destroy(trigger);
		player.enabled = false;

		Invoke("StartNewMaze", 4);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            Debug.Log("Finish!");
            player.enabled = false;

            Invoke("StartNewMaze", 4);
        }
    }
}
