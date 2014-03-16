using UnityEngine;
using System.Collections;

public class Spotlight : MonoBehaviour {	
	
		
	public float acceleration = 0.01f;
	public float speed = 1.0f;
	public float lowerXBoundForScene = -10.0f;
	public float upperXBoundForScene = 10.0f;
	public float lowerZBoundForScene = -10.0f;
	public float upperZBoundForScene = 10.0f;
	
	public HUD hud;
	
	public Vector3 startScale = new Vector3(16.0f,0.1f, 16.0f);
	public Vector3 endScale = new Vector3(6.0f,0.1f, 6.0f);
	public float startBrightness = 3.5f;
	public float endBrightness = 2f;
	public float shrinkTime = 100.0f;
	private float shrinkProgress = 0.0f;
	
	private bool m_beenReady = false;
	
	
	//Pathing
	public iTween.EaseType easeType= iTween.EaseType.linear;
	int pointCount = 6;
	float pointDeviation =20f;
	Vector3[] path = null;
	Vector3 rootPosition;
	private GameObject tracked;
	
	void GenerateRandomPath(){
		rootPosition = transform.position;
		path = new Vector3[pointCount];
		path[0]=rootPosition;
		for (int i = 1; i < pointCount; i++) {
			float randomZ = rootPosition.z + Random.Range(lowerZBoundForScene,upperZBoundForScene);
			float randomX = rootPosition.x + Random.Range(lowerXBoundForScene,upperXBoundForScene);
			path[i]=new Vector3(randomX,transform.position.y,randomZ);
		}
	}
	
	
	// Use this for initialization
	void Start () {
		transform.localScale = startScale;
		Shader.SetGlobalVector ("_SpotlightPos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0.0f));
		Shader.SetGlobalFloat ("_SpotlightRadius", radius);		
	}
	
	public float radius
	{
		get
		{
			return transform.localScale.x * 0.5f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_beenReady && hud.IsReady())
		{
			m_beenReady = true;
			tracked = new GameObject("Tracked");
			if (iTweenPath.GetPath ("Spotlight1") != null)
				path = iTweenPath.GetPath ("Spotlight1");
			else
				GenerateRandomPath();					
			
			Shader.SetGlobalVector ("_SpotlightPos", new Vector4(0,0,0,0));
			Shader.SetGlobalFloat ("_SpotlightRadius", 0.8f);
		
			iTween.MoveTo(tracked,iTween.Hash("path",path,"speed",speed + 2,"easetype",easeType,"looptype",iTween.LoopType.loop));		
			
			Physics.IgnoreLayerCollision(3,5);			
		}
		
		Light light = Light.GetLights(LightType.Spot,0)[0];
		light.intensity = Mathf.Lerp (startBrightness,endBrightness,shrinkProgress/shrinkTime);
		
		light.spotAngle =  (Mathf.Atan2(transform.localScale.x/2,light.transform.position.y) * 180 / Mathf.PI) * 2   + 5;
		
		if (!m_beenReady)
		{
			return;
		}

		
		if (hud.IsVictory())
		{
			//transform.position = new Vector3(0,0,0);
			iTween.Pause(tracked);
			transform.localScale += new Vector3(0.2f, 0, 0.2f);
			if (transform.localScale.x > startScale.x)
			{
				transform.localScale = startScale;
			}
			//Shader.SetGlobalVector ("_SpotlightPos", new Vector4(0, 0, 0, 0.0f));
			Shader.SetGlobalFloat ("_SpotlightRadius", radius);
			return;
		}
	
		transform.LookAt (tracked.transform.position);
		
		shrinkProgress += Time.deltaTime;
		
		transform.localScale = Vector3.Lerp (startScale, endScale, shrinkProgress/shrinkTime);
		
		
		
		speed += Time.deltaTime * acceleration;
		
		//Here we can accelerate it.
		iTween.MoveUpdate(tracked,iTween.Hash("speed",speed + 2));			
		
		if (Vector3.Distance (transform.position, tracked.transform.position) > Mathf.Abs (0.5f))
		{
			
			//transform.position = new Vector3(transform.position.x + transform.forward.x *Time.deltaTime * speed,
			//	transform.position.y, transform.position.z + transform.forward.z * Time.deltaTime * speed);
		}
		
		Vector4 vec = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0.0f);
		Shader.SetGlobalVector ("_SpotlightPos", vec);
		Shader.SetGlobalFloat ("_SpotlightRadius", radius);
		
		renderer.material.color = new Color(1,1,1,0.2f);
		
	}
	
	void FixedUpdate()
	{
		if (!m_beenReady || hud.IsVictory())
		{
			return;
		}
		
		rigidbody.AddForce (transform.forward * speed);
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, speed);
	}

}
