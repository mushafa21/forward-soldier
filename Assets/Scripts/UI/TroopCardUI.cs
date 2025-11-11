using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TroopCardUI : MonoBehaviour
{
    [Header("References")]
    public Button button;
    public Image troopPotrait;
    public TextMeshProUGUI troopCostText;
    public TroopSO troopSO;
    public GameObject selectedIndicator;
    public TextMeshProUGUI coolDownText;
    public Image coolDownImage;
    
    public bool isSelected = false;
    public bool isInCooldown = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(SelectTroop);
        InitTroop();
    }

    public void SetTroop(TroopSO troop)
    {
        troopSO = troop;
    }
    

    void SelectTroop()
    {
        print("Select Troop Called");
        TroopManager.Instance.SetCurrentTroop(this);
    }

    void InitTroop()
    {
        troopPotrait.sprite = troopSO.potrait;
        troopCostText.text = troopSO.soulCost.ToString();
    }

    public void StartCooldown()
    {
        if (troopSO == null) return;
        
        isInCooldown = true;
        coolDownImage.fillAmount = 1f; // Start with full fill
        coolDownImage.gameObject.SetActive(true);
        
        // Start the cooldown coroutine
        StartCoroutine(CooldownRoutine(troopSO.spawnCooldown));
    }
    
    private System.Collections.IEnumerator CooldownRoutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = 1f - (elapsed / duration); // Decreases from 1 to 0
            coolDownImage.fillAmount = progress;
            
            // Update cooldown text
            int secondsRemaining = Mathf.CeilToInt(duration - elapsed);
            coolDownText.text = secondsRemaining.ToString();
            coolDownText.gameObject.SetActive(true);
            
            yield return null;
        }
        
        // Cooldown finished
        isInCooldown = false;
        coolDownImage.gameObject.SetActive(false);
        coolDownText.gameObject.SetActive(false);
    }
    
    public void Select()
    {
        isSelected = true;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(true);
    }
    
    public void Deselect()
    {
        isSelected = false;
        if (selectedIndicator != null)
            selectedIndicator.SetActive(false);
    }
    


}
