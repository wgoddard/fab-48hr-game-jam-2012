using UnityEngine;
using System.Collections;

public class CameraMultiTrackZoom : MonoBehaviour {
	
	public GameObject []trackingTarget;
	public Mesh test;
	
	public float minCameraHeight = 2.0f;
	public float maxCameraHeight = 10.0f;
	
	private Vector3 cameraVel = Vector3.zero;
	

	// Use this for initialization
	void Start () {
	
	}
	
	bool AreObjectsVisibleInCamera(UnityEngine.Camera camera)
	{
		foreach (GameObject o in trackingTarget)
		{
			if (o.GetComponent<Player>() != null)
			{
				if (o.GetComponent<Player>().IsMonster ())
					continue;
				if (o.GetComponent<Player>().Health <= 0)
				{
					continue;
				}
			}
			
			Vector3 pos = o.transform.position;
			pos.y = 0;
			Vector3 screenPos = camera.WorldToScreenPoint(pos);
			if(screenPos.x < Screen.width * 0.1 || screenPos.x > Screen.width * 0.9 ||
				screenPos.y < Screen.height * 0.1 || screenPos.y > Screen.height )
			{
				return false;
			}
		}
		
		return true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 desiredCamPos = Vector3.zero;
		int targetCount = 0;
		foreach (GameObject o in trackingTarget)
		{
			if (o.GetComponent<Player>() != null)
			{
				if (o.GetComponent<Player>().IsMonster ())
					continue;
				if (o.GetComponent<Player>().Health <= 0)
				{
					continue;
				}
			}
			targetCount++;
			desiredCamPos += o.transform.position;
		}
		
		desiredCamPos /= targetCount;
		
		Vector3 camPos = camera.transform.position;
		camera.transform.position = desiredCamPos;
		
		camera.transform.position = camera.transform.position - camera.transform.forward * minCameraHeight;
		
		while (camera.transform.position.y < maxCameraHeight && !AreObjectsVisibleInCamera(camera))
		{
			camera.transform.position = camera.transform.position - camera.transform.forward * 0.1f;
		}
		
		camera.transform.position = Vector3.SmoothDamp(camPos, camera.transform.position, ref cameraVel, 0.1f);
	}
}
