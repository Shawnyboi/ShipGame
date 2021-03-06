﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// This class will be responsible for saving and loading all the necesarry data for game persisitence
/// </summary>

public class GameDataController {	
	private GameData m_GameData;
	private string m_SaveFileName;

	public GameDataController(string saveFileName = "save_file"){
		m_GameData = new GameData ();
		m_SaveFileName = saveFileName;

	}

	/// <summary>
	/// Save the necesarry data for the player to be able to resume this game
	/// </summary>
	/// <param name="overworldDataController">Overworld data controller.</param>
	/// <param name="playerDataController">Player data controller.</param>
	public void Save(OverworldDataController overworldDataController, PlayerDataController playerDataController){
		
		m_GameData.m_PastEvents = overworldDataController.GetPastEvents ();
		m_GameData.m_PlayerLocation = overworldDataController.GetPlayerLocation ();
		m_GameData.m_PlayerFleet = playerDataController.GetFleetData ();
		m_GameData.m_PlayerGold = playerDataController.GetPlayerGold ();
		m_GameData.m_AvailableUpgrades = playerDataController.GetUpgradeData ();

		BinaryFormatter binaryFormatter = new BinaryFormatter ();
		FileStream file = File.Create (UnityEngine.Application.persistentDataPath + "/" + m_SaveFileName + ".dat");

		binaryFormatter.Serialize (file, m_GameData);
		file.Close ();

	}

	/// <summary>
	/// Load the necesarry data for the player to resume the old game
	/// </summary>
	/// <param name="overworldDataController">Overworld data controller.</param>
	/// <param name="playerDataController">Player data controller.</param>
	public void Load(OverworldDataController overworldDataController, PlayerDataController playerDataController){
		
		if (File.Exists (UnityEngine.Application.persistentDataPath + "/" + m_SaveFileName + ".dat")) {
			
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			FileStream file = File.Open (UnityEngine.Application.persistentDataPath + "/" + m_SaveFileName + ".dat", FileMode.Open);

			GameData savedData = (GameData)binaryFormatter.Deserialize (file);
			m_GameData = savedData;

			overworldDataController.PassInOverworldData (m_GameData.m_PlayerLocation, m_GameData.m_PastEvents);
			playerDataController.PassInPlayerData (m_GameData.m_PlayerFleet, m_GameData.m_PlayerGold, m_GameData.m_AvailableUpgrades);

			file.Close ();

		}

	}

}
