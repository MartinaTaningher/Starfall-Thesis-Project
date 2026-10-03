using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Collegamenti")]
    [SerializeField] private Slider _energyBar;
    [SerializeField] private GameObject _canvasHUD;
    [SerializeField] private StoryManager _storyManager;

    [Header("Tutorial")]
    [SerializeField] private TextMeshProUGUI _movementTutorial;

    [Header("Ostacoli Fasi")]
    [SerializeField] private Cloud _cloud;
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private StarSpawner _spawner;

    [Header("Impostazioni energia")]
    [SerializeField] private float _maxEnergy = 100f;

    [Header("Elementi Scena")]
    [SerializeField] private SpriteRenderer _moonSprite;
    [SerializeField] private GameObject _windAnimation;

    private float _currentEnergy = 0f;
    private int _currentPhase = 1;

    //Configura il singleton
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //Inizializza l'interfaccia e nasconde gli elementi della scena all'avvio
    private void Start()
    {
        _energyBar.maxValue = _maxEnergy;
        _energyBar.value = 0f;

        if (_canvasHUD != null) _canvasHUD.SetActive(false);
        if (_cloud != null) _cloud.gameObject.SetActive(false);
        if (_windAnimation != null) _windAnimation.gameObject.SetActive(false);
    }

    public void StartMinigame()
    {
        if (_canvasHUD != null) _canvasHUD.SetActive(true);

        TriggerTutorial(_movementTutorial);
    }

    public void TriggerTutorial(TextMeshProUGUI text)
    {
        if (text != null)
        {
            StartCoroutine(FadeTutorial(text));
        }
    }

    //Gestisce la comparsa e la scomparsa graduale del testo a schermo
    private IEnumerator FadeTutorial(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
        Color c = text.color;
        float time = 0;

        while(time < 1f)
        {
            time += Time.deltaTime;
            text.color = new Color(c.r, c.g, c.b, time);
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            text.color = new Color(c.r, c.g, c.b, 1f - time);
            yield return null;
        }

        text.gameObject.SetActive(false);
    }

    //Incrementa l'energia, aggiorna la UI e verifica l'avanzamento delle fasi
    public void AddEnergy (float amount)
    {
        _currentEnergy += amount;
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);
        _energyBar.value = _currentEnergy;

        CheckPhases();

        if (_currentEnergy >= _maxEnergy)
        {
            TriggerEnding();
        }
    }

    //Attiva gli ostacoli delle varie fasi
    private void CheckPhases()
    {
        float percentage = _currentEnergy / _maxEnergy;

        if (percentage >= 0.6f && _currentPhase < 3)
        {
           _currentPhase = 3;
           if (_playerMover != null) _playerMover.WindActivation();
           if (_cloud != null) _cloud.BlowAway();
           if (_windAnimation != null) _windAnimation.SetActive(true);
            
        }
        else if (percentage >= 0.3f && _currentPhase < 2)
        {
            _currentPhase = 2;
            if (_cloud != null) _cloud.gameObject.SetActive(true);
            if (_cloud != null) _cloud.EnterInScene();
        }
    }

    //Blocca il minigioco, ripulisce le scene e avvia la sequenza narrativa
    private void TriggerEnding()
    {
        if (_spawner != null) _spawner.enabled = false;

        if (_playerMover != null) _playerMover.WindDisactivation();

        Star[] allStars = FindObjectsByType<Star>();

        foreach (Star star in allStars)
        {
            star.Vanish();
        }

        if (_canvasHUD != null) _canvasHUD.SetActive(false);

        if (_cloud != null) _cloud.gameObject.SetActive(false);
        if (_windAnimation != null) _windAnimation.gameObject.SetActive(false);
            
        if (_storyManager != null)
        {
            _storyManager.StartMoonEffect();
            _storyManager.StartBetrayalDialogue();
        }
    }

    public void ShowGoodEndingScreen()
    {
        if (_storyManager != null)
        {
            _storyManager.StartGoodEndingPanel();
        }
    }
}
