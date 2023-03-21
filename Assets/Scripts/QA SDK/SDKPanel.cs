using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rondo
{
    public class SDKPanel : MonoBehaviour
    {
        public GameObject optionsPanel;

        public GameObject messagesPanel;
        public GameObject triggersPanel;
        public GameObject pushPanel;

        public GameObject iamHandlersPanel;

        public Button triggersButton;
        public Button messagesButton;
        public Button iamHandlersButton;
        public Button pushButton;

        public Button modeButton;

        void Start()
        {
            triggersButton.onClick.AddListener(DidTapTriggers);
            messagesButton.onClick.AddListener(DidTapMessages);
            iamHandlersButton.onClick.AddListener(DidTapIAMHandlers);
            pushButton.onClick.AddListener(DidTapPush);

            modeButton.onClick.AddListener(DidTapMode);

            int currentMode = PlayerPrefs.GetInt("Mode");
            if (currentMode == 2)
            {
                modeButton.GetComponentInChildren<Text>().text = "Switch to Dev Mode";
            }

            Restore();
        }

        public void Restore()
        {
            optionsPanel.SetActive(true);
            messagesPanel.SetActive(false);
            triggersPanel.SetActive(false);
            pushPanel.SetActive(false);
            iamHandlersPanel.SetActive(false);
        }

        public void DidTapTriggers()
        {
            optionsPanel.SetActive(false);
            triggersPanel.SetActive(true);
        }

        public void DidTapMessages()
        {
            optionsPanel.SetActive(false);
            messagesPanel.SetActive(true);
        }

        public void DidTapIAMHandlers()
        {
            optionsPanel.SetActive(false);
            iamHandlersPanel.SetActive(true);
        }

        public void DidTapPush()
        {
            optionsPanel.SetActive(false);
            pushPanel.SetActive(true);
        }

        public void DidTapMode()
        {
            int currentMode = PlayerPrefs.GetInt("Mode");
            if (currentMode > 1)
            {
                PlayerPrefs.SetInt("Mode", (int)LeanplumWrapper.Mode.DEV);
                modeButton.GetComponentInChildren<Text>().text = "Switch to Prod Mode";
            }
            else
            {
                PlayerPrefs.SetInt("Mode", (int)LeanplumWrapper.Mode.PROD);
                modeButton.GetComponentInChildren<Text>().text = "Switch to Dev Mode";
            }
        }
    }
}
