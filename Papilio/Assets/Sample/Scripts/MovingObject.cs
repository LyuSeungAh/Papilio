using System.Collections;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    /// <summary>
    /// static : 정적 변수로서, 해당 스크립트가 적용된 모든 객체들은 Static으로 선언된 변수의 값을 공유함.
    /// </summary>
    static public MovingObject instance;
    
    /// <summary>
    /// transferMap 스크립트에 있는 transferMapName 변수의 값을 저장
    /// </summary>
    public string currentMapName;


    /// <summary>
    /// 스피드와 캐릭터 움직임을 위한 벡터 변수
    /// </summary>
    public float speed;

    private Vector3 vector;

    /// <summary>
    /// 대쉬를 눌렀을 때를 위한 변수
    /// </summary>
    public float runSpeed;

    private float applyRunSpeed;

    /// <summary>
    /// 대쉬 눌렀을 때 걷기 두 배씩 이동하는 것 막는 변수
    /// </summary>
    private bool applyRunFlag = false;

    /// <summary>
    /// 픽셀 단위로 움직이기 위해 필요한 변수
    /// </summary>
    public int walkCount;

    private int currentWalkCount;

    /// <summary>
    /// 코루틴이 반복실행되지 않게끔 제어해주는 변수.
    /// </summary>
    private bool canMove = true;

    /// <summary>
    /// 애니메이션 제어를 위한 부분
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 벽이나 물체에 막혔을 때 멈추게 하기 위한 스크립트
    /// </summary>
    private BoxCollider2D boxCollider;

    public LayerMask layerMask; //충돌할 때 어떤 레이어와 충돌했는지 판단, 통과할 수 없는 레이어로 설정해줌.

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);

            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            
            instance = this;
        }
        else
        {
            Destroy(this.gameObject); //처음 생성된 경우에만 instance의 값이 Null임. 왜냐하면 생성된 이후에 this 값을
            //주었기 때문. 그리고나서, 해당 스크립트가 적용된 객체가 또 생성될 경우, static으로 값을 공유한 instance의 값이
            //this 이기 때문에, 그 객체는 삭제됨.
        }

       
    }

    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) // 키 입력이 이루어졌을 경우
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

            if (vector.x != 0)
                vector.y = 0;
            // 만약에 벡터x의 값이 있다면 다른 벡터 값은 초기화한다.
            //정면을 보는데 왼쪽으로 간다던가 하는 경우 방지.

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y); //파라미터 DirX,Y에 vector변수 값 전달, 파라미터에서 전달받은 변수 값으로 
            //각각의 애니메이션 실행시켜줌.

            RaycastHit2D hit; //A지점, B지점이 있을 때, B지점을 향해 레이저를 쐈을 때 B지점까지 레이저가 도달한다 = 
            // hit = Null; 값이 리턴된다.
            // 도달하지 못하고 장애물에 충돌 시 hit = 장애물; 리턴된다.
            Vector2 start = transform.position; // A 지점, 캐릭터의 현재 위치 값
            Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount);
            // B 지점, 캐릭터가 이동하고자 하는 위치 값, 현재 값+ 앞으로 이동하고자 할 픽셀

            boxCollider.enabled = false; //자기자신이 인식되기 때문에 꺼줘야 한다.
            hit = Physics2D.Linecast(start, end, layerMask); //레이저 쐈을 때 
            boxCollider.enabled = true; //레이저 다 쏘면 다시 켜기

            if (hit.transform != null) //반환되는 값이 있을 경우 (레이어마스크에 벽이 반환된다면.)
            {
                break; //이후의 명령어를 실행시키지 않겠다.
            }

            animator.SetBool("Walking", true);


            //픽셀 단위로 움직이기 위한 스크립트
            //만약 walk Count = 20 이고 speed 2.4일 경우 20*2.4=48픽셀
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0) // 좌우 방향키가 눌렸을 경우
                {
                    transform.Translate(vector.x * (speed + applyRunSpeed), 0, 0);
                }
                else if (vector.y != 0) // 위아래 방향키가 눌렸을 경우
                {
                    transform.Translate(0, vector.y * (speed + applyRunSpeed), 0);
                }

                if (applyRunFlag) // 대쉬 버튼 눌려있을 경우 워크카운트를 두번 증가시킨다.
                {
                    currentWalkCount++;
                }

                currentWalkCount++;
                // 픽셀이 한번에 움직이는 것이 아니라 그 과정도 자연스럽게 보이기 위해 while문 안에다가 넣는다.
                yield return new WaitForSeconds(0.01f); //ex) 반복문이 20번 실행되면 0.2초 기다리면 된다.
            }

            currentWalkCount = 0;
        }

        canMove = true;

        animator.SetBool("Walking", false); // 다시 서있는 모션으로 바꾸어 줌
    }

    void Update()
    {
        if (canMove) // 
        {
            // 좌 화살표(-1 리턴), 우 화살표(1 리턴) || 아래 화살표(-1 리턴), 위 화살표(1 리턴)
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
    }
}