using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _graphics;

    private InputAction _moveAction;
    private PlayerMover _mover;

    private Animator _animator;

    //Inizializza le componenti e le azioni di input all'avvio
    void Awake()
    {
        _mover = GetComponent<PlayerMover>();
        _moveAction = InputSystem.actions.FindAction("Move");

        if (_graphics != null)
        {
            _animator = _graphics.GetComponent<Animator>();
        }
    }

    //Legge gli input del giocatore, aggiorna l'orientamento e l'animazione
    void Update()
    {
        Vector2 move = _moveAction.ReadValue<Vector2>();
        PlatformerLookAt(_graphics, move.x);
        _mover.SetInputHorizontal(move.x);

        if (_animator != null)
        {
            _animator.SetFloat("Speed", Mathf.Abs(move.x));
        }
    }

    //Capovolge la grafica del personaggio in base alla direzione del movimento
    public static void PlatformerLookAt(Transform graphics, float deltaX)
    {
        if (Mathf.Abs(deltaX) > Mathf.Epsilon)
        {
            graphics.transform.localEulerAngles = (deltaX > 0) ? Vector3.zero : new Vector3(0, 180, 0);
        }
    }

    //Rileva la collissione con le stelle per permetterne la raccolta
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Star starTouched = collision.GetComponent<Star>();

        if (starTouched != null)
        {
            starTouched.Collect();
        }
    }
}
