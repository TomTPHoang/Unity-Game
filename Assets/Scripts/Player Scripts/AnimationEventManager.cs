using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    private Inventory inventory;
    private EquipmentManager manager;
    private WeaponShooting shooting;

    private void Start()
    {
        GetReferences();
    }

    public void DestroyWeapon()
    {
        Destroy(manager.currentWeaponObject);
    }

    public void InstantiateWeapon()
    {
        manager.currentWeaponObject = Instantiate(inventory.GetItem(manager.currentlyEquippedWeapon).prefab, manager.WeaponHolderR);
        manager.currentWeaponBarrel = manager.currentWeaponObject.transform.GetChild(0);
        manager.currentWeaponAnim = manager.currentWeaponObject.GetComponent<Animator>();
    }

    public void StartReload()
    {
        shooting.canReload = false;
    }

    public void EndReload()
    {
        shooting.canReload = true;
    }

    private void GetReferences()
    {
        inventory = GetComponentInParent<Inventory>();
        manager = GetComponentInParent<EquipmentManager>();
        shooting = GetComponentInParent<WeaponShooting>();
    }
}
