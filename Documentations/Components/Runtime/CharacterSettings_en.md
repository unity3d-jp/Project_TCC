# CharacterSettings

#### **Namespace**: Unity.TinyCharacterController

---

## Summary:
`CharacterSettings` configures the necessary settings for the behavior of `IBrain` and TCC components. The character's height and width are determined by `CharacterSettings.Height` and `CharacterSettings.Radius`.

## Features and Operation:
- **Environment Settings**: Sets the layer for recognizing terrain colliders.
- **Character Settings**: Sets the force applied when colliding with other rigid bodies.
- **Character's Height**: Sets the character's height.
- **Character's Width**: Sets the character's width.
- **Collider Storage**: A list for storing Collider components under the GameObject.
- **Camera Settings**: Sets and retrieves information related to the character's camera.

## Properties
| Name | Description |
|------------------|------|
| `_environmentLayer` | Layer for recognizing terrain colliders. |
| `_mass` | Force applied when colliding with other rigid bodies. |
| `_height` | Character's height. |
| `_radius` | Character's width. |
| `HasCamera` | Indicates whether a camera is set. |
| `CameraMain` | Gets or sets the character's related camera information. |
| `CameraTransform` | Retrieves the camera's Transform. |
| `CameraYawRotation` | Retrieves the Y-axis rotation of the camera. |

## Methods
| Name | Function |
|------------------|------|
| ``public void`` GatherOwnColliders() | Collects colliders belonging to the character. |
| ``public bool`` IsOwnCollider(``Collider col``)` | Checks if the specified collider belongs to the character. |
| ``public bool`` ClosestHit(``RaycastHit[] hits, int count, float maxDistance, out RaycastHit closestHit``)` | Retrieves the closest RaycastHit excluding the character's own colliders. |
| ``public Vector3`` PlayerInputToWorldSpaceDirection(``Vector2 input``)` | Converts player input to world space direction on the screen. |

---
## Additional Notes
- `CharacterSettings` is a crucial component for controlling the physical characteristics and camera behavior of the character. It manages settings related to height, width, mass, and layers associated with the terrain and environment the character interacts with.
- The component provides settings for managing basic movements in gameplay, such as character movement and camera rotation, enabling natural and smooth character movements using the game's physics engine.
- Offers functionality needed for game character control, including management of the character's own colliders and calculation of movement direction based on player inputs.

