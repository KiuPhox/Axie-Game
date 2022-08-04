﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityMovementAI;


public class Enemy : LivingEntity
{
    public Vector2 randomSpeed;
    public Vector2 randomWanderTime;
    public float detectRange;

    [HideInInspector] public float stunTime;
    float attackCooldownTime;
    float nextAttackTime;

    LivingEntity targetEntity;
    GameObject closestChampion;
    bool isStunned = false;
    float wanderTime;

    Vector3 desiredDirection;

    SteeringBasics steeringBasics;
    Wander1 wander;
    Separation separation;
    Vector3 accel = new Vector3(0, 0, 0);

    List<MovementAIRigidbody> otherEnemies;

    public float separationWeight;

    public override void Start()
    {
        base.Start();
        wanderTime = Random.Range(randomWanderTime.x, randomWanderTime.y);

        attackCooldownTime = cooldownTime;
        playerChampions.AddBlobShadowForChampion(this.gameObject);

        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander1>();
        separation = GetComponent<Separation>();
    }

    private void Update()
    {
        closestChampion = GetClosestTargetInList(playerChampions.champions);
        if (closestChampion != null && !isStunned)
        {
            FlipBaseOnTargetPos(closestChampion.transform.position);
        }
        if (stunTime > 0)
        {
            isStunned = true;
            stunTime -= Time.deltaTime;
        }
        else
        {
            isStunned = false;
        }
    }

    private void FixedUpdate()
    {
        otherEnemies = new List<MovementAIRigidbody>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (e != this.gameObject)
            {
                otherEnemies.Add(e.GetComponent<MovementAIRigidbody>());
            }
        }

        if (!isStunned)
        {
            // Basic Movement
            accel = wander.GetSteering();
            accel += separation.GetSteering(otherEnemies) * separationWeight;
            if (closestChampion != null && !isStunned)
            {
                accel += steeringBasics.Arrive(closestChampion.transform.position);
            }
        }

        else if (isStunned)
        {
            // Stun
            accel = steeringBasics.Arrive(transform.position);
        }
        steeringBasics.Steer(accel);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Champion") && !isStunned)
        {
            if (Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldownTime;
                targetEntity = collision.gameObject.GetComponent<LivingEntity>();
                targetEntity.TakeDamage(championData.damage, GetComponent<LivingEntity>());
            }
        }
    }
}