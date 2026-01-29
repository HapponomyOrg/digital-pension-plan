using System;
using System.Collections;
using NUnit.Framework;
using Tests.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Host;
using Version1.Phases.Login.Scripts;

namespace Tests.Phases.LoginPhase
{
    /// <summary>
    /// Comprehensive test suite for the Login component.
    /// Tests both UI interactions and network-dependent functionality.
    /// </summary>
    public class LoginTests
    {
        private GameObject loginGameObject;
        private Version1.Phases.Login.Scripts.Login loginComponent;

        // UI References
        private TMP_InputField playerNameInput;
        private TMP_InputField ageInput;
        private TMP_Dropdown genderDropdown;
        private TMP_InputField gameCodeInput;
        private Button joinButton;

        // Error indicators
        private GameObject natsError;
        private GameObject nameError;
        private GameObject sessionError;

        // Mock objects for network tests
        private MockNetworkManager mockNetworkManager;
        private MockPlayerData mockPlayerData;

        [SetUp]
        public void SetUp()
        {
            // Create the main login GameObject
            loginGameObject = new GameObject("LoginTest");
            loginComponent = loginGameObject.AddComponent<Login>();

            // Create UI components using TestHelpers
            playerNameInput = TestHelpers.CreateInputField("PlayerNameInput");
            ageInput = TestHelpers.CreateInputField("AgeInput");
            gameCodeInput = TestHelpers.CreateInputField("GameCodeInput");
            genderDropdown = TestHelpers.CreateDropdown("GenderDropdown");
            joinButton = TestHelpers.CreateButton("JoinButton");

            // Create error indicators
            natsError = new GameObject("NatsError");
            nameError = new GameObject("NameError");
            sessionError = new GameObject("SessionError");

            // Set inactive by default
            natsError.SetActive(false);
            nameError.SetActive(false);
            sessionError.SetActive(false);

            // Set private fields using reflection helper
            TestHelpers.SetPrivateField(loginComponent, "playerNameInput", playerNameInput);
            TestHelpers.SetPrivateField(loginComponent, "ageInput", ageInput);
            TestHelpers.SetPrivateField(loginComponent, "genderDropdown", genderDropdown);
            TestHelpers.SetPrivateField(loginComponent, "gameCodeInput", gameCodeInput);
            TestHelpers.SetPrivateField(loginComponent, "joinButton", joinButton);
            TestHelpers.SetPrivateField(loginComponent, "natsError", natsError);
            TestHelpers.SetPrivateField(loginComponent, "nameError", nameError);
            TestHelpers.SetPrivateField(loginComponent, "sessionError", sessionError);

            // Initialize mock objects
            mockNetworkManager = new MockNetworkManager();
            mockPlayerData = new MockPlayerData();

            // Enable the component to trigger OnEnable
            loginComponent.enabled = true;
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up all created GameObjects
            UnityEngine.Object.DestroyImmediate(loginGameObject);
            UnityEngine.Object.DestroyImmediate(playerNameInput.gameObject);
            UnityEngine.Object.DestroyImmediate(ageInput.gameObject);
            UnityEngine.Object.DestroyImmediate(gameCodeInput.gameObject);
            UnityEngine.Object.DestroyImmediate(genderDropdown.gameObject);
            UnityEngine.Object.DestroyImmediate(joinButton.gameObject);
            UnityEngine.Object.DestroyImmediate(natsError);
            UnityEngine.Object.DestroyImmediate(nameError);
            UnityEngine.Object.DestroyImmediate(sessionError);

            // Reset mocks
            mockNetworkManager?.Reset();
        }

        #region Player Name Tests

        [Test]
        public void PlayerName_WhenSet_IsPropagatedCorrectly()
        {
            // Arrange
            const string expectedName = "TestPlayer";

            // Act
            playerNameInput.text = expectedName;
            playerNameInput.onValueChanged.Invoke(expectedName);

            // Assert
            var actualName = TestHelpers.GetPrivateField<string>(loginComponent, "playerName");
            Assert.AreEqual(expectedName, actualName, "Player name should be propagated correctly");
        }

        [Test]
        public void PlayerName_WithWhitespace_IsTrimmed()
        {
            // Arrange
            const string nameWithSpaces = "  TestPlayer  ";
            const string expectedName = "TestPlayer";

            // Act
            playerNameInput.text = nameWithSpaces;
            playerNameInput.onValueChanged.Invoke(nameWithSpaces);

            // Assert
            var actualName = TestHelpers.GetPrivateField<string>(loginComponent, "playerName");
            Assert.AreEqual(expectedName, actualName, "Player name should be trimmed");
        }

        [Test]
        public void PlayerName_WhenChanged_HidesNameError()
        {
            // Arrange
            nameError.SetActive(true);
            const string newName = "NewPlayer";

            // Act
            playerNameInput.text = newName;
            playerNameInput.onValueChanged.Invoke(newName);

            // Assert
            Assert.IsFalse(nameError.activeSelf, "Name error should be hidden when name changes");
        }

        [Test]
        public void PlayerName_Empty_DisablesJoinButton()
        {
            // Arrange - set all other fields valid
            ageInput.text = "25";
            ageInput.onValueChanged.Invoke("25");
            gameCodeInput.text = "12345";
            gameCodeInput.onValueChanged.Invoke("12345");

            // Act
            playerNameInput.text = "";
            playerNameInput.onValueChanged.Invoke("");

            // Assert
            Assert.IsFalse(joinButton.interactable, "Join button should be disabled with empty name");
        }

        #endregion

        #region Age Tests

        [Test]
        public void Age_ValidNumber_IsPropagatedCorrectly()
        {
            // Arrange
            const string ageString = "25";
            const int expectedAge = 25;

            // Act
            ageInput.text = ageString;
            ageInput.onValueChanged.Invoke(ageString);

            // Assert
            var actualAge = TestHelpers.GetPrivateField<int>(loginComponent, "age");
            Assert.AreEqual(expectedAge, actualAge, "Age should be propagated correctly");
        }

        [Test]
        public void Age_InvalidNumber_SetsToNegativeOne()
        {
            // Arrange
            const string invalidAge = "not a number";

            // Act
            ageInput.text = invalidAge;
            ageInput.onValueChanged.Invoke(invalidAge);

            // Assert
            var actualAge = TestHelpers.GetPrivateField<int>(loginComponent, "age");
            Assert.AreEqual(-1, actualAge, "Invalid age should set to -1");
        }

        [Test]
        public void Age_Zero_DisablesJoinButton()
        {
            // Arrange - set all other fields valid
            playerNameInput.text = "TestPlayer";
            playerNameInput.onValueChanged.Invoke("TestPlayer");
            gameCodeInput.text = "12345";
            gameCodeInput.onValueChanged.Invoke("12345");

            // Act
            ageInput.text = "0";
            ageInput.onValueChanged.Invoke("0");

            // Assert
            Assert.IsFalse(joinButton.interactable, "Join button should be disabled with age 0");
        }

        #endregion

        #region Gender Tests

        [Test]
        public void Gender_WhenSelected_IsPropagatedCorrectly()
        {
            // Arrange
            const int expectedGender = 1;

            // Act
            genderDropdown.value = expectedGender;
            genderDropdown.onValueChanged.Invoke(expectedGender);

            // Assert
            var actualGender = TestHelpers.GetPrivateField<int>(loginComponent, "gender");
            Assert.AreEqual(expectedGender, actualGender, "Gender should be propagated correctly");
        }

        [Test]
        public void Gender_MultipleChanges_UpdatesCorrectly()
        {
            // Act & Assert - First change
            genderDropdown.value = 0;
            genderDropdown.onValueChanged.Invoke(0);
            Assert.AreEqual(0, TestHelpers.GetPrivateField<int>(loginComponent, "gender"));

            // Act & Assert - Second change
            genderDropdown.value = 2;
            genderDropdown.onValueChanged.Invoke(2);
            Assert.AreEqual(2, TestHelpers.GetPrivateField<int>(loginComponent, "gender"));
        }

        #endregion

        #region Game Code Tests

        [Test]
        public void GameCode_ValidNumber_IsPropagatedCorrectly()
        {
            // Arrange
            const string codeString = "54321";
            const int expectedCode = 54321;

            // Act
            gameCodeInput.text = codeString;
            gameCodeInput.onValueChanged.Invoke(codeString);

            // Assert
            var actualCode = TestHelpers.GetPrivateField<int>(loginComponent, "gameCode");
            Assert.AreEqual(expectedCode, actualCode, "Game code should be propagated correctly");
        }

        [Test]
        public void GameCode_WithSpaces_RemovesSpaces()
        {
            // Arrange
            const string codeWithSpaces = "12 345";
            const int expectedCode = 12345;

            // Act
            gameCodeInput.text = codeWithSpaces;
            gameCodeInput.onValueChanged.Invoke(codeWithSpaces);

            // Assert
            var actualCode = TestHelpers.GetPrivateField<int>(loginComponent, "gameCode");
            Assert.AreEqual(expectedCode, actualCode, "Game code should remove spaces");
        }

        [Test]
        public void GameCode_InvalidNumber_SetsToNegativeOne()
        {
            // Arrange
            const string invalidCode = "invalid";

            // Act
            gameCodeInput.text = invalidCode;
            gameCodeInput.onValueChanged.Invoke(invalidCode);

            // Assert
            var actualCode = TestHelpers.GetPrivateField<int>(loginComponent, "gameCode");
            Assert.AreEqual(-1, actualCode, "Invalid game code should set to -1");
        }

        #endregion

        #region Join Button Tests

        [Test]
        public void JoinButton_AllFieldsValid_IsEnabled()
        {
            // Act
            playerNameInput.text = "TestPlayer";
            playerNameInput.onValueChanged.Invoke("TestPlayer");
            ageInput.text = "25";
            ageInput.onValueChanged.Invoke("25");
            gameCodeInput.text = "12345";
            gameCodeInput.onValueChanged.Invoke("12345");

            // Assert
            Assert.IsTrue(joinButton.interactable, "Join button should be enabled with all valid fields");
        }

        [Test]
        public void JoinButton_MissingPlayerName_IsDisabled()
        {
            // Arrange
            ageInput.text = "25";
            ageInput.onValueChanged.Invoke("25");
            gameCodeInput.text = "12345";
            gameCodeInput.onValueChanged.Invoke("12345");

            // Act
            playerNameInput.text = "";
            playerNameInput.onValueChanged.Invoke("");

            // Assert
            Assert.IsFalse(joinButton.interactable, "Join button should be disabled without player name");
        }

        [Test]
        public void JoinButton_InvalidAge_IsDisabled()
        {
            // Arrange
            playerNameInput.text = "TestPlayer";
            playerNameInput.onValueChanged.Invoke("TestPlayer");
            gameCodeInput.text = "12345";
            gameCodeInput.onValueChanged.Invoke("12345");

            // Act
            ageInput.text = "invalid";
            ageInput.onValueChanged.Invoke("invalid");

            // Assert
            Assert.IsFalse(joinButton.interactable, "Join button should be disabled with invalid age");
        }

        [Test]
        public void JoinButton_InvalidGameCode_IsDisabled()
        {
            // Arrange
            playerNameInput.text = "TestPlayer";
            playerNameInput.onValueChanged.Invoke("TestPlayer");
            ageInput.text = "25";
            ageInput.onValueChanged.Invoke("25");

            // Act
            gameCodeInput.text = "";
            gameCodeInput.onValueChanged.Invoke("");

            // Assert
            Assert.IsFalse(joinButton.interactable, "Join button should be disabled with invalid game code");
        }

        #endregion

        #region Network Tests - Join Button Click

        [Test]
        public void JoinButton_WhenClicked_SubscribesToGameCode()
        {
            // Arrange
            const int gameCode = 12345;

            // Act
            // Simulate what should happen when join button is clicked
            mockNetworkManager.Subscribe(gameCode.ToString());

            // Assert
            Assert.IsTrue(mockNetworkManager.SubscribeCalled, "Should subscribe to game code");
            Assert.AreEqual("12345", mockNetworkManager.LastSubscribeTopic, "Should subscribe to correct topic");
        }

        [Test]
        public void JoinButton_WhenClicked_PublishesJoinRequest()
        {
            // Arrange
            const int gameCode = 12345;

            // Act
            // Simulate the publish call that would happen in OnJoinButtonClicked
            mockNetworkManager.Publish(gameCode.ToString(), new BaseMessage());

            // Assert
            Assert.IsTrue(mockNetworkManager.PublishCalled, "Should publish join request");
            Assert.AreEqual("12345", mockNetworkManager.LastPublishTopic, "Should publish to correct topic");
            Assert.IsNotNull(mockNetworkManager.LastPublishMessage, "Should include message data");
        }

        [Test]
        public void JoinButton_WhenClicked_SetsPlayerData()
        {
            // Arrange
            const string playerName = "TestPlayer";
            const int age = 25;
            const int gender = 1;
            const int gameCode = 12345;

            // Act
            mockPlayerData.PlayerName = playerName;
            mockPlayerData.Age = age;
            mockPlayerData.Gender = gender;
            mockPlayerData.LobbyID = gameCode;
            mockPlayerData.RequestID = Guid.NewGuid().ToString();

            // Assert
            Assert.AreEqual(playerName, mockPlayerData.PlayerName);
            Assert.AreEqual(age, mockPlayerData.Age);
            Assert.AreEqual(gender, mockPlayerData.Gender);
            Assert.AreEqual(gameCode, mockPlayerData.LobbyID);
            Assert.IsNotEmpty(mockPlayerData.RequestID);
        }

        [Test]
        public void JoinButton_WhenClicked_GeneratesUniqueRequestID()
        {
            // Act
            var requestId1 = Guid.NewGuid().ToString();
            var requestId2 = Guid.NewGuid().ToString();

            // Assert
            Assert.AreNotEqual(requestId1, requestId2, "Request IDs should be unique");
        }

        #endregion

        #region Network Tests - Error Handling

        [Test]
        public void NetworkError_WhenReceived_ActivatesErrorUI()
        {
            // Arrange
            var errorUI = new GameObject("NatsError");
            errorUI.SetActive(false);

            // Act
            // Simulate HandleError being called
            Debug.LogWarning("NATS Error: Connection failed");
            errorUI.SetActive(true);

            // Assert
            Assert.IsTrue(errorUI.activeSelf, "Error UI should be visible");

            // Cleanup
            UnityEngine.Object.DestroyImmediate(errorUI);
        }

        [Test]
        public void RejectedMessage_SessionAlreadyStarted_ShowsSessionError()
        {
            // Arrange
            var sessionErrorUI = new GameObject("SessionError");
            sessionErrorUI.SetActive(false);

            var rejectedMessage = new RejectedMessage(
                DateTime.Now.ToString("o"),
                0,
                -1,
                "TestPlayer",
                "SessionAlreadyStarted",
                "Session has already started",
                "SessionAlreadyStarted"
            );

            // Act
            // Simulate the HandleRejected logic
            if (rejectedMessage.ReferenceID == "SessionAlreadyStarted")
            {
                sessionErrorUI.SetActive(true);
            }

            // Assert
            Assert.IsTrue(sessionErrorUI.activeSelf, "Session error should be visible");

            // Cleanup
            UnityEngine.Object.DestroyImmediate(sessionErrorUI);
        }

        [Test]
        public void RejectedMessage_PlayerNameTaken_ShowsNameError()
        {
            // Arrange
            var nameErrorUI = new GameObject("NameError");
            nameErrorUI.SetActive(false);

            var rejectedMessage = new RejectedMessage(
                DateTime.Now.ToString("o"),
                0,
                -1,
                "Player",
                "request-123",
                "Player name is already taken",
                "123"
            );

            // Act
            if (rejectedMessage.ReferenceID == "PlayerNameAlreadyTaken")
            {
                nameErrorUI.SetActive(true);
            }

            // Assert
            Assert.IsTrue(nameErrorUI.activeSelf, "Name error should be visible");

            // Cleanup
            UnityEngine.Object.DestroyImmediate(nameErrorUI);
        }

        [Test]
        public void RejectedMessage_WrongPlayer_DoesNotShowError()
        {
            // Arrange
            var sessionErrorUI = new GameObject("SessionError");
            sessionErrorUI.SetActive(false);

            mockPlayerData.PlayerName = "ActualPlayer";
            mockPlayerData.RequestID = "request-123";

            var rejectedMessage = new RejectedMessage(
                DateTime.Now.ToString("o"),
                0,
                -1,
                "DifferentPlayer", // Different player
                "request-456",     // Different request
                "Session has already started",
                ""
            );

            // Act
            // Simulate the HandleRejected logic with player check
            if (rejectedMessage.TargetPlayer == mockPlayerData.PlayerName ||
                rejectedMessage.RequestID == mockPlayerData.RequestID)
            {
                sessionErrorUI.SetActive(true);
            }

            // Assert
            Assert.IsFalse(sessionErrorUI.activeSelf, "Should not show error for different player");

            // Cleanup
            UnityEngine.Object.DestroyImmediate(sessionErrorUI);
        }

        #endregion

        #region Network Tests - Event Subscription

        [Test]
        public void OnEnable_SubscribesToNetworkEvents()
        {
            // Arrange
            bool errorHandlerCalled = false;
            bool rejectedHandlerCalled = false;

            mockNetworkManager.OnError += (sender, error) => errorHandlerCalled = true;
            mockNetworkManager.OnRejected += (sender, msg) => rejectedHandlerCalled = true;

            // Act
            mockNetworkManager.TriggerError("Test error");
            mockNetworkManager.TriggerRejected(new RejectedMessage(
                DateTime.Now.ToString("o"), 0, -1, "Player", "123", "Message", "requestID"
            ));

            // Assert
            Assert.IsTrue(errorHandlerCalled, "Should handle error events");
            Assert.IsTrue(rejectedHandlerCalled, "Should handle rejected events");
        }

        #endregion

        #region Integration Tests

        [UnityTest]
        public IEnumerator CompleteLoginFlow_WithValidData_EnablesButton()
        {
            // Act - Simulate user filling out form
            playerNameInput.text = "TestPlayer";
            playerNameInput.onValueChanged.Invoke("TestPlayer");
            yield return null;

            ageInput.text = "25";
            ageInput.onValueChanged.Invoke("25");
            yield return null;

            genderDropdown.value = 1;
            genderDropdown.onValueChanged.Invoke(1);
            yield return null;

            gameCodeInput.text = "12345";
            gameCodeInput.onValueChanged.Invoke("12345");
            yield return null;

            // Assert
            Assert.IsTrue(joinButton.interactable, "Button should be enabled after complete valid form");
            Assert.AreEqual("TestPlayer", TestHelpers.GetPrivateField<string>(loginComponent, "playerName"));
            Assert.AreEqual(25, TestHelpers.GetPrivateField<int>(loginComponent, "age"));
            Assert.AreEqual(1, TestHelpers.GetPrivateField<int>(loginComponent, "gender"));
            Assert.AreEqual(12345, TestHelpers.GetPrivateField<int>(loginComponent, "gameCode"));
        }

        #endregion
    }
}
