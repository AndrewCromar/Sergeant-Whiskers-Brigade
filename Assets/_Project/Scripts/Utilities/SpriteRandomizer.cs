using UnityEngine;

[ExecuteInEditMode]
public class SpriteRandomizer : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    private void Awake() {
        RandomizeSprite();
    }

    public void RandomizeSprite() {
        if (sprites.Length > 0 && spriteRenderer != null) {
            int randomIndex = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[randomIndex];
        }
    }
}
