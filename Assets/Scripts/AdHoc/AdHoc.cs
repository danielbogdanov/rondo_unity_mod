using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LeanplumSDK;

public class AdHoc : MonoBehaviour
{
    public GameObject trackEventItem;
    public GameObject sessionEventItem;
    public GameObject userIdItem;
    public GameObject userAttributeItem;
    public GameObject deviceLocationItem;
    public GameObject downloadInboxMessagesItem;
    public GameObject forceContentUpdateItem;

    void Start()
    {
        setupButtons(trackEventItem, didTapTrackEvent);
        setupButtons(sessionEventItem, didTapTrackSessionEvent);
        setupButtons(userIdItem, didTapSetUserIdEvent);
        setupButtons(userAttributeItem, didTapSetUserAttributes);
        setupButtons(deviceLocationItem, didTapSetDeviceLocation);
        setupButtons(downloadInboxMessagesItem, didTapDownloadInboxMessages);
        setupButtons(forceContentUpdateItem, didTapForceContentUpdate);
    }

    private void setupButtons(GameObject item, UnityAction function)
    {
        var button = item.GetComponentInChildren<Button>();
        button.onClick.AddListener(function);
    }

    private void didTapTrackEvent()
    {
        var input = trackEventItem.GetComponentInChildren<InputField>();
        var text = input.text;

        if (text.Length > 0)
        {
            // eventName,parameter1:value1
            string eventName = text;
            Dictionary<string, object> eventParams = new Dictionary<string, object>();
            string[] arr = text.Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 1)
            {
                eventName = arr[0];
                string[] args = arr[1].Split(new[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 2)
                {
                    eventParams.Add(args[0], args[1]);
                }
            }
            Leanplum.Track(eventName, eventParams);
        }
    }

    private void didTapTrackSessionEvent()
    {
        var input = sessionEventItem.GetComponentInChildren<InputField>();
        var text = input.text;

        if (text.Length > 0)
        {
            // stateName,parameter1:value1
            string stateName = text;
            Dictionary<string, object> eventParams = new Dictionary<string, object>();
            string[] arr = text.Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 1)
            {
                stateName = arr[0];
                string[] args = arr[1].Split(new[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 2)
                {
                    eventParams.Add(args[0], args[1]);
                }
            }
            Leanplum.AdvanceTo(stateName, eventParams);
        }
    }

    private void didTapSetUserIdEvent()
    {
        var input = userIdItem.GetComponentInChildren<InputField>();
        var text = input.text;
        if (text.Length > 0)
        {
            Leanplum.SetUserId(text);
        }
    }

    private void didTapSetUserAttributes()
    {
        var userAttributes = new Dictionary<string, object>();
        var keyInput = userAttributeItem.transform.Find("KeyInput").GetComponent<InputField>();
        var valueInput = userAttributeItem.transform.Find("ValueInput").GetComponent<InputField>();

        var key = keyInput.text;
        var value = valueInput.text;

        if (key.Length > 0)
        {
            userAttributes.Add(key, value);
            Leanplum.SetUserAttributes(userAttributes);
        }
    }

    private void didTapSetDeviceLocation()
    {
        var userAttributes = new Dictionary<string, object>();
        var latitudeInput = deviceLocationItem.transform.Find("LatitudeInput").GetComponent<InputField>();
        var longitudeInput = deviceLocationItem.transform.Find("LongitudeInput").GetComponent<InputField>();

        var latitude = ConvertToDouble(latitudeInput.text);
        var longitude = ConvertToDouble(longitudeInput.text);

        Leanplum.SetDeviceLocation(latitude, longitude);
    }

    private void didTapForceContentUpdate()
    {
        Leanplum.ForceContentUpdate(() =>
        {
            Debug.Log("Force Content Update Finished");
        });
    }

    private void didTapDownloadInboxMessages()
    {
        Leanplum.Inbox.DownloadMessages((success) =>
        {
            Debug.Log($"Download Inbox Messages Finished with Status: {success}");
        });
    }

    private double ConvertToDouble(string Value)
    {
        if (Value == null)
        {
            return 0;
        }
        else
        {
            double OutVal;
            double.TryParse(Value, out OutVal);

            if (double.IsNaN(OutVal) || double.IsInfinity(OutVal))
            {
                return 0;
            }
            return OutVal;
        }
    }
}
