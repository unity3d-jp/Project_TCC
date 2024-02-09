# DialogueScript

#### **Namespace**: Unity.ScenarioImporter
---

## Summary:
`DialogueScript` is a `ScriptableObject` class designed for managing dialogues and events within scenarios. It stores various sections and messages of a scenario, offering functionality to retrieve specific sections based on tags, facilitating dynamic narrative flow and event handling in games.

## Properties
| Name | Description |
|------------------|------|
| `DialogueEntries` | A list of `DialogueEntry` objects, representing individual dialogues or messages within the scenario. |

## Methods
| Name | Function |
|------------------|------|
|  ``public void`` GetSliceIndex( ``string tag, out int start, out int end`` )  | "Retrieves the range of the scenario associated with a specific tag, providing the start and end indices of the segment." |
|  ``public List<DialogueEntry>`` GetDialogueEntriesWithTag( ``string tag`` )  | "Obtains a list of dialogue entries associated with a given tag, allowing for targeted retrieval of scenario content." |

---
## Additional Notes
- Utilizing the `DialogueScript` class enables flexible management of scenario content, allowing developers to easily extract and utilize specific sections of dialogues or events based on tags. This is particularly useful for tailoring narrative content to specific game states or player actions.
- The `GetSliceIndex` method is instrumental in identifying the start and end points of scenario segments defined by tags, facilitating the extraction of coherent narrative sections for gameplay integration.
- The `GetDialogueEntriesWithTag` method simplifies the process of grouping and accessing dialogue entries by their associated tags, enhancing the organization and retrieval of scenario elements for dynamic narrative experiences.
