using UnityEngine;

public static class Transition
{
    public static TaleUtil.Action RipOut(float duration = 0.7f) =>
        Tale.Transition("rip", Tale.TransitionType.OUT, duration);

    public static TaleUtil.Action RipIn(float duration = 0.7f) =>
    Tale.Transition("rip", Tale.TransitionType.IN, duration);
}
