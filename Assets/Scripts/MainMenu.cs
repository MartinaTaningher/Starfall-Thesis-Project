using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _flashingText;
    [SerializeField] private Image _fadePanel;
    [SerializeField] private string _sceneName = "SampleScene";

    private bool _isLoading = false;

    //Per evitare continue allocazioni di memoria
    private WaitForSeconds _waitOn = new WaitForSeconds(1f);
    private WaitForSeconds _waitOff = new WaitForSeconds(0.5f);

    private void Start()
    {
        StartCoroutine(FlashingText());
    }

    private void Update()
    {
        //Se non è in corso un caricamento e viene premuta la barra spaziatrice
        if (!_isLoading && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(FadeChangeScene());
        }
    }

    //Esegue una transizione in dissolvenza verso il nero e carica la scena successiva
    private IEnumerator FadeChangeScene()
    {
        _isLoading = true;
        _flashingText.gameObject.SetActive(false);

        float time = 0f;
        float duration = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            _fadePanel.color = new Color(0, 0, 0, time / duration);
            yield return null;
        }

        SceneManager.LoadScene(_sceneName);
    }

    //Gestisce l'effetto lampeggiante attivando e disattivando il testo ciclicamente
    private IEnumerator FlashingText()
    {
        while (true)
        {
            _flashingText.enabled = true;
            yield return _waitOn;
            _flashingText.enabled = false;
            yield return _waitOff;
        }
    }
}