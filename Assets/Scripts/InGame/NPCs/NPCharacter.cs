using UnityEngine;

public class NPCharacter : MonoBehaviour
{
    [SerializeField] private string _characterName;

    [SerializeField] private Sprite _frontSprite;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;

    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float xDiff = PlayerController.Instance.transform.position.x - transform.position.x;

        if (xDiff < -1f) 
        {
            _spriteRenderer.sprite = _leftSprite;
        }
        else if (xDiff > 1f)
        {
            _spriteRenderer.sprite = _rightSprite;
        }
        else
        {
            _spriteRenderer.sprite = _frontSprite;
        }
    }
}
