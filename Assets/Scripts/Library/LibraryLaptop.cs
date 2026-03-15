using UnityEngine;

public class LibraryLaptop : MonoBehaviour
{
    public void OnMouseDown()
    {
        LibraryManager.Instance.UseLaptop();
    }
}
