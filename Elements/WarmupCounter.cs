using System;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class WarmupCounter : VisualElement
    {
        public static WarmupCounter currentCounter;
        public Shader currentUser;
        public Shader currentSolution;

        private TextElement counter;
        private VisualElement shaderArea;
        private Button startButton;
        private bool counting;
        private double starTime;

        private VisualElement solutionElement;
        private VisualElement userElement;

        private Warmup warmup;
        private int index = -1;
        private bool initialized;

        private RenderTexture rendTexA;
        private RenderTexture rendTexB;
        private ComputeBuffer error;

        private Material materialA;
        private Material materialB;

        public WarmupCounter()
        {
            this.AddToClassList("homework-warmup-counter");

            //Counter
            counter = this.Add<TextElement>("counter", "homework-warmup-counter-label");
            counter.text = "00:00:000";
            counter.SetCheckedPseudoState(false);

            //Shader area
            shaderArea = this.Add<VisualElement>("shader-area", "homework-warmup-shader-area");
            shaderArea.SetCheckedPseudoState(false);

            //Solution element
            solutionElement = shaderArea.Add<VisualElement>();
            solutionElement.style.width = solutionElement.style.height = 256;

            userElement = shaderArea.Add<VisualElement>();
            userElement.style.width = userElement.style.height = 256;

            //Button
            startButton = this.Add<Button>("start-btn", "homework-warmup-counter-btn");
            startButton.clicked += ButtonClicked;
            startButton.text = "Ready ?";
            startButton.SetCursor(MouseCursor.Link);

            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            rendTexA?.Release();
            rendTexB?.Release();
            error?.Release();
        }

        public void Bind(Warmup warmup)
        {
            this.warmup = warmup;
        }

        private void ButtonClicked()
        {
            if (!initialized)
                Initialize();
            else if (!counting)
                StartCounter();
            else
                NextShader();
        }

        private async void Initialize()
        {
            startButton.SetEnabled(false);
            AssetDatabase.OpenAsset(warmup.placeHolder);

            rendTexA = new RenderTexture(256, 256, 0);
            rendTexA.Create();
            rendTexB = new RenderTexture(256, 256, 0);
            rendTexB.Create();
            error = new ComputeBuffer(1, sizeof(float));

            await Task.Delay(1000);

            startButton.SetEnabled(true);
            startButton.text = "Start";
            initialized = true;
        }

        public void NextShader()
        {
            index++;
            int shaderCount = warmup.shaders.Count;

            if (index >= shaderCount)
            {
                StopCounter();
            }
            else
            {
                //Solution
                if (materialA != null)
                    UnityEngine.Object.DestroyImmediate(materialA);

                currentSolution = warmup.shaders[index].solution;
                materialA = new Material(currentSolution);

                Graphics.Blit(Texture2D.blackTexture, rendTexA, materialA);
                solutionElement.style.backgroundImage = Background.FromRenderTexture(rendTexA);


                //User shader
                if (materialB != null)
                    UnityEngine.Object.DestroyImmediate(materialB);

                currentUser = warmup.shaders[index].userShader;
                materialB = new Material(currentUser);

                Graphics.Blit(Texture2D.blackTexture, rendTexB, materialB);
                userElement.style.backgroundImage = Background.FromRenderTexture(rendTexB);


                //Open shader
                AssetDatabase.OpenAsset(warmup.shaders[index].userShader);

                if (index == shaderCount - 1)
                    startButton.text = "Stop";
            }
        }

        private void StartCounter()
        {
            currentCounter = this;
            index = -1;
            NextShader();
            counting = true;
            counter.SetCheckedPseudoState(true);
            shaderArea.SetCheckedPseudoState(true);
            startButton.text = "Next";
            starTime = EditorApplication.timeSinceStartup;
            UpdateTime();
        }

        private void StopCounter()
        {
            currentCounter = null;
            counting = false;
            counter.SetCheckedPseudoState(false);
            shaderArea.SetCheckedPseudoState(false);
            startButton.text = "Start";
            SetTime(true);
        }

        private async void UpdateTime()
        {
            if (!counting)
                return;

            SetTime(false);
            await Task.Delay(5);
            UpdateTime();
        }

        private void SetTime(bool withMilis)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            double time = (currentTime - starTime);
            DateTime t = default;
            t = t.AddMilliseconds(time * 1000);
            if (withMilis)
                counter.text = $"{t.Minute.ToString("00")}:{t.Second.ToString("00")}:{t.Millisecond.ToString("000")}";
            else 
                counter.text = $"{t.Minute.ToString("00")}:{t.Second.ToString("00")}";
        }

        public uint GetErrorFactor()
        {
            Graphics.Blit(Texture2D.blackTexture, rendTexB, materialB);
            uint[] array = new uint[] { 0 };
            error.SetData(array);

            ComputeShader checker = Database.Resources.warmupChecker;
            checker.SetTexture(0, "_TexA", rendTexA);
            checker.SetTexture(0, "_TexB", rendTexB);
            checker.SetBuffer(0, "_Error", error);
            checker.Dispatch(0, rendTexA.width / 8, rendTexA.height / 8, 1);

            error.GetData(array);
            return array[0];
        }
    }
}