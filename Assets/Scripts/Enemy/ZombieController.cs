using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent = null;
    private Animator anim = null;
    private ZombieStats stats = null;
    private Transform target;
    [SerializeField] private float stoppingDistance = 2;
    [SerializeField] private float rotationThreshold = 2f;
    [SerializeField] private AudioClip[] zombieGruntSoundClips;
    [SerializeField] private AudioClip[] zombieAttackSoundClips;
    private float timeOfLastAttack = 0;
    private bool targetReachedPlayer = false;
    private float currentSpeed = 0f; // Track current speed
    private float smoothTime = 0.3f; // Smooth time for interpolation
    private float timeOfLastGrunt = 0f;
    private float minGruntInterval = 8f; // Minimum interval between each grunt sound
    private float maxGruntInterval = 70f; // Maximum interval between each grunt sound
    private int maxConcurrentGruntSounds = 15; // Maximum number of concurrent sounds
    private int concurrentGruntSounds = 0; // Current number of grunt sounds
    private System.Random random;

    private void Start()
    {
        GetReferences();
        timeOfLastGrunt = Time.time;

        // Generate a unique seed for this zombie instance based on its position and rotation
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        random = new System.Random(seed);
    }

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
        //anim.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);

        // Play grunt sound if enough time has passed since the last grunt
        if (Time.time >= timeOfLastGrunt + GetRandomGruntInterval())
        {
            SoundFXManager3D.instance.PlayRandomSoundFXClip3D(zombieGruntSoundClips, transform, 0.05f);
            timeOfLastGrunt = Time.time;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= rotationThreshold)
        {
            RotateToTarget();
            //anim.SetFloat("Speed", 0f);

            if (!targetReachedPlayer)
            {
                targetReachedPlayer = true;
                timeOfLastAttack = Time.time;
            }

            if (Time.time >= timeOfLastAttack + stats.attackSpeed)
            {
                timeOfLastAttack = Time.time;
                anim.SetTrigger("attack");
            }
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime / smoothTime);
        }
        else
        {
            if (targetReachedPlayer)
            {
                targetReachedPlayer = false;
            }
            currentSpeed = Mathf.Lerp(currentSpeed, 1f, Time.deltaTime / smoothTime);

            //// Play grunt sound if enough time has passed since the last grunt
            //if (Time.time >= timeOfLastGrunt + GetRandomGruntInterval())
            //{
            //    SoundFXManager.instance.PlayRandomSoundFXClip(zombieGruntSoundClips, transform, 0.1f);
            //    timeOfLastGrunt = Time.time;
            //}
        }
        anim.SetFloat("Speed", currentSpeed);
    }

    private float GetRandomGruntInterval()
    {
        return (float)random.NextDouble() * (maxGruntInterval - minGruntInterval) + minGruntInterval;
    }

    public void PlayAttackSound()
    {
        SoundFXManager3D.instance.PlayRandomSoundFXClip3D(zombieAttackSoundClips, transform, 0.05f);
    }

    public void DealDamageToPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) <= stoppingDistance)
        {
            stats.DealDamage(target.GetComponent<CharacterStats>());
        }
    }

    private void RotateToTarget()
    {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        transform.LookAt(targetPosition);
    }

    private void GetReferences()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stats = GetComponent<ZombieStats>();
        target = FirstPersonController.instance;
    }
}
