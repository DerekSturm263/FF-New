using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBuildInfo : Display<SerializableWrapper<Build>, List<MonoBehaviour>>
{
    public override void UpdateDisplay(SerializableWrapper<Build> item)
    {
        (_component[0] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Value.SerializableData.Name);
        (_component[1] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Value.SerializableData.Description);
        (_component[2] as TMPro.TMP_Text).SetText(new DateTime(item.Value.SerializableData.LastEdittedDate).ToString("'Last Edited 'MM':'dd':'yyyy' at' hh':'mm':'ss' 'tt"));
        (_component[3] as TMPro.TMP_Text).SetText(new DateTime(item.Value.SerializableData.CreationDate).ToString("'Created 'MM':'dd':'yyyy' at' hh':'mm':'ss' 'tt"));
    }

    protected override SerializableWrapper<Build> GetValue() => new(QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build);
}
