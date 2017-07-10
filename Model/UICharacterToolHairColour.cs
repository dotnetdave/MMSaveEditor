﻿// Decompiled with JetBrains decompiler
// Type: UICharacterToolHairColour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0C496ACE-0409-4428-BF50-DF04E26AD4C0
// Assembly location: D:\Steam\steamapps\common\Motorsport Manager\MM_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class UICharacterToolHairColour : MonoBehaviour
{
  public UIGridList grid;

  public void Setup()
  {
    this.SetGrid();
  }

  private void SetGrid()
  {
    this.grid.DestroyListItems();
    this.grid.itemPrefab.SetActive(true);
    for (int inNum = 0; inNum < Portrait.hairColors.Length; ++inNum)
    {
      UICharacterToolTrait listItem = this.grid.CreateListItem<UICharacterToolTrait>();
      listItem.OnStart();
      listItem.Setup(inNum);
      listItem.SetColor(Portrait.hairColors[inNum]);
    }
    this.grid.itemPrefab.SetActive(false);
  }
}