using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ClickableText : MonoBehaviour
{
    [SerializeField] private string _link;
    public string Link => _link;

    private TextMeshProUGUI _text;
    private Button _button;

    // Start is called before the first frame update
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _button = gameObject.AddComponent<Button>();
        _button.onClick.AddListener(OpenLink);
        _button.image = null;
        _button.transition = Selectable.Transition.None;
    }

    private void OpenLink()
    {
        Application.OpenURL(_link);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OpenLink);
    }
}
