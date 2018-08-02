using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MazeController : MonoBehaviour {


	public GameObject[] StartOrder;
	public GameObject[] EndOrder;
	static GameObject[] StartOrderStatic;
	static GameObject[] EndOrderStatic;
	public Canvas canvas;
    public Text text;
    public GameObject player;

	private GameObject playerInstance;
    public GameObject startHall;
    public GameObject endHall;
    public GameObject overheadCam;
    public Transform startLoc;
    public Transform endLoc;
    static public bool hasEnded;
    public bool hasStarted;
    static public Material pathColor;
    static public int onCorner = 0;
    static public float totalTime;
    public static UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
    private Vector3 lastPos;
    private Vector3 currentPos;
    static private float totalDistance;
    static private float avgVelocity;
    static private List<string> path;
	public GameObject[] landmarks;
	static public ExperimentSettings _expInstance;
	private List<string> experimentInfo;
	private GameObject endLandmark;
	private float ShowLandMarkForSeconds = 2f;
	public int rotationDuration;
	public int sessionMaxDuration;
	static private bool timeExpired = false;

	void Start() {
		_expInstance = ExperimentSettings.GetInstance ();
		StartOrderStatic = StartOrder;
		EndOrderStatic = EndOrder;
		InitMaze ();
        totalDistance = 0;
        totalTime = 0;
        path = new List<string>();
        hasEnded = false;
        hasStarted = false;

			playerInstance = Instantiate (player,
				StartOrder[_expInstance.TestTrialIndex % 24].transform.position,
				StartOrder[_expInstance.TestTrialIndex % 24].transform.rotation);
			endLandmark = EndOrder[_expInstance.TestTrialIndex % 24];
			endLandmark.AddComponent<TrialEnd> ();
			if (_expInstance.TestTrialCtr > 0) {
				text.text = "";
				StartCoroutine(StartExperiment());
				InvokeRepeating("TrackPathEverySecond", 2.0f, 1.0f);
			};
		controller = playerInstance.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
		controller.enabled = false;
		lastPos = playerInstance.transform.position;
		currentPos = playerInstance.transform.position;
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
			if (totalTime < sessionMaxDuration) {
				currentPos = playerInstance.transform.position;
				totalDistance += Vector3.Distance (currentPos, lastPos);
				totalTime += Time.deltaTime;
				lastPos = currentPos;
			} else {
				timeExpired = true;
				MazeEnd();
			}
        }
    }

	public void InitMaze() {
		if (_expInstance.MazeSettings.MazeName == MazeNameEnum.Visuomotor) {
			endHall.AddComponent<End> ();		
		} 
	}

	IEnumerator StartExperiment()
    {
		text.text = "+";
		yield return new WaitForSeconds(1);
		text.fontSize = 32;
		text.text = endLandmark.name;
		yield return new WaitForSeconds(1);
        Image img = canvas.GetComponent<Image>();
        img.color = UnityEngine.Color.clear;

		text.rectTransform.anchoredPosition = new Vector2(0, 160f);

		DOTween.Init();
		playerInstance.transform.DORotate (new Vector3(0,playerInstance.transform.rotation.y + 359,0),rotationDuration, RotateMode.WorldAxisAdd);
		hasStarted = true;
		controller.enabled = true;
	}

    public void TrackPathEverySecond()
    {
        int second = Mathf.RoundToInt(totalTime);
		path.Add( second.ToString() + ": " + playerInstance.transform.position);
    }

    static public void MazeEnd()
    {
		ExperimentSettings _expInstance = ExperimentSettings.GetInstance ();
		_expInstance.TestTrialIndex++;
		_expInstance.TestTrialCtr++;
        hasEnded = true;
        controller.enabled = false;
        Text text = GameObject.Find("Text").GetComponent<Text>();
        text.text = "+";
        TrailRenderer trail = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        Material mat = Resources.Load("Yellow") as Material;
        trail.material = mat;
		avgVelocity = totalDistance / totalTime;
        List<string> experimentInfo = GetExperimentInfo();

        if (!File.Exists(_expInstance.FileName + "_data.txt"))
        {
            List<string> experimentHeader = GetExperimentHeader();
            System.IO.File.WriteAllLines(_expInstance.FileName + "_data.txt", experimentHeader.ToArray());
            System.IO.File.WriteAllLines(_expInstance.FileName + "_data_path.txt", experimentHeader.ToArray());

        }

        foreach (string line in experimentInfo)
        {
            System.IO.File.AppendAllText(_expInstance.FileName + "_data.txt", line + "\r\n");
            System.IO.File.AppendAllText(_expInstance.FileName + "_data_path.txt", line + "\r\n");
        }

        foreach (string line in path)
            System.IO.File.AppendAllText(_expInstance.FileName + "_data_path.txt", line + "\r\n");

        TakePhoto();


        // new lines at the end
        System.IO.File.AppendAllText(_expInstance.FileName + "_data.txt", "\r\n");
        System.IO.File.AppendAllText(_expInstance.FileName + "_data_path.txt", "\r\n");

        _expInstance.MazeSettings = new MazeSettings();
        SceneManager.LoadScene("Test");

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
        
        experimentInfo.Add("Phase: Test Trials");
        experimentInfo.Add("Test Trial No.: " + _expInstance.TestTrialCtr);
        experimentInfo.Add("Path No.: " + _expInstance.TestTrialIndex % 24);
        experimentInfo.Add("Path Type: " + _expInstance.TestTrialTypes[_expInstance.TestTrialIndex % 24].ToString());
        experimentInfo.Add ("Starting Landmark: " + StartOrderStatic[_expInstance.TestTrialIndex % 24].name);
		experimentInfo.Add ("End Landmark: " + EndOrderStatic[_expInstance.TestTrialIndex % 24].name);
		experimentInfo.Add ("Distance: " + totalDistance);
		experimentInfo.Add ("Time: " + totalTime);
		experimentInfo.Add ("Time expired: " + timeExpired.ToString());
		experimentInfo.Add ("Avg. Velocity: " + avgVelocity);
        experimentInfo.Add("\r\n");

        //New Stuff
        return experimentInfo;
	}

	static private void TakePhoto()
    {
		ExperimentSettings _expInstance = ExperimentSettings.GetInstance ();
        Camera cam = GameObject.Find("Overhead Cam").GetComponent<Camera>();
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
		ExperimentSettings _expInstance = ExperimentSettings.GetInstance ();
		string imageFileName = _expInstance.FileDir + "\\";
		imageFileName += "p" + _expInstance.ParticipantID.ToString() + "_SCM_Test"+ _expInstance.TestTrialTypes[_expInstance.TestTrialIndex % 24].ToString();
		imageFileName += "_path.png";
		return imageFileName;
	}
}