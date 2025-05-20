using UnityEngine;

public static class Transition
{
    public static TaleUtil.Action SweepOut(float duration = 0.7f) =>
        Tale.Transition("sweep", Tale.TransitionType.OUT, duration);

    public static TaleUtil.Action SweepIn(float duration = 0.7f) =>
    Tale.Transition("sweep", Tale.TransitionType.IN, duration);
}
