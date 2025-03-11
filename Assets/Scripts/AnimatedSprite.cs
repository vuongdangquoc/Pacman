using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimateSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer {  get; private set; }
    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int animationFrame { get; private set; }
    public bool loop = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(Advance), animationTime, animationTime);    
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Advance()
    {
        if (!spriteRenderer.enabled)
        {
            return;
        }
        animationFrame++;

        if(animationFrame >= sprites.Length && loop)
        {
            animationFrame = 0;
        }
        if (animationFrame >= 0 && animationFrame<  sprites.Length)
        {
            spriteRenderer.sprite = sprites[animationFrame];
        }
    }

    public void Restart()
    {
        this.animationFrame = -1;
        print(this.gameObject);
        Advance();
    }
}
