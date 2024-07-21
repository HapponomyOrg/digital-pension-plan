using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;

public class ClickableRawImage : MonoBehaviour, IPointerClickHandler
{
    public GameObject sellConfirmationPanel;
    public Slider sellSlider;
    public Transform marketScreen;
    public GameObject cardPrefab;
    public TextMeshProUGUI sellAmountText;
    public RawImage clickedCardTexture;

    private bool roundStarted = false;

    private List<GameObject> playerHand = new List<GameObject>();

    private void Start()
    {
        sellConfirmationPanel.SetActive(false);
        NatsClient.C.OnStartRound += (sender, msg) =>
        {
            roundStarted = true;
        };
    }
    private void Update()
    {
        UpdateSellAmount();
    }

    //logic of putting a card on the market.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!roundStarted) return;
        sellConfirmationPanel.SetActive(true);
        RawImage rawImage = eventData.pointerCurrentRaycast.gameObject.GetComponent<RawImage>();

        if (rawImage != null)
        {
            clickedCardTexture.texture = rawImage.texture;
        }
    }
    //Need to add logic behind buying a card from the markt.
    public void OnMarketClick(PointerEventData eventData)
    {

    }

    public void ConfirmSell()
    {
        sellConfirmationPanel.SetActive(false);
        int sellAmount = (int)sellSlider.value * 1000;
        Debug.Log("Kaart op markt gezet voor " + sellAmount + " munten.");
        MoveCardToMarket();
        DestroyPlayerCard();
    }

    public void CancelSell()
    {
        sellConfirmationPanel.SetActive(false);
    }
    private void MoveCardToMarket()
    {
        GameObject newCard = Instantiate(cardPrefab, marketScreen);
        Texture cardTexture = clickedCardTexture.texture;
        
        newCard.GetComponent<RawImage>().texture = cardTexture;
   
        Vector3 newPosition = Vector3.zero;
        newPosition.x = (marketScreen.childCount - 1) * 75; 
        newCard.transform.localPosition = newPosition;
    }

    private void DestroyPlayerCard()
    {
        if (playerHand.Count > 0)
        {
            GameObject cardToRemove = playerHand[playerHand.Count - 1];
            Destroy(cardToRemove);
            playerHand.RemoveAt(playerHand.Count - 1);
        }
    }
    public void UpdateSellAmount()
    {
        int sellAmount = (int)sellSlider.value * 1000;
        sellAmountText.text = sellAmount.ToString() + " munten"; 
    }
}
