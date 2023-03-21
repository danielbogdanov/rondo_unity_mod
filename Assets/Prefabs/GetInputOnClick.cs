using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeanplumSDK;

using UnityEngine.UI;

public class GetInputOnClick : MonoBehaviour

{
    public Button btnClick;
    public InputField inputUser;

    public void Start()
    {
        btnClick.onClick.AddListener(GetInputOnClickHandler);
    }
    public void GetInputOnClickHandler()
    {
        Dictionary<string,object> attributes = new Dictionary<string, object>();
        attributes.Add("email_address",inputUser.text);
        Debug.Log("Log Input: " + inputUser.text );
        Leanplum.SetUserAttributes(attributes);

    }
}
