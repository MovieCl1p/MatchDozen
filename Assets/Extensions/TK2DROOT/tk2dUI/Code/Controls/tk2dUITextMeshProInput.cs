#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
    #define TOUCH_SCREEN_KEYBOARD
#endif

using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// TextInput control
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/tk2dUITextMeshProInput")]
public class tk2dUITextMeshProInput : MonoBehaviour
{
    /// <summary>
    /// UItem that will make cause TextInput to become selected on click
    /// </summary>
    public tk2dUIItem selectionBtn;

    /// <summary>
    /// TextMesh while text input will be displayed
    /// </summary>
    public TextMeshPro inputLabel;

    /// <summary>
    /// TextMesh that will be displayed if nothing in inputLabel and is not selected
    /// </summary>
    public TextMeshPro emptyDisplayLabel;

    /// <summary>
    /// State to be active if text input is not selected
    /// </summary>
    public GameObject unSelectedStateGO;

    /// <summary>
    /// Stated to be active if text input is selected
    /// </summary>
    public GameObject selectedStateGO;

    /// <summary>
    /// Text cursor to be displayed at next of text input on selection
    /// </summary>
    public GameObject cursor;

    /// <summary>
    /// How long the field is (visible)
    /// </summary>
    public float fieldLength = 1;

    /// <summary>
    /// Maximum number of characters allowed for input
    /// </summary>
    public int maxCharacterLength = 30;

    /// <summary>
    /// Text to be displayed when no text is entered and text input is not selected
    /// </summary>
    public string emptyDisplayText;

    /// <summary>
    /// If set to true (is a password field), then all characters will be replaced with password char
    /// </summary>
    public bool isPasswordField = false;

    /// <summary>
    /// Each character in the password field is replaced with the first character of this string
    /// Default: * if string is empty.
    /// </summary>
    public string passwordChar = "*";

    public System.Action<tk2dUITextMeshProInput> OnTextChange;

    public System.Action<tk2dUITextMeshProInput> OnTextFinishedChange;

    public event System.Action<tk2dUITextMeshProInput> LostFocusEvent;

    public string SendMessageOnTextChangeMethodName = "";

    [SerializeField]
    [HideInInspector]
    private tk2dUILayout layoutItem = null;

    public tk2dUILayout LayoutItem
    {
        get
        {
            return layoutItem;
        }

        set
        {
            if (layoutItem != value)
            {
                if (layoutItem != null)
                {
                    layoutItem.OnReshape -= LayoutReshaped;
                }
                layoutItem = value;
                if (layoutItem != null)
                {
                    layoutItem.OnReshape += LayoutReshaped;
                }
            }
        }
    }

    private bool isSelected = false;

    private bool wasStartedCalled = false;

    private bool wasOnAnyPressEventAttached = false;

#if TOUCH_SCREEN_KEYBOARD
    private TouchScreenKeyboard keyboard = null;
#endif

    private Renderer _inputRenderer;
    private float _nextUpdate;
    private int _cursorPositionIndex;
    private string _text = string.Empty;

    private bool listenForKeyboardText = false;

    private bool isDisplayTextShown = false;

    public GameObject SendMessageTarget
    {
        get
        {
            if (selectionBtn != null)
            {
                return selectionBtn.sendMessageTarget;
            }
            
            return null;
        }

        set
        {
            if (selectionBtn != null && selectionBtn.sendMessageTarget != value)
            {
                selectionBtn.sendMessageTarget = value;
            
                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(selectionBtn);
                #endif
            }
        }
    }

    public bool IsFocus
    {
        get
        {
            return isSelected;
        }
    }

    /// <summary>
    /// Update the input text
    /// </summary>
    public string Text
    {
        get
        {
            return _text;
        }

        set
        {
            if (_text != value)
            {
                _text = value;
                if (_text.Length > maxCharacterLength)
                {
                    _text = _text.Substring(0, maxCharacterLength);
                }

                FormatTextForDisplay(_text);
                if (isSelected)
                {
                    StartCoroutine(SetCursorPosition());
                }
            }
        }
    }

    public void SetFocus()
    {
        SetFocus(true);
    }

    /// <summary>
    /// Sets or removes focus from the text input
    /// Currently you will need to manually need to remove focus and set focus on the new
    /// textinput if you wish to do this from a textInput callback, eg. auto advance when
    /// enter is pressed.
    /// </summary>
    public void SetFocus(bool focus)
    {
        if (!IsFocus && focus) 
        {
            InputSelected();
        }
        else if (IsFocus && !focus) 
        {
            InputDeselected();
        }
    }

    public void InputSelected()
    {
        _cursorPositionIndex = GetCursorIndex(selectionBtn.Touch);
        StartCoroutine(SetCursorPosition());

        if (isSelected)
        {
            return;
        }

        if (_text.Length == 0)
        {
            HideDisplayText();
        }

        isSelected = true;

        if (isPasswordField)
        {
            Text = string.Empty;
        }

        if (!listenForKeyboardText)
        {
            tk2dUIManager.Instance.OnInputUpdate += ListenForKeyboardTextUpdate;
        }

        listenForKeyboardText = true;
        SetState();

#if TOUCH_SCREEN_KEYBOARD
        if (Application.platform != RuntimePlatform.WindowsEditor 
            && Application.platform != RuntimePlatform.OSXEditor)
        {
#if UNITY_ANDROID //due to a delete key bug in Unity Android
            TouchScreenKeyboard.hideInput = false;
#else
            TouchScreenKeyboard.hideInput = true;
#endif
            keyboard = TouchScreenKeyboard.Open(_text, TouchScreenKeyboardType.Default, false, false, false, false);
            StartCoroutine(TouchScreenKeyboardLoop());
        }
#endif  
    }

    public void InputDeselected()
    {
        if (_text.Length == 0)
        {
            ShowDisplayText();
        }

        isSelected = false;

        if (OnTextFinishedChange != null)
        {
            OnTextFinishedChange(this);
        }

        if (listenForKeyboardText)
        {
            tk2dUIManager.Instance.OnInputUpdate -= ListenForKeyboardTextUpdate;
        }

        listenForKeyboardText = false;
        SetState();
#if TOUCH_SCREEN_KEYBOARD
        if (keyboard != null && !keyboard.done)
        {
            keyboard.active = false;
        }

        keyboard = null;
#endif
    }

    private void OnMove()
    {
        _cursorPositionIndex = GetCursorIndex(selectionBtn.Touch);
        StartCoroutine(SetCursorPosition());
    }

    private void Awake()
    {
        _inputRenderer = inputLabel.GetComponent<Renderer>();
        SetState();
        ShowDisplayText();
    }

    private void Start()
    {
        wasStartedCalled = true;
        if (tk2dUIManager.Instance__NoCreate != null)
        {
            tk2dUIManager.Instance.OnAnyPress += AnyPress;
        }
        wasOnAnyPressEventAttached = true;
    }

    private void Update()
    {
        if (isSelected)
        {
            _nextUpdate += Time.deltaTime;
            if (_nextUpdate >= 0.35f)
            {
                cursor.SetActive(!cursor.activeSelf);
                _nextUpdate = 0;
            }
        }
    }

    private void OnEnable()
    {
        if (wasStartedCalled && !wasOnAnyPressEventAttached)
        {
            if (tk2dUIManager.Instance__NoCreate != null)
            {
                tk2dUIManager.Instance.OnAnyPress += AnyPress;
            }
        }

        if (layoutItem != null)
        {
            layoutItem.OnReshape += LayoutReshaped;
        }

        selectionBtn.OnDown += InputSelected;
        selectionBtn.OnMove += OnMove;
    }

    private void OnDisable()
    {
        if (tk2dUIManager.Instance__NoCreate != null)
        {
            tk2dUIManager.Instance.OnAnyPress -= AnyPress;
            if (listenForKeyboardText)
            {
                tk2dUIManager.Instance.OnInputUpdate -= ListenForKeyboardTextUpdate;
            }
        }
        wasOnAnyPressEventAttached = false;

        selectionBtn.OnDown -= InputSelected;
        selectionBtn.OnMove -= OnMove;

        listenForKeyboardText = false;

        if (layoutItem != null)
        {
            layoutItem.OnReshape -= LayoutReshaped;
        }
    }

    private void FormatTextForDisplay(string modifiedText)
    {
        if (isPasswordField)
        {
            int charLength = modifiedText.Length;
            char passwordReplaceChar = (passwordChar.Length > 0) ? passwordChar[0] : '*';
            modifiedText = string.Empty;
            modifiedText = modifiedText.PadRight(charLength, passwordReplaceChar);
        }

        inputLabel.text = modifiedText;

        while (_inputRenderer.bounds.size.x > fieldLength)
        {
            modifiedText = modifiedText.Substring(1, modifiedText.Length - 1);
            inputLabel.text = modifiedText;
        }

        if (modifiedText.Length == 0 && !listenForKeyboardText)
        {
            ShowDisplayText();
        }
        else
        {
            HideDisplayText();
        }
    }

    private void ListenForKeyboardTextUpdate()
    {
        bool change = false;
        string newText = _text;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (LostFocusEvent != null)
            {
                LostFocusEvent(this);
                return;
            }
        }

        string inputStr = Input.inputString;
        for (int i = 0; i < inputStr.Length; i++)
        {
            char c = inputStr[i];
            if (c == "\b"[0])
            {   
                newText = RemoveCharacter(newText);
                change = true;
            }
            else if (c == "\n"[0] || c == "\r"[0])
            {
                
            }
            else if (c != 9 && c != 27)
            {
                newText = AddCharacter(newText, c);
                change = true;
            }
            else if (c == 9)
            {
                if (LostFocusEvent != null)
                {
                    LostFocusEvent(this);
                }
            }
        }

        if (change)
        {
            Text = newText;
            if (OnTextChange != null)
            {
                OnTextChange(this);
            }

            if (SendMessageTarget != null && SendMessageOnTextChangeMethodName.Length > 0)
            {
                SendMessageTarget.SendMessage(SendMessageOnTextChangeMethodName, this, SendMessageOptions.RequireReceiver);
            }
        }
    }

    private string AddCharacter(string text, char ch)
    {
        if (text.Length < maxCharacterLength)
        {
            text = text.Insert(_cursorPositionIndex, ch.ToString());
            _cursorPositionIndex++;
        }

        return text;
    }

    private string RemoveCharacter(string text)
    {
        if (text.Length != 0 && _cursorPositionIndex > 0)
        {
            text = text.Remove(_cursorPositionIndex - 1, 1);
            _cursorPositionIndex--;
        }

        return text;
    }

#if TOUCH_SCREEN_KEYBOARD
    private IEnumerator TouchScreenKeyboardLoop()
    {
        while (keyboard != null && !keyboard.done)
        {
            if (!keyboard.text.Equals(Text))
            {
#if UNITY_ANDROID
                Text = keyboard.text;
#else
                Text = ValidateKeyboardText(keyboard.text);
                keyboard.text = Text;
#endif
            }

            yield return null;
        }

        if (keyboard != null)
        {
#if UNITY_ANDROID
            Text = keyboard.text;
#else
            Text = ValidateKeyboardText(keyboard.text);
#endif
        }

        if (isSelected)
        {
            InputDeselected();
        }
    }
#endif

    private string ValidateKeyboardText(string text)
    {
        var newText = Text;

        if (text.Length < Text.Length)
        {
            var offset = Text.Length - text.Length;
            for (int index = 0; index < offset; index++)
            {
                newText = RemoveCharacter(newText);
            }
        }
        else if (text.Length > Text.Length)
        {
            var offset = text.Length - Text.Length;
            for (int index = 0; index < offset; index++)
            {
                newText = AddCharacter(newText, text[Text.Length + index]);
            }
        }

        return newText;
    }

    private void AnyPress()
    {
        if (isSelected && tk2dUIManager.Instance.PressedUIItem != selectionBtn)
        {
            InputDeselected();
        }
    }

    private void SetState()
    {
        tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(unSelectedStateGO, !isSelected);
        tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(selectedStateGO, isSelected);
        tk2dUIBaseItemControl.ChangeGameObjectActiveState(cursor, isSelected);
    }

    private IEnumerator SetCursorPosition()
    {
        yield return new WaitForEndOfFrame();

        float textSize = 0;
        var padding = ShaderUtilities.GetPadding(_inputRenderer.sharedMaterial, false, false);
        float offset = 0;
        for (int index = 0; index < inputLabel.text.Length && index < _cursorPositionIndex; index++)
        {
            var ch = inputLabel.text[index];
            var glyphInfo = inputLabel.font.characterDictionary[ch];
            textSize = offset + ((glyphInfo.xOffset + glyphInfo.width + padding) * inputLabel.fontScale);
            offset += glyphInfo.xAdvance * inputLabel.fontScale;
        }

        cursor.transform.localPosition = new Vector3(inputLabel.transform.localPosition.x + textSize,
            cursor.transform.localPosition.y,
            cursor.transform.localPosition.z);
    }

    private void ShowDisplayText()
    {
        if (!isDisplayTextShown)
        {
            isDisplayTextShown = true;
            if (emptyDisplayLabel != null)
            {
                emptyDisplayLabel.text = emptyDisplayText;
                tk2dUIBaseItemControl.ChangeGameObjectActiveState(emptyDisplayLabel.gameObject, true);
            }
            tk2dUIBaseItemControl.ChangeGameObjectActiveState(inputLabel.gameObject, false);
        }
    }

    private void HideDisplayText()
    {
        if (isDisplayTextShown)
        {
            isDisplayTextShown = false;
            tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(emptyDisplayLabel.gameObject, false);
            tk2dUIBaseItemControl.ChangeGameObjectActiveState(inputLabel.gameObject, true);
        }
    }

    private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
    {
        fieldLength += dMax.x - dMin.x;
        string tmpText = _text;
        _text = string.Empty;
        Text = tmpText;
    }

    private int GetCursorIndex(tk2dUITouch touch)
    {
        var cursorIndex = 0;
        if (inputLabel.text.Length == 0)
        {
            return cursorIndex;
        }

        var uiCamera = FindObjectOfType<tk2dCamera>();

        float pressOffset = uiCamera.ScreenCamera.ScreenToWorldPoint(touch.position).x - inputLabel.transform.position.x;
        if (pressOffset <= 0)
        {
            return cursorIndex;
        }

        float textSize = 0;
        var padding = ShaderUtilities.GetPadding(_inputRenderer.sharedMaterial, false, false);
        float offset = 0;
        for (int index = 0; index < inputLabel.text.Length; index++)
        {
            var ch = inputLabel.text[index];
            var glyphInfo = inputLabel.font.characterDictionary[ch];
            textSize = offset + ((glyphInfo.xOffset + glyphInfo.width + padding) * inputLabel.fontScale);
            offset += glyphInfo.xAdvance * inputLabel.fontScale;

            if (textSize >= pressOffset)
            {
                break;
            }

            cursorIndex++;
        }

        return cursorIndex;
    }
}
