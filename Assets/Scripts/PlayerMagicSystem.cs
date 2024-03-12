using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private void Update()
    {
        bool isSpellCastHeldDown = Input.GetMouseButtonDown(0);
        bool hasEnoughMana = currentMana-spellToCast.SpellToCast.ManaCost >= 0f;

        if(!castingMagic && isSpellCastHeldDown && hasEnoughMana)
        {
            castingMagic = true;
            currentMana -= spellToCast.SpellToCast.ManaCost;
            currentCastTimer=0;
            currentManaRechargeTimer=0;
            castSpell();
            print("casting");
        }

        if(castingMagic)
        {
            currentCastTimer+=Time.deltaTime;
            if(currentCastTimer > timeBetweenCast)
            {
                castingMagic = false;
            }
        }
        if(currentMana < maxMana && !castingMagic && !isSpellCastHeldDown)
        {
            currentManaRechargeTimer += Time.deltaTime;
            if(currentManaRechargeTimer> timeWaitForRecharge)
            {
                currentMana+= manaRechargeRate*Time.deltaTime;
                if(currentMana>maxMana)
                {
                    currentMana=maxMana;
                }
            }

        }
            
    }

    void castSpell()
    {
        Instantiate(spellToCast, castPoint.position, castPoint.rotation);


    }


}
