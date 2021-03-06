using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StudyVideoController : MonoBehaviour {
	
	[SerializeField] private Transform player;
	[SerializeField] private Transform[] path;
	[SerializeField] private Text text;
	[SerializeField] private Canvas canvas;
	[SerializeField] private float tourDuration;
	[SerializeField] private int pauseBeforeStart;
	[SerializeField] private int pauseAfterFinish;
	private bool hasStarted = false;
	public static ExperimentSettings _expInstance;

	void Start(){
		_expInstance = ExperimentSettings.GetInstance ();

		if (_expInstance.Phase == PhaseEnum.StudyVideoMatching) {
			Debug.Log("StudyVideoMatching ongoing, starting trial: " + _expInstance.VideoMatchingTrialIndex.ToString() );
		}
	}

	void Update () {
		if (Input.GetKeyDown ("space") && !hasStarted) {
			Image img = canvas.GetComponent<Image>();
			img.color = UnityEngine.Color.clear;
			text.gameObject.SetActive (false);
			StartCoroutine (PauseBeforeStart ());
			hasStarted = true;
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			SceneManager.LoadScene ("Menu");
		}
	}

	void StartPath () {

		DOTween.Init();
		Vector3[] waypoints = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++){
			waypoints.SetValue(new Vector3(path[i].position.x,path[i].position.y,path[i].position.z),i);
		}
		player.transform.DOPath (waypoints, tourDuration, PathType.Linear)
			.SetLookAt (.03f)
			.SetEase (Ease.Linear)
			.OnComplete(StartPauseAfterFinishCoroutine);
	}

	void StartPauseAfterFinishCoroutine(){
		StartCoroutine (PauseAfterFinish ());
	}

	IEnumerator PauseBeforeStart (){
		yield return new WaitForSeconds (pauseBeforeStart);
		StartPath ();
	}

	IEnumerator PauseAfterFinish (){
		_expInstance.VideoMatchingTrialIndex++;
		text.gameObject.SetActive (true);
		text.text = "That is the end of the tour.";
		yield return new WaitForSeconds (pauseAfterFinish);

		if (_expInstance.Phase == PhaseEnum.StudyVideoMatching) {
			if (_expInstance.VideoMatchingTrialIndex > 13) {
				Text text = GameObject.Find ("Text").GetComponent<Text> ();
				text.text = "You have completed the trials.";
				_expInstance.VideoMatchingTrialIndex = 0;
                yield return new WaitForSeconds(3);
                SceneManager.LoadScene("Menu");
            } else {
				//Debug.Log ("StudyVideoMatching ongoing, ending trial: " + _expInstance.VideoMatchingTrialIndex.ToString ());
				SceneManager.LoadScene (_expInstance.VideoMatchingOrder [_expInstance.VideoMatchingTrialIndex].ToString ());
			}
		}
	}
}