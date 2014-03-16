using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private float MaxSpeed;
	private float Acceleration;
	private float Decceleration;
	public float Force;
	public int JoystickNumber;
	public float DarknessSpeedDecrease;
	public Spotlight spotlight;
	public GameObject buttonMash;
	public GameObject Skull;
	public int mashesToReload = 5;
	public int Health;
	public bool MonsterHitInstantDeath = false;
	public bool MonsterHitLoseHealth = false;
	public bool DarknessLoseHealth = false;
	public bool HitsKnockBack = false;
	public float DarknessTimeToLoseHealth;
	public float ImmuneTime;
	public float StunTime = 3.0f;
	public float KnockbackForce;
	public float WindBackAnimationRate = 0.4f;
	public float WindBackAnimationStart = 0.8f;
	public int MonsterDamage;
	public int DarknessDamage;
	public string ColourName;
	public AudioClip PlayerWonClip;
	public AudioClip BoingClip;
	public AudioClip[] ThumpClips;
	public AudioClip PlayerHurtClip;
	
	public Texture pressedTexture;
	
	public Material drawMaterial;
	
	private Vector3 m_vel = new Vector3();
	private bool m_isInDarkness = false;
	private Vector3 m_force = new Vector3();
	private float m_stunTime = 0;
	
	private int mashesLeft = 0;
	private Vector3 mashTutLoc;
	
	private bool m_isMonster = false;
	
	public float angleDamping = 12.0f;
	
	private Vector3 m_direction = new Vector3(0,0,1);
	
	private float m_timeSpentInDarkness = 0;
	private float m_immuneTimer = 0;
	private bool m_isImmune = false;
	
	private bool m_overrideStart = false;
	
	private float m_windbackTime = 0;
	private bool m_isReady = false;
	
	private float m_redFlashTimer = 0;
	private bool m_isRed = false;
	private float m_redFlashTimeInterval = 0.05f;
	private bool m_isHurt = false;
	private float m_hurtTimer = 0;
	private float m_hurtTimeTotal = 1.5f;
	
	private bool m_darknessFadeToRed = true;
	private float m_lerpTimer = 0;

	// Use this for initialization
	void Start () 
	{
		gameObject.renderer.material = drawMaterial;
		gameObject.transform.FindChild("Man01").gameObject.renderer.material = drawMaterial;
		gameObject.transform.FindChild("weapon").gameObject.renderer.material = drawMaterial;
		gameObject.transform.FindChild("weapon").collider.enabled = false;
		this.gameObject.layer = 3;	
		
		
		animation.Play ("walk");
		buttonMash.guiTexture.enabled = false;
		Skull.guiTexture.enabled = false;
	}
	
	public bool IsReady()
	{
		return m_isReady;
	}
	
	public bool IsOverrideStart()
	{
		return m_overrideStart;
	}
	
	public void HideTut()
	{
		buttonMash.guiTexture.enabled = false;
	}
	
	public void ForceReady()
	{
		HideTut ();
		m_isReady = true;
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (Health < 0)
		{
			Health = 0;
		}
		
		if (Health <= 0)
		{
			m_force = new Vector3(0,0,0);
		}
		
		if (spotlight.hud.IsVictory())
		{
			m_force = new Vector3(0,0,0);
			return;
		}
		if (Input.GetButtonDown ("B_" + JoystickNumber) && Input.GetButtonDown ("A_" + JoystickNumber))
		{
			m_overrideStart = true;
			m_isReady = true;			
		}
		
		if (!m_isReady)
		{
			mashTutLoc = Camera.mainCamera.WorldToScreenPoint(transform.position);
			buttonMash.guiTexture.enabled = true;
			buttonMash.guiTexture.pixelInset = new Rect(mashTutLoc.x - Screen.width/2 - 64,mashTutLoc.y - Screen.height/2,128,128);
			if (Input.GetButtonDown ("A_" + JoystickNumber))
			{
				m_isReady = true;
			}
			
			return;
		}
		else
		{
			buttonMash.guiTexture.texture = pressedTexture;
			//b
		}
		
		if (!spotlight.hud.IsReady())
		{
			return;
		}
		
		float dt = Time.deltaTime;
		
		Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
		Skull.transform.position = Vector3.zero;
		Skull.transform.localScale = Vector3.zero;
		Skull.guiTexture.pixelInset = new Rect(screenPos.x, screenPos.y, 32, 46);
		
		if (m_isHurt)
		{
			m_hurtTimer += dt;
			m_redFlashTimer += dt;
			
			if (m_redFlashTimer >= m_redFlashTimeInterval)
			{
				m_redFlashTimer = 0;
				if (m_isRed)
				{
					m_isRed = false;
					gameObject.renderer.material.color = new Color(1, 1, 1, 1);
					gameObject.transform.FindChild("Man01").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
					gameObject.transform.FindChild("weapon").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
					gameObject.transform.FindChild("Man01Hat").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				}
				else
				{
					m_isRed = true;
					gameObject.renderer.material.color = new Color(1, 0, 0, 1);
					gameObject.transform.FindChild("Man01").gameObject.renderer.material.color = new Color(1, 0, 0, 1);
					gameObject.transform.FindChild("weapon").gameObject.renderer.material.color = new Color(1, 0, 0, 1);
					gameObject.transform.FindChild("Man01Hat").gameObject.renderer.material.color = new Color(1, 0, 0, 1);
				}
			}
			
			if (m_hurtTimer >= m_hurtTimeTotal)
			{
				m_isHurt = false;
				m_hurtTimer = 0;
				gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				gameObject.transform.FindChild("Man01").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				gameObject.transform.FindChild("weapon").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				gameObject.transform.FindChild("Man01Hat").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
			}
		}	
		
		
		if (m_isImmune)
		{
			m_immuneTimer += dt;
			if (m_immuneTimer >= ImmuneTime)
			{
				m_immuneTimer = 0;
				m_isImmune = false;
			}
		}
		
		if (DarknessLoseHealth)
		{
			if (!IsPlayerInSpotlight())
			{
				m_timeSpentInDarkness += dt;
				if (m_timeSpentInDarkness >= DarknessTimeToLoseHealth)
				{
					m_timeSpentInDarkness = 0;
					LoseHealth(DarknessDamage);
				}
				
				if (!m_isHurt)
				{
					float colour;
					if (m_darknessFadeToRed)
					{
						m_lerpTimer += dt * 2;
						if (m_lerpTimer >= 1)
						{
							m_darknessFadeToRed = false;
						}
					}
					else
					{
						m_lerpTimer -= dt * 2;
						if (m_lerpTimer <= 0)
						{
							m_darknessFadeToRed = true;;
						}
					}
					
					colour = Mathf.Lerp(1, 0, m_lerpTimer);
					
					gameObject.renderer.material.color = new Color(1, colour, colour, 1);
					gameObject.transform.FindChild("Man01").gameObject.renderer.material.color = new Color(1, colour, colour, 1);
					gameObject.transform.FindChild("weapon").gameObject.renderer.material.color = new Color(1, colour, colour, 1);
					gameObject.transform.FindChild("Man01Hat").gameObject.renderer.material.color = new Color(1, colour, colour, 1);
				}
			}
			else
			{
				gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				gameObject.transform.FindChild("Man01").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				gameObject.transform.FindChild("weapon").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				gameObject.transform.FindChild("Man01Hat").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
				m_timeSpentInDarkness = 0;
			}
		}
		
		if (Health == 0)
		{
			m_isRed = false;
			gameObject.renderer.material.color = new Color(1, 1, 1, 1);
			gameObject.transform.FindChild("Man01").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
			gameObject.transform.FindChild("weapon").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
			gameObject.transform.FindChild("Man01Hat").gameObject.renderer.material.color = new Color(1, 1, 1, 1);
		}
		
		/*if (Input.GetButtonDown ("B_" + JoystickNumber))
		{
			Application.LoadLevel (0);
		}*/
		
		//if mashesLeft > 0, we need to stop motion etc and also check for mash
		if (mashesLeft > 0)
		{
		//	buttonMash.guiTexture.enabled = true;
		//	buttonMash.guiTexture.pixelInset = new Rect(mashTutLoc.x - Screen.width/2 -16 - 32,mashTutLoc.y - Screen.height/2 - 16,96,96);
			if (Input.GetButtonDown ("B_" + JoystickNumber))
			{
		//		buttonMash.guiTexture.pixelInset = new Rect(mashTutLoc.x - Screen.width/2 -16 - 32,mashTutLoc.y - Screen.height/2 - 16,96,96);
				--mashesLeft;
				animation.Stop();
				animation["punch"].enabled = true;
				animation["punch"].weight = 1f;
				animation["punch"].normalizedTime = ((float)mashesLeft / (float)mashesToReload) * 0.2f;
				animation["punch"].speed = 0; //to make the animation pause
			}
			else
			{	
		//		buttonMash.guiTexture.pixelInset = new Rect(mashTutLoc.x - Screen.width/2 - 32,mashTutLoc.y - Screen.height/2,64,64);
			}
			return; //make a better early exit
		}
		else
		{
	//		buttonMash.guiTexture.enabled = false;
		}
		
		m_windbackTime = Mathf.Max(0.0f, m_windbackTime - dt);
		
		if (Input.GetButtonDown ("A_" + JoystickNumber) && m_windbackTime == 0.0f && Health > 0)
		{			
			//We have launched an attack
			mashesLeft += mashesToReload;
			mashTutLoc = Camera.mainCamera.WorldToScreenPoint(transform.position);
			animation["punch"].speed = 1;
			m_windbackTime = animation["punch"].length + ((1.0f / WindBackAnimationRate) * WindBackAnimationStart * animation["punch"].length);
			
			if(BoingClip != null)
			{
				AudioSource.PlayClipAtPoint(BoingClip, transform.position);			
				AudioSource.PlayClipAtPoint(ThumpClips[Random.Range(0, ThumpClips.Length - 1)], transform.position);
			}
			
			//animation["punch"].normalizedTime = 1.0f;
			animation.Play("punch");
			gameObject.transform.FindChild("weapon").collider.enabled = true;
			StartCoroutine(WaitAndCallback(animation["punch"].length));
			//Debug.Log("enabled collision");
			//Debug.Log("Time to wait: " + m_windbackTime.ToString());
		}
		
		float hAxis = -Input.GetAxis("L_XAxis_" + JoystickNumber);
		float vAxis = -Input.GetAxis("L_YAxis_" + JoystickNumber);
		
		if (m_stunTime == 0.0f)
		{
			m_force.x = hAxis;
			m_force.z = vAxis;
		}
		
		if (mashesLeft != 0 || Health <= 0 || m_windbackTime > 0.0f)
		{
			m_force.x = 0;
			m_force.z = 0;
		}
		
		if (m_force.x != 0.0f || m_force.z != 0.0f)
		{
			if (!animation.IsPlaying("walk"))
			{
				animation.Play("walk");
			}			
		}
		else
		{
			if (animation.IsPlaying("walk"))
			{
				animation.Stop("walk");
			}	
		}
		
		if (m_isInDarkness || m_isMonster)
		{
			if (DarknessSpeedDecrease != 0)
			{
				m_force /= DarknessSpeedDecrease;
			}
		}
		
		if(m_force.magnitude > 0.3f)
		{
			m_direction = m_force;
		}
		
		if (m_stunTime > 0.0f)
		{
			m_stunTime = Mathf.Max(0.0f, m_stunTime - dt);
		}
		
		Vector3 finalTarget =  transform.position + m_direction * 100;			
        Quaternion rotation = Quaternion.LookRotation(finalTarget);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime *angleDamping);
		//transform.LookAt (finalTarget);
	}
	
	void FixedUpdate()
	{
		rigidbody.AddForce(m_force * Force);
		//rigidbody.drag = 5;
		m_isInDarkness = !IsPlayerInSpotlight();
	}
	
	public bool IsPlayerInSpotlight ()
	{
		Vector3 lightPos = spotlight.transform.position;
		float radius = spotlight.radius;
		
		Vector3 pos = transform.position;
		float distance = Vector3.Distance(pos, lightPos);
		
		if (distance <= radius)
		{
			return true;
		}
		
		return false;
	}
	
	public bool IsMonster()
	{
		return m_isMonster;
	}
	
	void OnCollisionEnter(Collision col)
	{
		Collider collider = col.contacts[0].otherCollider;
		Enemy enemy = collider.gameObject.GetComponent<Enemy>();
		if (collider.tag == "Enemy" && !enemy.IsInSpotlight() && !m_isImmune)
		{
			if (MonsterHitInstantDeath)
			{
				TransformToMonster();
			}
			else if (MonsterHitLoseHealth)
			{
				if (!m_isImmune)
				{
					LoseHealth(MonsterDamage);
					m_isHurt = true;
					m_isImmune = true;
					
					if(PlayerHurtClip != null)
					{
						AudioSource.PlayClipAtPoint(PlayerHurtClip, transform.position);
					}
					
				}
			}
			
			if (HitsKnockBack)
			{
				KnockBack(enemy.transform.forward);				
			}
		}
		else if (collider.tag == "Player")
		{
			Player player = collider.gameObject.GetComponent<Player>();
			//player.PunchEmitter();
			if (player.IsMonster() && !m_isMonster && !m_isImmune)
			{
				if (MonsterHitInstantDeath)
				{
					TransformToMonster();
				}
				else if (MonsterHitLoseHealth)
				{
					if (!m_isImmune)
					{
						LoseHealth(MonsterDamage);
						m_isHurt = true;
						m_isImmune = true;
					}
				}
				
				if (HitsKnockBack)
				{
					KnockBack(player.transform.forward);
				}
			}
		}
	}
	
	public void KnockBack(Vector3 direction)
	{		
		rigidbody.AddForce(direction * KnockbackForce, ForceMode.Impulse);
		m_stunTime = StunTime;
	}
	
	public void TransformToMonster()
	{
		m_isMonster = true;
		Skull.guiTexture.enabled = true;
	}
	
	public void LoseHealth(int damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			TransformToMonster();
		}
	}
	
	IEnumerator WaitAndCallback(float waitTime)
	{
	    yield return new WaitForSeconds(waitTime);
	    AnimationFinished();
	}
	
	void AnimationFinished()
	{			
	    gameObject.transform.FindChild("weapon").collider.enabled = false;
		//Debug.Log("disabled collision");
		animation["punch"].normalizedTime = WindBackAnimationStart;
		animation["punch"].speed = -WindBackAnimationRate;		
		animation.Play("punch");
		
		
	}
	
	/*void OnTriggerEnter (Collider collider)
	{
		Debug.Log ("OnTriggerEnter");
		if (collider.attachedRigidbody != null)
			collider.attachedRigidbody.AddForce (3, 0,0,ForceMode.Impulse);
		//if (gameObject.animation.IsPlaying ("punch"))
		//collider.rigidbody.AddForce (30,0,30,ForceMode.Impulse);
	}*/
}