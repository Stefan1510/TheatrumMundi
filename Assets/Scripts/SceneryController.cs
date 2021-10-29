using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneryController : MonoBehaviour
{
    [SerializeField] private Button _kulissenButton;

    [HideInInspector] public bool selectedScenery = false;
    [HideInInspector] public bool sceneryActive = false;
    [HideInInspector] public float sceneryPositionX = 0.0f;
    [HideInInspector] public float sceneryPositionY = 0.1f;
    private RawImage _kulissenButtonImage;
    private GameObject[] allKulissen;
    private GameObject[] allKulissenButtons;
    [HideInInspector] public Material kulissenMaterial;
    Toggle _toggleSceneryActivate;
    Text _textSceneryname;
    Slider _sliderSceneryPositionX;
    Slider _sliderSceneryPositionY;
    InputField _inputFieldSceneryPositionX;
    InputField _InputFieldSceneryPositionY;

    private void Awake()
    {
        allKulissen = GameObject.FindGameObjectsWithTag("Kulisse");
        allKulissenButtons = GameObject.FindGameObjectsWithTag("KulissenButton");
        kulissenMaterial = GetComponent<MeshRenderer>().material;
        
        //_kulissenButtonImage = _kulissenButton.GetComponent<RawImage>();
        //_kulissenButtonImage.color = new Color(1f, 1f, 1f, 0.4f);

        _toggleSceneryActivate = GameObject.Find("ToggleSceneryActivate").GetComponent<Toggle>();
        _textSceneryname = GameObject.Find("TextSceneryName").GetComponent<Text>();
        _sliderSceneryPositionX = GameObject.Find("SliderSceneryPositionX").GetComponent<Slider>();
        _sliderSceneryPositionY = GameObject.Find("SliderSceneryPositionY").GetComponent<Slider>();
        _inputFieldSceneryPositionX = GameObject.Find("InputFieldSceneryPositionX").GetComponent<InputField>();
        _InputFieldSceneryPositionY = GameObject.Find("InputFieldSceneryPositionY").GetComponent<InputField>();

        gameObject.transform.SetParent(GameObject.Find("Schiene1").transform);
        
        //_kulissenButton.onClick.AddListener(() => ToggleSceneryButton());
        _sliderSceneryPositionX.onValueChanged.AddListener((val) => ChangeSceneryPositionX(val));
        _sliderSceneryPositionY.onValueChanged.AddListener((val) => ChangeSceneryPositionY(val));
        _toggleSceneryActivate.onValueChanged.AddListener((val) => ActivateKulisse(val));
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedScenery = false;
        _textSceneryname.text = "";
        gameObject.SetActive(false);
        _toggleSceneryActivate.isOn = true;

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
            /*foreach (GameObject kulissenButton in allKulissenButtons)
            {
                RawImage kb = kulissenButton.GetComponent<Button>().GetComponent<RawImage>();
                kb.color = new Color(1f, 1f, 1f, kb.color.a);
            }*/
            foreach(GameObject kulisse in allKulissen)
            {
                kulisse.GetComponent<SceneryController>().selectedScenery = false;
            }
            _kulissenButtonImage.color = new Color(0f, 1f, 0f, alpha);
            selectedScenery = true;
            _textSceneryname.text = gameObject.name;
            _toggleSceneryActivate.isOn = gameObject.activeSelf;
            _sliderSceneryPositionX.value = sceneryPositionX;
            _sliderSceneryPositionY.value = sceneryPositionY;
            Debug.Log("Button AN");
        }
        else
        {
            selectedScenery = false;
            //_kulissenButtonImage.color = new Color(1f, 1f, 1f, alpha);
            Debug.Log("Button AUS");
            _textSceneryname.text = "";
        }
    }
    void ActivateKulisse(bool val)
    {
        if (selectedScenery)
        {
            gameObject.SetActive(val);
            sceneryActive = val;
        }
    }
    void ChangeSceneryPositionX(float val)
    {
        Vector3 pos;
        if (selectedScenery)
        {
            pos = gameObject.transform.position;
            sceneryPositionX = val;
            gameObject.transform.position = new Vector3(pos.x, pos.y, val);
            _inputFieldSceneryPositionX.text = val.ToString("0.00");
        }

    }
    void ChangeSceneryPositionY(float val)
    {
        Vector3 pos;
        if (selectedScenery)
        {
            pos = gameObject.transform.position;
            sceneryPositionY = val;
            gameObject.transform.position = new Vector3(pos.x, val, pos.z);
            _InputFieldSceneryPositionY.text = val.ToString("0.00");
        }
    }
}
