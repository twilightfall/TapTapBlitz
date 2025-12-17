using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class Button : MonoBehaviour
{
    [SerializeField]
    ButtonPrompt colorPrompt;

    private void Start()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += OnTap;
    }

    public void SetColor(ButtonPrompt color)
    {
        colorPrompt = color;
    }

    public void OnTap(Finger finger)
    {
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    if (GameManager.instance.DidScore(colorPrompt))
                    {
                        GameManager.instance.SetPrompt();
                        GridManager.instance.ShuffleGrid();
                    }
                    else return;
                }
            }
        }
    }
}
