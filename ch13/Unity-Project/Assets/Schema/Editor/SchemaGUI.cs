using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SchemaEditor.Internal;
using UnityEditor;
using UnityEngine;

namespace SchemaEditor
{
    public static class SchemaGUI
    {
        public static void DoIconText(Rect position, string text, GUIStyle style, params Texture[] images)
        {
            GUIContent[] contents = MakeContents(text, style, images);

            float x = 0f;

            foreach (GUIContent content in contents)
            {
                if (string.IsNullOrWhiteSpace(content.text) && content.image == null)
                    continue;

                content.text = content.text.Trim();

                Vector2 size = style.CalcSize(content);
                EditorGUI.LabelField(new Rect(position.x + x, position.y, size.x, size.y), content, style);
                x += size.x;
            }
        }

        private static GUIContent[] MakeContents(string text, GUIStyle style, params Texture[] images)
        {
            MatchCollection matchCollection = Regex.Matches(text, @"{(\d+)}");
            List<int> i = new List<int>();

            foreach (Match match in matchCollection)
            {
                if (!int.TryParse(match.Groups[1].Value, out int index))
                    throw new ArgumentException("Text was not a valid format string");

                i.Add(index);
            }

            string[] s = Regex.Split(text, @"{\d+}");

            GUIContent[] info = new GUIContent[s.Length + i.Count];

            for (int j = 0; j < info.Length; j++)
            {
                GUIContent label = new GUIContent();

                if (j % 2 == 0)
                    label.text = s[j / 2];
                else
                    label.image = images[i[(j - 1) / 2]];

                info[j] = label;
            }

            return info;
        }

        public static Vector2 GetSize(string text, GUIStyle style, params Texture[] images)
        {
            GUIContent[] contents = MakeContents(text, style, images);

            Vector2 ret = Vector2.zero;

            foreach (GUIContent content in contents)
            {
                if (string.IsNullOrWhiteSpace(content.text) && content.image == null)
                    continue;

                content.text = content.text.Trim();

                Vector2 size = style.CalcSize(content);
                ret = new Vector2(ret.x + style.CalcSize(content).x, Mathf.Max(ret.y, size.y));
            }

            return ret;
        }

        public static void DrawRoundedBox(Rect position, Color background, float radius)
        {
            Texture2D tex = Icons.GetResource("px", false);
            GUI.DrawTexture(position, tex, ScaleMode.StretchToFill, true, 0, background, 0, radius);
        }

        public static void DrawRoundedBox(Rect position, Color background, Vector4 radius)
        {
            Texture2D tex = Icons.GetResource("px", false);
            GUI.DrawTexture(position, tex, ScaleMode.StretchToFill, true, 0, background, Vector4.zero, radius);
        }

        public static void DrawRoundedBox(Rect position, Color background, Color border, float radius,
            float borderWidth)
        {
            Texture2D tex = Icons.GetResource("px", false);
            // background
            GUI.DrawTexture(position, tex, ScaleMode.StretchToFill, true, 0, background, 0, radius);
            // border
            GUI.DrawTexture(position, tex, ScaleMode.StretchToFill, true, 0, border, borderWidth, radius);
        }

        public static void DrawRotatedTexture(Rect position, Texture image, float angle)
        {
            if (angle == 0f)
            {
                GUI.DrawTexture(position, image);
            }
            else
            {
                Matrix4x4 last = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, position.center);
                GUI.DrawTexture(position, image);
                GUI.matrix = last;
            }
        }

        public static void DoDescriptionLabel(ICanvasContextProvider context, string description, GUIStyle style)
        {
            if (string.IsNullOrEmpty(description) || context == null)
                return;

            Rect contextRect = context.GetViewRect();

            float availableWidth = contextRect.width -
                                   (Prefs.minimapEnabled && (Prefs.minimapPosition == 0 || Prefs.minimapPosition == 2)
                                       ? Prefs.minimapWidth
                                       : 0f) - 30f;

            float width = Mathf.Min(style.CalcSize(new GUIContent(description)).x, availableWidth);
            float height = style.CalcHeight(new GUIContent(description), width);

            switch (Prefs.minimapPosition)
            {
                case 0:
                case 1:
                    GUI.Label(new Rect(contextRect.xMax - width - 10f, contextRect.yMax - height - 10f, width, height),
                        description, style);
                    break;
                case 2:
                case 3:
                    GUI.Label(new Rect(contextRect.x + 10f, contextRect.yMax - height - 10f, width, height),
                        description, style);
                    break;
            }
        }

        private struct LabelInfo
        {
            public Rect position;
            public string text;
            public Texture image;
        }
    }
}