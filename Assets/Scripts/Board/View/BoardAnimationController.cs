using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnimationController : MonoBehaviour
{
    private Queue<IEnumerator> pendingSequences = new Queue<IEnumerator>();
    private Coroutine sequenceCO;

    public event Action SequenceStarted = delegate { };
    public event Action SequenceEnded = delegate { };

    public void QueueSequence(IEnumerator sequence)
    {
        pendingSequences.Enqueue(sequence);
        if (sequenceCO == null)
        {
            sequenceCO = StartCoroutine(Sequence());
        }
    }

    private IEnumerator Sequence()
    {
        SequenceStarted?.Invoke();
        while (pendingSequences.TryDequeue(out var next))
        {
            name = $"Animation Queue - Running - {pendingSequences.Count} pending";
            yield return next;
        }
        name = $"Animation Queue - Idle";
        sequenceCO = null;
        SequenceEnded?.Invoke();
    }
}
