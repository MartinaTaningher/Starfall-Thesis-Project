using UnityEngine;
using UnityEngine.Pool;

public class Star : MonoBehaviour
{
    [SerializeField] private float _fallSpeed = 6f;

    [Header("Impostazioni Gameplay")]
    [SerializeField] private float _energyValue = 5f;

    private IObjectPool<Star> _pool;

    //Associa la stella al suo Object Pool per la gestione del riciclo
    public void SetPool(IObjectPool<Star> pool) => _pool = pool;

    //Gestisce la caduta della stella e la ricicla nel pool quando esce dallo schermo
    void Update()
    {
        transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime);

        if (transform.position.y < -3.5f)
        {
            _pool.Release(this);
        }
    }

    //Raccoglie la stella, la disattiva e incrementa l'energia
    public void Collect()
    {
        if (!gameObject.activeInHierarchy) return;
        _pool.Release(this);
        GameManager.Instance.AddEnergy(_energyValue);    
    }

    //Fa sparire la stella senza assegnare l'energia
    public void Vanish()
    {
        if (gameObject.activeInHierarchy)
        {
            _pool.Release(this);
        }
    }
}
