using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class StarSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Star _starOnPrefab;
    [SerializeField] private Star _starOffPrefab;

    [Header("Impostazioni Spawn")]
    [SerializeField] private float _spawnInterval = 0.3f;
    [SerializeField] private float _spawnY = 6f; 
    [SerializeField] private float _minX = -5f; 
    [SerializeField] private float _maxX = 5f; 

    [Header("Difficoltà")]
    [Range(0, 100)]
    [SerializeField] private int _chanceStarOff = 38;

    private IObjectPool<Star> _goodStarPool;
    private IObjectPool<Star> _badStarPool;
    private float _timer;

    //Inizializza gli Object Pool per riciclare le stelle
    private void Awake()
    {
        _goodStarPool = new ObjectPool<Star>(
            createFunc: () => { Star s = Instantiate(_starOnPrefab); s.SetPool(_goodStarPool); return s; },
            actionOnGet: PosUp,
            actionOnRelease: (s) => s.gameObject.SetActive(false),
            actionOnDestroy: (s) => Destroy(s.gameObject),
            defaultCapacity: 5, maxSize: 10
            );

        _badStarPool = new ObjectPool<Star>(
            createFunc: () => { Star s = Instantiate(_starOffPrefab); s.SetPool(_badStarPool); return s; },
            actionOnGet: PosUp,
            actionOnRelease: (s) => s.gameObject.SetActive(false),
            actionOnDestroy: (s) => Destroy(s.gameObject),
            defaultCapacity: 5, maxSize: 10
            );
    }

    //Riposiziona la stella in un punto casuale in alto prima di riattivarla
    private void PosUp(Star star)
    {
        star.transform.position = new Vector3(Random.Range(_minX, _maxX), _spawnY, 0f);
        star.gameObject.SetActive(true);
    }

    //Gestisce il timer per generare nuove stelle a intervalli regolari
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0f;
            LaunchStar();
        }
    }

    //Sceglie casualmente se far cadere una stella luminosa o spenta in base alla probabilità
    private void LaunchStar()
    {
        int chance = Random.Range(1, 101);
        if (chance <= _chanceStarOff)
            _badStarPool.Get();
        else
        {
            _goodStarPool.Get();
        }
    }
}

