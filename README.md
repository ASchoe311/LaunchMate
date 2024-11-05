# Welcome to LaunchMate: Your Ultimate Companion for Game Launch Automation!

Tired of manually setting up launch actions for your games? Meet LaunchMate—the game-changing plugin for Playnite that automates the entire process!

With LaunchMate, you can effortlessly manage your gaming environment. Whether it's launching applications, opening web pages, running scripts, starting or stopping Windows services, or even stopping processes—LaunchMate handles it all! Set precise conditions for when these actions should occur, based on various aspects of your game metadata, and let LaunchMate do the heavy lifting.

## Key Features:

**Automated Game Environment Management**: Seamlessly manage secondary actions like launching apps, running scripts, and more, triggered by your game launches.

**Expanded Capabilities**:
- Launch an application.
- Open a webpage.
- Run a script.
- Start a Windows service.
- Stop a process.
- Stop a Windows service.

**Powerful and Dynamic Conditions**: Check for aspects like title, developers, genres, categories, and more to trigger actions. Also set conditions for things like whether a process or service is currently running, ensuring precise control over your gaming environment.

**Flexible Timing**: Delay the launch of actions to ensure your game launches first, giving you full control over the timing and order of events.

**Real-time Sync**: Make changes on the fly with automatic saving, ensuring your configurations are always up-to-date.

**User-Friendly Interface**: Navigate through an intuitive UI to manage your launch groups, set actions, and define conditions with ease.

Transform your gaming experience with LaunchMate, and focus on what matters most—enjoying your games. 🚀

# Installation
- Method 1: Install directly from within Playnite in the Addon Browser
- Method 2: Click the download link on Playnite's addon page [here](https://playnite.link/addons.html#LaunchMate_61d7fcec-322d-4eb6-b981-1c8f8122ddc8)
- Method 3: Download the latest .pext file from releases and open it using Playnite

## Usage

An indepth usage example can be seen [here](#example-usage).

Once installed, LaunchMate's settings can be found on the left sidebar or within the "Generic" section of Playnite's addon settings menu.

The main settings page displays a tab view of all Launch Groups you've added. To create a new one, click "Add". This will open the launch group editor on the right side of the settings window.

### The Launch Group Editor

In this window you can choose the action that will be executed by the launch group, the settings for the launch group, and select the conditions a game must meet to launch the target. 

The table at the bottom of the Launch Group Editor window is where you can set the conditions for your launch group. See ["How to use the conditions section"](#how-to-use-the-conditions-section) to understand how to use this table.

The Launch Group Editor window has a button at the bottom to preview matched games.


### How to use the conditions section

In this table you add the conditions that determine if the launch group will execute its action. Click "Add" to add a new condition. 

Choose which type of filter the condition will have using the drop down menu, and then fill in the filter. You can click the search icon next to the filter box for easier filter selection or you can type in the filter yourself.

Note that when you use the selection box the filter will be associated with the ID of whichever filter you select. This means that if the name of the filtered item changes the filter will still be associated with the renamed value. For example, if you select "Steam" as a source filter and then change the name of the "Steam" source in Playnite to "Steam Games", the filter will still trigger on "Steam Games". This will not happen if you type in the filter yourself.

See ["Condition Table Reference"](#condition-table-reference) for more information on each table column.

<br><br>

### Condition Table Reference

| Exclude | Filter Type | Filter | Fuzzy Match | Next Logical Operator |
| -------- | ------- | -------- | ------- | ------- |
| If set, the result of this condition will be negated | The game attribute to check the filter against | The filter with which to check the game attribute | If set, filter matches can be close but not exact | The logical operator placed between a condition and the condition that follows it |
| E.g. if the condition checks that the game name matches "Stardew" and Exclude is checked, it will be true for any game whose name does not match "Stardew"| If set to "All Games", it will match any game | Can be typed in or auto-filled using the search box | | E.g. if set to AND, the conditions are true only if both the condition and the one following it are true |

<br><br>

### Example usage

To launch the application blitz.gg with any game made by Riot Games except for Legends of Runeterra, you could do the following:

1. Open the LaunchMate menu

    ![Image showing how to open the LaunchMate menu](https://i.imgur.com/kyaEF9O.png)

2. Click "Add" to create a new launch group

    ![Image showing to click Add](https://i.imgur.com/dXV3WC8.png)

3. Name your launch group whatever you'd like, such as "Launch Blitz.gg"

    ![Image showing to change name](https://i.imgur.com/vuzKUEU.png)

4. Make sure the action type dropdown is set to "Launch an App"

    ![Image showing to change action type](https://i.imgur.com/u5v1oOj.png)

5. Use the file selector button to choose the executable for Blitz.gg

    ![Image showing to choose file](https://i.imgur.com/LaTWXKs.png)

6. Click "Add" under the conditions box to create a condition

    ![Image showing to add condition](https://i.imgur.com/EgHJzy4.png)

7. Set the filter type dropdown to "Source" and then click the search icon next to the filter box

    ![Image showing to add filter](https://i.imgur.com/wgDOE5R.png)

8. Choose "Riot Games" as the source in the search window that opens

    ![Image showing to select source](https://i.imgur.com/8e9W3kk.png)

9. Add two more conditions, one for the game name being "Legends of Runeterra" and another checking if blitz.gg is already running. Check the "Exclude" box for both so that the conditions check if the game name is NOT "Legends of Runeterra" and Blitz.gg is NOT running. Also make sure "Next Logical Operator" is set to "And" for all conditions

    ![Image showing all conditions](https://i.imgur.com/ePDUt09.png)

10. Make sure the launch group settings (enabled, delay, and auto-close) are set as desired

    ![Image showing settings](https://i.imgur.com/KxdZvaf.png)

11. Click "See Matched Games" to make sure the conditions are set properly (IMPORTANT: If no matches show up, make sure the app checked by the last condition is not running in the background already)

    ![Image showing matches](https://i.imgur.com/nQyTiji.png)

12. Click "Save" to save your settings and close the LaunchMate window. Your launch group will now execute its action on any game that matches its conditions.

    ![Image showing save](https://i.imgur.com/95yDjsU.png)
