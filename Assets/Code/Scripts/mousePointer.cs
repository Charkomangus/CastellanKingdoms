using UnityEngine;

namespace Assets.Code.Scripts
{
    public class mousePointer : MonoBehaviour {public Texture2D cursorImage;

        private int m_cursorWidth = 32;
        private int m_cursorHeight = 32;

        // Use this for initialization
        void Start () {
            Cursor.visible = false;
        }
	
        //Show Custom Cursor
        void OnGUI()
        {
            GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, m_cursorWidth, m_cursorHeight), cursorImage);
        }
    }
}
