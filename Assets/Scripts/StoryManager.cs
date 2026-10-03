
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [Header("Script Collegati")]
    [SerializeField] private TextTyper _textTyper;
    [SerializeField] private StarSpawner _starSpawner;
    [SerializeField] private BagSpawner _bagSpawner;
    [SerializeField] private GameObject _canvasHUD;
    [SerializeField] private SpriteRenderer _moonSprite;

    [Header("Impostazioni Estetiche")]
    [SerializeField] private Color _moonEnergyColor = Color.yellow;

    [Header("Elementi UI")]
    [SerializeField] private GameObject _dialogueCanvas;
    [SerializeField] private TextMeshProUGUI _moonText;
    [SerializeField] private GameObject _forwardButton;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private GameObject _choicesPanel;

    [Header("Schermata Finale Nera")]
    [SerializeField] private GameObject _endingPanel;
    [SerializeField] private UnityEngine.UI.Image _blackBg;
    [SerializeField] private TextMeshProUGUI _canvasFinalText;
    [TextArea(2, 5)][SerializeField] private string _goodScreenText;
    [TextArea(2, 5)][SerializeField] private string _badScreenText;

    [Header("Battute")]
    [TextArea(2, 5)][SerializeField] private string[] _moonLines;
    [TextArea(2, 5)][SerializeField] private string[] _buttonLines;
    [TextArea(2, 5)][SerializeField] private string[] _betrayalLines;
    [TextArea(2, 5)][SerializeField] private string[] _goodEndingLines;
    [TextArea(2, 5)][SerializeField] private string[] _badEndingLines;

    private int _linesIndex = 0;
    private bool _isBetrayal = false;
    private bool _isEnding = false;
    private Coroutine _coroutineMoonColor;

    private WaitForSeconds _waitTypingEnd = new WaitForSeconds(0.5f);
    private WaitForSeconds _waitBlackPause = new WaitForSeconds(0.5f);
    private WaitForSeconds _waitFinalReading = new WaitForSeconds(10.0f);
    private WaitForSeconds _waitBadSequence = new WaitForSeconds(2f);
    private WaitForSeconds _waitGoodSequence = new WaitForSeconds(1f);

    //Nasconde gli elementi dell'interfaccia non necessari all'avvio e prepara la scena iniziale
    private void Start()
    {
        if (_canvasHUD != null) _canvasHUD.SetActive(false);
        if (_choicesPanel != null) _choicesPanel.SetActive(false);
        if (_endingPanel != null) _endingPanel.SetActive(false);

        _dialogueCanvas.SetActive(false);

        if (_starSpawner != null) _starSpawner.enabled = false;

        StartCoroutine(DialogueStartDelay());
    }

    //Permette al giocatore di saltare l'animazione del testo/avanzare nel dialogo premendo la barra spaziatrice
    public void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (_textTyper.IsTyping)
            {
                _textTyper.Skip();
                if (!_isBetrayal) _forwardButton.SetActive(true);
            }
            else if (_isBetrayal && _choicesPanel != null && !_choicesPanel.activeSelf)
            {
                AdvanceDialogue();
            }
        }
    }

    public void OnNextButtonClicked()
    {
        if (_textTyper.IsTyping)
        {
            _textTyper.Skip();
            _forwardButton.SetActive(true);
        }
        else
        {
            AdvanceDialogue();
        }
    }

    //Inizializza una nuova sequenza di dialogo impostando lo stato narrativo
    private void StartDialogue(string[] newLine, bool isBetrayal, bool isEnding)
    {
        _moonLines = newLine;
        _isBetrayal = isBetrayal;
        _isEnding = isEnding;
        _linesIndex = 0;

        _dialogueCanvas.SetActive(true);
        if (_choicesPanel != null) _choicesPanel.SetActive(false);

        ShowCurrentLine();
    }

    private void AdvanceDialogue()
    {
        _linesIndex++;

        if (_linesIndex < _moonLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            HandleDialogueEnd();
        }
    }

    //Mostra la battuta corrente e gestisce il cambio colore della luna
    private void ShowCurrentLine()
    {
        _forwardButton.SetActive(false);
        _textTyper.StartTyping(_moonLines[_linesIndex], _moonText);

        if (_isBetrayal && !_isEnding && _moonLines[_linesIndex].Contains("che succede", System.StringComparison.OrdinalIgnoreCase))
        {
            ChangeMoonColor(Color.white, 2f);
        }

        if (!_isBetrayal && _linesIndex < _buttonLines.Length)
        {
            _textButton.text = _buttonLines[_linesIndex];
            StartCoroutine(WaitUntilTypingEnds());
        }
    }

    //Conclude il dialogo attivando l'azione successiva (minigioco, scelte o finale)
    private void HandleDialogueEnd()
    {
        if (!_isBetrayal)
        {
            _dialogueCanvas.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.StartMinigame();
            if (_starSpawner != null) _starSpawner.enabled = true;
            return;
        }

        if (_isEnding)
        {
            _dialogueCanvas.SetActive(false);
            _isBetrayal = false;

            if (_moonLines == _goodEndingLines && _bagSpawner != null)
            {
                _bagSpawner.LaunchBag();
            }
            else if (_moonLines == _badEndingLines && _moonSprite != null)
            {
                StartCoroutine(BadEndingSequence());
            }
        }
        else if (_choicesPanel != null)
        {
            _choicesPanel.SetActive(true);
        }
    }

    private IEnumerator WaitUntilTypingEnds()
    {
        while (_textTyper.IsTyping) yield return null;
        yield return _waitTypingEnd;
        _forwardButton.SetActive(true);
    }

    public void StartBetrayalDialogue() => StartDialogue(_betrayalLines, true, false);
    public void GoodChoice() => StartDialogue(_goodEndingLines, true, true);
    public void BadChoice() => StartDialogue(_badEndingLines, true, true);
    public void StartMoonEffect() => ChangeMoonColor(_moonEnergyColor, 2f);
    public void StartGoodEndingPanel() => StartCoroutine(GoodEndingSequence());

    //Cambia il colore dello sprite della luna
    private void ChangeMoonColor(Color targetColor, float duration)
    {
        if (_moonSprite == null) return;
        if (_coroutineMoonColor != null) StopCoroutine(_coroutineMoonColor);
        _coroutineMoonColor = StartCoroutine(ColorAnimation(targetColor, duration));
    }

    private IEnumerator ColorAnimation(Color targetColor, float duration)
    {
        float time = 0;
        duration = 2f;
        Color startingColor = _moonSprite.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            _moonSprite.color = Color.Lerp(startingColor, targetColor, time / duration);
            yield return null;
        }
        _moonSprite.color = targetColor;
    }

    //Gestisce la transizione finale verso il nero, mostra il testo conclusivo e riporta al menu principale
    private IEnumerator FadeToBlack(string textToShow)
    {
        _blackBg.color = new Color(0, 0, 0, 0);
        _endingPanel.SetActive(true);

        _canvasFinalText.text = "";
        _canvasFinalText.color = new Color(1, 1, 1, 1);

        float time = 0;
        float blackDuration = 2f;

        while (time < blackDuration)
        {
            time += Time.deltaTime;
            _blackBg.color = new Color(0, 0, 0, time / blackDuration);
            yield return null;
        }

        yield return _waitBlackPause;

        _textTyper.StartTyping(textToShow, _canvasFinalText);

        while (_textTyper.IsTyping)
        {
            yield return null;
        }

        yield return _waitFinalReading;

        time = 0;
        float textFadeOutDuration = 2f;

        while (time < textFadeOutDuration)
        {
            time += Time.deltaTime;
            _canvasFinalText.color = new Color(1, 1, 1, 1f - (time / textFadeOutDuration));
            yield return null;
        }

        _canvasFinalText.color = new Color(1, 1, 1, 0);
        SceneManager.LoadScene("MenuIniziale");
    }

    //Avviano le sequenze dei finali
    private IEnumerator BadEndingSequence()
    {
        Color transparent = new Color(_moonSprite.color.r, _moonSprite.color.g, _moonSprite.color.b, 0f);
        ChangeMoonColor(transparent, 2f);
        yield return _waitBadSequence;
        StartCoroutine(FadeToBlack(_badScreenText));
    }

    private IEnumerator GoodEndingSequence()
    {
        yield return _waitGoodSequence;
        StartCoroutine(FadeToBlack(_goodScreenText));
    }

    //Effettua una dissolvenza in apertura prima del primo dialogo
    private IEnumerator DialogueStartDelay()
    {
        _endingPanel.SetActive(true);
        _blackBg.color = new Color(0, 0, 0, 1);
        _canvasFinalText.color = new Color(1, 1, 1, 0);

        float time = 0;
        float duration = 1.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            _blackBg.color = new Color(0, 0, 0, 1f - (time / duration));
            yield return null;
        }

        _endingPanel.SetActive(false);
        StartDialogue(_moonLines, false, false);
    }
}