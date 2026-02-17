using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Phases;
using Version1.Utilities;

namespace Tests.Utilities
{
    /// <summary>
    /// Helper utilities for Unity test setup and reflection-based testing.
    /// Provides common methods for creating UI components and accessing private fields.
    /// </summary>
    public static class TestHelpers
    {
        #region UI Component Creation

        /// <summary>
        /// Creates a TextMeshPro InputField for testing
        /// </summary>
        /// <param name="name">Name for the GameObject</param>
        /// <returns>Configured TMP_InputField ready for testing</returns>
        public static TMP_InputField CreateInputField(string name)
        {
            var gameObject = new GameObject(name);
            var inputField = gameObject.AddComponent<TMP_InputField>();

            // Create the text component that TMP_InputField requires
            var textArea = new GameObject("TextArea");
            textArea.transform.SetParent(gameObject.transform);
            var textComponent = textArea.AddComponent<TextMeshProUGUI>();
            inputField.textComponent = textComponent;

            return inputField;
        }

        /// <summary>
        /// Creates a TextMeshPro Dropdown for testing
        /// </summary>
        /// <param name="name">Name for the GameObject</param>
        /// <returns>Configured TMP_Dropdown with default options (Male, Female, Other)</returns>
        public static TMP_Dropdown CreateDropdown(string name)
        {
            var gameObject = new GameObject(name);
            var dropdown = gameObject.AddComponent<TMP_Dropdown>();

            // Add common gender options
            dropdown.options.Add(new TMP_Dropdown.OptionData("Male"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Female"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Other"));

            return dropdown;
        }

        /// <summary>
        /// Creates a Button for testing
        /// </summary>
        /// <param name="name">Name for the GameObject</param>
        /// <returns>Configured Button ready for testing</returns>
        public static Button CreateButton(string name)
        {
            var gameObject = new GameObject(name);
            return gameObject.AddComponent<Button>();
        }

        /// <summary>
        /// Creates a simple GameObject (useful for error indicators)
        /// </summary>
        /// <param name="name">Name for the GameObject</param>
        /// <param name="activeByDefault">Whether the GameObject should start active</param>
        /// <returns>Created GameObject</returns>
        public static GameObject CreateGameObject(string name, bool activeByDefault = true)
        {
            var gameObject = new GameObject(name);
            gameObject.SetActive(activeByDefault);
            return gameObject;
        }

        #endregion

        #region Reflection Helpers

        /// <summary>
        /// Sets a private instance field value on an object using reflection
        /// </summary>
        /// <param name="target">The object containing the field</param>
        /// <param name="fieldName">Name of the private field</param>
        /// <param name="value">Value to set</param>
        public static void SetPrivateField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError($"Field '{fieldName}' not found on type '{type.Name}'");
                return;
            }

            field.SetValue(target, value);
        }

        /// <summary>
        /// Sets a private static field value on a type using reflection.
        /// Useful for injecting singleton instances in tests.
        /// </summary>
        /// <param name="type">The type containing the static field</param>
        /// <param name="fieldName">Name of the private static field</param>
        /// <param name="value">Value to set</param>
        public static void SetPrivateStaticField(Type type, string fieldName, object value)
        {
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Static);

            if (field == null)
            {
                Debug.LogError($"Static field '{fieldName}' not found on type '{type.Name}'");
                return;
            }

            field.SetValue(null, value);
        }

        /// <summary>
        /// Gets a private field value from an object using reflection
        /// </summary>
        /// <typeparam name="T">Expected type of the field</typeparam>
        /// <param name="target">The object containing the field</param>
        /// <param name="fieldName">Name of the private field</param>
        /// <returns>The field value cast to type T</returns>
        public static T GetPrivateField<T>(object target, string fieldName)
        {
            var type = target.GetType();
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError($"Field '{fieldName}' not found on type '{type.Name}'");
                return default;
            }

            var value = field.GetValue(target);
            return value is T typedValue ? typedValue : default;
        }

        /// <summary>
        /// Invokes a private method on an object using reflection
        /// </summary>
        /// <param name="target">The object containing the method</param>
        /// <param name="methodName">Name of the private method</param>
        /// <param name="parameters">Parameters to pass to the method</param>
        /// <returns>The return value of the method</returns>
        public static object InvokePrivateMethod(object target, string methodName, params object[] parameters)
        {
            var type = target.GetType();
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                Debug.LogError($"Method '{methodName}' not found on type '{type.Name}'");
                return null;
            }

            return method.Invoke(target, parameters);
        }

        /// <summary>
        /// Gets a private property value from an object using reflection
        /// </summary>
        /// <typeparam name="T">Expected type of the property</typeparam>
        /// <param name="target">The object containing the property</param>
        /// <param name="propertyName">Name of the private property</param>
        /// <returns>The property value cast to type T</returns>
        public static T GetPrivateProperty<T>(object target, string propertyName)
        {
            var type = target.GetType();
            var property = type.GetProperty(propertyName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (property == null)
            {
                Debug.LogError($"Property '{propertyName}' not found on type '{type.Name}'");
                return default;
            }

            var value = property.GetValue(target);
            return value is T typedValue ? typedValue : default;
        }

        #endregion

        #region PhaseManager / GameManager Helpers

        private static MockPhaseManager _mockPhaseManager;

        /// <summary>
        /// Sets the current round number on the MockPhaseManager,
        /// injecting it into GameManager if not already done.
        /// </summary>
        public static void SetRoundNumber(int number)
        {
            if (_mockPhaseManager == null)
            {
                _mockPhaseManager = new MockPhaseManager(number);
                InjectPhaseManager(_mockPhaseManager);
            }
            else
            {
                _mockPhaseManager.SetRoundNumber(number);
            }
        }

        /// <summary>
        /// Resets the MockPhaseManager and the GameManager singleton.
        /// Call this in TearDown to ensure a clean state between tests.
        /// </summary>
        public static void ClearMockPhaseManager()
        {
            _mockPhaseManager = null;
            SetPrivateStaticField(typeof(GameManager), "instance", null);
        }

        /// <summary>
        /// Uses reflection to inject an IPhaseManager into the GameManager singleton,
        /// bypassing the private setter on PhaseManager.
        /// </summary>
        private static void InjectPhaseManager(IPhaseManager phaseManager)
        {
            var gameManager = GameManager.Instance;
            var property = typeof(GameManager).GetProperty(
                "PhaseManager",
                BindingFlags.Public | BindingFlags.Instance
            );

            if (property == null)
            {
                Debug.LogError("Property 'PhaseManager' not found on GameManager.");
                return;
            }

            property.SetValue(gameManager, phaseManager);
        }

        #endregion

        #region NetworkManager Helpers

        private static MockNetworkManager _mockNetworkManager;

        /// <summary>
        /// Injects a MockNetworkManager into NetworkManager.Instance via the backing field.
        /// Requires NetworkManager.Instance to be typed as INetworkManager.
        /// Call this in SetUp for any test that triggers a Publish or Subscribe call.
        /// </summary>
        public static MockNetworkManager InjectMockNetworkManager()
        {
            if (_mockNetworkManager == null)
                _mockNetworkManager = new MockNetworkManager();
            else
                _mockNetworkManager.Reset();

            // Target the compiler-generated backing field for the auto-property.
            // The backing field name is "<Instance>k__BackingField" for auto-properties.
            var backingField = typeof(NetworkManager).GetField(
                "<Instance>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            if (backingField == null)
            {
                Debug.LogError("Backing field for 'Instance' not found on NetworkManager. " +
                               "Ensure NetworkManager.Instance is typed as INetworkManager.");
                return null;
            }

            backingField.SetValue(null, _mockNetworkManager);
            return _mockNetworkManager;
        }

        /// <summary>
        /// Clears the MockNetworkManager and resets NetworkManager.Instance to null.
        /// Call this in TearDown.
        /// </summary>
        public static void ClearMockNetworkManager()
        {
            _mockNetworkManager = null;

            var backingField = typeof(NetworkManager).GetField(
                "<Instance>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            backingField?.SetValue(null, null);
        }

        #endregion

        #region Form Validation Helpers

        /// <summary>
        /// Simulates filling out the entire login form with valid data
        /// </summary>
        public static void FillValidForm(
            TMP_InputField playerNameInput,
            TMP_InputField ageInput,
            TMP_Dropdown genderDropdown,
            TMP_InputField gameCodeInput,
            string playerName = "TestPlayer",
            string age = "25",
            int gender = 1,
            string gameCode = "12345")
        {
            playerNameInput.text = playerName;
            playerNameInput.onValueChanged.Invoke(playerName);

            ageInput.text = age;
            ageInput.onValueChanged.Invoke(age);

            genderDropdown.value = gender;
            genderDropdown.onValueChanged.Invoke(gender);

            gameCodeInput.text = gameCode;
            gameCodeInput.onValueChanged.Invoke(gameCode);
        }

        /// <summary>
        /// Clears all form fields
        /// </summary>
        public static void ClearForm(
            TMP_InputField playerNameInput,
            TMP_InputField ageInput,
            TMP_Dropdown genderDropdown,
            TMP_InputField gameCodeInput)
        {
            playerNameInput.text = "";
            playerNameInput.onValueChanged.Invoke("");

            ageInput.text = "";
            ageInput.onValueChanged.Invoke("");

            genderDropdown.value = 0;
            genderDropdown.onValueChanged.Invoke(0);

            gameCodeInput.text = "";
            gameCodeInput.onValueChanged.Invoke("");
        }

        #endregion
    }
}
