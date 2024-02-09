# TpsCameraControl

#### **Namespace**: Unity.TinyCharacterController.Control
---

## Summary:
`TpsCameraControl` is a component that manages changes in the camera's perspective. It updates the direction of the Transform specified by `CameraRoot` using `RotateCamera`, limiting changes in the vertical direction to the angle specified by `CameraPitch`. The speed of movement of the camera's perspective is corrected by `CameraUserSettings`. When `TurnPriority` is high, the character will face the direction specified by the camera.

## Features and Operation:
- **Updating Camera Perspective**: Updates the direction of the camera's root Transform using `UpdateCamera`.
- **Camera Perspective Limitation**: Limits changes in the vertical direction to the angle specified by `CameraPitch`.
- **Correction of Camera Movement Speed**: The speed of camera perspective movement is corrected using `CameraUserSettings`.
- **Character Orientation Change**: When `TurnPriority` is high, the character will face the direction specified by the camera.

## Properties
| Name | Description |
|------------------|------|
| `_cameraRoot` | The Transform to be updated by `UpdateCamera`. |
| `_cameraPitch` | The maximum elevation angle of `CameraRoot`. |
| `_userSettings` | User settings for operating the camera. |
| `TurnPriority` | Rotation priority. When this component has the highest priority, the character will face the direction of the camera. |
| `_characterTurnSpeed` | Rotation speed of the character. If this value is -1, the character immediately changes direction. |

## Methods
| Name | Function |
|------------------|------|
| ``void`` RotateCamera( ``Vector2 inputLook`` ) | Updates the camera rotation. `inputLook` represents the mouse delta position. |
| ``void`` ForceUpdateRotation( ``Quaternion rotation`` ) | Forces the camera to point in a specified direction. `CameraRoot` is also immediately updated. |
| ``void`` OnUpdate( ``float deltaTime`` ) | Performs camera updates. |

---
## Additional Notes
- The camera operation is designed to be resolution-independent.
- The camera root is moved to a separate object to prevent character orientation changes from interfering with camera orientation changes.