using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExamplesViewer : MonoBehaviour
{
    [SerializeField] 
    private Dropdown m_dropdown;
    [SerializeField]
    private string[] m_scenes;

    private string currentScene = null;
    private int indexSelected = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_dropdown.onValueChanged.AddListener(OnSceneSelected);
    }


    private void OnSceneSelected(int index)
    {
        indexSelected = index - 1;
        if (currentScene != null)
        {
            var ao = SceneManager.UnloadSceneAsync(currentScene);
            ao.completed += Ao_completed;
        }
        else
        {
            Ao_completed(null);
        }
        
    }

    private void Ao_completed(AsyncOperation obj)
    {
        if (indexSelected == -1)
        {
            currentScene = null;
            return;
        }
        SceneManager.LoadScene(m_scenes[indexSelected], LoadSceneMode.Additive);
        currentScene = m_scenes[indexSelected];
    }
}
