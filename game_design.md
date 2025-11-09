Game Design Document: Forward Soldier!

Game Title: Forward Soldier!
Genre: Roguelike Tower Defense (Lane-Pusher)
Platform: PC (Steam)
Art Style: 2D Pixel Art (High-contrast, vibrant magic effects)
Target Audience: Fans of strategy games like Clash Royale, "gacha" TD games like Battle Cats, and persistent upgrade loops.

1. Game Overview

1.1. Logline

As a battle mage, defend your castle and conquer the enemy's by summoning troops into multi-lane combat. Manage your mana and your army's cooldowns to overwhelm the enemy. After each victory, spend your earnings in a randomized shop to permanently upgrade your troops and spells, preparing you for the next challenging battle.

1.2. Game Inspirations

Clash Royale: Core real-time, lane-based "tug-of-war" combat. Mana generation and strategic troop deployment.

Battle Cats: Side-on 2D perspective (adapted here for top-down) and the satisfying feeling of spawning a "death ball" of troops, plus a deep meta-progression system.

Roguelikes: The progression system is inspired by roguelikes, featuring a randomized shop that offers different upgrade choices after each level, making each player's army develop uniquely.

2. Core Gameplay Pillars

Strategic Deployment: Victory isn't just about what you summon, but when and where. Splitting forces, countering specific lanes, and managing mana are keys to success.

Cooldown Management: The core battle loop revolves around juggling the individual cooldowns of your troops. Deciding whether to save a Knight or deploy a Soldier now is a critical choice.

Escalating Stakes: Each level gets progressively harder, requiring you to invest in upgrades and adapt your strategy to overcome stronger enemy waves.

Permanent, Randomized Progress: The shop offers a different set of upgrades after every battle. This random, persistent progression means you must strategically build your army's strength over the long term.

3. Gameplay Mechanics

3.1. The Two Loops

The game is split into two distinct loops: the Battle Loop (micro, second-to-second gameplay) and the Progression Loop (macro, level-to-level progression).

3.1.1. The Battle Loop (Micro)

This is the core combat, lasting 2-3 minutes per level.

Start: The level begins. The player's castle is on the left, the enemy's on the right. There are 2-4 predetermined, separate paths connecting them.

Mana Generation: The player has a Mana bar that automatically regenerates at a fixed rate (e.g., 1 Mana every 1.5 seconds).

Troop Deployment:

The player enters battle with a pre-selected "Loadout" of 2-5 troop types (e.g., Soldier, Archer, Mage).

Each troop card in the loadout has a Mana Cost and an individual Cooldown (e.g., Soldier: 5s, Knight: 12s).

To play a card, the player clicks the troop's icon and then clicks the "drop point" at the start of their chosen lane.

If the player has enough Mana, the troop is summoned and its icon grays out, starting its individual cooldown. The player's Mana is spent.

While one troop is on cooldown, the player is free to summon any other troop that is off-cooldown and affordable.

Spell Casting:

The player has one Spell Slot (e.g., Heal).

The spell does not cost Mana but has a global Cooldown (e.g., 25 seconds).

Using the spell triggers its effect and starts its cooldown.

Objective: The first side to destroy the opposing castle (reduce its HP to 0) wins the battle.

3.1.2. The Progression Loop (Macro)

This is the over-arching structure of the game.

Battle: The player engages in a Battle Loop (see 3.1.1) at the current level.

Reward: After winning a battle, the player earns Gold (permanent currency).

Hub (Shop): The player returns to the Hub / Shop screen.

Randomized Upgrades: The Shop presents a new, randomly selected set of 3-4 upgrade options.

Example Shop (Level 1):

Upgrade Soldier (Level 1 -> 2) - Cost: 100 Gold

Upgrade Heal Spell (Level 1 -> 2) - Cost: 150 Gold

Upgrade Mage (Level 1 -> 2) - Cost: 120 Gold

Example Shop (Level 2):

Upgrade Knight (Level 1 -> 2) - Cost: 200 Gold

Unlock "Fireball" Spell - Cost: 300 Gold

Upgrade Archer (Level 1 -> 2) - Cost: 120 Gold

Spend Gold: The player spends their Gold on the available permanent upgrades. Upgrades not purchased will be gone when the next shop refreshes.

Progress: The player clicks "Next Level" to proceed to the next, more difficult battle.

Loop End: The loop continues until the player is defeated. If defeated, they can restart from Level 1, but all their permanent upgrades are kept.

4. Game Content

4.1. Level Design

2D Top-Down View.

Levels are defined by the number, shape, and length of their paths.

Path Variation:

Standard: 2-3 straight paths.

Asymmetric: One path is much shorter, creating a "rush" lane.

Chokepoint: Paths narrow in the middle, forcing units to clump up (great for Mages).

Bridge: A single-file path where only one unit can pass at a time.

Enemies will also spawn troops on these paths from their end.

4.2. Player Troops (Initial Roster)

Soldier

Mana Cost: 2

Health: Medium

Damage: Medium

Speed: Medium

Special: Standard melee unit. Good all-rounder.

Archer

Mana Cost: 3

Health: Low

Damage: Medium

Speed: Medium

Special: Ranged unit. Stays behind melee troops.

Mage

Mana Cost: 5

Health: Low

Damage: High

Speed: Slow

Special: Ranged unit. Deals area-of-effect (AoE) damage.

Cavalry

Mana Cost: 4

Health: Medium

Damage: High

Speed: Very Fast

Special: Rushes down the lane. Good for surprising the enemy.

Knight

Mana Cost: 6

Health: High

Damage: High

Speed: Slow

Special: Tank unit. Absorbs a lot of damage.

4.3. Player Spells (Initial Roster)

Heal

Cooldown: 25s

Effect: Heals all allied troops on all lanes for a small amount.

Attack Boost

Cooldown: 30s

Effect: All allied troops gain +50% attack damage for 5 seconds.

Speed Boost

Cooldown: 20s

Effect: All allied troops gain +50% movement speed for 5 seconds.

Fireball

Cooldown: 15s

Effect: (Unlockable) Launch a fireball at any point on the map, dealing AoE damage.

4.4. Enemy Faction

The enemy faction uses the same set of troops as the player.

Soldier

Archer

Mage

Cavalry

Knight

Castle Spawns: The enemy castle will automatically spawn units on a timer, which gets faster and more complex as the game progresses. They will also use spells.

5. Controls & UI

5.1. Controls (PC)

Mouse: All core actions.

Click troop icon in loadout to select.

Click lane start point to deploy.

Click spell icon to activate.

Click shop upgrades to purchase.

Keyboard (Shortcuts):

1, 2, 3, 4, 5: Activate troop loadout slots 1-5.

Q (or Spacebar): Activate the equipped spell.

5.2. UI Wireframe (In-Battle)

Game World:

Player Castle HP Bar appears directly above the player's castle.

Enemy Castle HP Bar appears directly above the enemy's castle.

Screen Overlay (HUD):

Bottom-Center:

The Mana Bar (fills left-to-right).

The Troop Loadout (2-5 icons, showing unit art, mana cost, and cooldown timer overlay).

Bottom-Right:

The Spell Slot (1 large icon, showing cooldown timer overlay).

Center: The battlefield.

6. Art & Sound

6.1. Art Style

2D Pixel Art: Clean and readable. Units should be easily identifiable even when small.

Color Palette: High contrast between player units (e.g., blue/silver) and enemy units (e.g., red/gold) to ensure readability in mirror matches.

Effects: Spell effects (heals, explosions) will be bright, vibrant, and juicy, providing strong visual feedback.

6.2. Sound & Music

Music:

Hub/Shop: Calm, melodic fantasy theme.

Battle: Up-tempo, driving, orchestral/chiptune hybrid track that builds in intensity.

Sound Effects:

UI: Satisfying clicks and "shing" sounds for purchases and selections.

Troops: Distinct summoning sounds, attack sounds (sword clangs, arrow "thwips"). Cooldown "ready" chime.

Spells: Big, impactful sounds (e.g., a "whoosh" and "chime" for Heal, a "boom" for Fireball).