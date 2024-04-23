using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerImageLoad : MonoBehaviour
{
    public Image img;
    public RawImage rawImg;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TextureLoad());
    }

    IEnumerator TextureLoad()
    {
        string url = "https://previews.123rf.com/images/manoodsen/manoodsen2201/manoodsen220100348/181291331-%EA%B7%80%EC%97%AC%EC%9A%B4-%EB%8F%99%EB%AC%BC-%EB%A7%8C%ED%99%94-%EC%BA%90%EB%A6%AD%ED%84%B0-%EB%94%94%EC%9E%90%EC%9D%B8%EC%9D%98-%EB%B2%A1%ED%84%B0-%EA%B7%B8%EB%A6%BC-illustrator-%EC%84%B8%ED%8A%B8-%ED%86%A0%EB%81%BC-%EC%8B%9C%EB%B0%94%EA%B2%AC-%EA%B3%B0-%EC%86%90%EC%9C%BC%EB%A1%9C-%EA%B7%B8%EB%A6%B0-%EC%8A%A4%ED%8B%B0%EC%BB%A4-%EA%B2%A9%EB%A6%AC-image-kawaii-kid-%EA%B7%B8%EB%9E%98%ED%94%BD-%EC%8A%A4%EB%A7%88%EC%9D%BC-%EC%96%BC%EA%B5%B4-zoo.jpg";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
            rawImg.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        else
        {
            Debug.Log(www.error);
        }
    }

}
