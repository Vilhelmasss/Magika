using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Unity.Mathematics;

public class PlayerMagicSystem : MonoBehaviour
{
    [SerializeField] private Spell spellToCast;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana = 0f;
    [SerializeField] private float manaRechargeRate = 2f;
    [SerializeField] private float timeWaitForRecharge = 1f;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private GameObject manaBarNumber;
    private string currentCastedAbility = "";
    [SerializeField] private Transform castPoint;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float currChargeTime;

    [Header("LMB")]
    [SerializeField] private float LMBCooldownMax;
    [SerializeField] private GameObject LMBPerlinNoise;
    [SerializeField] private GameObject LMBCooldownNumber;
    private float LMBCooldownCurr = 0f;

    [Header("RMB")]
    [SerializeField] private float RMBCooldownMax;
    [SerializeField] private GameObject RMBPerlinNoise;
    [SerializeField] private GameObject RMBCooldownNumber;
    [SerializeField] private GameObject RMBWheelChargeUp;
    private float RMBCooldownCurr = 0f;

    private PlayerController playerController;
    public VisualEffect vfxGraph;
    private VisualEffect activeEffectInstance;


    // Start is called before the first frame update
    void Awake()
    {
        playerController = new PlayerController();
    }

    void AdjustManaUI()
    {
        manaBar.GetComponent<Image>().fillAmount = currentMana / maxMana;
        manaBarNumber.GetComponent<TextMeshProUGUI>().text = currentMana.ToString("F0");
    }

    private void Update()
    {
        ChargeMana();
        AdjustManaUI();

        HandleLMBUI();
        HandleLMBSpell();

        HandleRMBUI();
        HandleRMBSpell();
        HandleSpellRotation();
    }

    private void ChargeMana()
    {
        if (PlayerController.Instance.currentState == PlayerController.PlayerState.ChannelWalk ||
            PlayerController.Instance.currentState == PlayerController.PlayerState.ChannelStand)
            return;

        if (currentMana < maxMana)
        {
            currentMana += Time.deltaTime * manaRechargeRate;
        }
    }

    private bool HandleLMBSpell()
    {
        if (PlayerController.Instance.currentState != PlayerController.PlayerState.Idle &&
            PlayerController.Instance.currentState != PlayerController.PlayerState.Running &&
            PlayerController.Instance.currentState != PlayerController.PlayerState.Jumping)
            return false;

        bool hasEnoughMana = currentMana - spellToCast.SpellToCast.ManaCost >= 0f;

        if (Input.GetKeyDown(KeyCode.Mouse0) && hasEnoughMana && LMBCooldownCurr < 0)
        {
            currentCastedAbility = "LMB";
            currentMana -= spellToCast.SpellToCast.ManaCost;
            castSpell();
            currentCastedAbility = "";
            LMBCooldownCurr = LMBCooldownMax;
        }

        return true;
    }


    private void ReturnState()
    {
        PlayerController.Instance.currentState = PlayerController.PlayerState.Idle;
    }

    private bool HandleRMBSpell()
    {
        if ((PlayerController.Instance.currentState == PlayerController.PlayerState.ChannelWalk && currentCastedAbility != "RMB") ||
             PlayerController.Instance.currentState == PlayerController.PlayerState.ChannelStand || RMBCooldownCurr > 0)
            return false;

        float manaDrainPerSecond = 3f;


        if (Input.GetKey(KeyCode.Mouse1))
        {
            PlayerController.Instance.channelWalkingMultiplier = 0.3f;
            currentCastedAbility = "RMB";
            PlayerController.Instance.currentState = PlayerController.PlayerState.ChannelWalk;
            currentMana -= Time.deltaTime * manaDrainPerSecond;
            currChargeTime += Time.deltaTime;

            // to instantiate charge spell animation
            HandleEffectInstantiating();

            if (currChargeTime > maxChargeTime)
            {
                gameObject.GetComponent<HealthComponent>().TakeDamage(50f);
                PlayerController.Instance.currentState = PlayerController.PlayerState.Idle;
                RMBCooldownCurr = RMBCooldownMax;
                currentCastedAbility = "";
                currChargeTime = 0f;
                currentMana += manaRechargeRate * 10f;

                // to destroy the animation instance
                HandleEffectRemoval();
            }
        }
        else
        {
            if (currChargeTime > 0)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, currChargeTime * 15f);

                // to instantiate charge spell animation
                HandleEffectInstantiating();
                foreach (var col in hitColliders)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        col.GetComponent<HealthComponent>().TakeDamage(currChargeTime * 20f);
                }
                PlayerController.Instance.currentState = PlayerController.PlayerState.Idle;
                RMBCooldownCurr = RMBCooldownMax;
                currentCastedAbility = "";

                // to destroy the animation instance
                HandleEffectRemoval();
                
            }
            currChargeTime = 0f;
        }
        return true;
    }

    void castSpell()
    {
        Instantiate(spellToCast, castPoint.position, castPoint.rotation);
    }

    // --------------------------------------- UI ---------------------------------------

    private void HandleLMBUI()
    {
        if (currentCastedAbility != "LMB")
            LMBCooldownCurr -= Time.deltaTime;

        if (LMBCooldownCurr > 0)
        {
            LMBCooldownNumber.SetActive(true);
            LMBCooldownNumber.GetComponent<TextMeshProUGUI>().text = LMBCooldownCurr.ToString("F1");
        }
        else
            LMBCooldownNumber.SetActive(false);

        LMBPerlinNoise.GetComponent<Image>().fillAmount = LMBCooldownCurr / LMBCooldownMax;
    }


    private void HandleRMBUI()
    {
        if (currentCastedAbility != "RMB")
            RMBCooldownCurr -= Time.deltaTime;

        RMBWheelChargeUp.GetComponent<Image>().fillAmount = currChargeTime / maxChargeTime;

        if (RMBCooldownCurr > 0)
        {
            RMBCooldownNumber.SetActive(true);
            RMBCooldownNumber.GetComponent<TextMeshProUGUI>().text = RMBCooldownCurr.ToString("F1");
        }
        else
            RMBCooldownNumber.SetActive(false);

        RMBPerlinNoise.GetComponent<Image>().fillAmount = RMBCooldownCurr / RMBCooldownMax;
    }


    // --------------------------------------- Animations ---------------------------------------

    void UpdateEffectSize()
    {
        float normalizedCharge = Mathf.Clamp01(currChargeTime/maxChargeTime);
        //float size = Mathf.Lerp(0.1f, 35f, normalizedCharge);
        float size = 30f*currChargeTime;
        vfxGraph.SetFloat("EffectSize", size);   
        vfxGraph.SetFloat("LifeTime", currChargeTime); 

    }

    void HandleSpellRotation()
    {
        if(activeEffectInstance !=null)
        {
            activeEffectInstance.transform.rotation = Quaternion.identity;
        }
    }

    // --------------------------------------- Misc ---------------------------------------

    void HandleEffectInstantiating()
    {
        if(activeEffectInstance == null)
            {
                activeEffectInstance = Instantiate(vfxGraph, castPoint.position, quaternion.identity, castPoint);
                activeEffectInstance.transform.localRotation = Quaternion.identity;
            }
            activeEffectInstance.SetFloat("EffectSize", 5f);
            UpdateEffectSize();
    }
    void HandleEffectRemoval()
    {
        if(activeEffectInstance!=null)
            {
                Destroy(activeEffectInstance.gameObject);
                activeEffectInstance = null;
            }
    }
}
