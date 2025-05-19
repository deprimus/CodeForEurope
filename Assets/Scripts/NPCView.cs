using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
public class NPCView : MonoBehaviour
{
    [HideInInspector]
    public float MoveSpeed = 1f; 

    public NPCInteraction Interaction => _interaction;
    private NPCInteraction _interaction;
    private List<string> _dialogue;
    private Vector3 _spawnPoint;
    private Vector3 _arrivalPoint;

    public void Initialize(NPCInteraction interaction, Vector3 spawnPoint, Vector3 arrivalPoint)
    {
        _interaction = interaction;
        _dialogue = interaction.Dialogue;
        _spawnPoint = spawnPoint;
        _arrivalPoint = arrivalPoint;

        transform.position = _spawnPoint;
    }

    public async void BeginInteraction()
    {
        await MoveTo(_arrivalPoint);

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
        await MoveTo(_spawnPoint);

        await Task.Delay(250);
    }

    private async Task MoveTo(Vector3 target)
    {
        var distance = Vector3.Distance(transform.position, target);
        var duration = distance / MoveSpeed;
        transform.DOMove(target, duration);

        await Task.Delay((int)(duration * 1000));
    }

    private async Task EndInteraction()
    {
        BeaureauManager.Instance.OnInteractionEnded();
    }
}
