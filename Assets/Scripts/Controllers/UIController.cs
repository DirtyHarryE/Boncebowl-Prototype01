using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {

    bool m_WasInAimState;
    private Image m_ChargeMeter;

    private float m_AlphaAmt;
    
    private CanvasGroup m_ButtonGroup;
    private CanvasGroup m_InstructionGroup;

    public void Initialize()
    {
        //m_CrosshairGroup = GameObject.Find("Sphere/Main Camera/Crosshair").GetComponent<CanvasGroup>();
        m_ChargeMeter = GameObject.Find("Sphere/Main Camera/Crosshair/Charge").GetComponent<Image>();

        m_ButtonGroup = GameObject.Find("Canvas/Buttons Group").GetComponent<CanvasGroup>();
        m_InstructionGroup = GameObject.Find("Canvas/Instruction Group").GetComponent<CanvasGroup>();

        m_ButtonGroup.alpha = 1;
        m_InstructionGroup.alpha = 0;

        GazeButton.HotspotClickedEvent += ButtonPress;
    }

    void ButtonPress(GazeButton button)
    {
        Debug.Log("Button  : " + button.name);

        switch (button.Type)
        {
            case GazeButton.HotspotType.Begin:

                GameController.Instance.GetReady();

                break;
            case GazeButton.HotspotType.SelectRoll:
            case GazeButton.HotspotType.SelectStill:
                break;
        }
    }
    public void Update()
    {
        GroupUpdate();
        CrosshairUpdate();
    }
    void GroupUpdate()
    {
        bool showGroups = GameController.Instance.CurrentMode == GameController.GameMode.GetReady || GameController.Instance.CurrentMode == GameController.GameMode.SelectMode;

        m_ButtonGroup.gameObject.SetActive(showGroups);
        m_InstructionGroup.gameObject.SetActive(showGroups);

        float curButtonAlpha = m_ButtonGroup.alpha;
        float newButtonAlpha = Mathf.MoveTowards(curButtonAlpha, GameController.Instance.CurrentMode == GameController.GameMode.GetReady ? 0 : 1, Time.deltaTime * 2f);
        m_ButtonGroup.alpha = newButtonAlpha;
        m_InstructionGroup.alpha = 1f - newButtonAlpha;
    }
    void CrosshairUpdate()
    {



        bool isAiming = GameController.Instance.CurrentMode == GameController.GameMode.Looking;

        float curalpha = m_ChargeMeter.color.a;
        float newalpha = Mathf.MoveTowards(curalpha, isAiming ? 1 : 0, Time.deltaTime * 2f);
        m_ChargeMeter.color = new Color(1, 1, 1, newalpha);



        float amt = m_ChargeMeter.fillAmount;
        if (amt <= GameController.Instance.Charge)
        {
            amt = GameController.Instance.Charge;
        }
        else
        {
            amt = Mathf.Lerp(amt, GameController.Instance.Charge, 0.1f);
        }
        m_ChargeMeter.fillAmount = amt;
    }
}
