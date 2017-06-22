using System;
using System.Collections.Generic;
using UI.Diagnostics;
using UnityEngine;

namespace UI
{
    public sealed class UseGesterRecognizerAttribute : Attribute
    {

    }

    public sealed class UISceneNameAttribute : Attribute
    {
        public readonly String Name;

        public UISceneNameAttribute(string name)
        {
            this.Name = name;
        }
    }

    public abstract class UISceneController : UIController
    {
        private DiagnosticAttribute[] _diagnostics;
        private bool _containsDiagnostics;
        private MGInput _input;

        public MGInput SceneInput
        {
            get { return _input; }
        }

        public UISceneController()
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            List<DiagnosticAttribute> diagnostics = new List<DiagnosticAttribute>();

            foreach (Attribute attribute in this.GetType().GetCustomAttributes(true))
            {
                if (attribute is DiagnosticAttribute)
                {
                    diagnostics.Add(attribute as DiagnosticAttribute);
                }
            }

            _diagnostics = diagnostics.ToArray();
            _containsDiagnostics = _diagnostics.Length > 0;
            _input = this.gameObject.AddComponent<MGInput>();
        }

        internal override void OnAfterStart()
        {
            if (_containsDiagnostics)
            {
                for (int i = 0; i < _diagnostics.Length; i++)
                {
                    _diagnostics[i].OnStart();
                }
            }

            base.OnAfterStart();
        }

        protected virtual void OnGUI()
        {
            if (_containsDiagnostics)
            {
                float heightPerCounter = 20f;
                float startY = Screen.height/2 - (_diagnostics.Length*heightPerCounter)/2;
                float x = Screen.width - 20f;

                for (int i = 0; i < _diagnostics.Length; i++)
                {
                    DiagnosticAttribute counter = _diagnostics[i];

                    counter.Style.alignment = TextAnchor.MiddleRight;

                    string text = counter.ToString();
                    float counterWidth = counter.Style.fixedWidth*text.Length;

                    GUI.Label(new Rect(x - counterWidth, startY, counterWidth, heightPerCounter), text, counter.Style);

                    startY += heightPerCounter;
                }
            }
        }

        protected override sealed void Update()
        {
            base.Update();

            if (_containsDiagnostics)
            {
                for (int i = 0; i < _diagnostics.Length; i++)
                {
                    _diagnostics[i].OnUpdate();
                }
            }
        }
    }
}