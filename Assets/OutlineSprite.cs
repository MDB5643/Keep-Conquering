using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OutlineSprite : MonoBehaviour
{
        public float outlineWidth = 0.1f;
        private Material outlineMaterial;

        private void Start()
        {
            // Create a copy of the sprite's material and assign it
            if (GetComponent<SpriteRenderer>())
        {
            outlineMaterial = new Material(GetComponent<SpriteRenderer>().material);
            GetComponent<SpriteRenderer>().material = outlineMaterial;
        }
        // Create a copy of the sprite's material and assign it
        if (GetComponent<TilemapRenderer>())
        {
            outlineMaterial = new Material(GetComponent<TilemapRenderer>().material);
            GetComponent<TilemapRenderer>().material = outlineMaterial;
        }

        // Set the outline color and width
        outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
            outlineMaterial.SetColor("_OutlineColor", Color.black);
        }
}
