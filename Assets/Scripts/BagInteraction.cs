using UnityEngine;
using UnityEngine.InputSystem;

public class BagInteraction : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _changeBg;
    [SerializeField] private Sprite _starlightSky;

    private bool _isPlayerNear = false;

    //Imposta dinamicamente i riferimenti per lo sfondo
    public void Configure(SpriteRenderer bg, Sprite newBg)
    {
        _changeBg = bg;
        _starlightSky = newBg;
    }

    //Rileva quando il giocatore entra o esce dall'area di interazione
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _isPlayerNear = false;
    }

    private void Update()
    {
        //Se il giocatore è vicino e preme 'E'
        if (_isPlayerNear && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            //Cambia l'immagine dello sfondo
            if (_changeBg != null && _starlightSky != null)
            {
                _changeBg.sprite = _starlightSky;
            }

            //Parte il finale positivo
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowGoodEndingScreen();
            }

            //Distrugge l'oggetto dopo l'interazione
            Destroy(gameObject);
        }
    }
}
