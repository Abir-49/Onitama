using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MainMenuSetup
{
    [MenuItem("Onitama/Generate Main Menu")]
    public static void GenerateMainMenu()
    {
        if (EditorApplication.isPlaying) return;

        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "MainMenu";

        Camera.main.backgroundColor = new Color(0.95f, 0.95f, 0.95f); // Light Background
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // -- EventSystem natively configured
        var evtObj = new GameObject("EventSystem");
        evtObj.AddComponent<EventSystem>();
        
        System.Type newInputModule = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (newInputModule != null) evtObj.AddComponent(newInputModule);
        else evtObj.AddComponent<StandaloneInputModule>();

        // -- Canvas configured for Desktop Clarity
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f; // Ensures proportions maintain clarity on Desktop Resizes
        canvasObj.AddComponent<GraphicRaycaster>();

        var managerObj = new GameObject("MainMenuManager");
        var manager = managerObj.AddComponent<MainMenuManager>();

        // Load Default Unity UI Resources so Dropdowns have Scrollings!
        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        uiResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        uiResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        uiResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        uiResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
        uiResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
        uiResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        GameObject CreateUIPanel(string name, Transform parent, Vector2 pos, Vector2 size)
        {
            var pObj = DefaultControls.CreatePanel(uiResources);
            pObj.name = name;
            pObj.transform.SetParent(parent, false);
            var r = pObj.GetComponent<RectTransform>();
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            pObj.GetComponent<Image>().color = new Color(1, 1, 1, 0f); // Transparent
            return pObj;
        }

        GameObject CreateTextObj(string textStr, Transform parent, Vector2 pos, Vector2 size, int fontSize)
        {
            var txtObj = DefaultControls.CreateText(uiResources);
            txtObj.transform.SetParent(parent, false);
            var rect = txtObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            var txt = txtObj.GetComponent<Text>();
            txt.text = textStr;
            txt.fontSize = fontSize;
            txt.color = new Color(0.15f, 0.65f, 0.25f); // Green Text
            txt.alignment = TextAnchor.MiddleCenter;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            return txtObj;
        }

        Dropdown CreateDropdownObj(string name, Transform parent, Vector2 pos, Vector2 size, List<string> options)
        {
            var ddObj = DefaultControls.CreateDropdown(uiResources);
            ddObj.name = name;
            ddObj.transform.SetParent(parent, false);
            var rect = ddObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            
            var dd = ddObj.GetComponent<Dropdown>();
            dd.ClearOptions();
            dd.AddOptions(options);

            // Fix scaling on Dropdown text without hardcoded paths
            foreach (var t in ddObj.GetComponentsInChildren<Text>(true))
            {
                t.fontSize = 24; 
                t.resizeTextForBestFit = true;
                t.color = Color.black;
            }

            return dd;
        }

        InputField CreateInputFieldObj(string name, Transform parent, Vector2 pos, Vector2 size)
        {
            var inputObj = DefaultControls.CreateInputField(uiResources);
            inputObj.name = name;
            inputObj.transform.SetParent(parent, false);
            var rect = inputObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            
            // Fix text without hardcoded paths
            foreach (var t in inputObj.GetComponentsInChildren<Text>(true))
            {
                t.fontSize = 28;
                t.color = Color.black;
            }

            return inputObj.GetComponent<InputField>();
        }

        // --- BUILDING THE UI ---

        // TITLE
        CreateTextObj("ONITAMA SETTINGS", canvasObj.transform, new Vector2(0, 400), new Vector2(600, 100), 70);

        // DELAY SETTING
        CreateTextObj("Delay Per Move (sec):", canvasObj.transform, new Vector2(-200, 250), new Vector2(300, 50), 36);
        manager.delayInput = CreateInputFieldObj("DelayInput", canvasObj.transform, new Vector2(200, 250), new Vector2(250, 60));

        // MODE SETTING
        CreateTextObj("Game Mode:", canvasObj.transform, new Vector2(-200, 150), new Vector2(300, 50), 36);
        manager.modeDropdown = CreateDropdownObj("ModeDropdown", canvasObj.transform, new Vector2(200, 150), new Vector2(250, 60), new List<string>{"Random", "Customized"});

        // EXTRAS PANEL
        var panelObj = CreateUIPanel("CustomCardsPanel", canvasObj.transform, new Vector2(0, -50), new Vector2(1000, 300));
        manager.customCardsPanel = panelObj;

        // Custom Layout Settings
        CreateTextObj("Minimax Cards", panelObj.transform, new Vector2(-300, 140), new Vector2(300, 50), 30);
        manager.minimaxCard1 = CreateDropdownObj("M1", panelObj.transform, new Vector2(-300, 80), new Vector2(250, 60), new List<string>());
        manager.minimaxCard2 = CreateDropdownObj("M2", panelObj.transform, new Vector2(-300, 0), new Vector2(250, 60), new List<string>());

        CreateTextObj("Greedy Cards", panelObj.transform, new Vector2(300, 140), new Vector2(300, 50), 30);
        manager.greedyCard1 = CreateDropdownObj("G1", panelObj.transform, new Vector2(300, 80), new Vector2(250, 60), new List<string>());
        manager.greedyCard2 = CreateDropdownObj("G2", panelObj.transform, new Vector2(300, 0), new Vector2(250, 60), new List<string>());

        CreateTextObj("Extra Card / Switch", panelObj.transform, new Vector2(0, -50), new Vector2(400, 50), 30);
        manager.extraCard = CreateDropdownObj("Extra", panelObj.transform, new Vector2(0, -110), new Vector2(250, 60), new List<string>());

        // PLAY BUTTON
        var btnObj = DefaultControls.CreateButton(uiResources);
        btnObj.name = "PlayButton";
        btnObj.transform.SetParent(canvasObj.transform, false);
        var bRect = btnObj.GetComponent<RectTransform>();
        bRect.anchoredPosition = new Vector2(0, -380);
        bRect.sizeDelta = new Vector2(350, 100);
        btnObj.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.3f);
        
        var bTxt = btnObj.GetComponentInChildren<Text>(true);
        if (bTxt != null)
        {
            bTxt.text = "PLAY";
            bTxt.fontSize = 50;
            bTxt.color = Color.white;
        }
        
        var bBtn = btnObj.GetComponent<Button>();

        var linker = managerObj.AddComponent<MainMenuLinker>();
        linker.manager = manager;
        linker.playButton = bBtn;

        // CRITICAL FIX: Mark our dynamic references clean to the Editor Core before saving
        EditorUtility.SetDirty(managerObj);
        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(linker);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");

        // Attach to Build Settings
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.RemoveAll(s => s.path.Contains("MainMenu.unity") || s.path.Contains("scene.unity"));
        scenes.Insert(0, new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true));
        scenes.Add(new EditorBuildSettingsScene("Assets/scene.unity", true));
        EditorBuildSettings.scenes = scenes.ToArray();

        Debug.Log("Successfully generated Main Menu GUI Using Native DefaultControls. Scaling and scrolling will now be absolutely perfect! Press Play.");
    }
}
