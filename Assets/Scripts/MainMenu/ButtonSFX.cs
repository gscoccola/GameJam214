using System;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private bool _playSound = true;
    [SerializeField] private float _pitch = 1f;
    private void Awake()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (_playSound)
        {
            SFXManager.Instance.PlaySFX(SFXManager.Instance.UIClick, true, _pitch);
        }
    }
}
