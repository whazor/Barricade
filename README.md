Jeu de Barricade
================
This boardgame is a school assignment made by Tim Slot and Nanne Wielinga in March and April 2013. The game is called Jeu de Barricade and is a variation on Ludo. The emphasis of our school was on the architecture and WPF/C#. But we also spend alot of time on bots, usability, and design.

Here is a screenshot of a game:
![Screenshot of the short level](http://mycel.nl/barricade/Screenshot.png)

Made in
-------
This project is build in C# and the interface is build in WPF. Adding an extra interface will be easy thanks to the architecture.

Functionality
-------------

*   Two standard levels (short and long) 
*   Load own-made levels
*   Save levels during the game
*   Play against multiple people and/or computers
*   Three different bot strategies (friendly, rusher and random)
*   Animations at pawn movement
*   Cheating mode for testing

Architecture
------------
In support of the development, the architecture is found [here](Architectuur.pdf) (warning: Dutch!). The code is divided in layers and there is also a console application.

![Layer diagram](Barricade.Modeling/LayerDiagram.png)

