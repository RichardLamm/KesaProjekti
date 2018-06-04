using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CreateTexture : MonoBehaviour
{
    public int textureWidth = 128;
    public int textureHeight = 128;
    public int mapWidth = 640;
    public int mapHeight = 480;
    public int tileSize = 128;
    public float scale = 150f;
    public float animationSpeed = 1f;
    private float offset;
    SpriteRenderer render;

    // Use this for initialization
    void Start()
    {
        CreateWater();
    }

    // Update is called once per frame
    void Update()
    {
        offset += Time.deltaTime * animationSpeed;
        CreateWater();
    }

    void CreateWater()
    {
        render = gameObject.GetComponent<SpriteRenderer>();

        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        for (int i = 0; i < textureWidth; i++)
        {
            for (int j = 0; j < textureHeight; j++)
            {
                Color color = CalculateColor(i, j);
                texture.SetPixel(i, j, color);
            }
        }

        texture.Apply();

        Sprite newSprite = Sprite.Create(texture, new Rect(-mapWidth / 2 * tileSize, -mapHeight / 2 * tileSize, mapWidth / 2 * tileSize, mapHeight / 2 * tileSize), Vector2.one * 0.5f);

        render.sprite = newSprite;
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / textureWidth * scale + offset;
        float yCoord = (float)y / textureHeight * scale + offset;

        float value = Mathf.PerlinNoise(xCoord, yCoord) - 0.35f;
        if (value < 0) { value = 0f; }
        return new Color(value, value, 1);
    }
}
