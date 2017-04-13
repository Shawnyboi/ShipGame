using UnityEngine;
using System.Collections;

/// <summary>
/// This class will contain the information for all the ships upgradeable attributes
/// </summary>
public class ShipAttributes : MonoBehaviour {

	public float m_Speed;
	public float m_TurningSpeed;
	public float m_MaxHullStrength;
	public float m_CurrentHullStrength;
	public float m_Range;
	public float m_Damage;
	public float m_ReloadTime;
	public float m_PercentAccuracy;
	public float m_ShotVarianceDegrees;
	public float m_LaunchSpeed;
	public string m_ShipName;


	/// <summary>
	/// This function will return ship attribute data struct of the function it is called on
	/// </summary>
	/// <returns>The ship attribute data struct.</returns>

	public ShipAttributesData CreateShipAttributesData(){
		
		ShipAttributesData data = new ShipAttributesData();

		data.speed = m_Speed;
		data.turningSpeed = m_TurningSpeed;
		data.maxHullStrength = m_MaxHullStrength;
		data.currentHullStrength = m_CurrentHullStrength;
		data.range = m_Range;
		data.damage = m_Damage;
		data.reloadTime = m_ReloadTime;
		data.percentAccuracy = m_PercentAccuracy;
		data.shotVarianceDegrees = m_ShotVarianceDegrees;
		data.launchSpeed = m_LaunchSpeed;
		data.shipName = m_ShipName;

		return data;

	}

	/// <summary>
	/// This function copies all the values from a ship attributes data struct and puts them in the instance of this class
	/// </summary>
	/// <param name="data">Data struct.</param>
	public void CopyShipAttributesFromData(ShipAttributesData data){
		
		m_Speed = data.speed;
		m_TurningSpeed = data.turningSpeed;
		m_MaxHullStrength = data.maxHullStrength;
		m_CurrentHullStrength = data.currentHullStrength;
		m_Range = data.range;
		m_Damage = data.damage;
		m_ReloadTime = data.reloadTime;
		m_PercentAccuracy = data.percentAccuracy;
		m_ShotVarianceDegrees = data.shotVarianceDegrees;
		m_LaunchSpeed = data.launchSpeed;
		m_ShipName = data.shipName;

	}

}
