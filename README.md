# Early modular animation state tweening system
This is an early framework for a modular animation state tweening system.
In the context of this project, it is used to tween material property values between states that encapsulate the property values assigned in that state. 

It uses material creation as an authoring tool, to set up states visually, after which, state data is set in a scriptable object, and can be used and swapped when needed.

This is currently only applied to material values, however the system can also be repurposed to tween any needed float values for animation. Such as, game object Transform. Transform states could then be stored and shared across objects, triggering the tweens with swappable state data.

## Current setup in Unity:
- You can create materials in the project folder based on material states for each animation. 
- These are used to help visualise what the material will look like, and are not used in-game. 
- Then create the scriptable object to assign the material values of the state.

