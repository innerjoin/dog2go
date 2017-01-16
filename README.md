# dog2go - Online Edition of the Famous Board Game Dog
As part of our university studies at University of Applied Sciences Rapperswil (HSR), we have built an online version of a board game.
## About Dog
The game is known in Switzerland under the name ‘Dog’, Dutch people call it ‘Keezen’ and Canadians should be familiar with it being named ‘Tock’. You can find out more about the game on [Wikipedia](https://en.wikipedia.org/wiki/Tock) or on [this](http://www.dogspiel.info/) site (it is in germen though).
## Goal
The project's goal was to allow players to comepte against each other online over the wire. The interaction design should make it easy to play the game, even for people who are either only used to the board version or having not really an idea of how to play Dog.
The game cycle should be controlled by a server to have an easy way to validate meeple moves. 
The following requirements have been defined in advance:
+ Support to create a new game table for exactly four people
+ Show game tables with free seats
+ Allow to join a game table if there are free seats left
+ Uniquely identify a user through it's name (no authentication, as we wanted to allow anonymous access)
+ As soon as the last seat is taken, the game shall start automatically
+ Shuffle, spread and exchange cards
+ The player whose turn it is can make a move according to the game rules
+ The correctness of the move will be validated by the server
+ The visualization of card, board and meeples should be as similar as possible to the board game
+ It should be possible to play the game on smartphones, tablets and desktop PC's

## Result
Altough we couldn't implement everything we wanted to do, the game is working really well with only minor drawbacks.

We currently do not provide an online version of the game anymore, as we do not have a hosted IIS anymore. So you have to settle for the following screen shots if you do not wish to run it by yourself.
### Visualization of the Open Game Tables
![Visualization of the Open Game Tables](/assets/ChooseGameTable.PNG)
### Loading Screen
![Game Loading Screen](/assets/LoadingScreen.PNG)
### Game Table in Action
![Game Table in Action](/assets/GameTable.PNG)
### Mobile Friendly Side Menu
![Mobile Friendly Side Menu](/assets/GameTableMenu.PNG)

## About the Authors
+ Lukas Steiger ([@innerjoin](https://github.com/innerjoin))
+ Janick Engeler ([@jengi77](https://github.com/jengi77))
+ Yanick Gubler ([@ygubler](https://github.com/ygubler))
+ Alexis Suter
