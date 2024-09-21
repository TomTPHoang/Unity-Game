using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    //VARIABLES
    //[SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 7f;
    [SerializeField] private float waveCountdown = 0;
    private SpawnState state = SpawnState.COUNTING;
    private int currentWave = 1;

    //REFERENCES
    [SerializeField] private Transform[] spawners;
    [SerializeField] private List<CharacterStats> zombieList;
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private AudioClip waveStartSoundClip;
    [SerializeField] private AudioClip waveEndSoundClip;

    private void Start()
    {
        waveCountdown = 2f;
    }

    private void Update()
    {
        if(state == SpawnState.WAITING)
        {
            //check if all zombies are dead
            if(ZombiesAreDead())
            {
                PlayerHUD.instance.UpdateWaveNumberUIWaveEnd();
                SoundFXManager2D.instance.PlaySoundFXClip2D(waveEndSoundClip, transform, 0.05f);
                StartNextWave();
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
            if(waveCountdown <= 0f)
            {
                if(state != SpawnState.SPAWNING)
                {
                    StartCoroutine(SpawnWave());
                }
            }
        }
    }

    private Wave GenerateWave()
    {
        Wave wave = new Wave();
        wave.zombieAmount = Random.Range(1, 5);
        //wave.zombie = new /* Randomly choose a zombie type */;
        wave.delay = Random.Range(1f, 3f);
        return wave;
    }

    private IEnumerator SpawnWave()
    {
        PlayerHUD.instance.UpdateWaveNumberUIWaveStart();
        SoundFXManager2D.instance.PlaySoundFXClip2D(waveStartSoundClip, transform, 0.05f);

        //only runs when the level starts for the first time
        if (currentWave <= 1)
        {
            PlayerHUD.instance.UpdateWaveTitleUI();
        }


        state = SpawnState.SPAWNING;

        Wave newWave = GenerateWave();
        for(int i = 0; i < newWave.zombieAmount; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(newWave.delay);
        }

        state = SpawnState.WAITING;
    }

    private void SpawnZombie()
    {
        Transform randomSpawner = spawners[Random.Range(0, spawners.Length)];

        GameObject newZombie = Instantiate(zombiePrefab, randomSpawner.position, randomSpawner.rotation);
        CharacterStats newZombieStats = newZombie.GetComponent<CharacterStats>();

        zombieList.Add(newZombieStats);
    }

    private bool ZombiesAreDead()
    {
        foreach(CharacterStats zombie in zombieList)
        {
            if(!zombie.IsDead())
            {
                return false;
            }
        }
        return true;
    }

    private void StartNextWave()
    {
        Debug.Log("Wave " + currentWave + " Completed!");
        currentWave++;
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;
    }
}
