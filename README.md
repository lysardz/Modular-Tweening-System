# Early modular animation state tweening system
This is an early framework for a modular animation state tweening system.
In the context of this project, it is used to tween material property values between states that encapsulate the property values assigned in that state. 

It uses material creation as an authoring tool, to set up states visually, after which, state data is set in a scriptable object, and can be used and swapped when needed.

This is currently only applied to material values, however the system can also be repurposed to tween any needed float values for animation. Such as, game object Transform. Transform states could then be stored and shared across objects, triggering the tweens with swappable state data.

## Current setup in Unity:
- You can create materials in the project folder based on material states for each animation. 
- These are used to help visualise what the material will look like, and are not used in-game. 
- Then create the scriptable object to assign the material values of the state.
- The StateSetter is used to set the values of all the states, whenever you create a new state or edit the materials and want to update state data.

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/CreateNewState.gif" alt="Alt text">
<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/MaterialSetup.gif" alt="Alt text">

- Assign new states, select the scriptable object state to trigger, and set their settings in the inspector in the animation system script. The Animation System script controls the tweening and settings of properties.
- You can set Ease, strength and extra for each property separately. 
- These are the dotween Tweener settings.
- There is also an option for reset. This allows tweens to instantly lerp back to the previous state. This is for animations like clicks or attacks below, that need to return.

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/SettingStatesInspect.gif" alt="Alt text">

- There is a demo trigger script. Whenever you call the animation, you send in the state scriptable object you want to trigger, and the animation system handles it on the object. 

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/TriggerNewState.gif" alt="Alt text">

