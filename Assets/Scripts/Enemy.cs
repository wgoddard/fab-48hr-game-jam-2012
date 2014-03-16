using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	private Spotlight spotlight = null;
	private Player[] players;
	
	public Vector3 designatedAreaCentre = Vector3.zero;
	public float designatedAreaRadius = 3;
	public float designatedAreaPlayerEngageRadius = 10;
	public float playerEngaeRadius = 1;
	
	public float monsterAmount = 0;
	
	public float scaleNormal = 0.2f;
	public float scaleMonster = 0.4f;
	
	public float transitionTime = 0.2f;
	
	public float direction = 0;
	private float rotateDirection = 1;
	private float newRotationDirection = 0;
	private float rotateDirectionVel = 0;
	
	public float speed = 5;
	
	private float timeUntilNewRotateDirection = 0;
	
	public float smallBunnyKnockBackForce = 10;
	public float largeBunnyKnockBackForce = 5;
	
	private float timeUntilJump = 0;
	public float timeBetweenJumpsMin = 5;
	public float timeBetweenJumpsMax = 10;
	
	public float jumpForceMin = 10;
	public float jumpForceMax = 20;
	
	public AudioClip transformClip;
	public AudioClip[] grrrrSounds;
	
	private float timeUntilNextGrowl = 0;
	private float timeBetweenGrowlsMin = 10;
	private float timeBetweenGrowlsMax = 40;
	
	// Use this for initialization
	void Start () {
		
		spotlight = (Spotlight)GameObject.Find("Spotlight").GetComponent<Spotlight>();
		
		Object[] oPlayers = GameObject.FindObjectsOfType(typeof(Player));
		
		players = new Player[oPlayers.Length];
		
		for(int o = 0; o < oPlayers.Length; ++o)
		{
			players[o] = (Player)oPlayers[o];
		}
		
		designatedAreaCentre = transform.position;
		designatedAreaCentre.y = 0;
		
		if(IsInSpotlight()){
			monsterAmount = 0;
		}
		else{
			monsterAmount = 1;
		}
		
		direction = Random.Range(0, Mathf.PI * 2.0f);
		timeUntilJump = Random.Range(timeBetweenJumpsMin, timeBetweenJumpsMax) - timeBetweenJumpsMin;
		timeUntilNextGrowl = Random.Range(timeBetweenGrowlsMin, timeBetweenGrowlsMax) - timeBetweenGrowlsMin;
	}
	
	public bool IsInSpotlight()
	{
		Vector3 dif = spotlight.transform.position - transform.position;
		dif.y = 0;
		
		return dif.magnitude < spotlight.radius;
	}
	
	public bool IsInDesignatedArea()
	{
		Vector3 dif = transform.position - designatedAreaCentre;
		dif.y = 0;
		return dif.magnitude < designatedAreaRadius;
	}
	
	Player GetClosestPlayer()
	{
		Player closestP = null;
		float closestDistSqr = float.MaxValue;
		foreach(Player p in players)
		{
			if (p.Health <= 0)
			{
				continue;
			}
			Vector3 dif = p.transform.position - transform.position;
			dif.y = 0;
			if(!p.IsPlayerInSpotlight() && dif.sqrMagnitude < closestDistSqr)
			{
				closestDistSqr = dif.sqrMagnitude;
				closestP = p;
			}
		}
		
		return closestP;
	}
	
	public bool IsPlayerInMyShit()
	{	
		if(IsInSpotlight())
		{
			return false;
		}
		
		Player p = GetClosestPlayer();
		
		if(p == null)
		{
			return false;
		}
		
		Vector3 dif = transform.position - p.transform.position;
		dif.y = 0;
		
		Vector3 difToDesArea = transform.position - designatedAreaCentre;
		difToDesArea.y = 0;
		
		return (dif.magnitude < playerEngaeRadius) && (difToDesArea.magnitude < designatedAreaPlayerEngageRadius);
	}
	
	public static float WrapAngle(float angle)
	{
		return angle - Mathf.Floor(angle / (Mathf.PI * 2.0f)) * Mathf.PI * 2.0f;
	}
	
	public static float SmallestAngleDelta(float lhs, float rhs)
	{
		const float max = Mathf.PI * 2.0f;
		const float halfmax = Mathf.PI;

		float rslt = lhs - rhs;
		if (Mathf.Abs(rslt) > halfmax)
			rslt = (lhs > rhs ? rslt - max : lhs + max - rhs);
		return rslt;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		timeUntilNewRotateDirection -= Time.deltaTime;
		
		if(timeUntilNewRotateDirection <= 0)
		{
			timeUntilNewRotateDirection = Random.Range(0.5f, 1.5f);
			
			newRotationDirection = Mathf.Sign(Random.Range(-1.0f, 1.0f)) * Random.Range(1.0f, 2.0f);
		}
		
		if(!IsInDesignatedArea() || IsPlayerInMyShit())
		{
			Vector3 dif = designatedAreaCentre - transform.position;
			
			if(IsPlayerInMyShit())
			{
				dif = GetClosestPlayer().transform.position - transform.position;
			}
			
			float angleToArea = WrapAngle(Mathf.Atan2(dif.x, dif.z));
			//direction = angleToArea;
			newRotationDirection = -SmallestAngleDelta(direction, angleToArea) * 5.0f;
		}
		
		rotateDirection = Mathf.SmoothDamp(rotateDirection, newRotationDirection, ref rotateDirectionVel, 0.1f);
		
		direction += rotateDirection * Time.deltaTime;
		direction = WrapAngle(direction);
		
		float prevAmount = monsterAmount;
		
		monsterAmount = Mathf.Clamp(monsterAmount + Time.deltaTime / (IsInSpotlight() ? -transitionTime : transitionTime), 0.0f, 1.0f);
		
		if(transformClip != null &&  prevAmount < 0.5f && monsterAmount >= 0.5f)
		{
			AudioSource.PlayClipAtPoint(transformClip, transform.position, 0.3f);
		}
		
		if(monsterAmount >= 1.0f)
		{
			timeUntilJump -= Time.deltaTime;
			timeUntilNextGrowl -= Time.deltaTime;
		}
		
		if(timeUntilJump < 0)
		{
			float jumpDir = direction + Random.Range(-0.4f, 0.4f);			
			Vector3 force = new Vector3(Mathf.Sin(jumpDir), Random.Range(0.4f, 0.6f), Mathf.Cos(jumpDir));
			force *= Random.Range(jumpForceMin, jumpForceMax);
			rigidbody.AddForce(force, ForceMode.Impulse);
			timeUntilJump = Random.Range(timeBetweenJumpsMin, timeBetweenJumpsMax);
		}
		
		if(timeUntilNextGrowl < 0)
		{
			if(grrrrSounds.Length > 0)
			{
				AudioSource.PlayClipAtPoint(grrrrSounds[Random.Range(0, grrrrSounds.Length-1)], transform.position, 0.6f);
			}
			
			timeUntilNextGrowl = Random.Range(timeBetweenGrowlsMin, timeBetweenGrowlsMax);
		}
		
		// animations
		if(monsterAmount <= 0.0f)
		{
			if(!animation.IsPlaying("idle"))
			{
				animation.Play("idle");
			}
		}
		else if(monsterAmount >= 1.0f)
		{
			if(!animation.IsPlaying("idleDark"))
			{
				animation.Play("idleDark");
			}
		}
		else
		{
			if(!animation.IsPlaying("toDark"))
			{
				animation.Play("toDark");
			}
			
			animation["toDark"].speed = 0;
			animation["toDark"].weight = 1.0f;
			animation["toDark"].enabled = true;
			animation["toDark"].time = monsterAmount * animation["toDark"].length;
		}
		
		Vector3 euler = transform.eulerAngles;
		euler.y = direction * Mathf.Rad2Deg;
		transform.eulerAngles = euler;
		transform.localScale = Vector3.one * Mathf.Lerp(scaleNormal, scaleMonster, monsterAmount);
		
		if (IsInSpotlight())
		{
			this.gameObject.layer = 5;
		}
		else
		{
			this.gameObject.layer = 3;
		}
		
		GetComponentInChildren<Renderer>().material.SetFloat("_DarknessAmount", monsterAmount);
	}
	
	void Render()
	{
	}
	
	void FixedUpdate() {
		rigidbody.AddForce((new Vector3(Mathf.Sin(direction), 0, Mathf.Cos(direction))) * speed);
	}
	
	public void KnockBack(Vector3 direction)
	{
		rigidbody.AddForce(direction * Mathf.Lerp(smallBunnyKnockBackForce, largeBunnyKnockBackForce, monsterAmount), ForceMode.Impulse);
	}
}
