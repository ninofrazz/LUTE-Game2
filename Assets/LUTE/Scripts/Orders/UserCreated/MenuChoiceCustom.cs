using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

[OrderInfo("Menu",
              "MenuChoiceCustom",
              "Creates a button which will call another node for a menu")]
[AddComponentMenu("")]
public class MenuChoiceCustom : MenuChoice
{
    public int hello;

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override string GetSummary()
    {
        return base.GetSummary();
    }
}