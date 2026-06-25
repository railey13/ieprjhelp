using UnityEngine;
using UnityEngine.InputSystem;

public class HoverHighlight : MonoBehaviour
{
    [Header("Highlight Objects (can be anything)")]
    [SerializeField] private GameObject[] highlightObjects;

    private bool isHovered;
    void Start()
    {
        SetHighlight(false); 
    }
    void Update()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // checks if the ray hit THIS object or its children
            bool hitThisObject = hit.transform.IsChildOf(transform);

            if (hitThisObject != isHovered)
            {
                isHovered = hitThisObject;
                SetHighlight(isHovered);
            }
        }
        else
        {
            if (isHovered)
            {
                isHovered = false;
                SetHighlight(false);
            }
        }
    }

    public void SetHighlight(bool state)
    {
        if (highlightObjects == null) return;

        foreach (GameObject obj in highlightObjects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}