using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	public ParticleSystem KnockBackParticle1;
	public ParticleSystem KnockBackParticle2;
	

	// Use this for initialization
	void Start () {
		if (KnockBackParticle1 != null)
		{
			KnockBackParticle1.emissionRate = 0;
			KnockBackParticle2.emissionRate = 0;
		}
		animation.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		
		animation.Stop();
	}
	
	void OnTriggerEnter (Collider collider)
	{
		//Debug.Log ("OnTriggerEnter");
		if (collider.tag == "Player")
		{
			Player otherPlayer = collider.gameObject.GetComponent<Player>();
			Vector3 otherPlayerPos = otherPlayer.transform.position;
			Vector3 myPos = transform.position;
			Vector3 direction = otherPlayerPos - myPos;
			direction.y = 0;
			direction.Normalize();
			otherPlayer.KnockBack(direction);
			//KnockBackParticle1.Emit(1);
			//KnockBackParticle2.Emit(1);
		}
		else if (collider.tag == "Enemy")
		{
			Enemy enemy = collider.gameObject.GetComponent<Enemy>();
			Vector3 enemyPos = enemy.transform.position;
			Vector3 myPos = transform.position;
			Vector3 direction = enemyPos - myPos;
			direction.y = 0;
			direction.Normalize();
			enemy.KnockBack(direction);
		}
	}
}
