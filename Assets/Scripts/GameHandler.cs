using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static bool paused = false;
    public static bool stopped = false;
    public static bool converged = false;
    public static bool converged_continue = false;

    // start/stop
    public Button startButton;
    public Button stopButton;

    // pause
    public Button playButton;
    public Button pauseButton;

    // pop size
    public Slider sliderPopSize;
    public Text textPopSize;

    // chromosome size
    public Slider sliderChromosomeSize;
    public Text textChromosomeSize;

    // cutoff percentage
    public Slider sliderCutoff;
    public Text textCutoff;

    // keep elite percentage
    public Slider sliderElite;
    public Text textElite;

    // will mate
    public Slider sliderWillMate;
    public Text textWillMate;

    // mutation rate
    public Slider sliderMutRate;
    public Text textMutRate;

    // alpha
    public Slider sliderAlpha;
    public Text textAlpha;

    // sigma
    public Slider sliderSigma;
    public Text textSigma;

    // min bound
    public Slider sliderMinBound;
    public Text textMinBound;

    // max bound
    public Slider sliderMaxBound;
    public Text textMaxBound;

    // add obstacle button
    public Button addObstacleBtn;
    public GameObject obstaclePrefab;

    // help menu
    public GameObject helpPanel;
    public Canvas parentCanvas;
    GameObject help;
    public Button openHelpBtn;
    public static bool helpIsOpen = false;

    // best menu
    public GameObject bestPanel;
    GameObject best;
    public static bool bestIsOpen = false;
    public static string bestSol = "";

    // current gen
    public Text currGenText;

    private void Start()
    {
        Time.timeScale = 0f;
        paused = true;
        stopped = true;

        // start/stop
        startButton.onClick.AddListener(StartGame);
        stopButton.onClick.AddListener(StopGame);
        stopButton.enabled = false;
        stopButton.gameObject.SetActive(false);

        // pause
        playButton.onClick.AddListener(ResumeGame);
        pauseButton.onClick.AddListener(PauseGame);
        playButton.enabled = false;
        pauseButton.enabled = false;

        playButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (converged)
        {
            PauseGame();
            if (bestIsOpen == false)
            {
                 openBest();
                 bestIsOpen = true;
            }

        }
    }

    private void StartGame()
    {
        if (converged)
        {
            converged = false;
            converged_continue = true;
        }
        pauseButton.enabled = true;

        Time.timeScale = 1f;
        stopped = false;
        paused = false;
        startButton.enabled = false;
        startButton.gameObject.SetActive(false);

        stopButton.enabled = true;
        stopButton.gameObject.SetActive(true);

        addObstacleBtn.enabled = false;
        addObstacleBtn.GetComponent<Image>().color = Color.gray;

        DisableSliders();
    }

    private void StopGame()
    {
        if (converged)
        {
            converged = false;
            converged_continue = false;
        }
        Time.timeScale = 0f;
        stopped = true;
        paused = true;
        stopButton.enabled = false;
        stopButton.gameObject.SetActive(false);

        startButton.enabled = true;
        startButton.gameObject.SetActive(true);

        playButton.enabled = false;
        pauseButton.enabled = false;
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        addObstacleBtn.enabled = true;
        addObstacleBtn.GetComponent<Image>().color = Color.white;

        PopulationController.gen_num = 0;
        currGenText.text = "Gen : 0";

    EnableSliders();
    }

    private void EnableSliders()
    {
        sliderPopSize.enabled = true;
        sliderPopSize.GetComponentInChildren<Image>().color = Color.white;

        sliderChromosomeSize.enabled = true;
        sliderChromosomeSize.GetComponentInChildren<Image>().color = Color.white;
    }

    private void DisableSliders()
    {
        sliderPopSize.enabled = false;
        sliderPopSize.GetComponentInChildren<Image>().color = Color.red;

        sliderChromosomeSize.enabled = false;
        sliderChromosomeSize.GetComponentInChildren<Image>().color = Color.red;
    }

    private void ResumeGame()
    {

        if (converged)
        {
            converged = false;
            converged_continue = true;
        }

        Time.timeScale = 1f;
        paused = false;
        playButton.enabled = false;
        playButton.gameObject.SetActive(false);

        pauseButton.enabled = true;
        pauseButton.gameObject.SetActive(true);

        addObstacleBtn.enabled = false;
        addObstacleBtn.GetComponent<Image>().color = Color.gray;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        paused = true;
        pauseButton.enabled = false;
        pauseButton.gameObject.SetActive(false);

        playButton.enabled = true;
        playButton.gameObject.SetActive(true);

        addObstacleBtn.enabled = true;
        addObstacleBtn.GetComponent<Image>().color = Color.white;
    }

    private void ShowBestSolution(Chromosome c)
    {

    }

    public void updateSliderPopSize(float value)
    {
        textPopSize.text = "Population size = " + Mathf.RoundToInt(value);
    }

    public void updateSliderChromSize(float value)
    {
        textChromosomeSize.text = "Chrom. size = " + Mathf.RoundToInt(value);
    }

    public void updateSliderCutoff(float value)
    {
        textCutoff.text = "Cutoff = " + Math.Round(value, 2);
    }

    public void updateSliderElite(float value)
    {
        textElite.text = "Keep elite = " + Math.Round(value, 2);
    }

    public void updateSliderWillMate(float value)
    {
        textWillMate.text = "Will mate = " + Math.Round(value, 2);
    }

    public void updateSliderMutRate(float value)
    {
        textMutRate.text = "Mut. rate = " + Math.Round(value, 2);
    }

    public void updateSliderAlpha(float value)
    {
        textAlpha.text = "Alpha = " + Math.Round(value, 2);
    }

    public void updateSliderSigma(float value)
    {
        textSigma.text = "Sigma = " + Math.Round(value, 2);
    }

    public void updateSliderMinBound(float value)
    {
        textMinBound.text = "Min bound = " + Math.Round(value, 2);
    }

    public void updateSliderMaxBound(float value)
    {
        textMaxBound.text = "Max bound = " + Math.Round(value, 2);
    }

    public void addObstacle()
    {
        Instantiate(obstaclePrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y / 2, 0), Camera.main.transform.rotation);
    }

    public void doExitGame()
    {
        Application.Quit();
    }

    public void openHelp()
    {
        helpIsOpen = true;
        help = Instantiate(helpPanel);
        help.transform.SetParent(parentCanvas.transform, false);
        help.GetComponentInChildren<Button>().onClick.AddListener(closeHelp);
        openHelpBtn.enabled = false;
        openHelpBtn.GetComponent<Image>().color = Color.gray;

    }

    public void closeHelp()
    {
        helpIsOpen = false;
        help.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        Destroy(help);
        openHelpBtn.enabled = true;
        openHelpBtn.GetComponent<Image>().color = new Color(0.2520469f, 0.5253339f, 0.754717f, 1);
    }

    public void openBest()
    {
        best = Instantiate(bestPanel);
        best.transform.SetParent(parentCanvas.transform, false);
        best.GetComponentInChildren<Button>().onClick.AddListener(closeBest);


        best.GetComponentInChildren<Text>().text = bestSol;
    }

    public void closeBest()
    {
        best.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        Destroy(best);
    }

}
