using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<string> descriptions = new();

    [Header("UI")]
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Keys")]
    [SerializeField] private KeyCode nextKey = KeyCode.Return;
    [SerializeField] private KeyCode prevKey = KeyCode.Backspace;

    [Header("Options")]
    [SerializeField] private bool loop = false;

    private int _index;

    private void Start()
    {
        Apply();
    }

    private void Update()
    {
        if (sprites == null || sprites.Count == 0) return;

        if (Input.GetKeyDown(nextKey))
        {
            int next = _index + 1;
            if (next >= sprites.Count)
            {
                if (!loop) return;
                next = 0;
            }
            _index = next;
            Apply();
        }
        else if (Input.GetKeyDown(prevKey))
        {
            int prev = _index - 1;
            if (prev < 0)
            {
                if (!loop) return;
                prev = sprites.Count - 1;
            }
            _index = prev;
            Apply();
        }
    }

    private void Apply()
    {
        if (tutorialImage != null)
        {
            tutorialImage.enabled = sprites != null && sprites.Count > 0;
            if (tutorialImage.enabled)
                tutorialImage.sprite = sprites[_index];
        }

        if (tutorialText != null)
        {
            if (descriptions != null && _index < descriptions.Count)
                tutorialText.text = descriptions[_index];
            else
                tutorialText.text = "";
        }
    }
}
