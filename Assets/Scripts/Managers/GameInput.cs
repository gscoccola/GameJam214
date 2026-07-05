using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static InputActions Actions { get; private set; }

    private void Awake()
    {
        if (Actions == null)
        {
            Actions = new InputActions();
            Actions.Enable();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}