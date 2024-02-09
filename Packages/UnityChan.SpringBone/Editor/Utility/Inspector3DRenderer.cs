using UnityEngine;

namespace UTJ
{
    public class Inspector3DRenderer
    {
        public Inspector3DRenderer()
        {
            const string ShaderName = "Hidden/Internal-Colored";
            var shader = Shader.Find(ShaderName);
            renderMaterial = new Material(shader);
        }

        public void BeginRender(Rect rect)
        {
            // https://answers.unity.com/questions/1360515/how-do-i-draw-lines-in-a-custom-inspector.html?childToView=1360603#answer-1360603
            GUI.BeginClip(rect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            renderMaterial.SetPass(0);
        }

        public void EndRender()
        {
            GL.PopMatrix();
            GUI.EndClip();
        }

        // Must be in a GL.LINES block
        public static void DrawHollowRect(Rect rect, Color color)
        {
            GL.Color(color);
            GL.Vertex3(rect.x, rect.y, 0f);
            GL.Vertex3(rect.xMax, rect.y, 0f);
            GL.Vertex3(rect.xMax, rect.y, 0f);
            GL.Vertex3(rect.xMax, rect.yMax, 0f);
            GL.Vertex3(rect.xMax, rect.yMax, 0f);
            GL.Vertex3(rect.x, rect.yMax, 0f);
            GL.Vertex3(rect.x, rect.yMax, 0f);
            GL.Vertex3(rect.x, rect.y, 0f);
        }

        // Must be in a GL.LINES block
        public static void DrawArrow(Vector2 start, Vector2 end, Color color, float headRatio = 0.05f)
        {
            GL.Color(color);
            GL.Vertex3(start.x, start.y, 0f);
            GL.Vertex3(end.x, end.y, 0f);

            var startToEnd = end - start;
            var crossVector = headRatio * new Vector2(startToEnd.y, -startToEnd.x);
            var headEndOrigin = start + (1f - headRatio) * startToEnd;
            var headPoint1 = headEndOrigin - crossVector;
            var headPoint2 = headEndOrigin + crossVector;

            GL.Vertex3(end.x, end.y, 0f);
            GL.Vertex3(headPoint1.x, headPoint1.y, 0f);
            GL.Vertex3(end.x, end.y, 0f);
            GL.Vertex3(headPoint2.x, headPoint2.y, 0f);
        }

        // private

        private Material renderMaterial;
    }
}