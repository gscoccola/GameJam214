using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private int _targetLevelNumber;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            LevelLoader.Instance.LoadLevel("Level" + _targetLevelNumber);
        }
    }
}
