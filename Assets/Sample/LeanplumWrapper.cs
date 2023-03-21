//
// Copyright 2022, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanplumSDK;
using UnityEngine;

public class LeanplumWrapper : MonoBehaviour
{
    public string AppID;
    public string ProductionKey;
    public string DevelopmentKey;
    public string AppVersion;

    public Var<string> varText;
    public Var<int> varInt;
    public Var<bool> varBool;
    public Var<double> varDouble;

    public enum Mode
    {
        DEV = 1,
        PROD
    }

    void Awake()
    {
        if (Application.isEditor)
        {
            LeanplumFactory.SDK = new LeanplumNative();
        }
        else
        {
#if UNITY_EDITOR
            LeanplumFactory.SDK = new LeanplumNative();
            // NOTE: Currently, the native iOS and Android SDKs do not support Unity Asset Bundles.
            // If you require the use of asset bundles, use LeanplumNative on all platforms.
#elif UNITY_IPHONE
            LeanplumFactory.SDK = new LeanplumApple();
#elif UNITY_ANDROID
			LeanplumFactory.SDK = new LeanplumAndroid();
#else
			LeanplumFactory.SDK = new LeanplumNative();
#endif
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        SocketUtilsFactory.Utils = new SocketUtils();

        if (!string.IsNullOrEmpty(AppVersion))
        {
            Leanplum.SetAppVersion(AppVersion);
        }
        if (string.IsNullOrEmpty(AppID)
            || string.IsNullOrEmpty(ProductionKey)
            || string.IsNullOrEmpty(DevelopmentKey))
        {
            Debug.LogError("Please make sure to enter your AppID, Production Key, and " +
                           "Development Key in the Leanplum GameObject inspector before starting.");
        }

        bool debugMode = Debug.isDebugBuild;
        int mode = PlayerPrefs.GetInt("Mode");
        if (mode > 0)
        {
            if (mode == (int)Mode.DEV)
            {
                debugMode = true;
            }
            else if (mode == (int)Mode.PROD)
            {
                debugMode = false;
            }
        }

        Debug.Log($"Setting Leanplum in {(debugMode ? "Debug" : "Prod")} mode");

        if (debugMode)
        {
            Leanplum.SetAppIdForDevelopmentMode(AppID, DevelopmentKey);
        }
        else
        {
            Leanplum.SetAppIdForProductionMode(AppID, ProductionKey);
        }

        Leanplum.Inbox.InboxChanged += InboxChanged;
        Leanplum.Inbox.ForceContentUpdate += ForceContentUpdate;

#if UNITY_EDITOR
        MessageTemplates.DefineAlert();
        MessageTemplates.DefineConfirm();
        MessageTemplates.DefineCenterPopup();
        MessageTemplates.DefineOpenURL();
        MessageTemplates.DefineGenericDefinition();
        LeanplumNative.ShouldPerformActions(true);
        /* 
         * Syncs Variables to Dashboard in Dev mode
         * Uncomment to use
         * 
         * Leanplum.ForceSyncVariables(null);
         * Leanplum.ForceSyncVariables((success)=>
         * {
         *      Debug.Log("Force Syncing Variables completed with: " + success);
         * });
         * Leanplum.SetIncludeDefaultsInDevelopmentMode(true);
        */
#endif

        varText = Var.Define("var_text", "Default value in code");
        varInt = Var.Define("var_int", 1);
        varBool = Var.Define("var_bool", false);
        varDouble = Var.Define("var_double", 5.0);

        // Defines a custom message template.
        DefineCustomMessage();

        /* 
         * Used in the documentation example.
         * Simplified example from DefineCustomMessage.
         * 
         * DefineMyCustomMessage();
        */

        Leanplum.SetLogLevel(Constants.LogLevel.DEBUG);
        SetOnMessageHandlers();
        Leanplum.Start(OnStart);
    }

    public void OnStart(bool success)
    {
        if (success)
        {
            Debug.Log("Success starting Leanplum SDK");
        }
        else
        {
            Debug.Log("Failed starting Leanplum SDK");
        }
    }

    private void SetOnMessageHandlers()
    {
        Leanplum.OnMessageDisplayed((context) =>
        {
            Debug.Log($"OnMessageDisplayed: {context}");
        });

        Leanplum.OnMessageDismissed((context) =>
        {
            Debug.Log($"OnMessageDismissed: {context}");
        });

        Leanplum.OnMessageAction((action, context) =>
        {
            Debug.Log($"OnMessageAction: {action}, for context: {context}");
        });
    }

    void InboxChanged()
    {
        Debug.Log("Inbox changed delegate called");
    }

    void ForceContentUpdate(bool success)
    {
        Debug.Log("Inbox forceContentUpdate delegate called: " + success);
    }

    /// <summary>
    /// Used in the documentation example. Simplified example from DefineCustomMessage.
    /// </summary>
    void DefineMyCustomMessage()
    {
        string name = "My MOST Custom Message";
        string TitleArg = "Title";
        string ColorArg = "Font Color";
        string OnMainPageArg = "isOnMainPage";
        string PriceArg = "Price";
        string DictionaryArg = "Product Dimensions";
        string ImageArg = "Background Image";
        string AcceptActionArg = "Accept action";
        string CancelActionArg = "Cancel action";

        var dimensions = new Dictionary<string, double>
        {
            { "Height", 160.8 },
            { "Length", 78.1 },
            { "Width", 7.4 },
            { "Diagonal", 6.5 }
        };

        ActionArgs args = new ActionArgs();
        args.With<string>(TitleArg, "Hello");
        args.WithColor(ColorArg, new Color32(49, 77, 121, 255));
        args.With<bool>(OnMainPageArg, false);
        args.With<double>(PriceArg, 99.98);
        args.With<Dictionary<string, double>>(DictionaryArg, dimensions);

        args.WithFile(ImageArg);

        args.WithAction<object>(AcceptActionArg, null);
        args.WithAction<object>(CancelActionArg, null);

        Leanplum.DefineAction(name, Constants.ActionKind.MESSAGE, args, new Dictionary<string, object>(), (actionContext) =>
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Featured: {actionContext.GetBooleanNamed(OnMainPageArg)}");
            sb.AppendLine($"Price: {string.Format("{0:#.00}", actionContext.GetNumberNamed<double>(PriceArg))}");

            var itemsdict = actionContext.GetObjectNamed<Dictionary<string, double>>(DictionaryArg);

            if (itemsdict != null && itemsdict.Count > 0)
            {
                string[] size = itemsdict.Where(k => k.Key != "Diagonal").Select(kv => $"{kv.Key}: {kv.Value}mm").ToArray();
                sb.AppendLine($"{DictionaryArg}: {string.Join(" x ", size)}");
                sb.AppendLine($"Screen Size: {itemsdict["Diagonal"]} inches");
            }

            Color colorValue = actionContext.GetColorNamed(ColorArg);

            var model = new CustomMessageModel(actionContext);
            model.Title = actionContext.GetStringNamed(TitleArg);
            model.Message = sb.ToString();
            model.Color = colorValue;

            string mimg = actionContext.GetFile(ImageArg);
            Debug.Log($"{ImageArg}: " + mimg);
            if (string.IsNullOrEmpty(mimg) || (!mimg.StartsWith("http") && !System.IO.File.Exists(mimg)))
            {
                Debug.Log("File path is null or file does not Exist!");
            }
            model.FilePath = mimg;

            model.OnAccept += () =>
            {
                actionContext.RunTrackedActionNamed(AcceptActionArg);
            };

            model.OnCancel += () =>
            {
                actionContext.RunTrackedActionNamed(CancelActionArg);
            };

            model.Show();
        });
    }

    /// <summary>
    /// Defines a custom message template.
    /// Covers different types of action arguments.
    /// Provides debug information when testing.
    /// </summary>
    void DefineCustomMessage()
    {
        string name = "Custom Message";
        string TitleArg = "Title";
        string ColorArg = "Background Color";
        string OnMainPageArg = "isOnMainPage";
        string PriceArg = "Price";
        string ArrayArg = "Items";
        string DictionaryArg = "Features";
        string DimensionsArg = "Dimensions";
        string ImageArg = "MyImage";
        string AcceptActionArg = "Accept action";
        string CancelActionArg = "Cancel action";

        var dict = new Dictionary<string, object>();
        dict.Add("Personalization", "Yes");
        dict.Add("AB Testing", "Yes");
        dict.Add("Limits", 999);

        List<string> items = new List<string> { "Item One", "Item Two", "Item Three" };
        int[] dimensions = new int[] { 1, 2, 3 };

        ActionArgs args = new ActionArgs();
        args.With<string>(TitleArg, "Hello");
        args.WithColor(ColorArg, new Color32(49,77,121,255));
        args.With<bool>(OnMainPageArg, true);
        args.With<double>(PriceArg, 99.98);
        args.With<List<string>>(ArrayArg, items);
        args.With<Dictionary<string, object>>(DictionaryArg, dict);
        args.With<int[]>(DimensionsArg, dimensions);

        args.WithFile(ImageArg);

        args.WithAction<object>(AcceptActionArg, null);
        args.WithAction<object>(CancelActionArg, null);

        Leanplum.DefineAction(name, Constants.ActionKind.MESSAGE, args, new Dictionary<string, object>(), (actionContext) =>
        {
            Debug.Log(name);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Featured: {actionContext.GetBooleanNamed(OnMainPageArg)}");
            sb.AppendLine($"Price: ${actionContext.GetNumberNamed<double>(PriceArg)}");

            var dimensionsValue = actionContext.GetObjectNamed<int[]>(DimensionsArg);

            if (dimensionsValue == null || dimensionsValue.Length == 0)
            {
                Debug.Log($"Arg {DimensionsArg} is null or empty");
            }
            else
            {
                sb.AppendLine($"{DimensionsArg}: {string.Join(" x ", dimensionsValue)} ");
            }

            var itemslist = actionContext.GetObjectNamed<List<string>>(ArrayArg);

            if (itemslist == null || itemslist.Count == 0)
            {
                Debug.Log($"Arg {ArrayArg} is null or empty");
            }
            else
            {
                sb.AppendLine($"{ArrayArg}: {string.Join(", ", itemslist)} ");
            }

            var itemsDict = actionContext.GetObjectNamed<Dictionary<string, object>>(DictionaryArg);

            if (itemsDict == null || itemsDict.Count == 0)
            {
                Debug.Log($"Arg {DictionaryArg} is null or empty");
            }
            else
            {
                sb.AppendLine($"{DictionaryArg}: {string.Join(", ", itemsDict.Select(kv => $"{kv.Key}: {kv.Value}").ToArray())}");

                if (itemsDict.ContainsKey("imageUrl"))
                {
                    Debug.Log("imageUrl: " + NativeActionContext.GetFileURL(itemsDict["imageUrl"]?.ToString()));
                }
            }

            Color colorValue = actionContext.GetColorNamed(ColorArg);
            sb.AppendLine($"Color: {colorValue}");

            var model = new CustomMessageModel(actionContext);
            model.Title = actionContext.GetStringNamed(TitleArg);
            model.Message = "";
            model.Color = colorValue;

            string mimg = actionContext.GetFile(ImageArg);
            Debug.Log($"{ImageArg}: " + mimg);
            if (string.IsNullOrEmpty(mimg) || (!mimg.StartsWith("http") && !System.IO.File.Exists(mimg)))
            {
                Debug.Log("File path is null or file does not Exist!");
            }
            model.FilePath = mimg;

            model.OnAccept += () =>
            {
                actionContext.RunTrackedActionNamed(AcceptActionArg);
            };

            model.OnCancel += () =>
            {
                actionContext.RunTrackedActionNamed(CancelActionArg);
            };

            model.Show();

            Debug.Log($"{name}: Title: {actionContext.GetStringNamed(TitleArg)} {sb}");
        });
    }
}
