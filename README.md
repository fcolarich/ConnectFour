# RUNEDUEL
## A Connect-Four Game

![image](https://user-images.githubusercontent.com/56565104/127744496-e039c4a9-fa32-4a76-802e-ad9749d3179b.png)

### Introduction
The objective of this project was to create a classic Connect-Four Game. https://en.wikipedia.org/wiki/Connect_Four  
The features it should include was 2 player support, IA, winning and resetting.  
After I developed the basic game, that is, without cool graphics or anything else, just plain ol' unity cubes, I started playing around in my head what could make this kind of game interesting to play and to watch. Various Ideas came into mind, like the tokens were buildings that one had to "Pile up" to build something or well many ideas.  
Finally I settled with the concept of elements fighting each other in some way. Through some mental iterations, I arrived to the final concept of a duel of runes, where each player would choose an element, either fire or ice, and play with tokens of that element.  
This gave me more ideas into how to design the UI, music, animations and particle effects. I took some assets I had bought or downloaded, I created and edited them and I started pouring them into the game.  
The end result is this. A retro looking pixel art game, about magical runes.  
The biggest challenge, as always, was not the complexity of the task, but my own drive to make my best job possible. I never deliver half completed games.  
A game must begin and end. The player must have a goal to complete and a way to complete it.  
I also added options to customize the board size and how many tokens are needed to connect to win. This allows for some replayability in this simple game.

### Main Managers
The game have several managers that control how it behaves, I will be enumerating and describing them here:
* Board Manager: The first one. This manager was the original core of the game. Its where the board is created, where the tokens are placed and where matches are done. This manager controls the flow of the game, telling which player can play, giving time between plays, creating and activating the AI when needed, and clearing everything when the game is reset. It makes use of the object pool to spawn tiles, tokens and buttons and basically links together the logic and AI.
* Game Manager: The second one. This manager basically responds to UI commands to control the game, letting the player choose an element, setting game options, resetting and also beginning the game. This manager will capture all the options the player chose and send them to the board manager to start a new game with those custom options.
* UI Manager: The third one. This manager will control how and when the UI must appear or dissapear, controling the flow of information of the game. It comunicates with both the board manager and the game manager to sync some processes, like beginning a new game or restarting one.

### AI
* AIPlayer: Its where the AI of the game is controlled through. In each AI turn, it will try to place pieces following a custom logic that will check in the board if matches are made that fulfill certain conditions. 
- First of all, it will try to win, meaning, try to find a place in the board where placing one token, will give the AIplayer victory.
- Secondly, it will try to stop the other player from winning, trying to find a place in the board where placing one opponents token, would give them victory and occupying that place themselves.
- Thirdly, it will try to increase how many token it has somewhere, to be closer to the victory treshold defined.
- Forthly, it will try to stop the opponents from increasing its matches, by finding where they could put a token and placing it instead.
- Lastly, if it cannot do any of the above, it will put a piece in the first available empty slot. I thought about putting a much more complex AI in these cases, but after several tests it was not needed.

### Images
* During a Match
![image](https://user-images.githubusercontent.com/56565104/127744491-9b152195-581d-4d7c-8cf7-9f1fab914247.png)

* Victory
![image](https://user-images.githubusercontent.com/56565104/127744473-c06320c7-f077-442c-b8b5-49014c6cc15c.png)

### Ending Thoughts
Im pretty happy with the end result of the game. The look and feel is very coherent and the gameplay although basic, its entertaining.  
The AI of the opponents could be further improved and most of the particle effects need some reworking. I wanted to add more flavor and animations to the background as well. But that goes beyond the scope of this project.
