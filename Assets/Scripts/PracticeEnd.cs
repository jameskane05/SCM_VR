using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PracticeEnd : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("MazeEnd() being called due to maze completion");


		PracticeController.MazeEnd();
    }
    
}
