using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundRoadmapGenerater : MonoBehaviour
{
    [SerializeField]
    private List<BackgroundRoadmap> backgroundRoadmapList;

    [SerializeField]
    private CloudSpawner cloudSpawner;

    public Vector2 GetStartPosition() => backgroundRoadmapList[0].GetStartPosition();

    public Vector2 GetEndPosition() => backgroundRoadmapList[backgroundRoadmapList.Count - 1].GetEndPosition();
    
    public Vector2 GetEndMap1Position() => backgroundRoadmapList[0].GetEndPosition();

    public void SetPosition(Vector3 position) => transform.position = position;

    public void SetupAllBackground(Vector2 cameraScaleWorldSpace, float cameraAspect)
    {
        foreach (var background in backgroundRoadmapList)
        {
            CropAndScaleBackgrounds(cameraScaleWorldSpace, cameraAspect, background);
            
            var startX = background.GetStartPosition().x + background.GetEndPosition().x;
            background.SetPosition(new Vector3(startX/2, background.transform.position.y, background.transform.position.z));
            
            cloudSpawner.SetupCloud(background.GetEndPosition(), cameraScaleWorldSpace.y + 0.5f);
        }
    }

    private void CropAndScaleBackgrounds(Vector2 cameraScaleWorldSpace, float cameraAspect, BackgroundRoadmap backgroundRoadmap)
    {
        Texture2D textureOfSprite = backgroundRoadmap.SpriteBackground.texture;
        float spriteRatio = (float)textureOfSprite.width / textureOfSprite.height;
        Vector3 backgroundScale ;

        if (cameraAspect <= spriteRatio)
        {
            backgroundScale = new Vector2(cameraScaleWorldSpace.y, cameraScaleWorldSpace.y);

            int widthNeedCut = (int)((textureOfSprite.height * cameraScaleWorldSpace.x) / cameraScaleWorldSpace.y);
            int y = 0;
            int x = (textureOfSprite.width - widthNeedCut) / 2;

            Texture2D cropedTexture = CropTexture(textureOfSprite, x, y, widthNeedCut, textureOfSprite.height);

            Sprite newSprite = ConvertToSprite(cropedTexture, backgroundRoadmap.SpriteBackground.pixelsPerUnit);
            backgroundRoadmap.SetSprite(newSprite);
        }
        else
        {
            float newScaleVale = cameraScaleWorldSpace.x / spriteRatio;
            backgroundScale = new Vector2(newScaleVale, newScaleVale);
        }

        backgroundRoadmap.SetScale(backgroundScale);
    }

    private Texture2D CropTexture(Texture2D sourceTexture, int x, int y, int width, int height)
    {
        Color[] pixels = sourceTexture.GetPixels(x, y, width, height);

        Texture2D newTexture = new Texture2D(width, height);

        newTexture.SetPixels(pixels);
        newTexture.Apply(); 

        return newTexture;
    }

    private Sprite ConvertToSprite(Texture2D texture, float pixelPerUnit)
    {
        return Sprite.Create(texture,
                             new Rect(0, 0, texture.width, texture.height), 
                             new Vector2(0.5f, 0.5f), pixelPerUnit); 
    }
}
