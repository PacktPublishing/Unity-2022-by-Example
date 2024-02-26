using System;
using System.Collections.Generic;
using System.Linq;
using Schema;
using Schema.Internal;
using Schema.Utilities;
using SchemaEditor.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SchemaEditor.Internal.ComponentSystem.Components
{
    public sealed class ConditionalComponent : GUIComponent, IViewElement, ISelectable, IEditable, IGraphObjectProvider,
        IDeletable, ICopyable
    {
        public const float Separation = 8f;
        public const float Height = 32f;
        private static ConditionalComponent moving;
        private static Vector2 dxdyMouseDown;
        private static int desiredIndex;
        private static int originalIndex;
        private Rect _rect;

        private NodeComponent parent;
        private Color statusColor;
        private float t;
        public Rect rect => _rect;
        public Conditional conditional { get; private set; }

        public string uID { get; private set; }

        public bool isSelected { get; private set; }

        public bool IsCopyable()
        {
            return isSelected;
        }

        public Object GetCopyable()
        {
            return conditional;
        }

        public bool IsDeletable()
        {
            IEnumerable<NodeComponent> nodeComponents = canvas.selected
                .Where(x => x is NodeComponent)
                .Cast<NodeComponent>();

            return isSelected && !nodeComponents.Contains(parent);
        }

        public void Delete()
        {
            conditional.node.RemoveConditional(conditional);

            SceneView.RepaintAll();
        }

        public Object GetEditable()
        {
            return conditional;
        }

        public bool IsEditable()
        {
            return true;
        }

        public bool Equals(GraphObject graphObject)
        {
            Conditional conditional = graphObject as Conditional;

            if (conditional != null)
                return conditional.uID == uID;
            return false;
        }

        public bool IsSelectable()
        {
            return true;
        }

        public bool IsSelected()
        {
            return isSelected;
        }

        public bool IsHit(Vector2 position)
        {
            return rect.Contains(position);
        }

        public bool Overlaps(Rect rect)
        {
            return this.rect.Overlaps(rect, true);
        }

        public void Select(bool additive)
        {
            isSelected = true;

            dxdyMouseDown = canvas.mousePositionNoZoom - rect.position;
            originalIndex = Array.IndexOf(conditional.node.conditionals, conditional);

            SceneView.RepaintAll();
        }

        public void Deselect()
        {
            isSelected = false;

            SceneView.RepaintAll();
        }

        public Rect GetGridRect()
        {
            int index = Array.IndexOf(conditional.node.conditionals, conditional);
            int length = conditional.node.conditionals.Length;

            float height = 32f;

            int upCount = length - index;

            GUIContent content = conditional.GetConditionalContent();
            Texture2D icon = conditional.icon;

            Vector2 contentSize = Styles.Conditional.CalcSize(content);
            contentSize.x += icon != null ? 20f : 0f;

            Vector2 pos = new Vector2(parent.layout.gridRect.center.x - contentSize.x / 2f,
                parent.layout.gridRect.y - (height + 18f) * upCount);

            Rect r = new Rect(pos.x, pos.y, contentSize.x, height);
            return r;
        }

        private void DoHighlight()
        {
            if (!Application.isPlaying || !Prefs.enableStatusIndicators || canvas.activeInScene == null ||
                canvas.activeInScene.target != conditional.node.graph)
            {
                statusColor = new Color(0f, 0f, 0f, 0f);
                return;
            }

            ExecutableNode executableNode = canvas.activeInScene.tree.GetExecutableNode(conditional.node);

            switch (executableNode.GetLastConditionalStatus(Array.IndexOf(conditional.node.conditionals, conditional)))
            {
                case true:
                    statusColor = Prefs.successColor;
                    break;
                case false:
                    statusColor = Prefs.failureColor;
                    break;
            }
        }

        public override void Create(CreateArgs args)
        {
            ConditionalComponentCreateArgs createArgs = args as ConditionalComponentCreateArgs;

            if (createArgs == null)
                throw new ArgumentException();

            createArgs.layer = 2;

            if (createArgs.fromExisting != null)
            {
                conditional = createArgs.fromExisting;
                uID = conditional.uID;
            }
            else
            {
                conditional = createArgs.node.AddConditional(createArgs.conditionalType);
                uID = conditional.uID;
            }

            parent = (NodeComponent)canvas.FindComponent(conditional.node);
        }

        public override void OnGUI()
        {
            if (parent == null)
                return;

            int index = Array.IndexOf(conditional.node.conditionals, conditional);
            int length = conditional.node.conditionals.Length;

            if (moving == this)
            {
                float dy = parent.layout.body.y - Event.current.mousePosition.y;
                desiredIndex = length - Mathf.RoundToInt(dy / 50f);
                index = desiredIndex;
            }
            else if (moving != null)
            {
                if (index > originalIndex && index <= desiredIndex)
                    index--;
                else if (index < originalIndex && index >= desiredIndex)
                    index++;
            }

            index = Mathf.Clamp(index, 0, length - 1);

            DoEvents();

            DoHighlight();

            int upCount = length - index;

            GUIContent content = conditional.GetConditionalContent();
            Texture2D icon = conditional.icon;

            Vector2 contentSize = Styles.Conditional.CalcSize(content);
            contentSize.x += icon != null ? 20f : 0f;

            Vector2 pos = new Vector2(parent.layout.body.center.x - contentSize.x / 2f,
                parent.layout.body.y - (Height + Separation) * upCount);

            _rect = new Rect(pos.x, pos.y, contentSize.x, Height);

            Rect decorator_out = new Rect(_rect.center.x - Separation * 1.5f, _rect.yMax, Separation * 3, Separation);
            EditorGUI.DrawRect(decorator_out, Styles.WindowBackground);

            // GUI.DrawTexture(decorator_out, Icons.GetResource("decorator_out", false));
            // SchemaGUI.DrawRoundedBox(new Rect(_rect.center.x - 12f, decorator_out.yMax - 12f, 24f, 12f),
            //     Styles.windowBackground, new Vector4(8, 8, 0, 0));

            GUI.color = Color.white;

            Rect r = _rect;

            if (moving == this)
            {
                GUI.backgroundColor = new Color(Styles.WindowBackground.r, Styles.WindowBackground.g,
                    Styles.WindowBackground.b, 0.5f);
                Styles.Element.DrawIfRepaint(_rect.Pad(-10), false, false, false, false);
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.25f);
                Styles.Outline.DrawIfRepaint(_rect, false, false, false, false);

                r = new Rect(Event.current.mousePosition - dxdyMouseDown, new Vector2(contentSize.x, Height));
            }

            SchemaGUI.DrawRoundedBox(r, Styles.WindowBackground,
                isSelected ? Prefs.selectionColor : Styles.OutlineColor, 8f, 2f);
            Styles.Conditional.DrawIfRepaint(r, content, false, false, false, false);

            if (icon != null)
                GUI.DrawTexture(new Rect(r.x + Styles.Conditional.padding.left, r.y + 8f, 16f, 16f), icon);

            if (statusColor != new Color(0f, 0f, 0f, 0f))
            {
                GUI.color = statusColor;

                GUI.DrawTexture(new Rect(_rect.position, Vector2.one * 16f).UseCenter(),
                    Icons.GetResource("round", false));

                GUI.color = Color.white;
            }

            t = Time.realtimeSinceStartup;
        }

        private void DoEvents()
        {
            Event e = Event.current;

            switch (e.rawType)
            {
                case EventType.MouseDrag when e.button == 0 && canvas.selected.Length == 1 && isSelected &&
                                              canvas.selectionBoxComponent.hidden:
                    moving = this;

                    break;
                case EventType.MouseUp when e.button == 0:
                    if (moving != null)
                        moving.conditional.node.MoveConditional(moving.conditional,
                            Mathf.Clamp(desiredIndex, 0, moving.conditional.node.conditionals.Length - 1));

                    moving = null;
                    break;
            }
        }

        public class ConditionalComponentCreateArgs : CreateArgs
        {
            public Node node { get; set; }
            public Conditional fromExisting { get; set; }
            public Type conditionalType { get; set; }
        }
    }
}