using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//this class deals with information pertaining to the status of the ship
public class ShipStatusController : MonoBehaviour {
	//these variables have to do with displaying ship status
	public Canvas m_LifebarCanvas;
	public Slider m_Lifebar;
	public Canvas m_BlockedCanvas;
	public Text m_BlockedLabel;
	public Vector3 m_VerticalHealthBarDisplacement;
	public Vector3 m_VerticalLabelDisplacement;

	private ShipAttributes m_ShipAttributes;
	private float m_MaxHullStrength;
	private float m_CurrentHullStrength;
	private RectTransform m_LifebarTransform;
	private RectTransform m_BlockedLabelTransform;

	private ShipColliderController m_ShipColliderController;
	private bool m_ShipBurning;

	//this variable is connected to the sound that should play when the ship dies
	public AudioSource m_DeathSound;

	//These variables deal with the process of the ship dying
	public float m_DeathTime;
	private bool m_IsDying;

	//These variables interface with the level manager
	public LevelManager m_LevelManager;
	public bool m_PlayerTeam;







	void Awake(){
		
		m_LifebarCanvas.worldCamera = Camera.main;
		m_BlockedCanvas.worldCamera = Camera.main;

		m_ShipColliderController = gameObject.GetComponent<ShipColliderController> ();
		m_ShipAttributes = gameObject.GetComponent<ShipAttributes> ();
		m_LifebarTransform = m_Lifebar.GetComponent<RectTransform> ();

		m_BlockedCanvas.enabled = false;
		m_BlockedLabelTransform = m_BlockedLabel.GetComponent<RectTransform> ();

		m_DeathSound.enabled = true;
		m_IsDying = false;

		m_ShipBurning = false;
	}

	void Start(){
		
		m_MaxHullStrength = m_ShipAttributes.m_MaxHullStrength;
		m_CurrentHullStrength = m_ShipAttributes.m_CurrentHullStrength;
		m_Lifebar.maxValue = m_MaxHullStrength;
		m_Lifebar.value = m_CurrentHullStrength;

	}
	
	// Update is called once per frame
	void Update () {
		
		m_Lifebar.value = m_CurrentHullStrength;


	}

	void FixedUpdate(){
		
		m_LifebarTransform.position = gameObject.transform.position + m_VerticalHealthBarDisplacement; //We have to continously move the health bar and status label above the object
		m_BlockedLabelTransform.position = gameObject.transform.position + m_VerticalLabelDisplacement;


	}

	//Call this when the ship gets stun blocked or un stun blocked in order to display that fact on the status
	public void SetStunBlockStatus(bool status){
		
		m_BlockedCanvas.enabled = status;

	}

	//Do damage to the ship's hullstrength by the amount given as a parameter
	//If ship dies then destroy ship and play death aniimation
	public void Damage(float amt){
		
		Debug.Log ("Hit for " + amt + " Damage");
		m_CurrentHullStrength -= amt;
		m_ShipAttributes.m_CurrentHullStrength -= amt;

		if (m_CurrentHullStrength <= 0) {
			
			StartCoroutine(Die ());

		}
	}


	/// <summary>
	/// This corutine takes care of the damage dealt to the ship by the ship resting in a fire
	/// It takes an amount of damage equal to the damagePerSecond parameter every second
	/// </summary>
	/// <param name="DamagePerSecond">Damage per second.</param>
	public IEnumerator BurnShip(float damagePerSecond){
		
		if (m_ShipBurning == false) {
			Debug.Log ("burning Ship");
			m_ShipBurning = true;

			while (m_ShipBurning && m_IsDying == false) {
			
				yield return new WaitForSeconds (1.0f);

				if (m_ShipColliderController.ShipIsInFire ()) {

					Damage (damagePerSecond);

				} else {

					m_ShipBurning = false;

				}

				yield return null;

			}

			yield return null;

		}

		yield return null;


	}

	//Call this function when the ship runs out of HullStrength
	private IEnumerator Die(){
		
		if (!m_IsDying) {
			
			Debug.Log ("Ship destroyed");
			m_IsDying = true;
			m_DeathSound.Play ();
			yield return new WaitForSeconds (m_DeathTime);
			gameObject.SetActive (false);
			gameObject.GetComponent<ShipMovementController> ().m_Waypoint.SetActive (false);

			if (m_LevelManager != null) {
				
				m_LevelManager.ShipDestroyed (m_PlayerTeam);

			}

		}

		yield return null;
	}
}
