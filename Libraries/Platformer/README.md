# Platformer

This project is a starting point for creating a platformer game.

## Player State Machine (PSM)

The PSM controls the state of itself and its children. Beneath it should be a sprite animator and a simple physics body.

The state machine takes user input and tracks physics interactions. In reacting to these, it will change the current animation.