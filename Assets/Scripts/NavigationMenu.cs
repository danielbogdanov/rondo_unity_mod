using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rondo
{
    public class NavigationMenu : MonoBehaviour
    {
        public Button SDK;
        public Button AppInbox;
        public Button Variables;
        public Button AdHoc;

        public List<GameObject> panels = new List<GameObject>();

        void Start()
        {
            SDK.onClick.AddListener(didTapSDKButton);
            AppInbox.onClick.AddListener(didTapAppInboxButton);
            Variables.onClick.AddListener(didTapVariablesButton);
            AdHoc.onClick.AddListener(didTapAdHocButton);

            panels.Add(GameObject.Find("QASDK"));
            panels.Add(GameObject.Find("AppInbox"));
            panels.Add(GameObject.Find("Variables"));
            panels.Add(GameObject.Find("AdHoc"));

            didTapSDKButton();
        }

        public void didTapSDKButton()
        {
            var sdkPanel = findPanel("QASDK");
            enable(sdkPanel);
            sdkPanel.GetComponent<SDKPanel>().Restore();
        }

        public void didTapAppInboxButton()
        {
            var appInboxPanel = findPanel("AppInbox");
            enable(appInboxPanel);
            appInboxPanel.GetComponent<AppInbox>().restore();
        }

        public void didTapVariablesButton()
        {
            var variables = findPanel("Variables");
            enable(variables);
            variables.GetComponent<Variables>().restore();
        }

        public void didTapAdHocButton()
        {
            var adHoc = findPanel("AdHoc");
            enable(adHoc);
        }

        void enable(GameObject panel)
        {
            foreach (var p in panels)
            {
                if (p == panel)
                {
                    p.SetActive(true);
                }
                else
                {
                    p.SetActive(false);
                }
            }
        }

        GameObject findPanel(string name)
        {
            return panels.Find((first) =>
            {
                return first.name == name;
            });
        }
    }
}
