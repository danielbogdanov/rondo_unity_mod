using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeanplumSDK;

namespace Rondo
{

    public class MessagesMenu : MonoBehaviour
    {

        public Button buttonPrefab;
        public VerticalLayoutGroup verticalLayoutGroup;

        // Use this for initialization
        void Start()
        {
            var messages = new List<string>(new string[] {
                "alert",
                "centerPopup",
                "confirm",
                "interstitial",
                "richInterstitial",
                "webInterstitial",
                "banner"
            });

            var parent = verticalLayoutGroup.GetComponent<RectTransform>();

            foreach (var message in messages)
            {
                var button = Instantiate(buttonPrefab);
                button.name = message;
                button.transform.SetParent(parent);
                button.GetComponentInChildren<Text>().text = message;
                button.onClick.AddListener(() =>
                {
                    Leanplum.Track(button.name);
                });
            }
        }
    }
}