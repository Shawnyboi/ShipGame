using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// This is class is going to contain the necesarry information for a ship upgrade 
/// The player should be able to hold onto this object (in the player data controller) 
/// and apply it to a ship in the fleet viewer
/// </summary>

[Serializable]
public class ShipUpgrade {
	private readonly float m_SpeedAugment;
	private readonly float m_MaxHullStrengthAugment;
	private readonly float m_RangeAugment;
	private readonly float m_DamageAugment;
	private readonly float m_ReloadTimeAugment;
	private readonly string m_UpgradeName;

	/// <summary>
	/// The constructor in which the attribute augments are set
	/// </summary>
	/// <param name="upgradeName">Upgrade name.</param>
	/// <param name="speedAug">Speed aug.</param>
	/// <param name="hullStrengthAug">Hull strength aug.</param>
	/// <param name="rangeAug">Range aug.</param>
	/// <param name="damageAug">Damage aug.</param>
	/// <param name="reloadTimeAug">Reload time aug.</param>
	/// <param name="accuracyAug">Accuracy aug.</param>
	public ShipUpgrade(string upgradeName, float speedAug = 0f, float hullStrengthAug = 0f, float rangeAug = 0f, 
		float damageAug = 0f, float reloadTimeAug = 0f){

		m_UpgradeName = upgradeName;
		m_SpeedAugment = speedAug;
		m_MaxHullStrengthAugment = hullStrengthAug;
		m_RangeAugment = rangeAug;
		m_DamageAugment = damageAug;
		m_ReloadTimeAugment = reloadTimeAug;


	}

	/// <summary>
	/// This function takes in the "ship" from the plaer data controller and augments it. 
	/// This is the main purpose of the ShipUpgradeClass
	/// It checks if the upgrade has been applied, if it has, do nothing and return false
	/// </summary>
	/// <param name="shipAttributesData">Ship attributes data.</param>
	public bool AugmentShip(ShipAttributesData shipAttributesData){
		
		if (!shipAttributesData.upgradeList.Contains (m_UpgradeName)) {
			
			shipAttributesData.speed += m_SpeedAugment;
			shipAttributesData.damage += m_DamageAugment;
			shipAttributesData.maxHullStrength += m_MaxHullStrengthAugment;
			shipAttributesData.reloadTime += m_ReloadTimeAugment;
			shipAttributesData.range += m_RangeAugment;
			shipAttributesData.upgradeList.Add (m_UpgradeName);

			if (m_MaxHullStrengthAugment > 0f) {
				
				shipAttributesData.currentHullStrength = shipAttributesData.maxHullStrength;

			}

			return true;

		} else {
			return false;
		}

	}

	//declaration of a name property that can be accessed easily
	public string name{

		get{
			return m_UpgradeName;
		}
	}




}
