using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    public UIHandler mainHandler;
    public Slider[] sliders;
    public Toggle[] toggles;
    public TextMeshProUGUI[] textBoxes;
    public DeathmatchScript.gameState startingState;
    // Start is called before the first frame update
    void Start()
    {
        DeathmatchScript.state = startingState; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainHandler.updateSlider(new Slider[] {sliders[0]},new float[] {DeathmatchScript.scoreCap},new bool[] {false});
        UpdateScoreCap(sliders[0].gameObject);
        ToggleBots(toggles[0].gameObject);
        ToggleTeams(toggles[1].gameObject);
        UpdateTeamSize(sliders[1].gameObject);

    }
    public void UpdateScoreCap(GameObject sender)
    {
        mainHandler.updateSlider(new Slider[] {sliders[0]},new float[] {DeathmatchScript.scoreCap},new bool[] {false});
        mainHandler.updateTextBox(new TextMeshProUGUI[] {textBoxes[0]},new string[]{"Score to win (Per Player): " + sliders[0].value});
        textBoxes[0].text = "Score to win (Per Player): " + value;
    }
    public void ToggleBots(GameObject sender)
    {
        bool value = sender.GetComponent<Toggle>().isOn;
        DeathmatchScript.fillRoomWithBots = value;
    }
    public void ToggleTeams(GameObject sender)
    {
        bool value = sender.GetComponent<Toggle>().isOn;
        DeathmatchScript.teamDeathmatch = value;
    }
    public void UpdateTeamSize(GameObject sender)
    {
        int value = (int)sender.GetComponent<Slider>().value;
        DeathmatchScript.teamSize = value;
        textBoxes[1].text = "Team Size: " + value;
    }
    public void LoadNextScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        DeathmatchScript.startGate();
    }
    void Update()
    {
        startingState = DeathmatchScript.state;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
