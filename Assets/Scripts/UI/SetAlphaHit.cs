using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class SetAlphaHit : MonoBehaviour
    {
        void Start()
        {
            Image image = GetComponent<Image>();
            image.alphaHitTestMinimumThreshold = 0.1f;
        }

    }
}
