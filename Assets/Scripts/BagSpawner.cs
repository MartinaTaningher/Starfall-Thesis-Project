using TMPro;
using UnityEngine;

public class BagSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefabBag;
    [SerializeField] private Transform _originPoint;
    [SerializeField] private SpriteRenderer _currentBg;
    [SerializeField] private Sprite _starlightBg;
    [SerializeField] private TextMeshProUGUI _bagTutorial;

    //Metodo per generare l'oggetto nel mondo di gioco
    public void LaunchBag()
    {
        //Crea l'istanza nel punto di origine prestabilito
        GameObject bag = Instantiate(_prefabBag, _originPoint.position, Quaternion.identity);

        BagInteraction script = bag.GetComponent<BagInteraction>();

        //Passa i riferimeni dello sfondo all'oggetto appena creato
        if (script != null)
        {
            script.Configure(_currentBg, _starlightBg);
        }

        //Mostra il testo del tutorial a schermo
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerTutorial(_bagTutorial);
        }
    }
}
