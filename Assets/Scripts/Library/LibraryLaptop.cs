using UnityEngine;

public class LibraryLaptop : MonoBehaviour
{
    public void OnMouseDown()
    {
        LibraryManager.Instance.UseLaptop();
    }

    public void CloseLaptopUI()
    {
        EuroChat.Instance.Hide();
        LibraryManager.Instance.OnLaptopClosed();
    }
}
