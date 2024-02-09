using UnityEngine;
using UnityEngine.UI;

namespace Unity.SceneManagement.Samples.Samples.Scripts
{
    public class MenuControl : MonoBehaviour
    {
        [SerializeField] private Transform _uiRoot;

        private Button _cancelButton;
        private Text _count;
        private Counter _ownerCounter;
    

        private void Awake()
        {
            _uiRoot.Find("Background/CancelButton")?.TryGetComponent(out _cancelButton);
            _uiRoot.Find("Background/Count")?.TryGetComponent(out _count);

            _cancelButton.onClick.AddListener(OnCancelClick); 
        }

        private void Start()
        {
            gameObject.GetOwner()?.TryGetComponent(out _ownerCounter);

            Repaint();
        }

        private void Repaint()
        {
            if (_ownerCounter != null)
            {
                _count.text = _ownerCounter.Count.ToString("00");
            }
        }

        private void OnCancelClick()
        {
            gameObject.GetOwner().SetActive(false);
        }
    }
}
