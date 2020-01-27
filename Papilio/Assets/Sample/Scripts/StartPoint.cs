using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{

	public string startPoint; //맵이 이동되면 플레이어가 시작될 위치.

	private MovingObject thePlayer;
	private CameraManager theCamera;
	
	private void Start()
	{
		theCamera = FindObjectOfType<CameraManager>();
		thePlayer = FindObjectOfType<MovingObject>(); 
		
		if (startPoint == thePlayer.currentMapName) // 정보를 가져와야 함.
		{
			theCamera.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,theCamera.transform.position.z);
			thePlayer.transform.position = this.transform.position;
		}
	}
}
