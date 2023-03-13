using UnityEngine;
using UnityEngine.UI;

public class Flyer : MonoBehaviour
{
    [SerializeField] ObjectShelfAll objShelfAll;
    // void Start()
    // {

    // }

    // void Update()
    // {

    // }

    public void OnClick()
    {
        // disable 'figures in schiene ziehen'
        SceneManaging.flyerActive = true;

        objShelfAll.ButtonShelf05(false);
        gameObject.SetActive(true);
    }
    public void ClickCancel()
    {
        SceneManaging.flyerActive = false;
        gameObject.SetActive(false);
    }
}
