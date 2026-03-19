using UnityEngine;

public class LibraryView_Book : LibraryClickable
{
    public override void OnMouseDown()
    {
        if (!IsClickable)
            return;

        base.OnMouseDown();

        LibraryManager.Instance.UseBook();
    }

    private async void UseBook()
    {
        LibraryManager.Instance.UseBook();
    }
}
