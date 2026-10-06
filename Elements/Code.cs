using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

//using Unity.Mathematics;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class Code : VisualElement, ICommentable
    {
        private const string keywordColor = "#459cd6";
        private const string commentColor = "#9dfaa1";
        private const string structColor = "#86c691";
        private const string classColor = "#48c9b0";
        private const string functionColor = "#459cd6";

        [Multiline, UxmlAttribute]
        public string Text
        {
            get => text;
            set
            {
                text = value;
                textElement.text = Highlight(value);
            }
        }

        [Multiline, SerializeField]
        private string text;

        private TextElement textElement;

        public Code()
        {
            textElement = this.Add<TextElement>("code", "homework-code");
        }

        private static readonly HashSet<string> keywords = new HashSet<string>
        {
        "if", "else", "for", "while", "do", "switch", "case", "default", "break", "continue", "return", "discard",
        "struct", "cbuffer", "tbuffer", "typedef", "static", "const", "uniform", "in", "out", "inout", "void",
        "true", "false", "register", "groupshared", "shared", "volatile", "precise", "nointerpolation",
        "linear", "centroid", "row_major", "column_major", "unroll", "loop", "branch", "flatten"
        };

        private static readonly HashSet<string> objectTypes = new HashSet<string>
        {
        "Texture2D", "Texture3D", "TextureCube", "Texture2DArray", "SamplerState", "SamplerComparisonState",
        "StructuredBuffer", "RWStructuredBuffer", "ByteAddressBuffer", "RWByteAddressBuffer", "RWTexture2D",
        "sampler", "sampler2D", "sampler3D", "samplerCUBE"
        };

        private static readonly HashSet<string> functions = new HashSet<string>
        {
        "abs", "acos", "all", "any", "asin", "atan", "atan2", "ceil", "clamp", "clip", "cos", "cosh", "cross",
        "ddx", "ddy", "determinant", "distance", "dot", "exp", "exp2", "floor", "fmod", "frac", "fwidth",
        "length", "lerp", "log", "log2", "log10", "mad", "max", "min", "mul", "normalize", "pow", "rcp",
        "reflect", "refract", "round", "rsqrt", "saturate", "sign", "sin", "sincos", "sinh", "smoothstep",
        "sqrt", "step", "tan", "tanh", "transpose", "trunc", "tex2D", "tex2Dlod", "Sample", "SampleLevel", "Load"
        };

        private static readonly Regex scalarTypeRegex = new Regex(
            @"^(bool|int|uint|dword|half|float|double)([1-4](x[1-4])?)?$", RegexOptions.Compiled);

        private static readonly Regex structDeclarationRegex = new Regex(
            @"\bstruct\s+([A-Za-z_]\w*)", RegexOptions.Compiled);

        private static readonly Regex tokenRegex = new Regex(
            @"(?<comment>//[^\r\n]*|/\*[\s\S]*?\*/)" +
            @"|^(?<indent>[ \t]*)(?<directive>#[ \t]*\w+)" +
            @"|(?<word>\b[A-Za-z_]\w*\b)",
            RegexOptions.Compiled | RegexOptions.Multiline);

        public static string Highlight(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            HashSet<string> userStructs = new HashSet<string>();
            foreach (Match structMatch in structDeclarationRegex.Matches(source))
            {
                userStructs.Add(structMatch.Groups[1].Value);
            }

            return tokenRegex.Replace(source, match =>
            {
                if (match.Groups["comment"].Success)
                {
                    return Colorize(match.Value, commentColor);
                }

                if (match.Groups["directive"].Success)
                {
                    return match.Groups["indent"].Value + Colorize(match.Groups["directive"].Value, keywordColor);
                }

                string word = match.Groups["word"].Value;

                if (keywords.Contains(word) || scalarTypeRegex.IsMatch(word))
                {
                    return Colorize(word, keywordColor);
                }

                if (userStructs.Contains(word))
                {
                    return Colorize(word, structColor);
                }

                if (objectTypes.Contains(word))
                {
                    return Colorize(word, classColor);
                }

                if (functions.Contains(word))
                {
                    return Colorize(word, functionColor);
                }

                return word;
            });
        }

        private static string Colorize(string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }
    }
}
