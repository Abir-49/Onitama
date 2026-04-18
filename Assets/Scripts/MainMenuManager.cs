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
    public Dropdown firstSideDropdown;

    public Text firstSideLabel;
    private readonly List<Dropdown> cardDropdowns = new List<Dropdown>();

    private static readonly string[] availableCards = GameSettings.GetCardNames(GameSettings.PlayableCards);

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
        if (firstSideDropdown == null) firstSideDropdown = FindDeepChild(transform.root, "FirstSideDropdown")?.GetComponent<Dropdown>();
        if (firstSideLabel == null) firstSideLabel = FindDeepChild(transform.root, "FirstSideLabel")?.GetComponent<Text>();
    }

    Transform FindDeepChild(Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        bool searchedSceneRoots = false;

        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName) return c;
            foreach(Transform t in c) queue.Enqueue(t);

            // Also check all root objects in the scene once in case it's not under exactly this root.
            // Without this guard, missing objects can cause an infinite re-queue loop.
            if (!searchedSceneRoots && aParent == transform.root && queue.Count == 0)
            {
                searchedSceneRoots = true;
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
        if (modeDropdown != null)
        {
            modeDropdown.SetValueWithoutNotify(0); // 0 is Random
        }

        BuildCardDropdownList();
        PopulateCardDropdownsFromDefaults();
        RegisterCardDropdownListeners();
        EnsureUniqueSelections(null);
        PopulateFirstSideDropdown();

        if (modeDropdown != null)
        {
            modeDropdown.onValueChanged.AddListener(_ => OnModeChanged());
        }

        OnModeChanged();
    }

    private void BuildCardDropdownList()
    {
        cardDropdowns.Clear();

        if (minimaxCard1 != null) cardDropdowns.Add(minimaxCard1);
        if (minimaxCard2 != null) cardDropdowns.Add(minimaxCard2);
        if (greedyCard1 != null) cardDropdowns.Add(greedyCard1);
        if (greedyCard2 != null) cardDropdowns.Add(greedyCard2);
        if (extraCard != null) cardDropdowns.Add(extraCard);
    }

    private void PopulateCardDropdownsFromDefaults()
    {
        if (minimaxCard1 != null) PopulateDropdownWithPlaceholder(minimaxCard1);
        if (minimaxCard2 != null) PopulateDropdownWithPlaceholder(minimaxCard2);
        if (greedyCard1 != null) PopulateDropdownWithPlaceholder(greedyCard1);
        if (greedyCard2 != null) PopulateDropdownWithPlaceholder(greedyCard2);
        if (extraCard != null) PopulateDropdownWithPlaceholder(extraCard);
    }

    private void PopulateDropdownWithPlaceholder(Dropdown dropdown)
    {
        dropdown.ClearOptions();
        List<string> options = new List<string> { "-- Select a card --" };
        options.AddRange(availableCards);
        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(0); // Default to placeholder
    }

    private void RegisterCardDropdownListeners()
    {
        foreach (Dropdown dropdown in cardDropdowns)
        {
            if (dropdown == null)
            {
                continue;
            }

            Dropdown currentDropdown = dropdown;
            dropdown.onValueChanged.AddListener(_ =>
            {
                EnsureUniqueSelections(currentDropdown);
            });
        }
    }

    private void EnsureUniqueSelections(Dropdown changedDropdown)
    {
        HashSet<string> seen = new HashSet<string>();

        foreach (Dropdown dropdown in cardDropdowns)
        {
            if (dropdown == null)
            {
                continue;
            }

            string selectedName = GetSelectedCardName(dropdown);

            if (string.IsNullOrEmpty(selectedName))
            {
                dropdown.SetValueWithoutNotify(0);
                selectedName = GetSelectedCardName(dropdown);
            }

            bool duplicate = seen.Contains(selectedName);
            if (duplicate && (changedDropdown == null || dropdown == changedDropdown))
            {
                int newIndex = FindFirstAvailableIndex(seen);
                dropdown.SetValueWithoutNotify(newIndex);
                selectedName = GetSelectedCardName(dropdown);
            }

            if (!string.IsNullOrEmpty(selectedName))
            {
                seen.Add(selectedName);
            }
        }
    }

    private int FindFirstAvailableIndex(HashSet<string> blocked)
    {
        for (int i = 0; i < availableCards.Length; i++)
        {
            if (!blocked.Contains(availableCards[i]))
            {
                return i + 1; // +1 for placeholder offset
            }
        }

        return 0; // Return placeholder if all cards blocked
    }

    private string GetSelectedCardName(Dropdown dropdown)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0)
        {
            return null;
        }

        int clampedIndex = Mathf.Clamp(dropdown.value, 0, dropdown.options.Count - 1);
        string selectedText = dropdown.options[clampedIndex].text;

        // Index 0 is placeholder
        if (clampedIndex == 0 || selectedText == "-- Select a card --")
        {
            return null;
        }

        return selectedText;
    }

    private void PopulateFirstSideDropdown()
    {
        if (firstSideDropdown == null)
        {
            return;
        }

        firstSideDropdown.ClearOptions();
        firstSideDropdown.AddOptions(new List<string>
        {
            "Random",
            "Minimax first",
            "Greedy first"
        });

        firstSideDropdown.SetValueWithoutNotify((int)GameSettings.FirstPlayer);
    }

    // Called by the Dropdown OnValueChanged event
    public void OnModeChanged()
    {
        bool isCustomized = modeDropdown != null && modeDropdown.value == 1;

        if (isCustomized)
        {
            GameSettings.CurrentMode = GameMode.Customized;
        }
        else
        {
            GameSettings.CurrentMode = GameMode.Random;
        }

        if (customCardsPanel != null)
        {
            customCardsPanel.SetActive(isCustomized);
        }

        if (firstSideLabel != null)
        {
            firstSideLabel.gameObject.SetActive(isCustomized);
        }

        if (firstSideDropdown != null)
        {
            firstSideDropdown.gameObject.SetActive(isCustomized);
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
            EnsureUniqueSelections(null);

            GameSettings.MinimaxCards[0] = GetSelectedCardName(minimaxCard1);
            GameSettings.MinimaxCards[1] = GetSelectedCardName(minimaxCard2);
            GameSettings.GreedyCards[0] = GetSelectedCardName(greedyCard1);
            GameSettings.GreedyCards[1] = GetSelectedCardName(greedyCard2);
            GameSettings.ExtraCard = GetSelectedCardName(extraCard);

            if (firstSideDropdown != null)
            {
                GameSettings.FirstPlayer = (FirstPlayerChoice)Mathf.Clamp(firstSideDropdown.value, 0, 2);
            }
            else
            {
                GameSettings.FirstPlayer = FirstPlayerChoice.Random;
            }
        }
        else
        {
            GameSettings.FirstPlayer = FirstPlayerChoice.Random;
        }

        // Load the Onitama board logic scene
        SceneManager.LoadScene("scene");
    }
}
