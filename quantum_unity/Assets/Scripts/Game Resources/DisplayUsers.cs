using Extensions.Components.UI;
using UnityEngine;
using UnityEngine.Events;

public class DisplayUsers : Display<LocalPlayerInfo[], UnityEvent<LocalPlayerInfo>[]>
{
    [SerializeField] private bool _letNewPlayersJoin = true;
    [SerializeField] private GameObject[] _slots;

    public override void UpdateDisplay(LocalPlayerInfo[] item)
    {
        for (int i = 0; i < item.Length; ++i)
        {
            _component[i].Invoke(item[i]);

            _slots[i].transform.GetChild(0).gameObject.SetActive(false);
            _slots[i].transform.GetChild(1).gameObject.SetActive(true);
        }

        for (int i = item.Length; i < 4; ++i)
        {
            _slots[i].transform.GetChild(0).gameObject.SetActive(_letNewPlayersJoin && true);
            _slots[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    protected override LocalPlayerInfo[] GetValue() => PlayerJoinController.Instance.LocalPlayers.ToArray();
}
