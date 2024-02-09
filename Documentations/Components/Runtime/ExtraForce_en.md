# ExtraForce

#### **Namespace**: Unity.TinyCharacterController.Effect
---

## Summary:
`ExtraForce` is a component that applies external impacts to a character, decelerating due to air resistance and friction with the ground.

## Features and Operation:
- **Application of Friction and Air Resistance**: Applies friction and air resistance to the character, affecting its acceleration.
- **Setting Bounce Intensity**: Sets the character's bounce intensity, controlling the behavior upon collision.
- **Acceleration Stop Threshold**: Sets a threshold to stop movement when acceleration falls below a certain level.
- **Collision Callbacks with Other Colliders**: Triggers specific events when colliding with other colliders.
- **Gizmo Visualization**: Visualizes the character's movement in the editor using Gizmos.

## Properties
| Name | Description |
|------------------|------|
| `Bounce` | A `float` property that represents the character's bounce intensity. |
| `Friction` | A `float` property indicating the friction applied to the character. |
| `Drag` | A `float` property representing the strength of air resistance. |
| `Threshold` | A `float` property that sets the threshold below which acceleration stops, halting movement. |
| `OnHitOtherCollider` | A `UnityEvent<Collider>` property that triggers when colliding with other colliders. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` AddForce( ``Vector3 value`` )  | Method to add force to the character. |
|  ``public void`` SetVelocity( ``Vector3 value`` )  | Method to override the velocity. |

