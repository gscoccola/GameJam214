using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    //[SerializeField] private bool _isArea;
    [SerializeField] private int _targetAreaNumber;
    [SerializeField] private int _targetSpawnNumber;
    //[SerializeField] private int _targetLevelNumber;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            LevelLoader.Instance.LoadArea(_targetAreaNumber, _targetSpawnNumber);
        }
    }
}
