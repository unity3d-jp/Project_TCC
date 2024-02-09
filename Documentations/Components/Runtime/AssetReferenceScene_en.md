# AssetReferenceScene

#### **Namespace**: Unity.SceneManagement
---

## Summary:
`AssetReferenceScene` is a component representing an asset reference to a scene. This component manages references to scene assets and provides asset validation functionality.

## Features and Operation:
- **Scene Asset Reference**: `AssetReferenceScene` holds a reference to an asset of type `SceneReference`.
- **Asset Validation**: When used within the Unity editor, it validates whether the asset path is valid as a scene asset.
- **Editor-Only Asset Retrieval**: Used exclusively within the Unity editor, it retrieves the scene asset based on the specified GUID.

## Properties
| Name | Description |
|------------------|-------------|
| `editorAsset` | Used within the Unity editor, it retrieves the specified scene asset based on the GUID. |

## Methods
| Name | Function |
|------------------|-------------|
|  ``public`` AssetReferenceScene( ``string guid`` )  | Constructs a new scene asset reference using the specified GUID. |
|  ``public override bool`` ValidateAsset( ``string path`` )  | Validates whether the asset at the specified path is valid as a scene asset. |

---
## Additional Notes
- The `AssetReferenceScene` and `SceneReference` classes are designed to manage references to scene assets using the Addressable Asset System, making scene management in games more flexible and efficient.
