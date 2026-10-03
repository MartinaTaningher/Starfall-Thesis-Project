using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _speed = 5;

    [Header("Impostazioni Vento")]
    [Tooltip("Valore pos per spingere a destra, Valore neg per spingere a sinistra")]
    [SerializeField] private float _windForce = 2.3f;

    private float _inputHorizontal;
    private Rigidbody2D _rb;
    private bool _isWindActive = false;

    //Imposta l'input orizzontale e gestisce l'attivazione/disattivazione del vento
    public void SetInputHorizontal(float inputHorizontal) => _inputHorizontal = Mathf.Clamp(inputHorizontal, -1, 1);
    public void WindActivation() => _isWindActive = true;
    public void WindDisactivation() => _isWindActive = false;

    //Recupera il riferimento alla componente fisica all'avvio
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    //Applica il movimento fisico tenendo conto dell'input del giocatore e dell'eventuale spinta del vento
    private void FixedUpdate()
    {
        Vector2 velocity = _rb.linearVelocity;

        float finalSpeedX = _inputHorizontal * _speed;
        float windPush = 0f;

        if (_isWindActive)
        {
            windPush = _windForce;
        }

        _rb.linearVelocity = new Vector2(finalSpeedX + windPush, 0);
    }
}
