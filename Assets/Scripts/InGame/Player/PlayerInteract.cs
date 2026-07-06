using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : Singleton<PlayerInteract>
{
    [ReadOnly] public DialogTrigger CurrentDialogTrigger;
    private void Start()
    {
        GameInput.Actions.Platforming.Interact.performed += TryInteract;
    }

    private void OnDisable()
    {
        GameInput.Actions.Platforming.Interact.performed -= TryInteract;
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        if (CurrentDialogTrigger != null)
        {
            StartDialog();
        }
    }


    public void StartDialog()
    {
        CurrentDialogTrigger.StartDialog();
    }

}
