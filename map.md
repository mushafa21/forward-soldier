# Map Feature Setup Guide

This document explains how to set up and use the map feature imported from an external project. The map system generates procedural maps similar to those found in games like Slay the Spire, with nodes connected by paths that players can traverse.

## Dependencies

Before setting up the map system, ensure you have the following dependencies installed:

1. **DOTween** - For animations and transitions
2. **Newtonsoft.Json** - For map serialization and saving

These can be installed via the Unity Package Manager or by importing the packages directly.

## Map System Components

### Core Scripts

- `MapManager` - Main controller that handles map generation, saving, and loading
- `MapGenerator` - Creates the procedural map structure with nodes and connections
- `MapView` - Handles the visual display of the map in the world space
- `MapViewUI` - Handles the visual display of the map in UI space (inherits from MapView)
- `MapNode` - Represents individual nodes on the map
- `MapPlayerTracker` - Handles player interaction with nodes
- `MapConfig` - Scriptable object that defines map configuration
- `MapLayer` - Defines properties for each layer of the map
- `NodeBlueprint` - Defines the visual appearance and type for different node types

### Prefabs

- `Node.prefab` - World space node prefab
- `UINode.prefab` - UI space node prefab
- `LinePrefab.prefab` - World space line connecting nodes
- `UILine.prefab` - UI space line connecting nodes
- `MapObjects.prefab` - Contains map manager and view for world space
- `MapObjectsUI.prefab` - Contains map manager and view for UI space

### Scriptable Objects

- `MapConfig` - Defines the structure of the map (number of layers, node types, etc.)
- `NodeBlueprint` - Defines the visual representation for each node type

## Setup Process

### 1. Scene Setup

#### For World Space Map:
1. Create a new scene or open an existing one
2. Add the `MapObjects.prefab` to your scene
3. The prefab contains both `MapManager` and `MapView` components

#### For UI Space Map:
1. Create a new scene or open an existing one
2. Add the `MapObjectsUI.prefab` to your scene
3. The prefab contains both `MapManager` and `MapViewUI` components

### 2. Map Configuration

1. Create a new `MapConfig` asset using `Create > Map > MapConfig`
2. Add `NodeBlueprint` assets to the `nodeBlueprints` list - these define the different node types
3. Configure the layers in the `layers` array:
   - `nodeType`: The default node type for this layer
   - `distanceFromPreviousLayer`: Min/max distance to the previous layer
   - `nodesApartDistance`: Horizontal distance between nodes on this layer
   - `randomizePosition`: Amount of random position variation (0-1)
   - `randomizeNodes`: Chance to place a random node instead of the default (0-1)

4. Set the following properties:
   - `numOfPreBossNodes`: Min/max number of nodes on the layer before the boss
   - `numOfStartingNodes`: Min/max number of starting nodes (first layer)
   - `extraPaths`: Additional paths to generate beyond the minimum required

### 3. NodeBlueprint Setup

1. Create `NodeBlueprint` assets for each node type you want to use
2. Set the sprite to display for this node type
3. Set the node type (MinorEnemy, EliteEnemy, RestSite, Treasure, Store, Boss, Mystery)

Example node types:
- `NodeType.MinorEnemy` - Regular enemies
- `NodeType.EliteEnemy` - Stronger enemies
- `NodeType.RestSite` - Rest or healing locations
- `NodeType.Treasure` - Treasure/loot locations
- `NodeType.Store` - Shop locations
- `NodeType.Boss` - Boss encounter (typically only one)
- `NodeType.Mystery` - Unknown/random content

### 4. MapView Configuration

Configure the following settings on your `MapView` or `MapViewUI` component:

#### Common Settings:
- `mapManager`: Reference to the MapManager in the scene
- `orientation`: Direction of the map (BottomToTop, TopToBottom, RightToLeft, LeftToRight)
- `allMapConfigs`: List of all available MapConfig assets
- `nodePrefab`: Prefab to use for map nodes
- `orientationOffset`: Padding from screen edges
- `background`: Background sprite for the map (optional)
- `backgroundColor`: Color for the background
- `linePrefab`: Prefab to use for connecting lines
- `linePointsCount`: Number of points for smooth lines (min 3)
- `offsetFromNodes`: Distance from node to line start/end
- Color settings for visited, locked, and available states

#### MapViewUI Specific Settings:
- `scrollRectHorizontal`: ScrollRect for horizontal orientation maps
- `scrollRectVertical`: ScrollRect for vertical orientation maps
- `unitsToPixelsMultiplier`: Conversion from world units to UI pixels
- `padding`: Padding from scroll rect edges
- `backgroundPadding`: Padding for the background image
- `backgroundPPUMultiplier`: Pixels per unit multiplier for background
- `uiLinePrefab`: UILineRenderer prefab for UI lines

### 5. Handling Node Selection

The `MapPlayerTracker` component handles node selection:

1. When a node is clicked, `SelectNode()` is called
2. The system validates if the node can be accessed based on current path
3. If valid, the node is added to the path and `EnterNode()` is called after a delay
4. Override the switch statement in `EnterNode()` to handle different node types

Example integration:
```csharp
private static void EnterNode(MapNode mapNode)
{
    Debug.Log("Entering node: " + mapNode.Node.blueprintName + " of type: " + mapNode.Node.nodeType);
    switch (mapNode.Node.nodeType)
    {
        case NodeType.MinorEnemy:
            // Load combat scene or show combat UI
            break;
        case NodeType.RestSite:
            // Show rest/heal options
            break;
        case NodeType.Treasure:
            // Show treasure/reward UI
            break;
        case NodeType.Boss:
            // Load boss battle
            break;
        // Add cases for other node types...
    }
}
```

### 6. Map Saving and Loading

The system automatically saves the current map state using PlayerPrefs when:
- A node is selected
- The application exits

The map is loaded when starting the scene if a saved map exists and the player hasn't reached the boss yet.

## Customization Options

### Visual Customization
- Modify node sprites in NodeBlueprint assets
- Adjust colors in MapView component
- Change line materials and appearance
- Customize background settings

### Procedural Generation
- Adjust layer configurations in MapConfig
- Modify min/max values for distances and counts
- Change randomization settings
- Add/remove node types from random nodes list

### Orientation Options
The map supports four orientations:
- `BottomToTop`: Player starts at bottom, boss at top
- `TopToBottom`: Player starts at top, boss at bottom
- `LeftToRight`: Player starts at left, boss at right
- `RightToLeft`: Player starts at right, boss at left

## Troubleshooting

### Common Issues:
1. **Missing References**: Ensure all required references in the inspector are set
2. **JSON Serialization Errors**: Verify Newtonsoft.Json is properly imported
3. **Animation Issues**: Make sure DOTween is properly installed
4. **UI Elements Not Visible**: Check that UI elements are on the correct canvas and not culled

### Performance:
- For large maps, consider reducing the number of layers
- Adjust `linePointsCount` to balance visual quality with performance
- Limit the number of simultaneous maps loaded in memory

## Sample Scenes

The package includes sample scenes to demonstrate the functionality:
- `SampleScene.unity`: World space map implementation
- `SampleSceneUI.unity`: UI space map implementation

Use these scenes as reference for your implementation.