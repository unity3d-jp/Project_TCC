# PreDownloadFiles

#### **Namespace**: Unity.SceneManagement
---

## Summary:
`PreDownloadFiles` is a component designed to manage the pre-download of specified asset scenes. It checks the download size of assets and initiates the download of dependencies if necessary.

## Features and Operation:
- **Download Size Check**: At the start, it checks the download size of the specified scene assets.
- **Dependencies Download**: If the download size is greater than zero, it starts downloading the dependencies for the specified scenes.
- **Download Progress Tracking**: Through the `Update` method, it logs the progress of the download.

## Properties
| Name | Description |
|------|-------------|
| `_scenes` | List of scene assets to pre-download. |

## Methods
- There are no public methods.

---
## Additional Notes
- This component can be particularly beneficial in games or applications with large assets, improving user experience by front-loading download times during the initial stages of gameplay, thereby reducing wait times and ensuring smoother gameplay.
