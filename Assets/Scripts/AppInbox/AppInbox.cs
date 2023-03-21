using UnityEngine;
using UnityEngine.UI;
using LeanplumSDK;

public class AppInbox : MonoBehaviour
{
    public GameObject contentView;
    public VerticalLayoutGroup verticalLayoutGroup;
    public GameObject messageItem;

    public Color unreadColor;
    public Color readColor;

    void Start()
    {
        CreateMessageItems();

        Leanplum.Inbox.ForceContentUpdate += Inbox_ForceContentUpdate;
    }

    private void CreateMessageItems()
    {
        var messages = Leanplum.Inbox.Messages;
        var parent = verticalLayoutGroup.GetComponent<RectTransform>();

        foreach (var message in messages)
        {
            var go = Instantiate(messageItem);
            var item = go.GetComponent<MessageItem>();

            item.title.text = message.Title;
            item.subtitle.text = message.Subtitle;
            item.image.enabled = false;

            if (!item.image.enabled)
                item.GetComponent<Image>().color = message.IsRead ? readColor : unreadColor;

            item.button.onClick.AddListener(() =>
            {
                Leanplum.Inbox.Read(message);
                Debug.Log($"Read: {message.Id}");
                item.GetComponent<Image>().color = readColor;
            });

            go.transform.SetParent(parent);
        }
    }

    private void Inbox_ForceContentUpdate(bool success)
    {
        if (success)
        {
            RemoveMessageItems();
            CreateMessageItems();
        }
    }

    private void RemoveMessageItems()
    {
        var parent = verticalLayoutGroup.GetComponent<RectTransform>();
        var messageItems = parent.GetComponentsInChildren<MessageItem>(true);
        for (int i = messageItems.Length - 1; i >= 0; i--)
        {
            Destroy(messageItems[i].gameObject);
        }
    }

    public void restore()
    {

    }
}
