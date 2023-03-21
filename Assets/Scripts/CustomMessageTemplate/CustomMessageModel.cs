using System;
using LeanplumSDK;
using UnityEngine;

public class CustomMessageModel
{
    public ActionContext Context { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }

    internal Color Color { get; set; }
    internal string FilePath { get; set; }

    internal Action OnAccept { get; set; }
    internal Action OnCancel { get; set; }

    public CustomMessageModel(ActionContext context)
    {
        Context = context;
    }

    public void Show()
    {
        CustomMessageDialog.Create(this);
    }
}