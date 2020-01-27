using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string transferMapName; //이동할 맵의 이름

    private MovingObject thePlayer;


    private CameraManager theCamera;

    private void Start()
    {
        thePlayer = FindObjectOfType<MovingObject>(); //다수의 객체
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            theCamera = FindObjectOfType<CameraManager>();

            thePlayer.currentMapName = transferMapName; //플레이어에 이동할 맵의 이름을 넣어준다.

            SceneManager.LoadScene(transferMapName);
        }
    }
}