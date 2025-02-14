using System;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(ThirdPersonShooterController))]
public class Player : Entity, IDamageable
{
    [SerializeField] private HealthBarUI healthBar;

    [Header("Mana System")]
    [SerializeField] private ManaBar manaBar;
    [SerializeField] private float Mana;
    [SerializeField] private float manaRechargeRate = 6.5f;
    [SerializeField] private float healthRegenBuffRate = 5f;
    [SerializeField] public float currentMana;
    [SerializeField] public UnityEvent OnDeath;
    private Vector3 startPosition;
    private PlayerInput playerInput;
    private ThirdPersonShooterController ThirdPersonShooterController;
    private StarterAssetsInputs starterAssetInputs;
    private bool isRegeneratingHealth = false;
    private bool hasInfiniteMana = false;
    public static Player Instance { get; private set; }
    public virtual void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ThirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
        starterAssetInputs = GetComponent<StarterAssetsInputs>();
        

        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
        healthBar.SetMaxHealth(MaxHealth);
        currentMana = Mana;
        manaBar.SetMaxMana(Mana);
        startPosition = transform.position;
    }

    void Update()
    {
        
        if (isRegeneratingHealth)
        {
            RegenerateHealth();
        }

        if (hasInfiniteMana)
        {
            InfiniteMana();
        }
        else
        {
            RegenerateMana();
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.KeypadPeriod)&&Input.GetKey(KeyCode.LeftControl))
        {
            transform.position = startPosition;
        }
    }

    private IEnumerator HealthRegenBuff(int duration)
    {
        yield return new WaitForSeconds(duration);
        isRegeneratingHealth = false;
    }
    
    private IEnumerator InfiniteManaBuff(int duration)
    {
        yield return new WaitForSeconds(duration);
        hasInfiniteMana = false;
    }

    private void RegenerateHealth()
    {
        if (currentHealth < MaxHealth)
        {
            currentHealth += manaRechargeRate * Time.deltaTime;
            healthBar?.SetHealth(currentHealth);
        }
    }

    private void InfiniteMana()
    {
        currentMana = Mana;
        manaBar?.SetMana(currentMana);
    }

    public override void TakeDamage(float damage, Entity origin)
    {

        currentHealth -= damage;
        healthBar?.SetHealth(currentHealth);

        if (currentHealth <= 0 && !Death)
        {
            Death = true;
            OnDeath?.Invoke();
        }
    }

    public void UseMana(float mana)
    {
        currentMana -= mana;
        manaBar?.SetMana(currentMana);
    }

    public void RegenerateMana()
    {
        if (currentMana < Mana)
        {
            currentMana += manaRechargeRate * Time.deltaTime * 10f;
            manaBar?.SetMana(currentMana);
        }
    }

    public void RegenerateHealthCoroutine(int length)
    {
        isRegeneratingHealth = true;
        StartCoroutine(HealthRegenBuff(length));
    }

    public void InfiniteManaCoroutine(int length)
    {
        hasInfiniteMana = true;
        StartCoroutine(InfiniteManaBuff(length));
    }
    
    /// <summary>
    /// Toggles all input
    /// </summary>
    public void SetInputEnabled(bool value)
    {
        //playerInput.actions.Disable();
        if (value)
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.ActivateInput();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            playerInput.DeactivateInput();
        }
        starterAssetInputs.cursorInputForLook = value;
        starterAssetInputs.cursorLocked = value;
      


    }
}
