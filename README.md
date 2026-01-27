# Early modular animation state tweening system
This is an early framework for a modular animation state system for material changes - and it uses Dotween as a tween library for the tweening itself.
<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/Transitions.gif" alt="Alt text">

## Summary:
- Artist-friendly **inspector tween editing** functionality for material properties.
- **Swappable animation states** for any **material.**
- **Inspector level state creation** - immediately create, name and edit new states.
- **Assign triggers with state data** in code when needed.
- Animates MBP's, with shared material on objects.
- Capable of expanding toward **object transform tweening states**, or **any values needing lerping in code.**
## Goals of this project.
The original purpose of making this system, was to drive the animation of an SDF button project I had made.
Instead of using separate images for complex motions, I opted to create and drive the effects in a shader.
In this repo, I reapplied the system to a 3D object for demo purposes and to show its capabilities. 

You can find the project [here](https://chain-collision-39b.notion.site/SDF-Buttons-and-Animation-System-Setup-2e993cf6835a80728d9be448a5bbd4fa?source=copy_link), and here is a preview of the effect:

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/Shapemap.gif" alt="Alt text">

It uses material creation as an authoring tool, to set up states visually, after which, state data is set in a scriptable object, and can be used and swapped when needed.

This is currently only applied to material values, however the system can also be repurposed in future to tween any needed float values for animation. 
Such as game object Transform - where transform states could then be stored and shared across objects, triggering the tweens with swappable state data.

## Current setup in Unity:
### Creating states
- You can create materials in the project folder based on material states for each animation. 
- These are used to help visualize what the material will look like, and are not used in-game. 
- Then create the scriptable object to assign the material values of the state.
- The StateSetter is used to set the values of all the states, whenever you create a new state or edit the materials and want to update state data.
- NOTE: When adding more properties to animate, you can name them starting with “PR”. Only these are added, to avoid adding unwanted material properties.

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/CreateNewState.gif" alt="Alt text">
<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/MaterialSetup.gif" alt="Alt text">

### Assign animation states and set their settings in the inspector
- Assign new states, select the scriptable object state to trigger, and set their settings in the inspector in the animation system script. The Animation System script controls the tweening and settings of properties.
- You can set Ease, strength and extra for each property separately. 
- These are the dotween Tweener settings.
- There is also an option for reset. This allows tweens to instantly lerp back to the previous state. This is for animations like clicks or attacks below, that need to return.

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/SettingStatesInspect.gif" alt="Alt text">

### Transition demo
- There is a demo trigger script. Whenever you call the animation, you send in the state scriptable object you want to trigger, and the animation system handles it on the object. 

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/Transitions.gif" alt="Alt text">

- Here are two objects, with the same material, being animated through MBPs.

<img src="https://github.com/lysardz/Modular-Tweening-System/blob/main/SetupGifs/Material%20Property%20Blocks.gif" alt="Alt text">

## NOTES AND FUTURE CONSIDERATIONS
### On use cases:
- This system was useful, and originated for the purposes of my sdf button project. The project wanted to avoid using multiple image parts and animations, and instead animate property values for the specific effects.
- Not all situations would call for material value tweening. For instance, an effect could potentially only need a single value set to 0 and changed in the shader to drive the visuals.
- But this system allows artist controls and detailed tween settings for my use case.
- And, as I mentioned before, the core logic can be used in the context of any value changes for states, such as Transform manipulation.
  
### On material parameters:
- Right now the script handles adding any amount of new properties, but it does not currently handle removing them in the inspector, thus they need to be removed manually for now.

### On tweening considerations:
- Right now, rapid fire of animation can visually make transitions look like they stutter potentially - a safeguard should be considered in the future, such as tween count tracking and blocking. Or a buffer time that can be set, to counter animation triggers. 
- Material parameters are updated continuously here. In case of need, frame capping the material property setting in the loop itself can be considered. 
- A tweener is set for each property - this is so ease settings can be edited for each property. However, if tween settings don’t need to be set for each property, this system can work with 1 single tween, used to update all properties together instead.

- Tweener durations cannot be set dynamically in Dotween. Thus for this setup, all tweens share the same duration. However, this functionality can be set up mathematically in the future:
  - The tween driver lerps tweens by using 0-1 multiplied by a calculation to add the difference between new and old values to the current material.
  - This smoothly lerps it to the newest target value without setting values directly.
  - We can set a duration for each tween, then add up durations, and use the ratios as thresholds to edit the driver.
  - Thus different tweens would reach completion and finish at different times visually.
