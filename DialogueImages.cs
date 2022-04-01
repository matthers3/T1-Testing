using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueImages : MonoBehaviour
{
    [SerializeField] private List<Image> portraits = default;

    [Range(0.0f, 255.0f)]
    [SerializeField] private float shadeIntensity = 135.0f;
    private AudioClip voice;

    void Start()
    {
        foreach (Image portrait in portraits)
        {
            portrait.enabled = false;
            portrait.sprite = null;
        }
    }

    [YarnCommand("clear_portraits")]
    public void ClearPortraits()
    {
        List<string> iter = new List<string>() {"1", "2", "3"};
        foreach (string index in iter)
        {
            ClearPosition(index);
        }
    }

    [YarnCommand("clear_position")]
    public void ClearPosition(string position)
    {
        int intPosition = Int32.Parse(position) - 1;
        Image image = portraits[intPosition];
        Image namePlate = image.transform.Find("NamePlate")
            .gameObject.GetComponent<Image>();
        Text name = namePlate.GetComponentInChildren<Text>();

        image.enabled = false;
        image.sprite = null;
        namePlate.enabled = false;
        namePlate.sprite = null;
        name.text = "";
    }

    [YarnCommand("display_portrait")]
    public void DisplayPortrait(string path, string position, string flipped)
    {
        bool isFlipped = flipped == "true";
        int intPosition = Int32.Parse(position) - 1;
        Image image = portraits[intPosition];
        image.sprite =  Resources.Load<Sprite>("Portraits/" + path);

        Vector3 scale = image.gameObject.transform.localScale;
        if (isFlipped)
        {
            scale.x = -1;
        }
        else
        {
            scale.x = 1;
        }

        image.gameObject.transform.localScale = scale;
        // Flip the name or be cursed.
        image.GetComponentInChildren<Text>().gameObject.transform.localScale = scale;

        image.enabled = true;
    }


    [YarnCommand("highlight")]
    public void HighlightPosition(string position)
    {
        int intPosition = Int32.Parse(position) - 1;
        for (int i = 0; i < portraits.Count; i++)
        {
            float shadeToApply = shadeIntensity;
            int sortingOrder = 0;

            if (intPosition == i)
            {
                shadeToApply = 0;
                sortingOrder = 1;
            }

            Image image = portraits[i];
            Image namePlate = image.transform.Find("NamePlate")
                .gameObject.GetComponent<Image>();
            Text name = namePlate.GetComponentInChildren<Text>();

            Color32 colorWithShade = new Color32(
                (byte)(255 - shadeToApply),
                (byte) (255 - shadeToApply),
                (byte)(255 - shadeToApply),
                255
            );
            image.color = colorWithShade;
            namePlate.color = colorWithShade;
            name.color = colorWithShade;

            image.gameObject.GetComponent<Canvas>().sortingOrder = sortingOrder;
        }
    }

    [YarnCommand("highlight_all")]
    public void HighlightAll()
    {
        for (int i = 0; i < portraits.Count; i++)
        {
            Image image = portraits[i];
            Image namePlate = image.transform.Find("NamePlate")
                .gameObject.GetComponent<Image>();
            Text name = namePlate.GetComponentInChildren<Text>();

            Color32 colorWithShade = new Color32(
                (byte)(255),
                (byte) (255),
                (byte)(255),
                255
            );

            image.color = colorWithShade;
            namePlate.color = colorWithShade;
            name.color = colorWithShade;

            image.gameObject.GetComponent<Canvas>().sortingOrder = i;
        }
    }

    [YarnCommand("set_audio_voice")]
    public void SetAudioVoice(string path)
    {
        GetComponent<DialogueSounds>().SetAudio(path);
    }

    [YarnCommand("set_portrait_name")]
    public void SetPortraitName(string position, string plateType,string name)
    {
        int intPosition = Int32.Parse(position) - 1;
        GameObject namePlate = portraits[intPosition]
            .transform.Find("NamePlate").gameObject;

        namePlate.GetComponent<Image>().sprite = Resources
            .Load<Sprite>("Portraits/Plates/" + plateType);
        namePlate.GetComponent<Image>().enabled = true;

        // Set Name.
        namePlate.GetComponentInChildren<Text>().text = name;
    }

    [YarnCommand("end_demo")]
    public void EndDemo()
    {
        GoTitleButtonEvent end = new GoTitleButtonEvent();
        end.EndDemo(); 
    }

}