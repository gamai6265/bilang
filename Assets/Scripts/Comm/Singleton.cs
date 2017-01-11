using UnityEngine;

namespace Cloth3D{
    public abstract class Singleton<T> where T : new() {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
    }

    public class UnitySingleton<T> : MonoBehaviour
        where T : Component {
        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType(typeof (T)) as T;
                    if (_instance == null) {
                        var obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        _instance = (T) obj.AddComponent(typeof (T));
                    }
                }
                return _instance;
            }
        }

        public virtual void Awake() {
            DontDestroyOnLoad(gameObject);
            if (_instance == null) {
                _instance = this as T;
            } else {
                Destroy(gameObject);
            }
        }
    }
}