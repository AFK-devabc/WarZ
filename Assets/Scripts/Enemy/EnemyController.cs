using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public EnemyMovement movement;
    [SerializeField] public AttackBehavior attackBehavior;
    [SerializeField] public HealthBehavior healthBehavior;
    private NavMeshAgent navMeshAgent;
    [HideInInspector] public UnityEvent<Transform> SetTargetEvent;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetTargetEvent = new UnityEvent<Transform>();
        healthBehavior.TakeDamageEvent.AddListener(SetTargetWhenGetHit);
        GameManager.Instance.AddEnemy();
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;


    }
    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
        movement.enabled = enabled;
        attackBehavior.enabled = enabled;
        healthBehavior.enabled = enabled;
        navMeshAgent.enabled = enabled;
    }

    private void OnDestroy()
    {
        GameManager.Instance?.RemoveEnemy();
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;

    }
    private void SetTargetWhenGetHit()
    {
        SetTargetEvent.Invoke(GameObject.Find("Player").transform);
        healthBehavior.TakeDamageEvent.RemoveListener(SetTargetWhenGetHit);
    }

    private void Start()
    {
        if (attackBehavior)
        {
            SetTargetEvent.AddListener(attackBehavior.SetTarget);
            healthBehavior.DeadEvent.AddListener(attackBehavior.DisableBehavior);
        }
        if (movement)
        {
            SetTargetEvent.AddListener(movement.SetTarget);
            healthBehavior.DeadEvent.AddListener(movement.DisableBehavior);

        }
    }
}
