using Unity.Collections;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class MeshElement : VisualElement
    {
        [UxmlAttribute]
        public Mesh Mesh
        {
            get => mesh;
            set
            {
                mesh = value;
                if (main != null)
                    main.GetComponent<MeshFilter>().sharedMesh = mesh;
            }
        }

        private Mesh mesh;

        [UxmlAttribute]
        public Shader Shader
        {
            get => shader;
            set
            {
                if (shader != value)
                {
                    shader = value;
                    material = new Material(shader);
                    if (main != null)
                        main.GetComponent<MeshRenderer>().material = material;
                }
            }
        }

        [UxmlAttribute]
        public float ObjectSize
        {
            get => objectSize;
            set
            {
                objectSize = value;
                if (main != null)
                    main.transform.localScale = Vector3.one * objectSize;
            }
        }
        private float objectSize = 1.0f;

        [UxmlAttribute]
        public bool canRotate = true;
        [UxmlAttribute]
        public bool canDolly = true;
        [UxmlAttribute]
        public bool canPan = true;


        [UxmlAttribute]
        public Shader shader;

        [UxmlAttribute]
        public bool constantRepaint = true;

        private Material material;
        private PreviewRenderUtility preview;
        private GameObject root;
        private GameObject main;
        private RenderTexture renderTexture;

        private const float lookSensibility = 3;
        private const float dollySensibility = 2;
        private const float panSensibility = 2;
        private const float wheelSensibility = 0.05f;

        private float yaw = 0.78f;
        private float pitch = 0.67f;
        private float dolly = 2f;
        private Vector3 pivot;

        private Drag[] drags = new Drag[3];
        private double lastRepaint;

        public MeshElement()
        {
            this.focusable = true;
            this.generateVisualContent += OnGenerateVisualElement;

            this.RegisterCallback<PointerDownEvent>(OnPointerDown);
            this.RegisterCallback<PointerUpEvent>(OnPointerUp);
            this.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            this.RegisterCallback<WheelEvent>(OnWheel);
            this.RegisterCallback<KeyDownEvent>(OnKeyDown);
            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            if (mesh == null || shader == null)
                return;

            if (material == null)
                material = new Material(shader);

            preview = new PreviewRenderUtility();

            //Build scene
            root = new GameObject();
            root.hideFlags = HideFlags.HideAndDontSave;
            preview.AddSingleGO(root);

            //Main mesh
            main = new GameObject();
            main.hideFlags = HideFlags.DontSave;
            main.transform.SetParent(root.transform);
            main.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            main.transform.localScale = Vector3.one * objectSize;

            MeshFilter filter = main.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer rend = main.AddComponent<MeshRenderer>();
            rend.sharedMaterial = material;

            //Camera
            preview.camera.fieldOfView = 60;
            preview.camera.nearClipPlane = 0.03f;
            preview.camera.farClipPlane = 100f;
            preview.camera.transform.position = new Vector3(0, 0, -10);
            preview.camera.transform.LookAt(Vector3.zero);

            //Loop
            if (constantRepaint)
                EditorApplication.update += Loop;
        }


        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            if (root != null)
                EditorSceneManager.ClosePreviewScene(root.scene);

            preview?.Cleanup();
            if (root != null)
                GameObject.DestroyImmediate(root);

            preview = null;
            material = null;
            renderTexture = null;

            //Loop
            EditorApplication.update -= Loop;
        }

        private void OnGenerateVisualElement(MeshGenerationContext ctx)
        {
            if (preview == null)
                return;
            
            Rect rect = this.contentRect;
            preview.BeginPreview(rect, GUIStyle.none);
            PlaceCamera();
            preview.camera.Render();

            //Assign render texture
            RenderTexture targetTexture = preview.EndPreview() as RenderTexture;
            if (renderTexture != targetTexture)
            {
                renderTexture = targetTexture;
                EditorApplication.delayCall += () => { style.backgroundImage = Background.FromRenderTexture(renderTexture); };
            }
        }

        private void Loop()
        {
            if ((EditorApplication.timeSinceStartup - lastRepaint) < 0.016f || !constantRepaint)
                return;
            lastRepaint = EditorApplication.timeSinceStartup;

            this.MarkDirtyRepaint();
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            ref Drag drag = ref drags[e.button];
            drag.pointerId = e.pointerId;
            drag.pressed = true;
            drag.startPosition = e.originalMousePosition;
            e.StopPropagation();
            MouseCaptureController.CaptureMouse(this);
            MarkDirtyRepaint();
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            ref Drag drag = ref drags[e.button];
            if (drag.pointerId == e.pointerId)
            {
                drag.pressed = false;
                drag.currentPosition = e.originalMousePosition;
                e.StopPropagation();
                MouseCaptureController.ReleaseMouse(this);
            }
            MarkDirtyRepaint();
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            for (int i = 0; i < 3; i++)
            {
                ref Drag drag = ref drags[i];
                if (drag.pointerId == e.pointerId)
                {
                    if (drag.pressed)
                    {
                        Vector2 delta = e.originalMousePosition - drag.currentPosition;
                        OnDrag(i, delta);
                    }
                    drag.currentPosition = e.originalMousePosition;
                }
            }
            MarkDirtyRepaint();
        }

        private void OnKeyDown(KeyDownEvent e)
        {
            if (!canPan)
                return;

            if (e.keyCode == KeyCode.F)
            {
                yaw = 0.78f;
                pitch = 0.67f;
                dolly = 2f;
                pivot = Vector2.zero;
                e.StopPropagation();
            }
            MarkDirtyRepaint();
        }

        private void OnWheel(WheelEvent e)
        {
            if (!canDolly)
                return;

            dolly = Mathf.Max( dolly + e.delta.y * wheelSensibility * Mathf.Clamp(dolly, 0.1f, 10.0f), 0.01f);
            e.StopPropagation();
            MarkDirtyRepaint();
        }

        private void OnDrag(int button, Vector2 delta)
        {
            if (preview == null)
                return;

            switch (button)
            {
                //Look
                case 0:
                    if (!canRotate)
                        return;

                    yaw += delta.x / contentRect.height * lookSensibility;
                    pitch = Mathf.Clamp(pitch + delta.y / contentRect.height * lookSensibility, - Mathf.PI * 0.49f, Mathf.PI * 0.49f);
                    break;

                //Dolly
                case 1:
                    if (!canDolly)
                        return;

                    dolly = Mathf.Max(dolly - delta.x / contentRect.height * dollySensibility * Mathf.Clamp(dolly, 0.1f, 10.0f), 0.01f);
                    break;

                //Pan
                case 2:

                    if (!canPan)
                        return;

                    float s = Mathf.Clamp(dolly, 0.1f, 10.0f) * panSensibility / contentRect.height;
                    pivot += preview.camera.transform.up * delta.y * s -
                        preview.camera.transform.right * delta.x * s;
                    break;

                default:
                    break;
            }
        }

        private void PlaceCamera()
        {
            float h = Mathf.Cos(pitch);
            Vector3 orbit = new Vector3(Mathf.Sin(yaw) * h, Mathf.Sin(pitch), Mathf.Cos(yaw) * h);
            preview.camera.transform.position = pivot + orbit * dolly;
            preview.camera.transform.LookAt(pivot);
        }


        private struct Drag
        {
            public bool pressed;
            public int pointerId;
            public Vector2 startPosition;
            public Vector2 currentPosition;
        }
    }
}
