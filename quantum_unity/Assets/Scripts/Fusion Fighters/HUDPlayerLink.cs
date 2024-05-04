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
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private Color _emptyHealth;
    [SerializeField] private Color _fullHealth;
    [SerializeField] private Color _emptyEnergy;
    [SerializeField] private Color _fullEnergy;
    [SerializeField] private Color _emptyStock;
    [SerializeField] private Color _fullStock;

    private float _healthFill, _energyFill;

    private void Update()
    {
        _health.fillAmount = _healthFill;
        _energy.fillAmount = _energyFill;
    }

    public void UpdateHealth(FP newHealth, FP maxHealth)
    {
        _healthFill = (newHealth / maxHealth).AsFloat;
        _health.color = Color.Lerp(_emptyHealth, _fullHealth, _health.fillAmount);

        if (_portrait.material != _activateUltimate)
        {
            if (_health.fillAmount <= 0.2f)
            {
                _portrait.material = _lowHealth;
            }
            else
            {
                _portrait.material = null;
            }
        }

        if (_health.fillAmount <= 0.2f)
        {
            _lowHealthObj.SetActive(true);
        }
        else
        {
            _lowHealthObj.SetActive(false);
        }
    }

    public void UpdateEnergy(FP newEnergy, FP maxEnergy)
    {
        _energyFill = (newEnergy / maxEnergy).AsFloat;
        _energy.color = Color.Lerp(_emptyEnergy, _fullEnergy, _energy.fillAmount);

        if (newEnergy == maxEnergy)
        {
            _portrait.material = _activateUltimate;
        }
        else
        {
            if (_health.fillAmount <= 0.2f)
            {
                _portrait.material = _lowHealth;
            }
            else
            {
                _portrait.material = null;
            }
        }

        if (_health.fillAmount <= 0.2f)
        {
            _lowHealthObj.SetActive(true);
        }
        else
        {
            _lowHealthObj.SetActive(false);
        }
    }

    public void UpdateStocks(int newStocks, int maxStocks)
    {
        if (newStocks > _stocks.transform.childCount)
        {
            int count = newStocks - _stocks.transform.childCount;

            for (int i = 0; i < count; ++i)
            {
                Instantiate(_stock, _stocks.transform);
            }
        }

        for (int i = 0; i < maxStocks; ++i)
        {
            _stocks.transform.GetChild(i).GetComponent<Image>().color = newStocks >= i + 1 ? _fullStock : _emptyStock;
        }
    }
}
