using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TroopSystem;

namespace SpellSystem
{
    public class SpellManager : MonoBehaviour
    {
        [Header("Spell Management")]
        public List<SpellSO> availableSpells = new List<SpellSO>();
        public GameObject spellCardPrefab;
        public GameObject spellCardContainer;
        
        private static SpellManager instance;
        public static SpellManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SpellManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("SpellManager");
                        instance = obj.AddComponent<SpellManager>();
                    }
                }
                return instance;
            }
        }

        private List<SpellCardUI> _spellCards;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _spellCards = new List<SpellCardUI>();
            
            // Clear all existing spell card children
            foreach (Transform child in spellCardContainer.transform)
            {
                Destroy(child.gameObject);
            }
            
            // Create spell cards based on availableSpells
            foreach (SpellSO spellSO in availableSpells)
            {
                GameObject spellCardObj = Instantiate(spellCardPrefab, spellCardContainer.transform);
                SpellCardUI spellCardUI = spellCardObj.GetComponent<SpellCardUI>();
                
                if (spellCardUI != null)
                {
                    spellCardUI.SetSpell(spellSO);
                    _spellCards.Add(spellCardUI);
                }
            }
        }
    }
}