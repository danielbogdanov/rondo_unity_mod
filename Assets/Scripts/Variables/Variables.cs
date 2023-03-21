using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeanplumSDK;

public class Variables : MonoBehaviour
{
    public GameObject buttonPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    void Start()
    {
        updateVariables();
    }

    void updateVariables()
    {
        foreach (Transform child in verticalLayoutGroup.GetComponent<RectTransform>())
        {
            Destroy(child.gameObject);
        }

        var parent = verticalLayoutGroup.GetComponent<RectTransform>();
        var wrapper = GameObject.Find("Leanplum").GetComponent<LeanplumWrapper>();

        if (wrapper != null)
        {
            addButton("varText", "var_text: " + wrapper.varText?.Value);
            addButton("varInt", "var_int: " + wrapper.varInt?.Value);
            addButton("varBool", "var_bool: " + wrapper.varBool?.Value);
            addButton("varDouble", "var_double: " + wrapper.varDouble?.Value);
        }
    }

    void addButton(string name, string text)
    {
        var parent = verticalLayoutGroup.GetComponent<RectTransform>();

        var button = Instantiate(buttonPrefab);
        button.name = name;
        button.transform.SetParent(parent);
        button.GetComponentInChildren<Text>().text = text;
    }

    public void restore()
    {
        updateVariables();
    }
}