using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public int currentlyEquippedWeapon = 2;
    public GameObject currentWeaponObject = null;
    public Transform currentWeaponBarrel = null;

    public Transform WeaponHolderR = null;
    private Animator anim;
    public Animator currentWeaponAnim;
    private Inventory inventory;
    private PlayerHUD hud;

    [SerializeField] Weapon defaultMeleeWeapon = null;
    

    private void Start()
    {
        GetReferences();
        InitVariables();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && currentlyEquippedWeapon != 0)
        {
            UnequipWeapon();
            EquipWeapon(inventory.GetItem(0));
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && currentlyEquippedWeapon != 1)
        {
            UnequipWeapon();
            EquipWeapon(inventory.GetItem(1));
        }

        if(Input.GetKeyDown(KeyCode.Alpha3) && currentlyEquippedWeapon != 2)
        {
            UnequipWeapon();
            EquipWeapon(inventory.GetItem(2));
        }
    }

    private void EquipWeapon(Weapon weapon)
    {
        currentlyEquippedWeapon = (int)weapon.weaponStyle;
        anim.SetInteger("weaponType", (int)weapon.weaponType);
        hud.UpdateWeaponUI(weapon);
    }

    private void UnequipWeapon()
    {
        anim.SetTrigger("unequipWeapon");
    }

    private void InitVariables()
    {
        inventory.AddItem(defaultMeleeWeapon);
        EquipWeapon(inventory.GetItem(2));
    }

    private void GetReferences()
    {
        anim = GetComponentInChildren<Animator>();
        inventory = GetComponent<Inventory>();
        hud = GetComponent<PlayerHUD>();
    }

}
