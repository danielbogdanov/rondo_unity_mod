using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeanplumSDK;
using System.Linq;

namespace Rondo
{
    internal struct ActionManagerQueueState
    {
        internal static readonly string Enable = "Enable";
        internal static readonly string Disable = "Disable";
        internal static readonly string Pause = "Pause";
        internal static readonly string Resume = "Resume";
    }

    internal class ButtonActionModel
    {
        internal string Name { get; set; }
        internal string Tag { get; set; }
        internal IAMHandlersMenu.ButtonAction Action { get; set; }

        internal ButtonActionModel()
        {
        }

        internal ButtonActionModel(string name, IAMHandlersMenu.ButtonAction action):this(name, null, action)
        {
        }

        internal ButtonActionModel(string name, string tag, IAMHandlersMenu.ButtonAction action)
        {
            Name = name;
            Tag = tag;
            Action = action;
        }
    }

    public class IAMHandlersMenu : MonoBehaviour
    {
        private bool queueEnabled = true;
        private bool queuePaused = false;

        internal delegate void ButtonAction(Button button);
        internal static readonly string ActiveText = "(active)";

        public Button buttonPrefab;
        public VerticalLayoutGroup verticalLayoutGroup;

        // Use this for initialization
        void Start()
        {
            var models = new List<ButtonActionModel>
            {
                new ButtonActionModel(queueEnabled ? ActionManagerQueueState.Disable : ActionManagerQueueState.Enable, DisableQueue),
                new ButtonActionModel(queuePaused ? ActionManagerQueueState.Resume : ActionManagerQueueState.Pause, PauseQueue),

                new ButtonActionModel("Trigger Delayed Messages", TriggerDelayedMessages),

                new ButtonActionModel("Show", "shouldDisplay", Show),
                new ButtonActionModel("Discard", "shouldDisplay", Discard),
                new ButtonActionModel("Delay 10", "shouldDisplay", Delay10),
                new ButtonActionModel("Delay Indefinite", "shouldDisplay", Delay),
                new ButtonActionModel("Remove Display handler", "shouldDisplay", RemoveDisplayHandler),

                new ButtonActionModel("Prioritize First", "prioritizeMessages", PrioritizeFirst),
                new ButtonActionModel("Reversed", "prioritizeMessages", Reversed),
                new ButtonActionModel("Remove Prioritize handler", "prioritizeMessages", RemovePrioritizeHandler)
            };

            var parent = verticalLayoutGroup.GetComponent<RectTransform>();

            foreach (var model in models)
            {
                var button = Instantiate(buttonPrefab);
                button.name = model.Name;
                button.transform.SetParent(parent);
                button.GetComponentInChildren<Text>().text = model.Name;
                if (!string.IsNullOrEmpty(model.Tag))
                {
                    button.gameObject.tag = model.Tag;
                }
                button.onClick.AddListener(() =>
                {
                    model.Action.Invoke(button);
                });
            }
        }

        void DisableQueue(Button button)
        {
            queueEnabled = !queueEnabled;
            button.GetComponentInChildren<Text>().text = queueEnabled ? ActionManagerQueueState.Disable : ActionManagerQueueState.Enable;
            Leanplum.SetActionManagerEnabled(queueEnabled);
        }

        void PauseQueue(Button button)
        {
            queuePaused = !queuePaused;
            button.GetComponentInChildren<Text>().text = queuePaused ? ActionManagerQueueState.Resume : ActionManagerQueueState.Pause;
            Leanplum.SetActionManagerPaused(queuePaused);
        }

        void TriggerDelayedMessages(Button button)
        {
            Leanplum.TriggerDelayedMessages();
        }

        void Show(Button button)
        {
            MarkActiveButtonByTag(button);

            Leanplum.ShouldDisplayMessage((context) =>
            {
                LogShouldDisplay("Show", context);
                return MessageDisplayChoice.Show();
            });
        }

        void Discard(Button button)
        {
            MarkActiveButtonByTag(button);

            Leanplum.ShouldDisplayMessage((context) =>
            {
                LogShouldDisplay("Discard", context);
                return MessageDisplayChoice.Discard();
            });
        }

        void Delay10(Button button)
        {
            MarkActiveButtonByTag(button);

            Leanplum.ShouldDisplayMessage((context) =>
            {
                LogShouldDisplay("Delay 10", context);
                return MessageDisplayChoice.Delay(10);
            });
        }

        void Delay(Button button)
        {
            MarkActiveButtonByTag(button);

            Leanplum.ShouldDisplayMessage((context) =>
            {
                LogShouldDisplay("Delay Indefinitely", context);
                return MessageDisplayChoice.Delay(-1);
            });
        }

        void LogShouldDisplay(string message, ActionContext context)
        {
            Debug.Log($"ShouldDisplayMessage: {message} for context: {context}");
        }

        void RemoveDisplayHandler(Button button)
        {
            MarkActiveButtonByTag(button, false);

            Leanplum.ShouldDisplayMessage(null);
        }

        void PrioritizeFirst(Button button)
        {
            MarkActiveButtonByTag(button);

            Leanplum.PrioritizeMessages((contexts, trigger) =>
            {
                LogPrioritize("PrioritizeFirst", contexts, trigger);
                return contexts.Take(1).ToArray();
            });
        }

        void Reversed(Button button)
        {
            MarkActiveButtonByTag(button);

            Leanplum.PrioritizeMessages((contexts, trigger) =>
            {
                LogPrioritize("Reversed", contexts, trigger);
                return contexts.Reverse().ToArray();
            });
        }

        private void LogPrioritize(string message, ActionContext[] contexts, Dictionary<string, object> trigger)
        {
            string triggerLog = LeanplumSDK.MiniJSON.Json.Serialize(trigger);
            string[] contextsArr = contexts.Select(x => x.ToString()).ToArray();
            string contextsLog = string.Join(", ", contextsArr);
            Debug.Log($"PrioritizeMessages: {message} for trigger: {triggerLog} with contexts: {contextsLog}");
        }

        void RemovePrioritizeHandler(Button button)
        {
            MarkActiveButtonByTag(button, false);

            Leanplum.PrioritizeMessages(null);
        }

        void MarkActiveButtonByTag(Button button, bool markActive = true)
        {
            var buttons = GameObject.FindGameObjectsWithTag(button.gameObject.tag);
            foreach (var b in buttons)
            {
                Text textComponent = b.GetComponentInChildren<Text>();
                textComponent.text = textComponent.text.Replace(ActiveText, string.Empty);
            }
            if (markActive)
                button.GetComponentInChildren<Text>().text += ActiveText;
        }
    }
}