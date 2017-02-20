using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class will be responsible for controlling all the functions of the main menu
/// notably starting up the game loop and creating the game manager
/// </summary>
public class MainMenuController : MonoBehaviour {

	//These three object which exist from scene to scene
	public GameObject m_GameManagerPrefab;
	public GameObject m_PlayerDataControllerPrefab;
	public GameObject m_OverworldDataControllerPrefab;

	/// <summary>
	/// This function will be called when the StartGame button gets pressed
	/// It will start up all the controllers without loading any data from previous playing
	/// </summary>
	public void StartGame (bool newGame = true){
		
		PlayerDataController playerDataController = Instantiate (m_PlayerDataControllerPrefab).GetComponent<PlayerDataController>();
		playerDataController.InitializePlayerDataController (newGame, 0);

		OverworldDataController overWorldDataController = Instantiate (m_OverworldDataControllerPrefab).GetComponent<OverworldDataController>();
		overWorldDataController.InitializeOverworldDataController (newGame, 0);
		GameManager gameManager= Instantiate (m_GameManagerPrefab).GetComponent<GameManager>();
		gameManager.InitializeGameManager (overWorldDataController, playerDataController, newGame);

		
	}





	/// <summary>
	/// This will be called when quit is pushed it will end the application
	/// </summary>
	public void Quit(){

	}

}
