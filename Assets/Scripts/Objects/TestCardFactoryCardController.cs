using UnityEngine;
using UnityEngine.UI;
using Factories;
using Globals;

public class TestCardFactoryCardController : MonoBehaviour
{
    public Button startButton;
    public Dropdown generationDropdown;
    public Dropdown rarityDropdown;
    
    public Canvas cardCanvas;
    private GameObject cardInstance;

    void Start()
    {
        //cardCanvas = 
        //startButton.onClick.AddListener(OnStartClicked);
        generationDropdown.ClearOptions();
        for (int i=1; i <= CardFactory.numberOfGenerations; i++)
        {
            generationDropdown.options.Add(new Dropdown.OptionData("gen" + i));
        }
    }

    public void OnStartClicked()
    {
        GameManager.AddCoins(1);
        startButton.interactable = false;
        if (cardInstance != null)
        {
            Destroy(cardInstance);
        }

        StartCoroutine(CardFactory.GetCard(generationDropdown.options[generationDropdown.value].text, rarityDropdown.options[rarityDropdown.value].text,
            (GameObject newCardInstance) =>
            {
                cardInstance = newCardInstance;
                cardInstance.transform.SetParent(cardCanvas.transform);
                cardInstance.transform.localPosition = new Vector3(0f, -75f, 0f);
                cardInstance.transform.localScale = new Vector3(1f, 1f, 1f);
                cardInstance.SetActive(true);
                startButton.interactable = true;
                Card card = cardInstance.GetComponent<Card>();
                GameManager.AddCardToCollection(card.createdFrom);
            }));
    }

}
