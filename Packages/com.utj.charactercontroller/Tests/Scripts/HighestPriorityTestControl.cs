using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;
using UnityEngine.Events;

public class HighestPriorityTestControl : MonoBehaviour, 
        IMove,
        ITurn,
        IPriorityLifecycle<ITurn>,
        IPriorityLifecycle<IMove>
{
    private IBrain _brain;
    
    public int TurnPriority = 0;
    public int MovePriority = 0;

    public bool HasMoveHighestPriority = false;
    public bool HasTurnHighestPriority = false;

    public int TurnUpdateCount = 0;
    public int MoveUpdateCount = 0;

    public UnityEvent<ITurn> OnAcquireHighestTurnPriority = new();
    public UnityEvent OnLoseHighestTurnPriority = new();
    public UnityEvent<IMove> OnAcquireHighestMovePriority = new();
    public UnityEvent OnLoseHighestMovePriority = new();
    
    int IPriority<ITurn>.Priority => TurnPriority;

    int IPriority<IMove>.Priority => MovePriority;

    private void Awake()
    {
        TryGetComponent(out _brain);
    }

    void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
    {
        HasMoveHighestPriority = true;
        OnAcquireHighestMovePriority.Invoke(_brain.CurrentMove);
    }

    void IPriorityLifecycle<IMove>.OnLoseHighestPriority()
    {
        HasMoveHighestPriority = false;
        OnLoseHighestMovePriority.Invoke();
    }
    void IPriorityLifecycle<ITurn>.OnAcquireHighestPriority()
    {
        HasTurnHighestPriority = true;
        OnAcquireHighestTurnPriority.Invoke(_brain.CurrentTurn);
    }

    void IPriorityLifecycle<ITurn>.OnLoseHighestPriority()
    {
        HasTurnHighestPriority = false;
        OnLoseHighestTurnPriority.Invoke();
    }

    void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
    {
        MoveUpdateCount += 1;
    }

    void IPriorityLifecycle<ITurn>.OnUpdateWithHighestPriority(float deltaTime)
    {
        TurnUpdateCount += 1;
    }

    Vector3 IMove.MoveVelocity => Vector3.right;

    int ITurn.TurnSpeed => -1;

    float ITurn.YawAngle => 90;
}
