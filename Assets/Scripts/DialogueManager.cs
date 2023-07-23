using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;

    public static DialogueManager singleton;

    [SerializeField] Transform dialogueBoxTransform;

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
        }
        singleton = this;
    }

    internal void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conv");

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        
        nameText.text = dialogue.name;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        this.dialogueText.text = sentence;
    }

    private void EndDialogue()
    {
        dialogueBoxTransform.gameObject.SetActive(false);
    }

    public Queue<string> sentences;



    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
