using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] TheKnightData data;
    // ===================== EXP ==================
    public int level;
    public float currentExp;
    bool isLevel_1 = false;
    bool isLevel_2 = false;
    bool isLevel_3 = false;
    bool isLevel_4 = false;
    bool isLevel_5 = false;

    public float getExp() { return currentExp; }
    [SerializeField] LevelText levelText;

    // ===================== POSITION ==================
    public float posX { get { return transform.position.x; } }
    public float posY { get { return transform.position.y; } }

    // ===================== PROJECTILE ==================
    public GameObject projectilePrefab;
    public GameObject projectileMeleePrefab;

    // ===================== HEALTH ==================
    public HealthBar healthBar;
    public HealthBar healthBarCorner;
    private float maxHealth = 20.0f;
    public float getMaxHealth()
    {
        return maxHealth;
    }
    public float health { get { return currentHealth; } }
    float currentHealth;
    float invincibleTimer;
    bool isInvincible;
    public float timeInvincible = 1.0f;
    public bool isDeath = false;

    // ===================== ATTACK ==================
    float attackCooldown;
    [SerializeField] bool isMeleeCombat = false;
    bool isAttackable = true;
    public float attackSpeed = 0.5f;
    public float damage = 1f;
    public float attackForce = 5.0f;
    public float knockBack = 1.0f;

    // ===================== ANIMATION ==================
    public Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    // ===================== AUDIO ==================
    AudioSource audioSrc;
    public AudioClip hitSound;
    public AudioClip AttackSound;
    void Start()
    {
        currentHealth = maxHealth;
        currentExp = 0;
        audioSrc = GetComponent<AudioSource>();
        if (MyGameManager.Instance.LoadSave() != null)
        {
            data = MyGameManager.Instance.LoadSave();
            if (gameObject.name == "Archer")
            {
                currentExp = data.getChar1();
            }
            else if (gameObject.name == "Mage")
            {
                currentExp = data.getChar2();
            }
            else if (gameObject.name == "Soldier")
            {
                currentExp = data.getChar3();
            }
            CheckLevel();
        }
    }
    // Update is called once per frame
    void Update()
    {
        // ================= HEALTH ====================
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        healthBarCorner.SetMaxHealth(maxHealth);
        healthBarCorner.SetHealth(currentHealth);
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackCooldown <= 0)
        {
            isAttackable = true;
        }

        Vector2 move = new Vector2(controller.horizontal, controller.vertical);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        // ============== ANIMATION =======================
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // Change attack range according to model


        // Attack
        if (Input.GetButtonDown("Fire1"))
        {
            if (isAttackable)
            {
                
                if (isMeleeCombat)
                {
                    MeleeAttack();
                }
                else
                {
                    RangedAttack();
                }
                attackCooldown = attackSpeed;
                isAttackable = false;
            }
        }

    }

    void RangedAttack()
    {
        // Play animation
        animator.SetTrigger("Shoot");
        // Create projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<Projectile>().SetDamage(damage);
        projectile.GetComponent<Projectile>().SetSource(gameObject.transform.tag);
        projectile.GetComponent<Projectile>().SetKnockBack(knockBack);
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 source = new Vector2(transform.position.x, transform.position.y);
        projectile.GetComponent<Rigidbody2D>().AddForce((target - source).normalized * attackForce, ForceMode2D.Impulse);
        playSound(AttackSound);
    }

    void MeleeAttack()
    {
        // Play animation
        animator.SetTrigger("Shoot");
        GameObject projectile = Instantiate(projectileMeleePrefab, transform.position, transform.rotation);
        projectile.GetComponent<ProjectileForMelee>().SetDamage(damage);
        projectile.GetComponent<ProjectileForMelee>().SetSource(gameObject.transform.tag);
        projectile.GetComponent<ProjectileForMelee>().SetKnockBack(knockBack);
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 source = new Vector2(transform.position.x, transform.position.y);
        projectile.GetComponent<Rigidbody2D>().AddForce((target - source).normalized * attackForce, ForceMode2D.Impulse);
        playSound(AttackSound);


    }

    public void ModifyHealth(float amount)
    {
        if (amount< 0) 
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            animator.SetTrigger("Hit");
            playSound(hitSound);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth == 0)
        {
            isDeath = true;
        }
    }

    public void ModifyExp(float amount)
    {
        currentExp += amount;
        CheckLevel();
    }

    public void CheckLevel()
    {
        if (currentExp >= 10 && currentExp < 20 && isLevel_1 == false)
        {
            isLevel_1 = true;
            level = 1;
            levelUp(level);
        }
        else if (currentExp >= 20 && currentExp < 40 && isLevel_2 == false)
        {
            isLevel_2 = true;
            level = 2;
            levelUp(level);
        }
        else if (currentExp >= 40 && currentExp < 80 && isLevel_3 == false)
        {
            isLevel_3 = true;
            level = 3;
            levelUp(level);
        }
        else if (currentExp >= 80 && currentExp < 120 && isLevel_4 == false)
        {
            isLevel_4 = true;
            level = 4;
            levelUp(level);
        }
        else if (currentExp >= 120 && isLevel_5 == false) 
        { 
            isLevel_5 = true;
            level = 5;
            levelUp(level);
        }
        levelText.SetLevel(level);
    }

    public void playSound(AudioClip clip)
    {
        audioSrc.PlayOneShot(clip);
    }
    public void setLevel()
    {
        levelText.SetLevel(level);
    }
    public void levelUp(int level)
    {
        switch (level)
        {
            case 1:
                {
                    maxHealth = maxHealth + 1.5f;
                    damage = damage + 1.0f;
                    transform.localScale = new Vector3(1.1f, 1.1f, 0f);
                    if (isMeleeCombat)
                    {
                        maxHealth = maxHealth + 1.5f;
                        damage = damage + 1.0f;
                    }
                    currentHealth = maxHealth;
                    break;
                }
            case 2:
                {
                    maxHealth = maxHealth + 2.0f;
                    damage = damage + 1.2f;
                    transform.localScale = new Vector3(1.15f, 1.15f, 0f);
                    if (isMeleeCombat)
                    {
                        maxHealth = maxHealth + 2.0f;
                        damage = damage + 1.2f;
                    }
                    currentHealth = maxHealth;
                    break;
                }
            case 3:
                {
                    maxHealth = maxHealth + 2.0f;
                    damage = damage + 1.3f;
                    transform.localScale = new Vector3(1.2f, 1.2f, 0f);
                    if (isMeleeCombat)
                    {
                        maxHealth = maxHealth + 1.0f;
                        damage = damage + 1.0f;
                    }
                    currentHealth = maxHealth;
                    break;
                }
            case 4:
                {
                    maxHealth = maxHealth + 2.5f;
                    damage = damage + 1.4f;
                    transform.localScale = new Vector3(1.25f, 1.25f, 0f);
                    if (isMeleeCombat)
                    {
                        maxHealth = maxHealth + 1.0f;
                        damage = damage + 1.5f;
                    }
                    currentHealth = maxHealth;
                    break;
                }
            case 5:
                {
                    maxHealth = maxHealth + 3.0f;
                    damage = damage + 1.5f;
                    transform.localScale = new Vector3(1.3f, 1.3f, 0f);
                    if (isMeleeCombat)
                    {
                        maxHealth = maxHealth + 1.5f;
                        damage = damage + 1.5f;
                    }
                    currentHealth = maxHealth;
                    break;
                }
        }
    }
}
