using Cloth3D;
using UnityEngine;

public enum FadeStatus {
    FadeIn,
    FadeWaiting,
    FadeOut
}

public class Splash : MonoBehaviour {
    public string levelToLoad;
    private float m_alpha;
    private readonly float m_fadeSpeed;
    private SpriteRenderer m_splashSpriteRenderer;
    private FadeStatus m_status;
    private readonly float m_waitTime;
    public Sprite splashSprite;
    public float timeFadingInFinished;
    public bool waitForInput;

    public Splash() {
        levelToLoad = "";
        m_fadeSpeed = 0.3f;
        m_waitTime = 0.5f;
        m_status = FadeStatus.FadeIn;
    }

    private void Awake() {
        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    private void Start() {
        if (Application.levelCount <= 1 || levelToLoad == "") {
            LogSystem.Warn("Invalid levelToLoad value.");
        }
        var m_splashSpriteGO = new GameObject("SplashSprite");
        m_splashSpriteGO.AddComponent<SpriteRenderer>();
        m_splashSpriteRenderer = m_splashSpriteGO.GetComponent<SpriteRenderer>();
        m_splashSpriteRenderer.sprite = splashSprite;
        var m_splashSpriteTransform = m_splashSpriteGO.gameObject.transform;
        m_splashSpriteTransform.position = new Vector2(0f, 0f);
        m_splashSpriteTransform.parent = transform;
    }

    // LogicTick is called once per frame
    private void Update() {
        var fadeStatus = m_status;
        if (fadeStatus == FadeStatus.FadeIn) {
            m_alpha += m_fadeSpeed*Time.deltaTime;
        } else if (fadeStatus == FadeStatus.FadeWaiting) {
            if ((!waitForInput && Time.time >= timeFadingInFinished + m_waitTime) || (waitForInput && Input.anyKey)) {
                m_status = FadeStatus.FadeOut;
            }
        } else if (fadeStatus == FadeStatus.FadeOut) {
            m_alpha -= m_fadeSpeed*Time.deltaTime;
        }
        UpdateSplashAlpha();
    }

    private void UpdateSplashAlpha() {
        if (m_splashSpriteRenderer != null) {
            var spriteColor = m_splashSpriteRenderer.material.color;
            spriteColor.a = m_alpha;
            m_splashSpriteRenderer.material.color = spriteColor;
            if (m_alpha > 1f) {
                m_status = FadeStatus.FadeWaiting;
                timeFadingInFinished = Time.time;
                m_alpha = 1f;
            }
            if (m_alpha < 0) {
                if (Application.levelCount >= 1 && levelToLoad != "") {
                    Application.LoadLevel(levelToLoad);
                }
            }
        }
    }
}