using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{    public static PlayerHUD instance = null;

    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private WeaponUI weaponUI;
    [SerializeField] private WaveCounterUI waveNumberUI;
    [SerializeField] private WaveTitleUI waveTitleUI;
    [SerializeField] private Score scoreUI;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthBar.SetValue(currentHealth, maxHealth);
    }

    public void UpdateWeaponUI(Weapon newWeapon)
    {
        weaponUI.UpdateInfo(newWeapon.icon, newWeapon.magazineSize, newWeapon.storedAmmo);
    }

    public void UpdateWeaponAmmoUI(int currentAmmo, int storedAmmo)
    {
        weaponUI.UpdateAmmoUI(currentAmmo, storedAmmo);
    }

    public void UpdateWaveNumberUIWaveStart()
    {
        waveNumberUI.WaveStartWaveNumberUISequence();
    }

    public void UpdateWaveNumberUIWaveEnd()
    {
        waveNumberUI.WaveEndWaveNumberUISequence();
    }

    public void UpdateWaveTitleUI()
    {
        waveTitleUI.WaveStartWaveTitleUISequence();
    }

    public void UpdateScoreAmount()
    {
        scoreUI.AddToScore();
    }
}
