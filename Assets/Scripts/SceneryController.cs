using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneryController : MonoBehaviour
{
    [SerializeField] private Button _kulissenButton;

    private bool _selectedScenery = false;
    private RawImage _kulissenButtonImage;
    private GameObject[] allKulissen;
    [HideInInspector] public Material kulissenMaterial;
    Toggle _toggleSceneryActivate;
    Text _textSceneryname;

    private void Awake()
    {
        allKulissen = GameObject.FindGameObjectsWithTag("Kulisse");
        kulissenMaterial = GetComponent<MeshRenderer>().material;
        _kulissenButtonImage = _kulissenButton.GetComponent<RawImage>();
        _kulissenButtonImage.color = new Color(1f, 1f, 1f, 0.4f);
        _toggleSceneryActivate = GameObject.Find("ToggleSceneryActivate").GetComponent<Toggle>();
        _textSceneryname = GameObject.Find("TextSceneryName").GetComponent<Text>();
        gameObject.transform.SetParent(GameObject.Find("Schiene1").transform);
        _kulissenButton.onClick.AddListener(() => toggleSceneryButton());
    }

    // Start is called before the first frame update
    void Start()
    {
        _selectedScenery = false;
        _textSceneryname.text = "";
        gameObject.SetActive(false);
        //_toggleSceneryActivate.isOn = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void toggleSceneryButton()
    {
        float alpha = _kulissenButtonImage.color.a;
        if (!_selectedScenery)
        {
            _kulissenButtonImage.color = new Color(0f, 1f, 0f, alpha);
            _selectedScenery = true;
            _textSceneryname.text = gameObject.name;
            Debug.Log("Button AN");
        }
        else
        {
            _selectedScenery = false;
            _kulissenButtonImage.color = new Color(1f, 1f, 1f, alpha);
            Debug.Log("Button AUS");
            _textSceneryname.text = "";
        }
    }
}
