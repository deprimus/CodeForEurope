using UnityEngine;

public class LibraryLaptop : LibraryClickable
{
    public override void OnMouseDown()
    {
        base.OnMouseDown();

        LibraryManager.Instance.UseLaptop();
    }

    public void CloseLaptopUI()
    {
        EuroChat.Instance.Hide();
        LibraryManager.Instance.OnLaptopClosed();
    }
}
