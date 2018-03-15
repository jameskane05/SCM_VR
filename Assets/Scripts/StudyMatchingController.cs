﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StudyMatchingController : MonoBehaviour {

	[SerializeField] private GameObject player;
	private GameObject playerInstance;
	private Transform[] pathWaypoints;
	private Vector3[] pathPositions;
	static public ExperimentSettings _expInstance;
	private bool hasStarted = false;
	[SerializeField] private float tourDuration;
	[SerializeField] private GameObject[] spheres;
	[SerializeField] private int pauseBeforeStart;
	[SerializeField] private int pauseAfterFinish;
	[SerializeField] Canvas canvas;
	[SerializeField] Text text;
	[SerializeField] private GameObject triviaPanel;
	private bool triviaStarted = false;
	private GameObject activeSpheres;

	void Start () {
		_expInstance = ExperimentSettings.GetInstance ();

		if (_expInstance.Phase == PhaseEnum.StudyVideoMatching) {
			Debug.Log("StudyVideoMatching ongoing, starting trial: " + _expInstance.VideoMatchingTrialIndex.ToString() );
		}
		GameObject path = GameObject.Find (spheres [_expInstance.MatchingTrialIndex % 9].name [0].ToString());
		Debug.Log ("path gameObject name: " + path.ToString ());
		Transform[] pathWaypoints = path.GetComponentsInChildren<Transform>();
		Debug.Log("pathWayoints length: " + pathWaypoints.Length);
		pathPositions = new Vector3[pathWaypoints.Length - 1];
		for (int i = 1; i <= pathPositions.Length; i++) {
			pathPositions [i - 1] = new Vector3(pathWaypoints [i].position.x,pathWaypoints [i].position.y+1,pathWaypoints [i].position.z);
		}
		activeSpheres = spheres[_expInstance.MatchingTrialIndex % 9];
		activeSpheres.SetActive (true);

		playerInstance = Instantiate (player, new Vector3(pathPositions[0].x,pathPositions[0].y,pathPositions[0].z), pathWaypoints[1].rotation);
	}

	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown ("space") && !hasStarted) {
			Image img = canvas.GetComponent<Image>();
			img.color = UnityEngine.Color.clear;
			text.gameObject.SetActive (false);
			StartCoroutine (PauseBeforeStart ());
			hasStarted = true;
		}
	}

	IEnumerator PauseBeforeStart (){
		yield return new WaitForSeconds (pauseBeforeStart);
		StartPath ();
	}

	void StartPath(){
		DOTween.Init();
		playerInstance.transform.DOPath (pathPositions, tourDuration, PathType.Linear)
			.SetLookAt (.03f)
			.SetEase (Ease.Linear)
			.OnComplete(StartPauseAfterFinishCoroutine);
		DOTween.Play (player);
	}

	void StartPauseAfterFinishCoroutine(){
		StartCoroutine (PauseAfterFinish ());
	}


	IEnumerator PauseAfterFinish (){
		yield return new WaitForSeconds (pauseAfterFinish);
		triviaPanel.SetActive (true);


		//WRITE TO FILE MATCHING ANSWER - INCLUDING WHAT PATH, ANSWER GIVEN, CORRECT ANSWER.




		while (!Input.GetKeyDown (KeyCode.Alpha5) && !Input.GetKeyDown (KeyCode.Alpha9))
			yield return null;



		if (_expInstance.Phase == PhaseEnum.StudyVideoMatching) {
			if (!File.Exists (_expInstance.FileName))
				System.IO.File.WriteAllText (_expInstance.FileName, 
					"Matching Path No.: " + (_expInstance.MatchingTrialIndex % 9) + "\r\n");
			else
				System.IO.File.AppendAllText (_expInstance.FileName, 
					"Matching Path No.: " + (_expInstance.MatchingTrialIndex % 9) + "\r\n");


			if (Input.GetKeyDown (KeyCode.Alpha5)) {
				_expInstance.MatchingAnswers [_expInstance.MatchingTrialIndex % 9] = "Match";
				if (activeSpheres.name [3].ToString () == "a") {
					_expInstance.MatchingScore++;
					System.IO.File.AppendAllText (_expInstance.FileName, "Answer Given: match, correct\r\n");
				}
				else 
					System.IO.File.AppendAllText (_expInstance.FileName, "Answer Given: match, incorrect\r\n");
				
			} else if (Input.GetKeyDown (KeyCode.Alpha9) && _expInstance.Phase == PhaseEnum.StudyVideoMatching) {
				_expInstance.MatchingAnswers [_expInstance.MatchingTrialIndex % 9] = "Mismatch";
				if (activeSpheres.name [3].ToString () == "i") {
					_expInstance.MatchingScore++;
					System.IO.File.AppendAllText (_expInstance.FileName, "Answer Given: mismatch, correct\r\n");
				}
				else 
					System.IO.File.AppendAllText (_expInstance.FileName, "Answer Given: mismatch, incorrect\r\n");
				
			}


			_expInstance.VideoMatchingTrialIndex++;
			_expInstance.MatchingTrialIndex++;

			if (_expInstance.VideoMatchingTrialIndex > 17) {
				Text text = GameObject.Find ("Text").GetComponent<Text> ();
				text.text = "You have completed the trials.";

				System.IO.File.AppendAllText (_expInstance.FileName, "Total Matching Score: " + _expInstance.MatchingScore + " out of 9 \r\n\r\n");
				_expInstance.VideoMatchingTrialIndex = 0;
			} else {
				Debug.Log ("StudyVideoMatching ongoing, ending trial: " + _expInstance.VideoMatchingTrialIndex.ToString ());
				SceneManager.LoadScene (_expInstance.VideoMatchingOrder [_expInstance.VideoMatchingTrialIndex - 1].ToString ());
			}
		}

		Debug.Log ("MatchingTrialIndex: " + (_expInstance.MatchingTrialIndex).ToString());
		Debug.Log ("MatchingScore: " + _expInstance.MatchingScore.ToString());
		Debug.Log ("VideoMatchingTrialIndex: " + (_expInstance.VideoMatchingTrialIndex % 19).ToString());

		if (_expInstance.Phase == PhaseEnum.StudyMatching) {
			SceneManager.LoadScene (0);
		}
	}
}