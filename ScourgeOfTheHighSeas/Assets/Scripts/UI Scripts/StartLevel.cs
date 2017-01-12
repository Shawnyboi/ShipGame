using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour {

	//public int m_FirstSceneIndex;

	//This function is intended to be called in the main menu
	//Probably won't last for the final product
	//the whole class is a temporary fix anyway
	public void StartLevelBySceneIndex(int sceneIndex){
		SceneManager.LoadScene (sceneIndex);
	}
}
