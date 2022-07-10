using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject galleryCanvas;
    [SerializeField] private GameObject waitMeshesCanvas;
    [SerializeField] private GameObject tapToPlaceCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject powerUpCanvas;
    [SerializeField] private GameObject pointsLifeCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject radar;

    [SerializeField] private GameObject startResumeButton;

    public static UIManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        GameManager.instance.OnMinimumMeshesFound += ShowTapToPlaceUI;
        GameManager.instance.OnEggLaid += HideTapToPlaceUI;
        GameManager.instance.OnSnakeBorn += ShowPrincipalMenu;
        GamePlayManager.instance.OnPowerUp += HidePowerUpApple;
        GameManager.instance.OnGameOver += GaveOver;
        mainMenuCanvas.SetActive(true);
        Debug.Log("Main Menu");
    }

    public void Update()
    {
        if (!controlsCanvas.activeSelf)
            return;
        CountPoints();

    }
    private void ShowMeshGeneratorUI()
    {
        waitMeshesCanvas.SetActive(true);
    }

    private void ShowTapToPlaceUI()
    {
        waitMeshesCanvas.SetActive(false);
        tapToPlaceCanvas.SetActive(true);
    }

    private void HideTapToPlaceUI()
    {
        tapToPlaceCanvas.SetActive(false);
    }

    private void ShowPrincipalMenu()
    {
        tapToPlaceCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        pointsLifeCanvas.SetActive(true);
    }

    private void CountPoints()
    {
        pointsLifeCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GamePlayManager.instance.Points.ToString();
        pointsLifeCanvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = GamePlayManager.instance.LifeCount.ToString();
    }

    private void HidePowerUpApple()
    {
        powerUpCanvas.transform.GetChild(0).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(1).gameObject.SetActive(false);
        powerUpCanvas.SetActive(false);
    }

    public void ShowPowerUpApple()
    {

        if (GamePlayManager.instance.IsPowerUpAppleActivated)
        {
            powerUpCanvas.transform.GetChild(2).gameObject.SetActive(false);
            powerUpCanvas.transform.GetChild(3).gameObject.SetActive(false);
            powerUpCanvas.SetActive(true);
            powerUpCanvas.transform.GetChild(0).gameObject.SetActive(true);
            powerUpCanvas.transform.GetChild(1).gameObject.SetActive(true);
        }

    }

    public void ShowPowerUpGrass(int grass) 
    {
        powerUpCanvas.transform.GetChild(2).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(3).gameObject.SetActive(false);
        powerUpCanvas.SetActive(true);
        powerUpCanvas.transform.GetChild(4).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(5).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = "X" + grass;

        StartCoroutine(HideGrassHelmentUI(4, 5));
    }

    public void ShowPowerUpHelmet() 
    {
        powerUpCanvas.SetActive(true);
        powerUpCanvas.transform.GetChild(2).gameObject.SetActive(true);
        powerUpCanvas.transform.GetChild(3).gameObject.SetActive(true);
        StartCoroutine(HideGrassHelmentUI(2, 3));
    }

    IEnumerator HideGrassHelmentUI(int GOone, int GOtwo) 
    {
        yield return new WaitForSeconds(2.0f);


        powerUpCanvas.transform.GetChild(GOone).gameObject.SetActive(false);
        powerUpCanvas.transform.GetChild(GOtwo).gameObject.SetActive(false);
        powerUpCanvas.SetActive(false);

    }

    public void GaveOver() 
    {
        controlsCanvas.SetActive(false);
        pointsLifeCanvas.SetActive(false);
        radar.SetActive(false);
        gameOverCanvas.SetActive(true);
    }
    
    public void ShowGallery() 
    {
        galleryCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }

    public void TurnOffCanvas() 
    {
        controlsCanvas.SetActive(false);
        pointsLifeCanvas.SetActive(false);
        radar.SetActive(false);
    }

    public void ActivateGalleryCanvas() 
    {

        galleryCanvas.SetActive(true);
        startResumeButton.SetActive(true);
    }

}
