using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogManager : MonoBehaviour
{
    public GameObject DialogPanel;

    public TextMeshProUGUI textDialog;

    Story currentStory;

    [Header("Choice UI")]

    public GameObject[] choices;

    TextMeshProUGUI[] choicesText;

    public static DialogManager Instance;

    public bool isDialogPlaying { get; private set; }

    [Header("Tags")]

    public TextMeshProUGUI nameText;

    const string Speaker_Tag = "speaker";

    const string Portrait_Tag = "portrait";

    const string Layout_Tag = "layout";

    public Animator portraitAnimator;

    public Animator layoutAnimator;

    public static DialogManager GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("There's more than one Dialog Manager in the scene");

        Instance = this;
    }

    private void Start()
    {
        isDialogPlaying = false;
        DialogPanel.SetActive(false);

        //get all of the choice text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogWarning("There are more choices than what the UI can manage");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        StartCoroutine(ChooseFirstChoice());
    }


    private void Update()
    {
        //Convert this into an event and use listener for better performnce
        if (!isDialogPlaying) return;

        if(currentStory.currentChoices.Count == 0 && PlayerMovement.GetInstance().GetInteractPressed())
        {
            ContinueDialog();
        }
    }

    private void ContinueDialog()
    {
        if (currentStory.canContinue)
        {
            textDialog.text = currentStory.Continue();
        }

        else
            ExitDialog();
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(":");

            if(splitTag.Length != 2)
            {
                Debug.LogError("String could not be appropriately parse");
            }

            //use Trim to get rid of any whitespace
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch(tagKey)
            {
                case Speaker_Tag:
                    nameText.text = tagValue;
                    break;
                case Portrait_Tag:
                    portraitAnimator.Play(tagValue);
                    break;
                case Layout_Tag:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Unknown tag: " + tag);
                    break;
            }
        }
    }

    public void EnterDialog(TextAsset JsonInk)
    {
        currentStory = new Story(JsonInk.text);
        isDialogPlaying = true;
        DialogPanel.SetActive(true);

        if (currentStory.canContinue)
            textDialog.text = currentStory.Continue();

        else
            ExitDialog();

        DisplayChoices();

        HandleTags(currentStory.currentTags);
    }

    public void ExitDialog()
    {
        DialogPanel.SetActive(false);
        isDialogPlaying = false;
        textDialog.text = "";
    }

    IEnumerator ChooseFirstChoice()
    {
        //Event System require clearing first before using 
        //then wait for atlease one frame before select the current object
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);

    }

    public void Decide(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        PlayerMovement.GetInstance().GetInteractPressed();

        // Disable the choice buttons.
        foreach (GameObject choice in choices)
        {
            choice.gameObject.SetActive(false);
        }

        ContinueDialog();
    }
}
