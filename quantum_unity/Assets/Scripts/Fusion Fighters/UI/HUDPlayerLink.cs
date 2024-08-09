using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class HUDPlayerLink : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _name;
    [SerializeField] private TMPro.TMP_Text _playerNum;
    [SerializeField] private Image _health;
    [SerializeField] private HorizontalLayoutGroup _stocks;
    [SerializeField] private Image _energy;
    [SerializeField] private Material _activateUltimate;
    [SerializeField] private Material _lowHealth;
    [SerializeField] private GameObject _lowHealthObj;
    [SerializeField] private Image _portrait;
    [SerializeField] private TMPro.TMP_Text _ready;
    [SerializeField] private Image _readyFill;
    [SerializeField] private TMPro.TMP_Text _lifeCount;

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

    public void SetPlayerNumber(FighterIndex index)
    {
        _playerNum.SetText(index.Type == FighterType.Human ? $"P{index.GlobalNoBots + 1}" : "Bot");
    }

    public void SetPlayerName(string name)
    {
        _name.SetText(name);
    }

    public void SetPlayerIcon(Sprite icon)
    {
        _portrait.sprite = icon;
    }

    public void UpdateReadiness(bool isReady)
    {
        _ready.SetText(isReady ? "Ready!" : "Not Yet...");
    }

    public void UpdateReadinessValue(float value)
    {
        _readyFill.fillAmount = value;
    }

    public void ShowReadiness(bool show)
    {
        _readyFill.gameObject.SetActive(show);
    }

    public void UpdateHealth(FP newHealth, FP maxHealth)
    {
        if (maxHealth != 0)
            _healthFill = (newHealth / maxHealth).AsFloat;
        else
            _healthFill = 0;

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
        if (maxEnergy != 0)
            _energyFill = (newEnergy / maxEnergy).AsFloat;
        else
            _energyFill = 0;

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
        bool showLifeAsNumber = maxStocks > 10 || maxStocks == -1;

        _stocks.gameObject.SetActive(!showLifeAsNumber);
        _lifeCount.gameObject.SetActive(showLifeAsNumber);
        
        if (showLifeAsNumber)
        {
            if (maxStocks == -1)
                _lifeCount.SetText($"Lives: \u221E");
            else
                _lifeCount.SetText($"Lives: {newStocks}/{maxStocks}");
        }
        else
        {
            for (int i = 0; i < maxStocks; ++i)
            {
                _stocks.transform.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = maxStocks; i < _stocks.transform.childCount; ++i)
            {
                _stocks.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < maxStocks; ++i)
                _stocks.transform.GetChild(i).GetComponent<Image>().color = newStocks >= i + 1 ? _fullStock : _emptyStock;
        }
    }
}
