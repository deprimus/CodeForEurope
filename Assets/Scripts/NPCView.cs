using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
public class NPCView : MonoBehaviour
{
    [NonSerialized]
    const float MoveSpeed = 3f;

    [SerializeField]
    SkinnedMeshRenderer _meshRenderer;

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


        if (_meshRenderer != null)
        {
            _meshRenderer.material.color = Color.black;
        }
        else
        {
            Debug.LogError("NPC has no MeshRenderer assigned; either this NPC is a placeholder, or this is a bug");
        }

        transform.localScale = Vector3.zero;
    }

    public async void BeginInteraction()
    {
        if (_animator != null)
        {
            _animator.Play("Walk");
        }

        if (_meshRenderer != null)
        {
            _meshRenderer.material.DOColor(Color.white, 2f);
        }

        Scale(_initialScale, 0f);

        await MoveTo(_arrivalPoint);

        if (_animator != null)
        {
            _animator.Play("Idle");
        }

        await Task.Delay(250);

        foreach (var dialogue in _dialogue)
        {
            Tale.Dialog(_interaction.NPC.Name, dialogue);
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
            _animator.Play("Walk");
        }

        if (_meshRenderer != null)
        {
            _meshRenderer.material.DOColor(Color.black, 2f);
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
