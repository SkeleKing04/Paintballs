using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIFunctions : MonoBehaviour
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
        mainHandler.updateSlider(sliders[0],DeathmatchScript.scoreCap,false,true);
        UpdateScoreCap(sliders[0].gameObject);
        ToggleBots(toggles[0].gameObject);
        ToggleTeams(toggles[1].gameObject);
        UpdateTeamSize(sliders[1].gameObject);
    }
    public void UpdateScoreCap(GameObject sender)
    {
        mainHandler.updateSlider(sender.GetComponent<Slider>(),DeathmatchScript.scoreCap,false,true);
        mainHandler.updateTextBox(textBoxes[0],"Score to win (Per Player): " + sliders[0].value);
    }
    public void ToggleBots(GameObject sender)
    {
        mainHandler.updateTogglable(sender.GetComponent<Toggle>(),DeathmatchScript.fillRoomWithBots,false);
    }
    public void ToggleTeams(GameObject sender)
    {
        mainHandler.updateTogglable(sender.GetComponent<Toggle>(),DeathmatchScript.teamDeathmatch, false);
    }
    public void UpdateTeamSize(GameObject sender)
    {
        mainHandler.updateSlider(sender.GetComponent<Slider>(),DeathmatchScript.teamSize,false,true);
        mainHandler.updateTextBox(sender.GetComponent<TextMeshProUGUI>(),"Team Size: " + DeathmatchScript.teamSize);
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
