using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PracticeController : MonoBehaviour {
	public Canvas canvas;
    public Text text, continueReminderText;
    public GameObject player;
    public GameObject endHall;
    public GameObject overheadCam;
    public Transform startLoc;
    public Transform endLoc;
    static public bool hasEnded;
    public bool hasStarted;
    static public Material pathColor;
    static public float totalTime;
    public static UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
    private Vector3 lastPos;
    private Vector3 currentPos;
    static private float totalDistance;
    static private float avgVelocity;
    static private List<string> path;
	static public ExperimentSettings _expInstance;
	private List<string> experimentInfo;

	void Start() {
		_expInstance = ExperimentSettings.GetInstance ();
        totalDistance = 0;
        totalTime = 0;
        path = new List<string>();
        hasEnded = false;
		Camera cam = overheadCam.GetComponent<Camera>();
		cam.enabled = false;
        hasStarted = false;
		controller = player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
		controller.enabled = false;
		lastPos = player.transform.position;
		currentPos = player.transform.position;
    }

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.E) && !hasEnded)
        {
            MazeEnd();
            SceneManager.LoadScene(0);
        }

        else if (Input.GetKeyDown(KeyCode.E) && hasEnded)
            SceneManager.LoadScene(0);

        if (Input.GetKeyDown("space") && !hasStarted)
		{
			StartCoroutine(StartExperiment());
			InvokeRepeating("TrackPathEverySecond", 2.0f, 1.0f);
        }

        // Trial has begun, do the following:
        if (hasStarted && !hasEnded) {
			if (totalTime < 120) {
				currentPos = player.transform.position;
				totalDistance += Vector3.Distance (currentPos, lastPos);
				totalTime += Time.deltaTime;
				lastPos = currentPos;
			} else {
				MazeEnd();
			}
        }
    }

	IEnumerator StartExperiment()
    {
		text.text = "+";
		yield return new WaitForSeconds(.5f);
		if (continueReminderText != null)
		{
			continueReminderText.gameObject.SetActive(true);
		}
		Image img = canvas.GetComponent<Image>();
        img.color = UnityEngine.Color.clear;
		text.text = "";
		hasStarted = true;
		controller.enabled = true;
	}

    public void TrackPathEverySecond()
    {
        int second = Mathf.RoundToInt(totalTime);
		path.Add( second.ToString() + ": " + player.transform.position);
    }

    static public void MazeEnd()
    {
        hasEnded = true;
        controller.enabled = false;
        Text text = GameObject.Find("Text").GetComponent<Text>();
        text.text = "You have reached the end.";
        TrailRenderer trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        Material mat = Resources.Load("Yellow") as Material;
        trail.material = mat;
		avgVelocity = totalDistance / totalTime;
		List<string> experimentInfo = GetExperimentInfo ();
        List<string> experimentHeader = GetExperimentHeader();
        
        if (!File.Exists(_expInstance.FileName + "_practice_data.txt"))
        {
            System.IO.File.WriteAllLines(_expInstance.FileName + "_practice_data.txt", experimentHeader.ToArray());
            System.IO.File.WriteAllLines(_expInstance.FileName + "_practice_data_path.txt", experimentHeader.ToArray());
        }

        foreach (string line in experimentInfo)
        {
            System.IO.File.AppendAllText(_expInstance.FileName + "_practice_data.txt", line + "\r\n");
            System.IO.File.AppendAllText(_expInstance.FileName + "_practice_data_path.txt", line + "\r\n");
        }

        foreach (string line in path) System.IO.File.AppendAllText(_expInstance.FileName + "_practice_data_path.txt", line + "\r\n");

        // new lines at the end
        System.IO.File.AppendAllText(_expInstance.FileName + "_practice_data.txt", "\r\n");
        System.IO.File.AppendAllText(_expInstance.FileName + "_practice_data_path.txt", "\r\n");

        TakePhoto();
    }


    static private List<string> GetExperimentHeader()
    {
        List<string> experimentHeader = new List<string>();

        ExperimentSettings _expInstance = ExperimentSettings.GetInstance();
        experimentHeader.Add("Participant ID: " + _expInstance.ParticipantID);
        experimentHeader.Add("Experimenter Initials: " + _expInstance.ExperimenterInitials);
        experimentHeader.Add("Date: " + _expInstance.Date);
        experimentHeader.Add("\r\n");
        return experimentHeader;
    }

    static private List<string> GetExperimentInfo () {
		List<string> experimentInfo = new List<string>();
		ExperimentSettings _expInstance = ExperimentSettings.GetInstance ();
        experimentInfo.Add ("Phase: " + _expInstance.MazeSettings.MazeName.ToString());
		experimentInfo.Add ("Distance: " + totalDistance);
		experimentInfo.Add ("Time: " + totalTime);
		experimentInfo.Add ("Avg. Velocity: " + avgVelocity);
        return experimentInfo;
	}

	static private void TakePhoto()
    {
		ExperimentSettings _expInstance = ExperimentSettings.GetInstance ();
        Camera cam = GameObject.Find("Overhead Cam").GetComponent<Camera>();
		cam.enabled = true;
        RenderTexture currentRT = RenderTexture.active;
        var rTex = new RenderTexture(cam.pixelHeight, cam.pixelHeight, 16);
        cam.targetTexture = rTex;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        tex.Apply(false);
        RenderTexture.active = currentRT;
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
		string imageFileName = NameImageFile ();
        System.IO.File.WriteAllBytes(imageFileName, bytes);
    }

	static private string NameImageFile ()
	{
        ExperimentSettings _expInstance = ExperimentSettings.GetInstance();
        string imageFileName = _expInstance.FileDir + "\\";
        imageFileName += "p" + _expInstance.ParticipantID.ToString() + "_SCM_" + _expInstance.MazeSettings.MazeName.ToString();
        imageFileName += "_path.png";
        return imageFileName;
    }
}