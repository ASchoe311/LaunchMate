![GitHub Downloads (all assets, latest release)](https://img.shields.io/github/downloads/ASchoe311/LaunchMate/latest/total)

# LaunchMate Plugin for [Playnite](https://playnite.link/)

Have you ever wanted to attach secondary applications to your games that launch and close with them? This is the way to do it! 

Say goodbye to manually adding launch actions or scripts for each game, because LaunchMate allows you to select an application to launch and then set conditions that will make it launch with any game that meets them. It's that simple!

You can have it check games for many different aspects like title, developers, genres, categories, and more. LaunchMate also supports delaying the launch of applications if you need to ensure the game is launched first.

### Please contribute translations on [Crowdin](https://crowdin.com/project/launchmate)!

## Installation
- Method 1: Install directly from within Playnite in the Addon Browser
- Method 2: Click the download link on Playnite's addon page [here]([https://playnite.link/](https://playnite.link/addons.html#LaunchMate_61d7fcec-322d-4eb6-b981-1c8f8122ddc8))
- Method 3: Download the latest .pext file from releases and open it using Playnite

## Usage

Once installed, LaunchMate's settings can be found within the "Generic" section of Playnite's addon settings menu.

The main settings page displays a table of all Launch Groups you've added. To create a new one, click "Add".

This will open the Launch Group Editor window for a new Launch Group. In this window you can select the application the group will launch, set the delay timer for application launch, and select the conditions a game must meet to launch the application.

See ["How to use the conditions section"](#how-to-use-the-conditions-section) for more information on conditions.

The Launch Group Editor window has a button at the bottom to preview matched games. This button can also be found for each Launch Group on the main settings window.


### How to use the conditions section

The table within the Launch Group Editor display all **Condition Groups** for that Launch Group. Click "Add" to create a new group. See ["Condition Groups Table Reference"](#condition-groups-table-reference)

This will open a new window with a table of conditions for that group. Once again, click "Add" to add a new condition. See ["Condition Table Reference"](#condition-table-reference)

#### Condition Groups Table Reference

| Not | Conditions | Next Logical Operator |
| -------- | ------- | -------- |
| If set, the result of this condition group will be negated | A textual representation of the conditions within the group | The logical operator placed between a condition group and the condition group that follows it |
| See matching column in ["Condition Table Reference"](#condition-table-reference) for context | -> means "matches" | E.g. if set to AND, the condition groups are true only if both the condition group and the one following it are true |
| | ~> means "fuzzy matches" | |
| | NOT means "does not match" | |
| | NOT~ means "does not fuzzy match" | |

#### Condition Table Reference

|Not|Filter Type|Filter|Fuzzy Match|Next Logical Operator|
| -------- | ------- | -------- | ------- | ------- |
| If set, the result of this condition will be negated | The game attribute to check the filter against | The filter with which to check the game attribute | If set, filter matches can be close but not exact | The logical operator placed between a condition and the condition that follows it |
| E.g. if the condition checks that the game name matches "Stardew" and this is Not is checked, it will be true for any game whose name does not match "Stardew"| If set to "All Games", it will match any game | This filter supports regex or standard strings | | E.g. if set to AND, the conditions are true only if both the condition and the one following it are true |

#### Example usage

To launch the application blitz.gg with any game made by Riot Games except for Legends of Runeterra, you have a few options. Two of them would be:

1. Set up two condition groups, the first checks that "Source" matches "Riot Games" and the second checks that "Name" matches "Runeterra". Check "Not" on the second group and set the "Next Logical Operator" of the first group to "AND".

    ![Image showing a condition group setup](https://i.imgur.com/HvROKwx.png)

2. Set up one condition group containing both a condition that "Source" matches "Riot Games" and a condition that "Name" does not match "Runeterra". Set the "Next Logical Operator" of the first condition to "AND".

    ![Image showing a condition group setup](https://i.imgur.com/BzMBR14.png)

Either of these methods results in the following matches:
    
![Image showing a condition group setup](https://i.imgur.com/2xVxs6f.png)
