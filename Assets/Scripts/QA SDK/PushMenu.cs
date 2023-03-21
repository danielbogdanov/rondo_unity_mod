using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeanplumSDK;

namespace Rondo
{
    public class PushMenu : MonoBehaviour
    {

        public Button buttonPrefab;
        public VerticalLayoutGroup verticalLayoutGroup;

        void Start()
        {
            var pushEvents = new List<string>(new string[] {
                "pushRender",
                "pushAction",
                "pushImage",
                "pushExistingAction",
                "pushURL",
                "pushOptions",
                "pushLocal",
                "pushLocalCancel",
                "pushMuted"
            });

            var parent = verticalLayoutGroup.GetComponent<RectTransform>();

            AddRegisterForIOSPushButton(parent);

            foreach (var e in pushEvents)
            {
                var button = Instantiate(buttonPrefab);
                button.name = e;
                button.transform.SetParent(parent);
                button.GetComponentInChildren<Text>().text = e;
                button.onClick.AddListener(() =>
                {
                    Leanplum.Track(button.name);
                });
            }
        }

        void AddRegisterForIOSPushButton(Transform parent)
        {
            var button = Instantiate(buttonPrefab);
            button.name = "Register For IOS Remote Notifications";
            button.transform.SetParent(parent);
            button.GetComponentInChildren<Text>().text = "Register For IOS Remote Notifications";
            button.onClick.AddListener(() =>
            {
                Leanplum.RegisterForIOSRemoteNotifications();
            });
        }
    }
}