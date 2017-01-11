using System.IO;
using UnityEngine;

/// <summary>
/// 屏幕截屏
/// </summary>
public class ScreenShotControl :MonoBehaviour {
    public Camera ShotCamera;
    public GameObject DirectionalLight;
    void Awake() {
        //ShotCamera = GetComponent<Camera>();
    }
    
    public Texture2D CaptureCamera(Vector3 pos, Vector3 targetPos, Vector3 upDir, Rect rect) {
        Quaternion rot = Quaternion.LookRotation(targetPos-pos, upDir);
        ShotCamera.transform.position = pos;
        ShotCamera.transform.rotation = rot;
        ShotCamera.gameObject.SetActive(true);
        RenderTexture rt = new RenderTexture((int) rect.width, (int) rect.height, 0);
        ShotCamera.targetTexture = rt;
        ShotCamera.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        ShotCamera.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(rt);

        
        //screenShot.Compress(false);
//        byte[] bytes = screenShot.EncodeToJPG();
//        File.WriteAllBytes(Application.persistentDataPath + "/" +  "screenShot.jpg", bytes);
//        Debug.Log("xxxx:"+ Application.persistentDataPath);
        ShotCamera.gameObject.SetActive(false);
        //EventCenter.Instance.Broadcast(EventId.ScreenShotDone);
        return screenShot;
    }

    public void LightOn(bool tof) {
        DirectionalLight.SetActive(tof);
    }
}