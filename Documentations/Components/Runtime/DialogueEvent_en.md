# DialogueEvent

#### **Namespace**: Unity.ScenarioImporter
---

## Summary:
The `DialogueEvent` struct represents a specific event occurring within a scenario, encapsulating the event's name and its associated arguments. It is employed to introduce dynamic elements into dialogues and scenarios, enriching the interactive storytelling experience.

## Properties
| Name | Description |
|------------------|------|
| `EventName` | The identifier or name of the event. |
| `EventArgs` | An array of arguments associated with the event, providing additional context or parameters required for event execution. |

---
## Additional Notes
- The `DialogueEvent` struct enables event-driven interactions within scenarios and dialogues, allowing for a more engaging and responsive narrative structure. By specifying events and their parameters, developers can craft complex narrative mechanisms, such as character interactions, item acquisitions, or branching storylines based on certain conditions.
- Designed as a serializable struct, it allows for straightforward configuration and modification within the Unity Editor, streamlining the game development workflow. This accessibility facilitates the management of in-game events even for team members without extensive programming knowledge, promoting a collaborative and efficient development environment.
