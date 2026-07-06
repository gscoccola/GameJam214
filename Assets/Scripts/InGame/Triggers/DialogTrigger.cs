using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private int _sceneIndex;
    [SerializeField] private bool _requiresInteract = true;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }



    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (_requiresInteract) PlayerInteract.Instance.CurrentDialogTrigger = this;
            else StartDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && PlayerInteract.Instance.CurrentDialogTrigger == this)
        {
            PlayerInteract.Instance.CurrentDialogTrigger = null;
        }
    }

    public void StartDialog()
    {
        TextManager.Instance.RenderScene(_sceneIndex);
    }
}