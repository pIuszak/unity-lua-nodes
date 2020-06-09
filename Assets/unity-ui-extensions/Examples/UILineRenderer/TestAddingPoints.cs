using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.Examples
{
    public class TestAddingPoints : MonoBehaviour
    {
        public UILineRenderer LineRenderer;
        public Text XValue;
        public Text YValue;

        // Use this for initialization


        public void ClearPoints()
        {
            LineRenderer.Points = new Vector2[0];
        }
    }
}