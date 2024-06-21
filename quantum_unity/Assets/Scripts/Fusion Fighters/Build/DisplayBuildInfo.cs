using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBuildInfo : Display<SerializableWrapper<Build>, List<MonoBehaviour>>
{
    [SerializeField] private string _format1;
    [SerializeField] private string _format2;

    public override void UpdateDisplay(SerializableWrapper<Build> item)
    {
        (_component[0] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Name);
        (_component[1] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Description);
        (_component[2] as TMPro.TMP_Text).SetText(string.Format(_format1, new DateTime(item.LastEditedDate).ToUniversalTime().ToString("U")));
        (_component[3] as TMPro.TMP_Text).SetText(string.Format(_format2, new DateTime(item.CreationDate).ToUniversalTime().ToString("U")));
    }

    protected override SerializableWrapper<Build> GetValue() => BuildController.Instance.CurrentlySelected;
}
