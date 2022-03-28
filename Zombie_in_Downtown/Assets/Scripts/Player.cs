using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("이동관련속성.")]
    [Tooltip("기본이동속도.")]
    public float MoveSpeed = 2.0f;//이동속도.
    public float RunSpeed = 3.5f;//달리기속도.
    public float DirectionRotationSpeed = 100.0f;//이동방향을 변경하기 위한 속도.
    public float BodyRotateSpeed = 2.0f;//몸통의 방향을 변경하기 위한 속도.
    [Range(0.01f, 5.0f)]
    public float VelocityChangeSpeed = 0.1f;//속도가 변경되기 위한 속도.
    private Vector3 CurrentVelocity = Vector3.zero;
    private Vector3 MoveDirection = Vector3.zero;
    private CharacterController myCharacterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;

    public string name;

    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameObject managerObj;
    public GameManager manager;

    public int stage;
    public float playTime;

    public int ammo;
    public int coin;
    public int health;
    public int score;
    public int hasGranades;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGranades;

    Rigidbody rigid;
    bool wDown;//walk
    bool jDown;//jump
    bool iDown;//item
    bool fDown;//fire
    bool gDown;//grenade
    bool rDown;//reload
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool scopeDown;

    bool isMoving;
    bool isFireReady = true;
    bool isSwap;
    bool isReload;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;
    bool isTalk;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex =-1;
    Animator anim;
    Renderer[] meshs;

    public GameObject userName;

    float fireDelay;
    bool lastShoot = true;

    public Scope scope;
  
    public AudioSource throwSound;
    public AudioSource shootSound;
    public AudioSource caseSound;
    public AudioSource reloadSound;
    public AudioSource walkSound;
    public AudioSource dieSound;

    public bool GetIsFireReady() { return isFireReady; }

    void Awake()
    {
        if (managerObj == null)
        {
            managerObj = GameObject.FindWithTag("GameManager");
            manager = managerObj.GetComponent<GameManager>();
        }

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        meshs = GetComponentsInChildren<Renderer>();
        Debug.Log(meshs.Length);
        PlayerPrefs.SetInt("MaxScore", 0);

        myCharacterController = GetComponent<CharacterController>();

        name = PlayerPrefs.GetString("playerName");
        userName.GetComponent<TextMesh>().text = name;

        //설정.
        /*
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Ammo", 0);
        PlayerPrefs.SetInt("Coin", 0);
        PlayerPrefs.SetInt("Health", 0);
        PlayerPrefs.SetInt("HasGranades", 0);

        PlayerPrefs.SetInt("HasWeapons1", 0);
        PlayerPrefs.SetInt("HasWeapons2", 0);
        PlayerPrefs.SetInt("HasWeapons3", 0);

        transform.position = new Vector3(-18, 0, 0);
        */
        stage = 4;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //name = PlayerPrefs.GetString("playerName");
        //userName.GetComponent<TextMesh>().text = name;

        Debug.Log(PlayerPrefs.GetString("playerName"));

       // managerObj = GameObject.FindWithTag("GameManager");
       //     manager = managerObj.GetComponent<GameManager>();

        //if(name != string.Empty) Debug.Log(name);
        if (isFireReady && !isDead)
        {
            //이동.
            Move();
        }
        //몸통의 방향을 이동 방향으로 돌려줍니다.
        BodyDirectionChange();

        Jump();
        Grenade();
        Attack();
        Reload();
        Swap();
        Interaction();
        Scope();

        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Grenade");
        rDown = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
       if(Input.GetButtonDown("Fire2")) scopeDown= !scopeDown;

        anim.SetBool("isRun", myCharacterController.velocity != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        if (isMoving && !walkSound.isPlaying) walkSound.Play(); 
        if (!isMoving) walkSound.Stop();
    }

    public void savePlayer() 
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Ammo", ammo);
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetInt("HasGranades", hasGranades);

        PlayerPrefs.SetInt("HasWeapons1", hasWeapons[0] ? 1 : 0);
        PlayerPrefs.SetInt("HasWeapons2", hasWeapons[1] ? 1 : 0);
        PlayerPrefs.SetInt("HasWeapons3", hasWeapons[2] ? 1 : 0);
    }

    public void loadPlayer()
    {
        score = PlayerPrefs.GetInt("Score");
        ammo = PlayerPrefs.GetInt("Ammo");
        coin = PlayerPrefs.GetInt("Coin");
        health = PlayerPrefs.GetInt("Health");
        hasGranades = PlayerPrefs.GetInt("HasGranades");

        hasWeapons[0] = (PlayerPrefs.GetInt("HasWeapons1") == 1) ? true : false;
        hasWeapons[1] = (PlayerPrefs.GetInt("HasWeapons2") == 1) ? true : false;
        hasWeapons[2] = (PlayerPrefs.GetInt("HasWeapons3") == 1) ? true : false;
    }

    //이동 관련 함수.
    void Move()
    {
        //MainCamera 게임오브젝트의 트랜스폼 컴포넌트.
        Transform CameraTransform = Camera.main.transform;
        //카메라가 바라보는 방향의 월드상에서는 어떤 방향인지 얻어옵니다.
        Vector3 forward = CameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 targetDirection;

        //우리가 이동하고자 하는 방향.
        
        targetDirection = horizontal * right + vertical * forward;
        

        //현재 이동하는 방향에서 원하는 방향으로 조금씩 회전을 하게 됩니다.
        MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection,
            DirectionRotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);

        //방향이기 때문에 크기는 없애고 방향만 가져옵니다.
        MoveDirection = MoveDirection.normalized;
        //이동 속도.
        float speed = MoveSpeed;

        //이번 프레임에 움직일 양.
        Vector3 moveAmount;
        if (!isBorder) moveAmount = (MoveDirection * speed * Time.deltaTime);
        else
        {
            MoveDirection.x =0.0f;
            moveAmount = (MoveDirection * speed * Time.deltaTime);
        }
        //실제 이동.
        collisionFlags = myCharacterController.Move(moveAmount);
    }
    //현재 내 캐릭터의 이동속도를 얻어옵니다.
    float GetVelocitySpeed()
    {
        //멈춰있다면.
        if (myCharacterController.velocity == Vector3.zero)
        {
            isMoving = false;

            //현재 속도를 0으로.
            CurrentVelocity = Vector3.zero;
        }
        else
        {
            isMoving = true;

            //StopCoroutine(walkSound_play());
            //StartCoroutine(walkSound_play());

            Vector3 goalVelocity = myCharacterController.velocity;
            goalVelocity.y = 0.0f;
            CurrentVelocity = Vector3.Lerp(CurrentVelocity, goalVelocity,
                VelocityChangeSpeed * Time.fixedDeltaTime); 
        }
        //currentVelocity의 크기를 리턴합니다.
        return CurrentVelocity.magnitude;
    }

    IEnumerator walkSound_play()
    {
        walkSound.Play();
        yield return new WaitForSeconds(0.1f);
        //walkSound.Stop();
    }

    //몸통의 방향을 이동방향으로 돌려줍니다.
    void BodyDirectionChange()
    {
        //1.키보드에 의한 회전
        //움직이고 있다면.
        if (GetVelocitySpeed() > 0.0f)
        {
            //Vector3 newForward = myCharacterController.velocity;
            Vector3 newForward = myCharacterController.velocity;
            newForward.y = 0.0f;
            transform.forward = Vector3.Lerp(transform.forward, newForward,
                BodyRotateSpeed * Time.deltaTime);
        }

        /*
        //2.마우스에 의한 회전
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                //CameraTransform.LookAt(transform.position + nextVec);
                nextVec.y = 0.0f;
                transform.forward = Vector3.Lerp(transform.forward, nextVec,
                    4.0f * Time.deltaTime);
            }
        }
        */
    }

    void Jump()
    {
        if (jDown)
        {
            //rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
        }
    }

    void Grenade()
    {
        if (hasGranades == 0)
            return;
        if (gDown && !isReload && !isSwap)
        {
            StartCoroutine(throwSound_play());

            Vector3 nextVec = transform.forward * 5;
            nextVec.y = 5;

            GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
            Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
            rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
            rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

            hasGranades--;
            //Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            //RaycastHit rayHit;
            //if (Physics.Raycast(ray, out rayHit, 100))
            //{
            //    Vector3 nextVec = rayHit.point - transform.position;
            //    nextVec.y = 10;

           
            //}
        }
    }

    IEnumerator throwSound_play()
    {
        throwSound.Play();
        yield return new WaitForSeconds(1f);
        throwSound.Stop();
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isShop && !isDead &&!isTalk)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;

            if (equipWeapon.curAmmo != 0 || (equipWeapon.curAmmo == 0 && lastShoot))
            {
                if (equipWeapon.curAmmo == 0 && lastShoot) lastShoot = false;

                if (equipWeapon.type == Weapon.Type.Range)
                {
                    StopCoroutine(shootSound_play());
                    StartCoroutine(shootSound_play());
                    StopCoroutine(caseSound_play());
                    StartCoroutine(caseSound_play());
                }
            }
        }
    }

    IEnumerator shootSound_play()
    {
        shootSound.Play();
        yield return new WaitForSeconds(1f);
        //shootSound.Stop();
    }

    IEnumerator caseSound_play()
    {
        yield return new WaitForSeconds(0.5f);
        caseSound.Play();
        yield return new WaitForSeconds(1f);
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (rDown && !isSwap && isFireReady && !isShop && !isDead && !isTalk)
        {
            isReload = true;

            Invoke("ReloadOut", 0.5f);
        }
    }

    void ReloadOut()
    {
        //StopCoroutine(reloadSound_play());
        if (ammo != 0 && equipWeapon.curAmmo != equipWeapon.maxAmmo)
        {
            StartCoroutine(reloadSound_play());
            lastShoot = true;
        }

        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        //equipWeapon.curAmmo += reAmmo;

        int totalAmmo = equipWeapon.curAmmo + reAmmo;
        int excessAmmo;//초과탄약.
        if (totalAmmo > equipWeapon.maxAmmo)
        {
            excessAmmo = totalAmmo - equipWeapon.maxAmmo;
            ammo -= (equipWeapon.maxAmmo - excessAmmo);
            equipWeapon.curAmmo = equipWeapon.maxAmmo;
        }
        else
        {
            equipWeapon.curAmmo = totalAmmo;
           ammo -= reAmmo;
        }
        isReload = false;

    }

    IEnumerator reloadSound_play()
    {
        reloadSound.Play();
        yield return new WaitForSeconds(1f);
        reloadSound.Stop();
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isDead)
        {
            if(equipWeapon !=null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
        }

        isSwap = true;

        Invoke("SwapOut", 0.4f);
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if (iDown && nearObject != null && !isDead)
        {
            //if (nearObject.tag == "Weapon")
            //{
            //    Item item = nearObject.GetComponent<Item>();
            //    int weaponIndex = item.value;
            //    hasWeapons[weaponIndex] = true;
            //
            //    Destroy(nearObject);
            //}
            if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
            else if (nearObject.tag == "Npc")
            {
                Talk talk = nearObject.GetComponent<Talk>();
                talk.Enter();
                isTalk = true;
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 1, Color.green);
        //isBorder = Physics.Raycast(transform.position, transform.forward, 1,
        //    LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    hasGranades += item.value;
                    if (hasGranades > maxHasGranades)
                        hasGranades = maxHasGranades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                if (other.GetComponent<Rigidbody>() != null)
                    Destroy(other.gameObject);
              
                StartCoroutine(OnDamage());
            }
        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;
        foreach (Renderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        if (health <= 0 && !isDead)
        {
            OnDie();
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (Renderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

    void OnDie()
    {
        dieSound.Play();
        anim.SetTrigger("doDie");
        isDead = true;
        //manager.GameOver();

        //int maxScore = PlayerPrefs.GetInt("MaxScore");
        //if (player.score > maxScore)
        //{
        //    //bestText.gameObject.SetActive(true);
        //    PlayerPrefs.SetInt("MaxScore", player.score);
        //}

        SceneManager.LoadScene(4);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop" || other.tag == "Npc")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "Shop")
        {
            if (nearObject != null)
            {
                Shop shop = nearObject.GetComponent<Shop>();
                if (shop != null)
                    shop.Exit();
                isShop = false;
                nearObject = null;
            }
        }
        else if (other.tag == "Npc")
        {
            if (nearObject != null)
            {
                Talk talk = nearObject.GetComponent<Talk>();
                if (talk != null) talk.Exit();
                isTalk = false;
                nearObject = null;
            }
        }
    }

    public void SetPlayerName()
    {
        name = PlayerPrefs.GetString("playerName");
        userName.GetComponent<TextMesh>().text = name;
    }

    void Scope()
    {
        if (scopeDown && equipWeapon != null && equipWeapon.isRifle)
        {
            scope.isScoped = true;
        }
        else scope.isScoped = false;
    }
}
