using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBuildInfo : Display<SerializableWrapper<Build>, List<MonoBehaviour>>
{
    protected override SerializableWrapper<Build> GetValue()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateDisplay(SerializableWrapper<Build> item)
    {
        (_component[0] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Value.SerializableData.Name);
        (_component[1] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Value.SerializableData.Description);
        (_component[2] as TMPro.TMP_Text).SetText(new DateTime(item.Value.SerializableData.LastEdittedDate).ToString("'Last Editted 'MM':'dd':'yyyy' at' hh':'mm':'ss' 'tt"));
        (_component[3] as TMPro.TMP_Text).SetText(new DateTime(item.Value.SerializableData.CreationDate).ToString("'Created 'MM':'dd':'yyyy' at' hh':'mm':'ss' 'tt"));
    }
}
