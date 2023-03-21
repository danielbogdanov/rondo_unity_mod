using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class CustomMessageDialog : MonoBehaviour
{
    protected CustomMessageModel Model { get; set; }

    internal static void Create(CustomMessageModel model)
    {
        CustomMessagePlaceholder placeholder = FindObjectOfType<CustomMessagePlaceholder>();

        GameObject gameObject = new GameObject($"CustomMessageGameObject:{model.Context}");
        gameObject.AddComponent<CustomMessageDialog>();
        gameObject.AddComponent<CanvasGroup>();

        var iamPanel = gameObject.GetComponent<CustomMessageDialog>();
        iamPanel.Model = model;

        gameObject.transform.SetParent(placeholder.transform.parent);
    }

    // Use this for initialization
    void Start()
    {
        CustomMessagePlaceholder panelPlaceholder = FindObjectOfType<CustomMessagePlaceholder>();
        GameObject prefab = panelPlaceholder.customMessagePrefab;
        GameObject currentMessage = Instantiate(prefab, gameObject.transform);
        currentMessage.name = $"CustomMessageDialog:{Model.Context}";
        var message = currentMessage.GetComponentInChildren<CustomMessagePrefab>();


        message.Title.text = Model.Title;
        message.MessageText.text = Model.Message;

        // Set Message and Buttons Text Color
        message.MessageText.color = Model.Color;
        message.AcceptButton.GetComponentInChildren<Text>().color = Model.Color;
        message.CancelButton.GetComponentInChildren<Text>().color = Model.Color;

        if (!string.IsNullOrEmpty(Model.FilePath))
        {
            // Set background as white, so image appears correctly (without a color mask) 
            message.MessagePanel.GetComponent<Image>().color = Color.white;
            if (Model.FilePath.StartsWith("http"))
            {
                StartCoroutine(
                GetText((tex)=>
                {
                    message.MessagePanel.GetComponent<Image>().sprite = Sprite.Create(tex,
                        new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }));
            }
            else
            {
                if (File.Exists(Model.FilePath))
                {
                    var tex = LoadImage(new Vector2(300, 300), Model.FilePath);
                    message.MessagePanel.GetComponent<Image>().sprite = Sprite.Create(tex,
                        new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
        }

        message.AcceptButton.onClick.AddListener(() =>
        {
            Model.OnAccept?.Invoke();
            StartCoroutine(FadeOut());
        });

        message.CancelButton.onClick.AddListener(() =>
        {
            Model.OnCancel?.Invoke();
            StartCoroutine(FadeOut());
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static Texture2D LoadImage(Vector2 size, string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;
        texture.LoadImage(bytes);

        return texture;
    }

    private IEnumerator GetText(Action<Texture2D> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(Model.FilePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded image
                var tex = DownloadHandlerTexture.GetContent(uwr);
                callback(tex);
            }
        }
    }

    private IEnumerator FadeOut()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2;
            yield return null;
        }
        Destroy(gameObject);
        Model.Context.Dismissed();
        yield return null;
    }
}
