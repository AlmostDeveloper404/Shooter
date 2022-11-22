using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SingleEventSystem : MonoBehaviour
{
    private static SingleEventSystem instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = new GameObject().AddComponent<SingleEventSystem>();
        instance.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(instance.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length == 1)
            return;

        foreach (var eventSystem in eventSystems)
        {
            if (!IsDontDestroyOnLoadActivated(eventSystem))
                Destroy(eventSystem.gameObject);
        }
    }

    private static bool IsDontDestroyOnLoadActivated(Component eventSystem)
    {
        return eventSystem.gameObject.scene.name == "DontDestroyOnLoad";
    }
}
