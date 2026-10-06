using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class ShaderProperties : VisualElement
    {
        private ShaderElement ShaderElement
        {
            get => shaderElement;
            set
            {
                if (value == shaderElement)
                    return;

                if (shaderElement != null)
                    shaderElement.beforeRendering -= BeforeRendering;
                
                shaderElement = value;
                if (value != null)
                    value.beforeRendering += BeforeRendering;

                BindProperties();
            }
        }
        private ShaderElement shaderElement;

        private ShaderElement TwinShaderElement
        {
            get => twinShaderElement;
            set
            {
                if (value == twinShaderElement)
                    return;

                if (twinShaderElement != null)
                    twinShaderElement.beforeRendering -= BeforeRendering;

                twinShaderElement = value;
                if (value != null)
                    value.beforeRendering += BeforeRendering;
            }
        }
        private ShaderElement twinShaderElement;

        public ShaderProperties()
        {
            this.AddToClassList("homework-shader-properties");
            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        private HashSet<Action<Material>> values;

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            if (ShaderElement == null)
                EditorApplication.delayCall += SearchShaderElement;
        }

        private void SearchShaderElement()
        {
            BubbleUpShearch(this);
        }

        private void BubbleUpShearch(VisualElement from)
        {
            if (from.parent == null)
                return;

            VisualElement p = from.parent;

            if (p is ShaderExercice ex)
            {
                ShaderElement = ex.shaderSolution.shaderElement;
                TwinShaderElement = ex.shaderUser.shaderElement;
                return;
            }

            for (int i = 0; i < p.childCount; i++)
            {
                if (p[i] != from)
                {
                    if (p[i] is ShaderElement element)
                    {
                        ShaderElement = element;
                        return;
                    }
                    else if (p[i] is ShaderExercice exercice)
                    {
                        ShaderElement = exercice.shaderSolution.shaderElement;
                        TwinShaderElement = exercice.shaderUser.shaderElement;
                        return;
                    }
                }
            }

            BubbleUpShearch(p);
        }

        private void BindProperties()
        {
            Clear();
            values = new HashSet<Action<Material>>();

            if (shaderElement.Shader != null)
            {
                Shader shader = shaderElement.Shader;

                for (int i = 0; i < shader.GetPropertyCount(); i++)
                {
                    ShaderPropertyType propertyType = shader.GetPropertyType(i);
                    string propertyName = shader.GetPropertyDescription(i);
                    int id = shader.GetPropertyNameId(i);
                    string[] attributes = shader.GetPropertyAttributes(i);
                    ShaderPropertyFlags flags = shader.GetPropertyFlags(i);

                    bool HasFlag(ShaderPropertyFlags flag)
                    {
                        return (flags & flag) == flag;
                    }

                    if (HasFlag(ShaderPropertyFlags.HideInInspector))
                        continue;

                    switch (propertyType)
                    {
                        case ShaderPropertyType.Color:
                            {
                                ColorField field = new ColorField(propertyName);
                                field.value = (Color)shader.GetPropertyDefaultVectorValue(i);
                                values.Add((Material m) => m.SetColor(id, field.value));
                                Add(field);

                                if (HasFlag(ShaderPropertyFlags.HDR))
                                    field.hdr = true;
                            }
                            break;

                        case ShaderPropertyType.Vector:
                            {
                                if (HasFlag(ShaderPropertyFlags.Vector2))
                                {
                                    Vector2Field field = new Vector2Field(propertyName);
                                    field.value = (Vector2)shader.GetPropertyDefaultVectorValue(i);
                                    values.Add((Material m) => m.SetVector(id, field.value));
                                    Add(field);
                                }
                                else if (HasFlag(ShaderPropertyFlags.Vector3))
                                {
                                    Vector3Field field = new Vector3Field(propertyName);
                                    field.value = (Vector3)shader.GetPropertyDefaultVectorValue(i);
                                    values.Add((Material m) => m.SetVector(id, field.value));
                                    Add(field);

                                }
                                else
                                {
                                    Vector4Field field = new Vector4Field(propertyName);
                                    field.value = shader.GetPropertyDefaultVectorValue(i);
                                    values.Add((Material m) => m.SetVector(id, field.value));
                                    Add(field);
                                }
                            }
                            break;

                        case ShaderPropertyType.Float:
                            {
                                bool isEnum = false;
                                bool isToggle = false;
                                string enumValues = "";
                                for (int j = 0; j < attributes.Length; j++)
                                {
                                    if (attributes[j].StartsWith("Enum"))
                                    {
                                        isEnum = true;
                                        enumValues = attributes[j].Remove(0, "Enum(".Length);
                                        enumValues = enumValues.Remove(enumValues.Length - 1);
                                    }
                                    else if (attributes[j] == "ToggleUI")
                                        isToggle = true;
                                }

                                if (isEnum)
                                {
                                    string[] namesAndValues = enumValues.Split(',');
                                    Dictionary<int, string> choices = new Dictionary<int, string>();
                                    for (int j = 0; j < namesAndValues.Length; j += 2)
                                        choices.Add(int.Parse(namesAndValues[j + 1]), namesAndValues[j]);

                                    DropdownField field = new DropdownField(propertyName);
                                    field.choices = choices.Values.ToList();
                                    field.value = choices[(int)shader.GetPropertyDefaultFloatValue(i)];

                                    values.Add((Material m) =>
                                    {
                                        int v = 0;
                                        foreach (var kvp in choices)
                                        {
                                            if (kvp.Value == field.value)
                                            {
                                                v = kvp.Key;
                                                break;
                                            }
                                        }
                                        m.SetFloat(id, v);
                                    });
                                    Add(field);
                                }
                                else if (isToggle)
                                {
                                    Toggle field = new Toggle(propertyName);
                                    field.value = shader.GetPropertyDefaultFloatValue(i) > 0.5f;
                                    values.Add((Material m) => m.SetFloat(id, field.value ? 1.0f : 0.0f));
                                    Add(field);
                                }
                                else
                                {
                                    FloatField field = new FloatField(propertyName);
                                    field.value = shader.GetPropertyDefaultFloatValue(i);
                                    values.Add((Material m) => m.SetFloat(id, field.value));
                                    Add(field);
                                }
                            }
                            break;

                        case ShaderPropertyType.Range:
                            {
                                Slider field = new Slider(propertyName);
                                field.showInputField = true;
                                Vector2 limits = shader.GetPropertyRangeLimits(i);
                                field.lowValue = limits.x;
                                field.highValue = limits.y;
                                field.value = shader.GetPropertyDefaultFloatValue(i);
                                values.Add((Material m) => m.SetFloat(id, field.value));
                                Add(field);
                            }
                            break;

                        case ShaderPropertyType.Texture:
                            {
                                //ObjectField field = new ObjectField(propertyName);
                                //field.dataSourceType = typeof(Texture);
                                //field.value = shader.Get(i);
                                //values.Add((Material m) => m.SetFloat(id, field.value));
                                //Add(field);
                            }
                            break;

                        case ShaderPropertyType.Int:
                            {
                                IntegerField field = new IntegerField(propertyName);
                                field.value = shader.GetPropertyDefaultIntValue(i);
                                values.Add((Material m) => m.SetInt(id, field.value));
                                Add(field);
                            }
                            break;
                    }

                }
            }
        }

        private void BeforeRendering(Material material)
        {
            if (material == null)
                return;

            foreach (var item in values)
            {
                item.Invoke(material);
            }
        }
    }
}

