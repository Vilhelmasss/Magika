using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMagicSystem : MonoBehaviour
{
    [SerializeField] private Spell spellToCast;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private float manaRechargeRate = 2f;
    [SerializeField] private float timeWaitForRecharge = 1f;
    private float currentManaRechargeTimer;
    [SerializeField] private float timeBetweenCast = 0.25f;
    private float currentCastTimer;
    private float castingCounter;
    public TextMeshProUGUI manaTextMesh;
    public TextMeshProUGUI chargeUpState;
    public TextMeshProUGUI lastCharge;
    [SerializeField] private float maxChargeTimeLab;
    [SerializeField] private float currChargeTimeLab;

    [SerializeField] private Transform castPoint;

    private bool castingMagic = false;
    private PlayerController playerController;
    // Start is called before the first frame update
    void Awake()
    {
        playerController = new PlayerController();
    }

    // Update is called once per frame
    private void OnEnable()
    {
        // playerController.Enable();

    }

    void AdjustManaUI()
    {
        manaTextMesh.text = currentMana.ToString("F1") + " " + maxMana.ToString();
    }

    private void Update()
    {
        AdjustManaUI();
        HandleChargingSpell();
        bool isSpellCastHeldDown = Input.GetMouseButtonDown(0);
        bool hasEnoughMana = currentMana - spellToCast.SpellToCast.ManaCost >= 0f;

        // if(!castingMagic && isSpellCastHeldDown && hasEnoughMana)
        // {
        //     castingMagic = true;
        //     currentMana -= spellToCast.SpellToCast.ManaCost;
        //     currentCastTimer=0;
        //     currentManaRechargeTimer=0;
        //     castSpell();
        //     print("casting");
        // }

        if (!castingMagic && isSpellCastHeldDown && hasEnoughMana)
        {
            castingCounter = 0;
            castingCounter += Time.deltaTime;

            if (castingCounter < 30f)
            {
                castingMagic = true;
                currentMana -= spellToCast.SpellToCast.ManaCost;
                currentCastTimer = 0;
                currentManaRechargeTimer = 0;
                castSpell();
                print("casting");
            }
            else
            {
                Debug.Log("Casting failed");
                currentMana -= spellToCast.SpellToCast.ManaCost;
                currentCastTimer = 0;
                currentManaRechargeTimer = 0;

            }

        }

        if (castingMagic)
        {
            currentCastTimer += Time.deltaTime;
            if (currentCastTimer > timeBetweenCast)
            {
                castingMagic = false;
            }
        }
        if (currentMana < maxMana && !castingMagic && !isSpellCastHeldDown)
        {
            currentManaRechargeTimer += Time.deltaTime;
            if (currentManaRechargeTimer > timeWaitForRecharge)
            {
                currentMana += manaRechargeRate * Time.deltaTime;
                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }
            }

        }

    }
    private bool HandleChargingSpell()
    {
        Debug.Log("Got here");
        chargeUpState.text = currChargeTimeLab.ToString("F1");
        if (Input.GetKey(KeyCode.Mouse1))
        {
            currentMana -= Time.deltaTime * 3f;
            Debug.Log("Lul");
            currChargeTimeLab += Time.deltaTime;
            if (currChargeTimeLab > maxChargeTimeLab)
            {
                lastCharge.text = $"Overtime: {maxChargeTimeLab.ToString("F1")}";
                gameObject.GetComponent<HealthComponent>().TakeDamage(50f);
                currChargeTimeLab = 0f;
            }
            else
            {
            }
        }
        else
        {
            if (currChargeTimeLab > 0)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, currChargeTimeLab * 15f);
                foreach (var col in hitColliders)
                {
                    if (col.gameObject.layer == 6)
                    {
                        Debug.Log("Here");
                        col.GetComponent<HealthComponent>().TakeDamage(currChargeTimeLab * 20f);
                    }
                }
                lastCharge.text = $"Successful: {currChargeTimeLab.ToString("F1")}";
            }
            currChargeTimeLab = 0f;
        }
        return false;
    }


    void castSpell()
    {
        Instantiate(spellToCast, castPoint.position, castPoint.rotation);
    }
}
