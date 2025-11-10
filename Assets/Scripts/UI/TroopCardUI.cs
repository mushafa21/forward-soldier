using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopCardUI : MonoBehaviour
{
    public Button button;
    public Image troopPotrait;
    public TextMeshProUGUI troopCostText;
    public TroopSO troopSO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(SelectTroop);
        InitTroop();
    }
    

    void SelectTroop()
    {
        print("Select Troop Called");
        TroopManager.Instance.SetCurrentTroop(troopSO);
    }

    void InitTroop()
    {
        troopPotrait.sprite = troopSO.potrait;
        troopCostText.text = troopSO.soulCost.ToString();
    }
    
    
}
