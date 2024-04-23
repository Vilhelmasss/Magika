using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagicSystem : MonoBehaviour
{
    [SerializeField] private Spell spellToCast;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private float manaRechargeRate = 2f;
    [SerializeField] private float timeWaitForRecharge = 1f;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private GameObject manaBarNumber;
    [SerializeField] private float timeBetweenCast = 0.25f;
    private float currentManaRechargeTimer;
    private float currentCastTimer;
    private float castingCounter;
    private string currentCastedAbility = "";
    public TextMeshProUGUI manaTextMesh;
    [SerializeField] private Transform castPoint;

    [Header("RMB")]
    private float RMBCooldownCurr = 0f;
    [SerializeField] private float RMBCooldownMax;
    [SerializeField] private GameObject RMBPerlinNoise;
    [SerializeField] private GameObject RMBCooldownNumber;
    [SerializeField] private GameObject RMBWheelChargeUp;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float currChargeTime;
    private PlayerController playerController;
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
        HandleLMBSpell();
        HandleRMBUI();
        HandleRMBSpell();
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && hasEnoughMana)
        {
            currentCastedAbility = "LMB";
            currentMana -= spellToCast.SpellToCast.ManaCost;
            currentCastTimer = 0;
            currentManaRechargeTimer = 0;
            castSpell();
            currentCastedAbility = "";
        }

        return true;
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

            if (currChargeTime > maxChargeTime)
            {
                gameObject.GetComponent<HealthComponent>().TakeDamage(50f);
                PlayerController.Instance.currentState = PlayerController.PlayerState.Idle;
                RMBCooldownCurr = RMBCooldownMax;
                currentCastedAbility = "";
                currChargeTime = 0f;
                currentMana += manaRechargeRate * 10f;
            }
        }
        else
        {
            if (currChargeTime > 0)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, currChargeTime * 15f);
                foreach (var col in hitColliders)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        col.GetComponent<HealthComponent>().TakeDamage(currChargeTime * 20f);
                }
                PlayerController.Instance.currentState = PlayerController.PlayerState.Idle;
                RMBCooldownCurr = RMBCooldownMax;
                currentCastedAbility = "";
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


}
