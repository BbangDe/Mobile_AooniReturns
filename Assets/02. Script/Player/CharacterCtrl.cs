using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.Tilemaps;
using Fusion;
using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public enum CharacterType
{
    Human,
    Oni,
    Bucket
}

public class CharacterCtrl : NetworkBehaviour
{
    int player_layerMask;

    //==Edit: Added=====================

    private Vector2 targetPos;
    public float gridSize = 1f;

    public bool char_inMove;

    Vector3 originaltPos;

    private Vector2 lastInputDir_forObstacle = Vector2.zero;
    private Vector2 lastInputDir = Vector2.zero;
    public float moveTimer; //좀 더 RPG MAKER 느낌의 움직임을 위한 타이머

    //==================================

    // 241029 외주 수정 ===================================
    public float walk_speed;
    bool turnEnable;
    bool canWalk;
    bool nowWalk;

    int now_dir;

    Vector3 next_pos;
    // 241029 외주 수정 ===================================

    [SerializeField] Vector2 mapMinBounds;
    [SerializeField] Vector2 mapMaxBounds;

    [Networked] float speed { get; set; }

    [Networked, OnChangedRender(nameof(OnChangeDead))] public bool Dead { get; set; } = false;
    [Networked, OnChangedRender(nameof(OnChangeState))] public CharacterType CurrState { get; private set; } = CharacterType.Human;
    //[Networked, OnChangedRender(nameof(OnChangeDir))] public Vector2 CurrDir { get; private set; } = new Vector2(0, -1);
    public Vector2 CurrDir { get; private set; } = new Vector2(0, -1);
    public Vector2 LastChangeDir { get; private set; } = new Vector2(0, -1);
    [Networked, OnChangedRender(nameof(OnChangeWalk))] public bool IsWalk { get; private set; } = false;

    private Rigidbody2D rb2d;
    private CircleCollider2D characterCollider;

    private OniCtrl oniCtrl;
    private HumanCtrl humanCtrl;
    private BucketCtrl bucketCtrl;

    private JoystickPanel joystick;
    private Animator currAnimator;

    private TweenerCore<float, float, FloatOptions> speedTween;
    private float speedTarget;
    public ArrowCtrl arrow;

    public bool canChange;
    public bool nowChange;

    public float Speed
    {
        get => speed;
        set
        {
            if (value == 0f)
            {
                speedTween?.Kill();
                speed = 0f;
                speedTarget = 0f;
                return;
            }

            if (speedTarget == value)
            {
                return;
            }

            speedTween?.Kill();
            speedTarget = value;
            speedTween = DOTween.To(() => speed, v =>
            {
                speed = v;
            },
            value, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<CircleCollider2D>();

        oniCtrl = GetComponentInChildren<OniCtrl>(true);
        humanCtrl = GetComponentInChildren<HumanCtrl>(true);
        bucketCtrl = GetComponentInChildren<BucketCtrl>(true);

        currAnimator = humanCtrl.GetComponent<Animator>();

        // 241029 외주 수정 ===================================
        turnEnable = true;
        nowWalk = false;
        canWalk = true;

        now_dir = 0;
        // 241029 외주 수정 ===================================

        //====ADDED: 추가된 코드(캐릭터 타일 그리드에 맞춰 이동)=====================================================================

        GameObject main_cam = GameObject.FindGameObjectWithTag("MainCamera");
        main_cam.transform.parent = this.gameObject.transform;
        targetPos = transform.position;

        player_layerMask = 1 << LayerMask.NameToLayer("Player");
        //=======================================================================================================================
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BucketTrigger")
        {
            canChange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BucketTrigger")
        {
            canChange = false;
        }
    }

    public override void Spawned()
    {
        Debug.LogError(Object.Id);
        App.Manager.Player.SubmitPlayer(this);

        if (!HasStateAuthority)
        {
            return;
        }

        Object.RequestStateAuthority();

        joystick = App.Manager.UI.GetPanel<JoystickPanel>();
    }

    public void SetCharacterState(int _index)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        SetCharacterDead(false);
        CurrState = (CharacterType)_index;
    }

    private void OnChangeState()
    {
        switch (CurrState)
        {
            case CharacterType.Human:
                humanCtrl.gameObject.SetActive(true);

                currAnimator = humanCtrl.GetComponent<Animator>();
                SetupAnimator();

                oniCtrl.gameObject.SetActive(false);
                break;

            case CharacterType.Oni:
                oniCtrl.gameObject.SetActive(true);
                oniCtrl.Setup();

                currAnimator = oniCtrl.GetComponent<Animator>();
                SetupAnimator();

                humanCtrl.gameObject.SetActive(false);
                break;
            case CharacterType.Bucket:
                bucketCtrl.gameObject.SetActive(true);

                humanCtrl.gameObject.SetActive(false);
                break;
        }
    }

    public void ChangeBarrel()
    {
        if(nowChange)
        {
            nowChange = false;
            characterCollider.enabled = true;

            humanCtrl.gameObject.SetActive(true);
        }
        else if(canChange)
        {
            canChange = false;
            characterCollider.enabled = false;
            nowChange = true;

            humanCtrl.gameObject.SetActive(false);
        }
    }

    private void SetupAnimator()
    {
        currAnimator.SetFloat("MoveX", CurrDir.x);
        currAnimator.SetFloat("MoveY", CurrDir.y);
        currAnimator.SetBool("isWalk", IsWalk);
    }

    public void SetCharacterDead(bool _isDead)
    {
        Dead = _isDead;
    }

    private void OnChangeDead()
    {
        characterCollider.enabled = !Dead;

        if (Dead)
        {
            oniCtrl.gameObject.SetActive(false);
            humanCtrl.gameObject.SetActive(false);
            bucketCtrl.gameObject.SetActive(false);
        }
    }

    #region Calculate Position
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            CalculatePosition();
        }
    }

    private void CalculatePosition()
    {
        var xDir = joystick.Horizontal;
        var yDir = joystick.Vertical;

        if (Mathf.Abs(xDir) > Mathf.Abs(yDir))
        {
            yDir = 0; // 좌우
        }
        else
        {
            xDir = 0; // 상하
        }

        
        var dir = new Vector2(xDir, yDir);
        lastInputDir_forObstacle = dir.normalized;

        player_layerMask = ~player_layerMask;

        //============================================================================================================

        //startPosition = new Vector3(
        //       transform.position.x / gridSize * gridSize,
        //       transform.position.y / gridSize * gridSize,
        //       transform.position.z);
        //
        //// Calculate the end position based on the input direction (lastInputDir_forObstacle)
        //Vector3 endMask = new Vector3(
        //    startPosition.x + lastInputDir_forObstacle.x * gridSize,
        //    startPosition.y + lastInputDir_forObstacle.y * gridSize,
        //    startPosition.z);
        //
        //RaycastHit2D raycastHit2D = Physics2D.Linecast(startPosition, endMask);
        //
        //if (raycastHit2D.collider != null)
        //{
        //    //Debug.Log(raycastHit2D.collider.name);  // Log the object hit by the Linecast
        //
        //    // Check if the hit object is tagged as an obstacle
        //    if (raycastHit2D.collider.CompareTag("Obstacle"))
        //    {
        //        // If an obstacle is detected, stop the character's movement and reset its position
        //        Vector3 originalPos = new Vector3(
        //            Mathf.Floor(transform.position.x),
        //            Mathf.Floor(transform.position.y),
        //            transform.position.z);
        //
        //        transform.position = originalPos;  // Keep the current position (stop movement)
        //        char_inMove = false;               // Stop movement flag
        //        IsWalk = false;                    // Stop walking animation
        //
        //        return;  // Exit the method, preventing further movement
        //    }
        //}

        //============================================================================================================

        RaycastHit2D hit = Physics2D.Raycast(transform.position, lastInputDir_forObstacle, 1f, player_layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                //transform.position = originaltPos; // Keep the current position
                //char_inMove = false; // Stop movement flag
                //IsWalk = false; // Stop walking animation
                                //rb2d.velocity = 6 * CurrDir;
                //return;
            }
        
        }


        // 241029 외주 수정 ===================================

        CurrDir = lastInputDir.normalized;

        // 상하좌우 방향으로만 이동
        if (Mathf.Abs(CurrDir.x) > Mathf.Abs(CurrDir.y))
        {
            Vector2 edit_dir = CurrDir;
            edit_dir.y = 0;
            CurrDir = edit_dir; // 수직 이동을 0으로 설정
        }
        else
        {
            Vector2 edit_dir = CurrDir;
            edit_dir.x = 0;
            CurrDir = edit_dir; // 수평 이동을 0으로 설정
        }

        if (canWalk)
        {
            if (CurrState.Equals(CharacterType.Bucket))
            {
                return;
            }


            if (CurrDir.x != 0 || CurrDir.y != 0)
            {
                if(CurrDir.y != 0)
                {
                    if(CurrDir.y < 0)
                    {
                        next_pos = transform.position + (Vector3.down * gridSize);
                        now_dir = 4
                            ;
                    }
                    else
                    {
                        next_pos = transform.position + (Vector3.up * gridSize);
                        now_dir = 3;
                    }
                }
                else
                {
                    if (CurrDir.x < 0)
                    {
                        next_pos = transform.position + (Vector3.left * gridSize);
                        now_dir = 2;
                    }
                    else
                    {
                        next_pos = transform.position + (Vector3.right * gridSize);
                        now_dir = 1;
                    }
                }

                canWalk = false;
                StartCoroutine(MoveCoroutine());
            }
        }

        /*
        if(turnEnable)
        {
            CurrDir = lastInputDir.normalized;  // 241029 외주 수정

            now_dir = 0;

            if (CurrDir.x == 1)
            {
                now_dir = 1;
                OnChangeDir();
            }
            else if (CurrDir.x == -1)
            {
                now_dir = 2;
                OnChangeDir();
            }
            else if (CurrDir.y == 1)
            {
                now_dir = 3;
                OnChangeDir();
            }
            else if (CurrDir.y == -1)
            {
                now_dir = 4;
                OnChangeDir();
            }

            if(now_dir != 0)
            {
                nowWalk = true;
                turnEnable = false;
            }
        }
        */

        //MoveCharacter();
        // 241029 외주 수정 ===================================


        //====ADDED: 예외사항 - 조이스틱을 빠르게 드래그하고 놨을때 캐릭터가 이동중이라면 이동 종료까지 대기=============================
        //MoveTowardsGrid();
        /*
        if (Mathf.Abs(xDir) < 1f && Mathf.Abs(yDir) < 1f && hit.collider == null)
        {
            if (char_inMove)
            {
                MoveTowardsGrid();
                StoppingAnim();
            }
            return;
        }
        */
        //==================================================================================================================

        lastInputDir = dir.normalized;

        IsWalk = lastInputDir != Vector2.zero;

        if (lastInputDir == Vector2.zero)
        {
            moveTimer = 1;
            speed = 0f;
            return;
        }

        //CurrDir = lastInputDir.normalized;

        /*
        if (hit.collider == null)
        {
            MoveTowardsGrid();
        }
        */
        //====DEPRECATED: 기존의 코드==============================================================================

        //CurrDir = dir.normalized;
        //rb2d.velocity = 24 * CurrDir;
    }

    #region ADDED: 그리드 단위로 캐릭터 이동
    void MoveCharacter()
    {
        if (!nowWalk)
        {
            return;
        }

        rb2d.MovePosition(rb2d.position + CurrDir * speed * Time.fixedDeltaTime);
        
        //transform.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        /*
        if(!nowWalk)
        {
            return;
        }

        walkTimeCount += Time.deltaTime;
        if(walkTimeCount > 0.01f)
        {
            if(now_dir == 1)
            {
                transform.position += Vector3.right * (oneStep * walkSpeed);
                for(int i=0; i<walkSpeed; i++) nowWalkCount++;
            }
            else if (now_dir == 2)
            {
                transform.position += Vector3.left * (oneStep * walkSpeed);
                for (int i = 0; i < walkSpeed; i++) nowWalkCount++;
            }
            else if (now_dir == 3)
            {
                transform.position += Vector3.up * (oneStep * walkSpeed);
                for (int i = 0; i < walkSpeed; i++) nowWalkCount++;
            }
            else if (now_dir == 4)
            {
                transform.position += Vector3.down * (oneStep * walkSpeed);
                for (int i = 0; i < walkSpeed; i++) nowWalkCount++;
            }

            if(nowWalkCount >= walkCount)
            {
                turnEnable = true;
                nowWalk = false;
                nowWalkCount = 0f;
                walkTimeCount = 0f;

                //transform.position = next_pos;
            }
        }
        */
    }

    void MoveTowardsGrid()
    {
        moveTimer += 0.275f;

        // 241029 외주 수정 ===================================
        if (moveTimer > 1 && turnEnable)  // 241029 외주 수정
        {
            CurrDir = lastInputDir.normalized;  // 241029 외주 수정
            //캐릭터의 현재 위치인 transform.position을 gridSize로 나누기 => 값을 반올림(Mathf.Round)하여 현재 위치를 그리드 단위로 맞춤
            Vector3 currentPos = new Vector3(
              Mathf.Round(transform.position.x / gridSize) * gridSize,
              Mathf.Round(transform.position.y / gridSize) * gridSize,
              transform.position.z);

            targetPos = currentPos;

            if (lastInputDir.x > 0) // 이동 - 오른쪽
            {
                targetPos.x += gridSize;
            }
            else if (lastInputDir.x < 0) // 이동 - 왼쪽
            {
                targetPos.x -= gridSize;
            }

            if (lastInputDir.y > 0) // 이동 - 위
            {
                targetPos.y += gridSize;
            }
            else if (lastInputDir.y < 0) // 이동 - 아래
            {
                targetPos.y -= gridSize;
            }

            moveTimer = 0f;
            turnEnable = false; // 241029 외주 수정
            char_inMove = true;
        }
        // 241029 외주 수정 ===================================

        //transform.position = Vector3.Lerp(transform.position, targetPos, walkSpeed);

        //transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.25f);

        // 만약 캐릭터가 그리드의 중앙에 거의 근접했으면 그리드 단위로 맞춤
        if (Vector3.Distance(transform.position, targetPos) < 0.25f)
        {
            Vector3 next_pos = new Vector3(
                     Mathf.Round(transform.position.x / gridSize) * gridSize,
                     Mathf.Round(transform.position.y / gridSize) * gridSize,
                     transform.position.z);

            transform.position = next_pos;

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                turnEnable = true;
                originaltPos = transform.position;
            }

        }
        
    }

    void StoppingAnim() //기존에 이동하던 캐릭터가 그리드 중앙에 근접하면 강제로 멈춤 처리
    {
        
        if (char_inMove && Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            char_inMove = false;
            IsWalk = false;
        }
        
    }

    IEnumerator MoveCoroutine()
    {
        float now_pos = 0f;
        float new_pos = 0f;

        if(now_dir == 1 || now_dir == 2)
        {
            now_pos = transform.position.x;
            new_pos = next_pos.x;
        }
        else
        {
            now_pos = transform.position.y;
            new_pos = next_pos.y;
        }

        OnChangeDir();

        if (now_dir == 1 || now_dir == 3)
        {
            while (now_pos < new_pos)
            {
                if (now_dir == 1)
                {
                    transform.position += (Vector3.right * walk_speed * Time.deltaTime);
                    now_pos = transform.position.x;
                }
                else
                {
                    transform.position += (Vector3.up * walk_speed * Time.deltaTime);
                    now_pos = transform.position.y;
                }

                yield return null;
            }
        }
        else
        {
            while (now_pos > new_pos)
            {
                if (now_dir == 2)
                {
                    transform.position += (Vector3.left * walk_speed * Time.deltaTime);
                    now_pos = transform.position.x;
                }
                else
                {
                    transform.position += (Vector3.down * walk_speed * Time.deltaTime);
                    now_pos = transform.position.y;
                }

                yield return null;
            }
        }
        transform.position = next_pos;
        now_dir = 0;

        canWalk = true;
        originaltPos = transform.position;

        yield return null;
    }

    #endregion


    #endregion

    private void OnChangeDir()
    {
        currAnimator.SetFloat("MoveX", CurrDir.x);
        currAnimator.SetFloat("MoveY", CurrDir.y);

        LastChangeDir = CurrDir;
    }

    private void OnChangeWalk()
    {
        currAnimator.SetBool("isWalk", IsWalk);
    }

    public void MoveToRandomPosition(Vector2 _randomPosition)
    {
        transform.position = _randomPosition;
    }

    private Vector2 GetRandomPosition()
    {
        float randomX = UnityEngine.Random.Range(mapMinBounds.x, mapMaxBounds.x);
        float randomY = UnityEngine.Random.Range(mapMinBounds.y, mapMaxBounds.y);
        return new Vector2(randomX, randomY);
    }

    private bool IsPositionColliding(Vector2 position)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(position, 0.5f);
        return hitCollider != null;
    }
}