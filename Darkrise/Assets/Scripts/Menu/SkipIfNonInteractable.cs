using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkipNonInteractable : MonoBehaviour, ISelectHandler
{

    // thanks to TechCor from the Unity forums for this script! :) I added rewired support to it

    private Selectable m_Selectable;
    private Player player;
    private int playerId = 0;

    // Use this for initialization
    void Awake()
    {
        m_Selectable = GetComponent<Selectable>();
        player = ReInput.players.GetPlayer(playerId);
    }

    public void OnSelect(BaseEventData evData)
    {
        // Don't apply skipping unless we are not interactable.
        if (m_Selectable.interactable) return;

        if (player.GetAxis("UIVertical") < 0)
        {
            Selectable select = m_Selectable.FindSelectableOnDown();
            if (select == null || !select.gameObject.activeInHierarchy)
                select = m_Selectable.FindSelectableOnUp();
            StartCoroutine(DelaySelect(select));
        }
        else if (player.GetAxis("UIVertical") > 0)
        {
            Selectable select = m_Selectable.FindSelectableOnUp();
            if (select == null || !select.gameObject.activeInHierarchy)
                select = m_Selectable.FindSelectableOnDown();
            StartCoroutine(DelaySelect(select));
        }
    }

    // Delay the select until the end of the frame.
    // If we do not, the current object will be selected instead.
    private IEnumerator DelaySelect(Selectable select)
    {
        yield return new WaitForEndOfFrame();

        if (select != null || !select.gameObject.activeInHierarchy)
            select.Select();
        else
            Debug.LogWarning("Please make sure your explicit navigation is configured correctly.");
    }
}
