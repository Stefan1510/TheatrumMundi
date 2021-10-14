using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneryController : MonoBehaviour
{
    [SerializeField] private Button _kulissenButton;

    [HideInInspector] public bool selectedScenery = false;
    [HideInInspector] public bool SceneryActive = false;
    [HideInInspector] public float SceneryPositionX = -0.1f;
    [HideInInspector] public float SceneryPositionY = 0.1f;
    private RawImage _kulissenButtonImage;
    private GameObject[] allKulissen;
    private GameObject[] allKulissenButtons;
    [HideInInspector] public Material kulissenMaterial;
    Toggle _toggleSceneryActivate;
    Text _textSceneryname;
    Slider _sliderSceneryPositionX;
    Slider _sliderSceneryPositionY;

    private void Awake()
    {
        allKulissen = GameObject.FindGameObjectsWithTag("Kulisse");
        allKulissenButtons = GameObject.FindGameObjectsWithTag("KulissenButton");
        kulissenMaterial = GetComponent<MeshRenderer>().material;
        
        _kulissenButtonImage = _kulissenButton.GetComponent<RawImage>();
        _kulissenButtonImage.color = new Color(1f, 1f, 1f, 0.4f);

        _toggleSceneryActivate = GameObject.Find("ToggleSceneryActivate").GetComponent<Toggle>();
        _textSceneryname = GameObject.Find("TextSceneryName").GetComponent<Text>();
        _sliderSceneryPositionX = GameObject.Find("SliderSceneryPositionX").GetComponent<Slider>();
        _sliderSceneryPositionY = GameObject.Find("SliderSceneryPositionY").GetComponent<Slider>();

        gameObject.transform.SetParent(GameObject.Find("Schiene1").transform);
        
        _kulissenButton.onClick.AddListener(() => ToggleSceneryButton());
        _toggleSceneryActivate.onValueChanged.AddListener((val) => ActivateKulisse(val));
        _sliderSceneryPositionX.onValueChanged.AddListener((val) => ChangeSceneryPositionX(val));
        _sliderSceneryPositionY.onValueChanged.AddListener((val) => ChangeSceneryPositionY(val));
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedScenery = false;
        _textSceneryname.text = "";
        gameObject.SetActive(false);
        //_toggleSceneryActivate.isOn = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ToggleSceneryButton()
    {
        float alpha = _kulissenButtonImage.color.a;
        if (!selectedScenery)
        {
            foreach (GameObject kulissenButton in allKulissenButtons)
            {
                RawImage kb = kulissenButton.GetComponent<Button>().GetComponent<RawImage>();
                kb.color = new Color(1f, 1f, 1f, kb.color.a);
            }
            foreach(GameObject kulisse in allKulissen)
            {
                kulisse.GetComponent<SceneryController>().selectedScenery = false;
            }
            _kulissenButtonImage.color = new Color(0f, 1f, 0f, alpha);
            selectedScenery = true;
            _textSceneryname.text = gameObject.name;
            _toggleSceneryActivate.isOn = gameObject.activeSelf;
            _sliderSceneryPositionX.value = SceneryPositionX;
            _sliderSceneryPositionY.value = SceneryPositionY;
            Debug.Log("Button AN");
        }
        else
        {
            selectedScenery = false;
            _kulissenButtonImage.color = new Color(1f, 1f, 1f, alpha);
            Debug.Log("Button AUS");
            _textSceneryname.text = "";
        }
    }
    void ActivateKulisse(bool val)
    {
        if (selectedScenery)
            gameObject.SetActive(val);
    }
    void ChangeSceneryPositionX(float val)
    {
        Vector3 pos;
        if (selectedScenery)
        {
            pos = gameObject.transform.position;
            SceneryPositionX = val;
            gameObject.transform.position = new Vector3(pos.x, pos.y, val);
        }

    }
    void ChangeSceneryPositionY(float val)
    {

    }
}
