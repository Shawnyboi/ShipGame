using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameData {

	//A list of all the indeces of events that occured in the game already
	public List<int> m_PastEvents;
	public List<ShipAttributesData> m_PlayerFleet;
	public List<ShipUpgrade> m_AvailableUpgrades;
	public int m_PlayerLocation;
	public int m_PlayerGold;

}
