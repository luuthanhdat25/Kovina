using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundRoadmap : MonoBehaviour
{
    [SerializeField]
    private Transform backgroundFirst;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Texture2D texture2d;

    [SerializeField]
    private Transform endMap1Point;

    [SerializeField]
    private Transform endPoint;

    public Vector2 GetStartPosition() => startPoint.position;

    public Vector2 GetEndPosition() => endPoint.position;
    
    public Vector2 GetEndMap1Position() => endMap1Point.position;

    public Vector2 GetBackroundSize()
    {
        float y = backgroundFirst.localScale.y;
        float x = backgroundFirst.localScale.x;
        /*foreach (Transform background in backgroundList)
        {
            x += background.localScale.x;
        }*/
        return new Vector2 (x, y);
    } 

    public void SetScaleAllBackground(float scaleX, float scaleY, Vector3 centerFirstBackground, float asp)
    {
        var r = scaleY / backgroundFirst.localScale.y;
        backgroundFirst.localScale = new Vector2(scaleY, scaleY);
        backgroundFirst.localPosition = new Vector3(centerFirstBackground.x, centerFirstBackground.y, backgroundFirst.localPosition.z);
        Debug.Log($"Width, Height: {texture2d.width}, {texture2d.height}");
        int deltaWidth = (int)((texture2d.height * scaleX) / scaleY);
        int y = 0;
        int x = (texture2d.width - deltaWidth) / 2;
        Texture2D cropedTexture = CropTexture(texture2d, x, y, deltaWidth, texture2d.height);
        Debug.Log($"After crop Width, Height: {cropedTexture.width}, {cropedTexture.height}");
        //if

        Sprite newSprite = ConvertToSprite(cropedTexture);
        backgroundFirst.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    // Hàm để cắt Texture2D
    Texture2D CropTexture(Texture2D sourceTexture, int x, int y, int width, int height)
    {
        Color[] pixels = sourceTexture.GetPixels(x, y, width, height);

        Texture2D newTexture = new Texture2D(width, height);

        newTexture.SetPixels(pixels);
        newTexture.Apply(); 

        return newTexture;
    }

    // Hàm để chuyển Texture2D thành Sprite
    Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture,
                             new Rect(0, 0, texture.width, texture.height), 
                             new Vector2(0.5f, 0.5f), texture.height); 
    }
}
