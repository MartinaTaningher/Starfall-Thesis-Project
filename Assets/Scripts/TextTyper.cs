using TMPro;
using UnityEngine;

public class TextTyper : MonoBehaviour
{
    [SerializeField] private float charsPerSecond = 30f;

    private TMP_Text _dialogueText;
    private string _fullText;
    private float _timer;
    private int _visibleCount;
    private bool _isTyping = false;

    public bool IsTyping => _isTyping;

    //Calcola progressivamente i caratteri da mostrare in base al tempo trascorso
    private void Update()
    {
        if (!_isTyping) return;

        _timer += Time.deltaTime;
        int charsToShow = Mathf.FloorToInt(_timer * charsPerSecond);

        if (charsToShow != _visibleCount)
        {
            _visibleCount = charsToShow;
            _dialogueText.maxVisibleCharacters = _visibleCount;

            if (_visibleCount >= _fullText.Length)
            {
                _isTyping = false;
            }
        }
    }

    //Inizializza e avvia l'effetto "macchina da scrivere" sulla componente testuale
    public void StartTyping (string textToShow, TMP_Text textComponent)
    {
        _dialogueText = textComponent;
        _fullText = textToShow;

        _dialogueText.text = _fullText;
        _dialogueText.maxVisibleCharacters = 0;

        _timer = 0f;
        _visibleCount = 0;
        _isTyping = true;
    }

    //Interrompe l'animazione e mostra il testo intero
    public void Skip()
    {
        _dialogueText.maxVisibleCharacters = _fullText.Length;
        _isTyping = false;
    }
}
