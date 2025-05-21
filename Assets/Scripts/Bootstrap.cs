using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        Transition.SweepOut(0f);
        Tale.Scene();
    }
}
