# AimCheck

#### **Namespace**: Unity.TinyCharacterController.Check
---

## Summary:
`AimCheck` is a component that retrieves the object located at the center of the screen. It continuously checks every frame as long as the component is enabled, obtaining information such as the target object and the distance to the target. If the object is visible on the screen but not from the character's perspective, it prioritizes the character's perspective.

## Features and Operation:
- **Perspective-Based Aiming**: Detects the object at the center of the screen from the character's perspective.
- **Customizable Aim**: Customizes the starting point of the aim and the target point on the screen.
- **Additional Hit Mask**: Includes additional contactable layers in addition to terrain detection in CharacterSettings.
- **Maximum Range**: Sets the maximum distance that can be detected by AimCheck.
- **Unity Event Invocation**: Invokes a Unity Event when the object in front of the viewpoint changes.

## Properties
| Name | Description |
|------------------|------|
| `IsUseAimParallax` | Whether to include the character's perspective in the visibility check. |
| `_origin` | The starting point from which AimCheck casts a ray. |
| `_aimTargetViewportPoint` | The target point on the screen. |
| `_additionalHitMask` | Additional contactable layers in addition to the terrain detection in CharacterSettings. |
| `_maxRange` | The maximum distance that can be detected by AimCheck. |
| `OnChangeAimedGameObject` | Unity Event invoked when the object in front of the viewpoint changes. |

## Methods
| Name | Function |
|------------------|------|
| ``public bool`` HitCheck( ``Vector2 center, out Vector3 point, out Vector3 normal, out float distance, out GameObject hitGameObject, out Vector3 direction`` ) | Performs an aiming check from the center of the screen. |
| ``public bool`` HitCheck( ``out Vector3 point, out Vector3 normal, out float distance, out GameObject hitGameObject, out Vector3 direction`` ) | Immediately performs the detection of the object in the line of sight of AimCheck. |

