using System.Collections.Generic;
using UnityEngine;

public partial class CustomAnimatorGraphAsset
{
  public enum Resolutions
  {
    _8 = 8,
    _16 = 16,
    _32 = 32,
    _64 = 64
  }

  public Resolutions weight_table_resolution = Resolutions._32;

  public RuntimeAnimatorController controller;

  public List<AnimationClip> clips = new List<AnimationClip>();
}
