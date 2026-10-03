using UnityEngine;

public class Cloud : MonoBehaviour
{
    [Header("Impostazioni Entrata")]
    [Tooltip("Velocità con cui la nuvola entra nello schermo")]
    [SerializeField] private float _entranceSpeed = 5f;
    [Tooltip("Coordinata X dove la nuvola si fermerà al centro")]
    [SerializeField] private float _stopPointX = -0.36f;

    [Header("Impostazioni Vento")]
    [SerializeField] private float _windForce = 2.3f;
    [SerializeField] private float _rightLimit = 25f;

    private enum State { Idle, Entering, BlownAway }
    private State _currentState = State.Idle;

    //Attiva la nuvola e avvia il suo ingresso in scena
    public void EnterInScene()
    {
        gameObject.SetActive(true);
        _currentState = State.Entering;
    }
    
    //Cambia lo stato per far andare via la nuvola
    public void BlowAway()
    {
        _currentState = State.BlownAway;
    }

    void Update()
    {
        //Gestisce il movimento di entrata fino al punto di stop
        if (_currentState == State.Entering)
        {
            transform.Translate(Vector3.right * _entranceSpeed * Time.deltaTime, Space.World);

            if (transform.position.x >= _stopPointX)
            {
                transform.position = new Vector3(_stopPointX, transform.position.y, transform.position.z);
                _currentState = State.Idle;
            }
        }
        //Gestisce il movimento dovuto al vento e la disattiva una volta uscita dalla scena
        else if (_currentState == State.BlownAway)
        {
            transform.Translate(Vector3.right * _windForce * Time.deltaTime, Space.World);

            if (transform.position.x > _rightLimit)
            {
                gameObject.SetActive(false);
            }
        }
    }
}