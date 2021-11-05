using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;

public class DialogueCaller : MonoBehaviour
{
    public string speakerName;

    //text object where the main text will appear
    public TextMeshProUGUI textBody;
    //where name of speaker will appear
    public TextMeshProUGUI speakerText;

    //object to indicate the text has stopped animating
    public GameObject nextText;
    //objects to be activated/deactivated with dialogues
    public GameObject[] dialogueUI;

    //array of dialogues that can be started by this caller
    public Dialogue[] dialogues;

    //true while waiting for the player to ask for next line
    private bool waitingNext = false;
    //true while and if dialogue has options that player needs to choose
    private bool waitingAnswer = false;

    public Button choiceButtonPrefab;

    public Transform choiceParent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            NextText();
    }

    //calls dialogue in the array provided of index textID
    public async void StartDialogue(int textId)
    {
        for (int i = 0; i < dialogueUI.Length; i++)
        {
            dialogueUI[i].SetActive(true);
        }
        speakerText.text = speakerName;
        //animate each text line in the dialogue
        for (int i = 0; i < dialogues[textId].textSequences.Length; i++)
        {
            await DisplayText(dialogues[textId].textSequences[i]);
            waitingNext = true;
            nextText.SetActive(true);
            while (waitingNext)
                await Task.Yield();
        }
        //if dialogue has multiple choices, spawn buttons for each one and wait for the player to choose
        for (int i = 0; i < dialogues[textId].multipleChoices.Length; i++)
        {
            int j = i;
            choiceParent.gameObject.SetActive(true);
            Button choice = Instantiate(choiceButtonPrefab, choiceParent);
            choice.GetComponentInChildren<TextMeshProUGUI>().text = dialogues[textId].multipleChoices[i];
            choice.onClick.AddListener(() => ChooseOption(textId, j));
            waitingAnswer = true;
        }
        while (waitingAnswer)
            await Task.Yield();
        //call end event and close dialogue
        dialogues[textId].endEvent.Invoke();
        CloseDialogue();
    }

    public void ChooseOption(int dialogueId, int choiceID)
    {
        waitingAnswer = false;
        dialogues[dialogueId].choiceEvents[choiceID].Invoke();
    }

    public void CloseDialogue()
    {
        for (int i = choiceParent.childCount - 1; i >= 0; i--)
        {
            Destroy(choiceParent.GetChild(i).gameObject);
        }
        choiceParent.gameObject.SetActive(false);
        for (int i = 0; i < dialogueUI.Length; i++)
        {
            dialogueUI[i].SetActive(false);
        }
        waitingNext = false;
    }

    public void NextText()
    {
        waitingNext = false;
        nextText.SetActive(false);
    }

    private async Task DisplayText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                textBody.text = text;
                break;
            }
            textBody.text = text.Substring(0, i + 1);
            await Task.Yield();
        }
    }
}

[System.Serializable]
public class Dialogue
{
    [Tooltip("Lines the dialogue will display")]
    public string[] textSequences;
    [Tooltip("Text that represent each choice that comes after the lines. Must be of equal size to choice events")]
    public string[] multipleChoices;
    [Tooltip("Events that play when matching choice button is pressed. Must be of equal size to multiple choices")]
    public UnityEvent[] choiceEvents;
    public UnityEvent endEvent;
}
