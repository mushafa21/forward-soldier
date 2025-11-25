Create these script  EnemyCardUI : works similar to TroopCardUI, but the only action is OnPointer and no clicks

BattlePreparationUI :
- Have a container that display EnemyCardUI based on GameManager enemyTroops, clear the child on start before showing it
- Have a start button where it hides the battle preparation canvas and start the battle
- Have a container that display TroopCardUI based on GameManager unlockedTroops, these TroopCardUI have different interactions when clicked (details on bottom)



Modify these script



LevelManager : 
- when the level starts, it turns on the battle camera first then pause for 2 seconds, then show the BattlePreparationUI canvas
- The battle only stars when BattlePreparationUI start button is clicked, battle starts is EnemyManager stars working and spawning enemies , SoulManager starts regenerating soul, and TroopManager starts to deploying troops

TroopCardUI : 
- Have a different function on clicked :
- When the troop is on BattlePreparationUI and not selected then the TroopManager maxSelectableTroops is not pass the limit, if selected then turn on the disabled imagee and can't be clicked
- when the troop is inside the TroopManager selectable troops and while the battle has not started then remove that troop from the list and make the same troop in the BattlePreparationUI troop list enabled again
- when the battle starts then starts the current click interaction, selecting which one to deploy

TroopManager : the selectableTroops is selected from the BattlePreparationUI
