using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeanplumSDK;

namespace Rondo
{
    public class TriggersMenu : MonoBehaviour
    {
        public Button buttonPrefab;
        public VerticalLayoutGroup verticalLayoutGroup;

        void Start()
        {
            var allEvents = new List<string>(new string[] {
                "EVENT: TESTEVENT",
                "STATE: TESTSTATE",
                "USER ATTRIBUTE CHANGE",
                "THIS SHOULD WORK 3 TIMES PER SESSION",
                "THIS SHOULD WORK 3 TIMES PER LIFETIME",
                "CHAINED IN APP"
            });

            var parent = verticalLayoutGroup.GetComponent<RectTransform>();

            foreach (var e in allEvents)
            {
                var button = Instantiate(buttonPrefab);
                button.name = e;
                button.transform.SetParent(parent);
                button.GetComponentInChildren<Text>().text = e;
                button.onClick.AddListener(() =>
                {
                    if (button.name == allEvents[0])
                    {
                        didTapEventButton();
                    }
                    else if (button.name == allEvents[1])
                    {
                        didTapStateButton();
                    }
                    else if (button.name == allEvents[2])
                    {
                        didTapUserAttributeChangeButton();
                    }
                    else if (button.name == allEvents[3])
                    {
                        didTapSessionLimitButton();
                    }
                    else if (button.name == allEvents[4])
                    {
                        didTapLifetimeLimitButton();
                    }
                    else if (button.name == allEvents[5])
                    {
                        didTapChainButton();
                    }
                });
            }
        }

        public void didTapEventButton()
        {
            Leanplum.Track("testEvent");
        }

        public void didTapStateButton()
        {
            Leanplum.AdvanceTo("testState");
        }

        public void didTapUserAttributeChangeButton()
        {
            var map = new Dictionary<string, object>();
            map.Add("age", Random.Range(15, 50));
            Leanplum.SetUserAttributes(map);
        }

        public void didTapSessionLimitButton()
        {
            Leanplum.AdvanceTo("sessionLimit");
        }

        public void didTapLifetimeLimitButton()
        {
            Leanplum.AdvanceTo("lifetimeLimit");
        }

        public void didTapChainButton()
        {
            Leanplum.Track("chainedInApp");
        }
    }
}
