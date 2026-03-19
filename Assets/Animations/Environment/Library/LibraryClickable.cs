using UnityEngine;
using UnityEngine.EventSystems;

public class LibraryClickable : MonoBehaviour
{
    protected bool IsClickable => !EventSystem.current.IsPointerOverGameObject();

    private Outline _outline;
    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    public void OnMouseEnter()
    {
        if (!IsClickable)
            return;
            
        _outline.enabled = true;
    }

    public void OnMouseExit()
    {
        _outline.enabled = false;
    }

    public virtual void OnMouseDown()
    {
        if (!IsClickable)
            return;

        _outline.enabled = false;
    }
}
