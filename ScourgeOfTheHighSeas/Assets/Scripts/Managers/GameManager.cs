using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


	void Awake (){
		DontDestroyOnLoad (gameObject);// we won't the game manager to stay around all the time

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
