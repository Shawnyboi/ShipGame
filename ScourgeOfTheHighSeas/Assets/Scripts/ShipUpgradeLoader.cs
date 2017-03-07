using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// This class will load all the different available upgrades
/// from an xml file after which they can be accessed more easily
/// </summary>
public class ShipUpgradeLoader  {
	
	private List<ShipUpgrade> m_UpgradeList;
	private const string m_UpgradeFileName = "ship_upgrade_data";

	/// <summary>
	/// Default constructor loads the upgrade list with constant data path
	/// </summary>
	public ShipUpgradeLoader(){
		m_UpgradeList = new List<ShipUpgrade> ();
		LoadUpgradeList(m_UpgradeFileName);
	}

	/// <summary>
	/// This function loads the data from the xml file containing the upgrade information
	/// and creates the ship upgrade list member object
	/// </summary>
	private void LoadUpgradeList(string dataPath){

		//get the xml file as a string and turn that into an "XmlDocument" Object
		TextAsset upgradeDataFile = Resources.Load (dataPath) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (upgradeDataFile.text);

		//get the xml file as a string and turn that into an "XmlDocument" Object
		foreach (XmlNode upgrade in xmlDocument["ShipUpgrades"].ChildNodes) {

			//each iteration will create a new upgrade object
			string upgradeName = upgrade.Attributes["name"].Value;
			float damageAug = float.Parse (upgrade ["DamageAugment"].InnerText);
			float speedAug = float.Parse (upgrade ["SpeedAugment"].InnerText);
			float hullAug = float.Parse (upgrade ["MaxHullStrengthAugment"].InnerText);
			float rangeAug = float.Parse (upgrade ["RangeAugment"].InnerText);
			float reloadAug = float.Parse (upgrade ["ReloadTimeAugment"].InnerText);
			ShipUpgrade shipUpgrade = new ShipUpgrade (upgradeName, speedAug, hullAug, rangeAug, damageAug, reloadAug);
			m_UpgradeList.Add (shipUpgrade);


		}




	}

	/// <summary>
	/// Can be called from outside the class to get a ship upgrade from the list
	/// Passes in the name of the desired upgrade as a parameter
	/// </summary>
	public ShipUpgrade GetUpgrade(string upgradeName){
		ShipUpgrade returnUpgrade = m_UpgradeList.Find (upgrade => upgrade.name == upgradeName);
		return returnUpgrade;
	}


}
