using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static public CameraManager instance;

    public GameObject target; //카메라가 따라갈 대상
    public float moveSpeed; //카메라가 얼마나 빠른 속도로 대상을 쫓을건지 관리.
    private Vector3 targetPosition; //대상의 현재 위치 값

    public BoxCollider2D bound;

    /// <summary>
    /// 박스콜라이더의 x,y,z 영역 최소, 최대 값
    /// </summary>
    private Vector3 minBound;

    private Vector3 maxBound;

    /// <summary>
    /// 카메라의 중심점이 가운데에 있기 때문에 카메라가 화면 밖으로 나가지 않으려면 절반만 움직여줘야 함.
    /// width는 자신의 반 너비만큼 더해주고, height는 반 높이만큼 빼줘야 한다.
    /// </summary>
    private float halfWidth;

    private float halfHeight;

    /// <summary>
    /// 카메라의 반 높이 값을 구할 속성을 이용하기 위한 변수
    /// </summary>
    private Camera theCamera;

    private void Awake() //start보다 awake가 더 먼저 실행됨.
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }


    private void Start()
    {
        theCamera = GetComponent<Camera>();
        minBound = bound.bounds.min; //bounds: 박스콜라이더의 영역
        maxBound = bound.bounds.max;
        halfHeight = theCamera.orthographicSize; //카메라의 반높이
        halfWidth = halfHeight * Screen.width / Screen.height; //반너비 구하는 공식 (해상도)
    }


    private void Update()
    {
        if (target.gameObject != null) //카메라가 쫓을 대상이 있을 때
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);
            //this인 이유. 카메라의 위치 z값. 카메라가 플레이어 값과 일치되면 겹쳐서 보이지 않는다.

            this.transform.position =
                Vector3.Lerp(this.transform.position, targetPosition,
                    moveSpeed * Time.deltaTime); //Lerp:벡터A부터 B까지 float t의 속도로 움직이게 하는 것.(보간)

            float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
            // (10,0,100)일 경우 10이 값, 0이 최소값, 100이 최댓값
            // 10 return
            // (-100,0,100)일 경우
            // 0 return
            // 즉, 값이 최소값에서 최댓값의 사이 범위로 움직인다.
            float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);

            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound)
    {
        bound = newBound;
        minBound = bound.bounds.min; //bounds: 박스콜라이더의 영역
        maxBound = bound.bounds.max;
    }
}