// -----------------------------------------------------------------------------
// NPCView.cs
//
// MonoBehaviour for handling the in-game visual representation and behavior of NPCs.
// Manages NPC movement, scaling, rotation, and dialogue presentation during interactions.
// Integrates with the Tale utility for dialogue and prop manipulation.
//
// Main Functions:
// - Initialize(NPCInteraction, Vector3, Vector3): Sets up the NPC for interaction.
// - BeginInteraction(): Starts the NPC's approach and dialogue sequence.
// - OnChoicePicked(): Handles NPC's reaction and exit after a player choice.
// - MoveTo(), Scale(), Rotate(): Animate NPC movement and appearance.
//
// Fields:
// - _interaction: The current NPCInteraction.
// - _dialogue: Dialogue lines for the interaction.
// - _spawnPoint, _arrivalPoint: Positions for NPC movement.
// - _animator: Animator component for NPC animations.
// -----------------------------------------------------------------------------

using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
public class NPCView : MonoBehaviour
{
    [NonSerialized]
    const float MoveSpeed = 3f;

    public NPCInteraction Interaction => _interaction;
    private NPCInteraction _interaction;
    private List<string> _dialogue;
    private Vector3 _spawnPoint;
    private Vector3 _arrivalPoint;

    Vector3 _initialScale;

    private Animator _animator;

    public void Initialize(NPCInteraction interaction, Vector3 spawnPoint, Vector3 arrivalPoint)
    {
        _interaction = interaction;
        _dialogue = interaction.Dialogue;
        _spawnPoint = spawnPoint;
        _arrivalPoint = arrivalPoint;

        transform.position = _spawnPoint;
        _initialScale = transform.localScale;

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("NPC has no Animator component; either this NPC is a placeholder, or this is a bug");
        }

        transform.localScale = Vector3.zero;
    }

    public async void BeginInteraction()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Walk");
        }

        Scale(_initialScale, 0f);

        await MoveTo(_arrivalPoint);

        if (_animator != null)
        {
            _animator.SetTrigger("Idle");
        }

        await Task.Delay(250);

        foreach (var dialogue in _dialogue)
        {
            Tale.Dialog(_interaction.NPC.Name, dialogue, null, "loop", true);
        }

        Tale.Exec(() =>
        {
            EndInteraction();
        });
    }

    public async Task OnChoicePicked()
    {
        await Rotate();

        if (_animator != null)
        {
            _animator.SetTrigger("Walk");
        }

        Scale(Vector3.zero, 2f);

        await MoveTo(_spawnPoint);

        await Task.Delay(250);
    }

    private async Task MoveTo(Vector3 target)
    {
        var distance = Vector3.Distance(transform.position, target);
        var duration = distance / MoveSpeed;
        transform.DOMove(target, duration).SetEase(Ease.Linear);

        await Task.Delay((int)(duration * 1000));
    }

    async void Scale(Vector3 scale, float delay)
    {
        await Task.Delay((int)(delay * 1000));

        var duration = 0.5f;

        transform.DOScale(scale, duration);

        await Task.Delay((int)(duration * 1000));
    }

    async Task Rotate()
    {
        var duration = 0.5f;

        transform.DORotate(new Vector3(0f, transform.eulerAngles.y - 180f, 0f), duration);

        await Task.Delay((int)(duration * 1000));
    }

    private async Task EndInteraction()
    {
        BeaureauManager.Instance.OnInteractionEnded();
    }
}
