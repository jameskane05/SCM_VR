using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrialEnd : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("MazeEnd() being called due to maze completion");
		StartCoroutine(MazeController.MazeEnd());
    }
    
}
