using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ExperimentController : MonoBehaviour {

    [SerializeField] private InputField ParticipantIDInput;
	[SerializeField] private InputField ExperimenterInitialsInput;
	[SerializeField] private GameObject FirstPanel;
	[SerializeField] private GameObject MainOptionsPanel;
    [SerializeField] private GameObject StudyOptionsPanel;
    public int StudyVideoMatchingTrials = 14;

	public static ExperimentSettings _expInstance;

    private void Start()
    {
		_expInstance = ExperimentSettings.GetInstance ();
		_expInstance.MazeSettings = new MazeSettings ();

        

        if (!string.IsNullOrEmpty(_expInstance.ExperimenterInitials))  // if this already exists then we're coming out of a maze
			OpenSubmenu ();
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Escape)) 
			Application.Quit();
        if (Input.GetKey(KeyCode.E) && FirstPanel.activeSelf == false)
			SceneManager.LoadScene(0);
    }

	public void EnterExperimentInfo() {
		_expInstance.ParticipantID = ParticipantIDInput.text;
		_expInstance.ExperimenterInitials = ExperimenterInitialsInput.text;
		_expInstance.Date = DateTime.Now;
		_expInstance.VideoMatchingOrder = RandomizeVideoMatchingOrder ();
		SetDir (_expInstance);
		OpenSubmenu();
    }

	void OpenSubmenu() {
		// Cursor is disabled coming out of maze scenes
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
        
		FirstPanel.SetActive (false);

        if (_expInstance.Phase == PhaseEnum.StudyMatching || _expInstance.Phase == PhaseEnum.StudyVideo || _expInstance.Phase == PhaseEnum.StudyVideoMatching)
            StudyOptionsPanel.SetActive(true);
        else MainOptionsPanel.SetActive(true);
    }

	public void SetDir(ExperimentSettings _expInstance)
    {
		string currentDir = System.IO.Directory.GetCurrentDirectory ();
        string newDir = _expInstance.ParticipantID + "_SCM";
        _expInstance.FileDir = newDir + "_" + DateTime.Now.ToString("ddHHmm");
        System.IO.Directory.CreateDirectory(_expInstance.FileDir);
		_expInstance.FileName = _expInstance.FileDir + "\\" + newDir;


        _expInstance.TestTrialOrder = new int[24] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        Debug.Log(_expInstance.TestTrialOrder.ToString());
        String str = "";
        for (int i = 0; i < _expInstance.TestTrialOrder.Length; i++)
        {
            str += _expInstance.TestTrialOrder[i].ToString() + " ";
        }
        Debug.Log(str);
        _expInstance.TestTrialOrder = KnuthShuffle(_expInstance.TestTrialOrder);
         str = "";
        for (int i = 0; i < _expInstance.TestTrialOrder.Length; i++)
        {
            str += _expInstance.TestTrialOrder[i].ToString() + " ";
        }
        Debug.Log(str);
        //_expInstance.TestTrialIndex = UnityEngine.Random.Range(0, 23);



        _expInstance.MatchingTrialIndex = UnityEngine.Random.Range(0, 4);
        _expInstance.MatchingAnswersGiven = new string[5];
    }

    public static T[] KnuthShuffle<T>(T[] array)
    {
        System.Random random = new System.Random();
        for (int i = 0; i < array.Length; i++)
        {
            int j = random.Next(i, array.Length); // Don't select from the entire array on subsequent loops
            T temp = array[i]; array[i] = array[j]; array[j] = temp;
        }
        return array;
    }

    public void LoadMazeJoystickPractice () {
        _expInstance.Phase = PhaseEnum.Practice;
        _expInstance.MazeSettings.MazeName = MazeNameEnum.Practice;
		SceneManager.LoadScene("JP");
	}

	public void LoadVisuomotorExpertise () {
        _expInstance.Phase = PhaseEnum.Practice;
        _expInstance.MazeSettings.MazeName = MazeNameEnum.Visuomotor;
		SceneManager.LoadScene("VME");
	}
		
	public void LoadStudyPhaseVideo() {
		_expInstance.Phase = PhaseEnum.StudyVideo;
		SceneManager.LoadScene("Learning");
	}

	public void LoadStudyPhaseMatching() {
		_expInstance.Phase = PhaseEnum.StudyMatching;
		SceneManager.LoadScene("Matching");
	}

	public void LoadStudyPhaseVideoMatching () {
		_expInstance.Phase = PhaseEnum.StudyVideoMatching;
		SceneManager.LoadScene (_expInstance.VideoMatchingOrder[0].ToString());
	}

	public void LoadTestTrials() {
		_expInstance.Phase = PhaseEnum.TestTrials;
		SceneManager.LoadScene ("Test");
	}

	public VideoMatchingPhaseEnum[] RandomizeVideoMatchingOrder(){
        VideoMatchingPhaseEnum[] deck = new VideoMatchingPhaseEnum[_expInstance.matchingTrialCount + _expInstance.videoTrialCount];
        int deckNumber = UnityEngine.Random.Range(1,6);
        Debug.Log("deckNumber: " + deckNumber.ToString());
        switch (deckNumber)
        {
            case 1:
                Debug.Log("Deck 1 selected");
                deck[0] = VideoMatchingPhaseEnum.Learning;
                deck[1] = VideoMatchingPhaseEnum.Matching;
                deck[2] = VideoMatchingPhaseEnum.Learning;
                deck[3] = VideoMatchingPhaseEnum.Learning;
                deck[4] = VideoMatchingPhaseEnum.Matching;
                deck[5] = VideoMatchingPhaseEnum.Learning;
                deck[6] = VideoMatchingPhaseEnum.Learning;
                deck[7] = VideoMatchingPhaseEnum.Matching;
                deck[8] = VideoMatchingPhaseEnum.Learning;
                deck[9] = VideoMatchingPhaseEnum.Learning;
                deck[10] = VideoMatchingPhaseEnum.Matching;
                deck[11] = VideoMatchingPhaseEnum.Learning;
                deck[12] = VideoMatchingPhaseEnum.Matching;
                deck[13] = VideoMatchingPhaseEnum.Learning;
                break;
            case 2:
                Debug.Log("Deck 2 selected");
                deck[0] = VideoMatchingPhaseEnum.Learning;
                deck[1] = VideoMatchingPhaseEnum.Matching;
                deck[2] = VideoMatchingPhaseEnum.Learning;
                deck[3] = VideoMatchingPhaseEnum.Learning;
                deck[4] = VideoMatchingPhaseEnum.Matching;
                deck[5] = VideoMatchingPhaseEnum.Learning;
                deck[6] = VideoMatchingPhaseEnum.Learning;
                deck[7] = VideoMatchingPhaseEnum.Matching;
                deck[8] = VideoMatchingPhaseEnum.Learning;
                deck[9] = VideoMatchingPhaseEnum.Learning;
                deck[10] = VideoMatchingPhaseEnum.Matching;
                deck[11] = VideoMatchingPhaseEnum.Learning;
                deck[12] = VideoMatchingPhaseEnum.Learning;
                deck[13] = VideoMatchingPhaseEnum.Matching;
                break;
            case 3:
                Debug.Log("Deck 3 selected");
                deck[0] = VideoMatchingPhaseEnum.Learning;
                deck[1] = VideoMatchingPhaseEnum.Learning;
                deck[2] = VideoMatchingPhaseEnum.Matching;
                deck[3] = VideoMatchingPhaseEnum.Learning;
                deck[4] = VideoMatchingPhaseEnum.Learning;
                deck[5] = VideoMatchingPhaseEnum.Matching;
                deck[6] = VideoMatchingPhaseEnum.Learning;
                deck[7] = VideoMatchingPhaseEnum.Learning;
                deck[8] = VideoMatchingPhaseEnum.Matching;
                deck[9] = VideoMatchingPhaseEnum.Learning;
                deck[10] = VideoMatchingPhaseEnum.Learning;
                deck[11] = VideoMatchingPhaseEnum.Matching;
                deck[12] = VideoMatchingPhaseEnum.Learning;
                deck[13] = VideoMatchingPhaseEnum.Matching;
                break;
            case 4:
                deck[0] = VideoMatchingPhaseEnum.Learning;
                deck[1] = VideoMatchingPhaseEnum.Learning;
                deck[2] = VideoMatchingPhaseEnum.Matching;
                deck[3] = VideoMatchingPhaseEnum.Learning;
                deck[4] = VideoMatchingPhaseEnum.Matching;
                deck[5] = VideoMatchingPhaseEnum.Learning;
                deck[6] = VideoMatchingPhaseEnum.Learning;
                deck[7] = VideoMatchingPhaseEnum.Matching;
                deck[8] = VideoMatchingPhaseEnum.Learning;
                deck[9] = VideoMatchingPhaseEnum.Learning;
                deck[10] = VideoMatchingPhaseEnum.Matching;
                deck[11] = VideoMatchingPhaseEnum.Learning;
                deck[12] = VideoMatchingPhaseEnum.Learning;
                deck[13] = VideoMatchingPhaseEnum.Matching;
                break;
            case 5:
                deck[0] = VideoMatchingPhaseEnum.Learning;
                deck[1] = VideoMatchingPhaseEnum.Learning;
                deck[2] = VideoMatchingPhaseEnum.Matching;
                deck[3] = VideoMatchingPhaseEnum.Learning;
                deck[4] = VideoMatchingPhaseEnum.Matching;
                deck[5] = VideoMatchingPhaseEnum.Learning;
                deck[6] = VideoMatchingPhaseEnum.Learning;
                deck[7] = VideoMatchingPhaseEnum.Matching;
                deck[8] = VideoMatchingPhaseEnum.Learning;
                deck[9] = VideoMatchingPhaseEnum.Learning;
                deck[10] = VideoMatchingPhaseEnum.Matching;
                deck[11] = VideoMatchingPhaseEnum.Learning;
                deck[12] = VideoMatchingPhaseEnum.Matching;
                deck[13] = VideoMatchingPhaseEnum.Learning;
                break;
            case 6:
                Debug.Log("Deck 4 selected");

                deck[0] = VideoMatchingPhaseEnum.Matching;
                deck[1] = VideoMatchingPhaseEnum.Learning;
                deck[2] = VideoMatchingPhaseEnum.Learning;
                deck[3] = VideoMatchingPhaseEnum.Matching;
                deck[4] = VideoMatchingPhaseEnum.Learning;
                deck[5] = VideoMatchingPhaseEnum.Learning;
                deck[6] = VideoMatchingPhaseEnum.Matching;
                deck[7] = VideoMatchingPhaseEnum.Learning;
                deck[8] = VideoMatchingPhaseEnum.Learning;
                deck[9] = VideoMatchingPhaseEnum.Matching;
                deck[10] = VideoMatchingPhaseEnum.Learning;
                deck[11] = VideoMatchingPhaseEnum.Learning;
                deck[12] = VideoMatchingPhaseEnum.Matching;
                deck[13] = VideoMatchingPhaseEnum.Learning;
                break;
        }
        


        string debugString = "";
        foreach (VideoMatchingPhaseEnum name in deck)
        {
            debugString += name + ", ";
        }
        Debug.Log(debugString);


        return deck;
	}
}