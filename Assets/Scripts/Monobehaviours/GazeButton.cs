using UnityEngine;
using System.Collections;
using HappyFinish.VR;
using System;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class GazeButton : Hotspot
{
    private Button m_Button;
    
    private Color m_NormalColor;

    public delegate void HotspotClickedDelegate(GazeButton sender); // Instead of doing "HotspotInfoClicked(hotspotType, mission, persona);" the parameter is the class
    public static event HotspotClickedDelegate HotspotClickedEvent = delegate { }; // Doing "= delegate {};" there is no need to do "if (HotspotInfoClicked != null)"

    public enum HotspotType { SelectRoll, SelectStill, Begin}
    [SerializeField]
    public HotspotType Type;


    void Start()
    {
        m_Button = this.GetComponent<Button>();

        ColorBlock cb = m_Button.colors;
        m_NormalColor = cb.normalColor;

        HotspotClickedEvent += OtherButton;
    }

    void OtherButton(GazeButton button)
    {

        if (
            (button.Type == HotspotType.SelectRoll && Type == HotspotType.SelectStill) ||
            (button.Type == HotspotType.SelectStill && Type == HotspotType.SelectRoll) )
        {
            ColorBlock cb = m_Button.colors;
            cb.normalColor = m_NormalColor;
            m_Button.colors = cb;

        }
    }

    protected override void ClickBehaviour()
    {

        Debug.Log("Clicked : " + name);

        HotspotClickedEvent(this);
        if (Type == HotspotType.SelectRoll || Type == HotspotType.SelectStill)
        {
            ColorBlock cb = m_Button.colors;
            cb.normalColor = cb.pressedColor;
            m_Button.colors = cb;
        }
    }

    protected override void EnterBehaviour()
    {

    }

    protected override void ExitBehaviour()
    {

    }

    protected override void StayBehaviour()
    {

    }
}
