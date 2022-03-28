using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, Boss};
    public Type enemyType;
    public float maxHealth;
    public float curHealth;
    public int score;
    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    public bool isChase;
    public bool isAttack;

    public GameObject bulletPos;

    Rigidbody rigid;
    BoxCollider boxCollider;
    //Material mat;
    Renderer[] meshs;
    NavMeshAgent nav;
    Animator anim;

    public Player player;
    public void setPlayer(Player _player) { player = _player; }

    bool isDead;

    public RectTransform healthBar;
    public Image barImg;
    public Image damagedBarImg;
    private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 1f;
    private float damagedHealthShrinkTimer;
    float changedHp;

    public AudioSource shootSound;
    public AudioSource damageSound;
    public AudioSource attackSound;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //mat = GetComponentInChildren<MeshRenderer>().material;
        meshs = GetComponentsInChildren<Renderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        changedHp = curHealth;

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

        curHealth = Mathf.Lerp(curHealth, changedHp, Time.deltaTime * 5f);
        if (curHealth < 0.5) isDead = true;
    }

    void LateUpdate()
    {
        if (enemyType != Type.Boss)
        {
            BarColor();
            //StartCoroutine(BarAnimation());
            healthBar.localScale = new Vector3((float)curHealth / maxHealth, 1, 1);
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType)
        {
            case Type.A:
            case Type.B:
            case Type.C:
                targetRadius = 0.4f;
                targetRange = 0.6f;
                break;
            case Type.Boss:
                targetRadius = 0.2f;
                targetRange = 15f;
                break;
        }
        
        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,
            targetRadius, transform.forward,
            targetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack && player.health>0 && !isDead)
        {
            StartCoroutine(Attack());    
        }

        if (player.health <= 0) Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        
        switch (enemyType)
        {
            case Type.A:
            case Type.B:
            case Type.C:
                yield return new WaitForSeconds(0.2f);
                if (!attackSound.isPlaying) attackSound.Play();
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(0.5f);
                break;
            case Type.Boss:
                yield return new WaitForSeconds(0.5f);

                shootSound.Play();

                GameObject instantBullet = Instantiate(bullet, bulletPos.transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 10;

                yield return new WaitForSeconds(2f);
                break;
        }
       
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();

            if (curHealth - weapon.damage > 0)
            {
                changedHp = curHealth - weapon.damage;
            }
            else changedHp = 0;

            Vector3 reactVec = transform.position - other.transform.position;
            if (!isDead) StartCoroutine(OnDamage(reactVec, false));
            else StartCoroutine(OnDie(reactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (curHealth - bullet.damage > 0)
            {
                changedHp = curHealth - bullet.damage;
            }
            else changedHp = 0;

            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            if (!isDead) StartCoroutine(OnDamage(reactVec, false));
            else StartCoroutine(OnDie(reactVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        if (curHealth - 100 > 0)
        {
            changedHp = curHealth - 100;
        }
        else changedHp = 0;

        Vector3 reactVec = transform.position - explosionPos;
        if (!isDead) StartCoroutine(OnDamage(reactVec, true));
        else StartCoroutine(OnDie(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        if (curHealth > 3)
        {
            foreach (Renderer mesh in meshs)
            {
                mesh.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);

            foreach (Renderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }

            reactVec = reactVec.normalized;
            reactVec *= -1;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec, ForceMode.Impulse);
        }
        else
        {
            changedHp = 0;
            isDead = true;
        }
    }

    IEnumerator OnDie(Vector3 reactVec, bool isGrenade)
    {
        //curHealth = 0;
        //isDead = true;
       
        if (!damageSound.isPlaying) damageSound.Play();
       
        foreach (Renderer mesh in meshs)
        {
            mesh.material.color = Color.grey;
        }

        gameObject.layer = 14;
        isChase = false;
        nav.enabled = false;
        anim.SetTrigger("doDie");
        Player player = target.GetComponent<Player>();
        player.score += score;
        int ranCoin = Random.Range(0, 3);
        Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

        switch (enemyType)
        {
            case Type.A:
                manager.enemyCntA--;
                if (manager.enemyCntA < 0) manager.enemyCntA = 0;
                break;
            case Type.B:
                manager.enemyCntB--;
                if (manager.enemyCntB < 0) manager.enemyCntB = 0;
                break;
            case Type.C:
                manager.enemyCntC--;
                if (manager.enemyCntC < 0) manager.enemyCntC = 0;
                break;
            case Type.Boss:
                manager.bossCnt--;
                if (manager.bossCnt < 0) manager.bossCnt = 0;
                break;
        }

        if (isGrenade)
        {
            reactVec = reactVec.normalized;
            reactVec *= -1;
            reactVec += Vector3.up * 3;

            rigid.freezeRotation = false;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
        }
        else
        {
            reactVec = reactVec.normalized;
            reactVec *= -1;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 3, ForceMode.Impulse);
        }
        Destroy(gameObject, 4);

        yield return null;
    }

    void BarColor()
    {
        int N = 4;
        float M = 1.5f;
        float healthCalc = (float)(((float)curHealth / (float)maxHealth)) * 100f;
        float N_root = (float)Mathf.Pow((healthCalc / 100f), (1f / M));
        float N_power = (float)Mathf.Pow((healthCalc / 100f), N);

        if (healthCalc < 50)
        {
            barImg.color = Color.Lerp(Color.red, Color.yellow, (float)N_root);
        }
        else if (healthCalc >= 50)
        {
            barImg.color = Color.Lerp(Color.yellow, Color.green, (float)N_power);
        }
    }
}