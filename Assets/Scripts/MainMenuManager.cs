using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public InputField delayInput;
    public Dropdown modeDropdown;
    
    public GameObject customCardsPanel;
    
    public Dropdown minimaxCard1;
    public Dropdown minimaxCard2;
    public Dropdown greedyCard1;
    public Dropdown greedyCard2;
    public Dropdown extraCard;

    private string[] availableCards = new string[] { "Dragon", "Horse", "Mantis", "Ox", "Rabbit" };

    void Awake()
    {
        // Safe fallback in case Inspector references drop
        if (delayInput == null) delayInput = FindDeepChild(transform.root, "DelayInput")?.GetComponent<InputField>();
        if (modeDropdown == null) modeDropdown = FindDeepChild(transform.root, "ModeDropdown")?.GetComponent<Dropdown>();
        
        if (customCardsPanel == null)
        {
            var p = FindDeepChild(transform.root, "CustomCardsPanel");
            if (p != null) customCardsPanel = p.gameObject;
        }
        
        if (minimaxCard1 == null) minimaxCard1 = FindDeepChild(transform.root, "M1")?.GetComponent<Dropdown>();
        if (minimaxCard2 == null) minimaxCard2 = FindDeepChild(transform.root, "M2")?.GetComponent<Dropdown>();
        if (greedyCard1 == null) greedyCard1 = FindDeepChild(transform.root, "G1")?.GetComponent<Dropdown>();
        if (greedyCard2 == null) greedyCard2 = FindDeepChild(transform.root, "G2")?.GetComponent<Dropdown>();
        if (extraCard == null) extraCard = FindDeepChild(transform.root, "Extra")?.GetComponent<Dropdown>();
    }

    Transform FindDeepChild(Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName) return c;
            foreach(Transform t in c) queue.Enqueue(t);

            // Also check all root objects in the scene just in case it's not under exactly this root
            if (aParent == transform.root && queue.Count == 0)
            {
                foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (go.transform != aParent) 
                    {
                        if (go.name == aName) return go.transform;
                        foreach(Transform child in go.transform) queue.Enqueue(child);
                    }
                }
            }
        }
        return null;
    }

    void Start()
    {
        if (delayInput != null) delayInput.text = "1.0";
        if (modeDropdown != null) modeDropdown.value = 0; // 0 is Random
        
        // Setup initial default cards
        if (minimaxCard1 != null) PopulateDropdown(minimaxCard1, "Dragon");
        if (minimaxCard2 != null) PopulateDropdown(minimaxCard2, "Horse");
        if (greedyCard1 != null) PopulateDropdown(greedyCard1, "Mantis");
        if (greedyCard2 != null) PopulateDropdown(greedyCard2, "Ox");
        if (extraCard != null) PopulateDropdown(extraCard, "Rabbit");

        if (modeDropdown != null) OnModeChanged();
    }

    private void PopulateDropdown(Dropdown dropdown, string defaultCard)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(availableCards));
        dropdown.value = System.Array.IndexOf(availableCards, defaultCard);
    }

    // Called by the Dropdown OnValueChanged event
    public void OnModeChanged()
    {
        if (modeDropdown.value == 0) // Random Mode
        {
            customCardsPanel.SetActive(false);
            GameSettings.CurrentMode = GameMode.Random;
        }
        else // Customized Mode
        {
            customCardsPanel.SetActive(true);
            GameSettings.CurrentMode = GameMode.Customized;
        }
    }

    // Called by the Play Button
    public void OnPlayClicked()
    {
        if (float.TryParse(delayInput.text, out float delay))
        {
            GameSettings.DelayPerMove = delay;
        }

        if (GameSettings.CurrentMode == GameMode.Customized)
        {
            GameSettings.MinimaxCards[0] = availableCards[minimaxCard1.value];
            GameSettings.MinimaxCards[1] = availableCards[minimaxCard2.value];
            GameSettings.GreedyCards[0] = availableCards[greedyCard1.value];
            GameSettings.GreedyCards[1] = availableCards[greedyCard2.value];
            GameSettings.ExtraCard = availableCards[extraCard.value];
        }

        // Load the Onitama board logic scene
        SceneManager.LoadScene("scene");
    }
}
