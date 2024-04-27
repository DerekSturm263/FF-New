using Photon.Deterministic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPlayerLink : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _name;
    [SerializeField] private TMPro.TMP_Text _playerNum;
    [SerializeField] private Image _health;
    [SerializeField] private HorizontalLayoutGroup _stocks;
    [SerializeField] private GameObject _stock;
    [SerializeField] private Image _energy;
    [SerializeField] private Material _activateUltimate;
    [SerializeField] private Material _lowHealth;
    [SerializeField] private GameObject _lowHealthObj;
    [SerializeField] private Image _portrait;

    [Header("Settings")]
    [SerializeField] private Color _emptyHealth;
    [SerializeField] private Color _fullHealth;
    [SerializeField] private Color _emptyEnergy;
    [SerializeField] private Color _fullEnergy;
    [SerializeField] private Color _emptyStock;
    [SerializeField] private Color _fullStock;

    public void UpdateHealth(FP newHealth, FP maxHealth)
    {
        _health.fillAmount = (newHealth / maxHealth).AsFloat;
        _health.color = Color.Lerp(_emptyHealth, _fullHealth, _health.fillAmount);

        if (_health.fillAmount <= 0.2f)
        {
            _portrait.material = _lowHealth;
            _lowHealthObj.SetActive(true);
        }
        else
        {
            _portrait.material = null;
            _lowHealthObj.SetActive(false);
        }
    }

    public void UpdateEnergy(FP newEnergy, FP maxEnergy)
    {
        _energy.fillAmount = (newEnergy / maxEnergy).AsFloat;
        _energy.color = Color.Lerp(_emptyEnergy, _fullEnergy, _energy.fillAmount);

        if (newEnergy == maxEnergy)
        {
            _portrait.material = _activateUltimate;
        }
        else
        {
            _portrait.material = null;
        }
    }

    public void UpdateStocks(int newStocks, int maxStocks)
    {
        if (newStocks == maxStocks)
        {
            for (int i = 0; i < maxStocks; ++i)
            {
                GameObject stock = Instantiate(_stock, _stocks.transform);
                stock.GetComponent<Image>().color = _fullStock;
            }
        }
        else
        {
            if (newStocks < 0)
                return;

            for (int i = maxStocks - 1; i >= newStocks; --i)
            {
                _stocks.transform.GetChild(i).GetComponent<Image>().color = _emptyStock;
            }
        }
    }
}
