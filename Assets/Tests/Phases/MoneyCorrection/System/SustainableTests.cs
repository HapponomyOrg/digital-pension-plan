using NUnit.Framework;
using Tests.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1;
using Version1.Phases.MoneyCorrection.scripts;
using Version1.PlayerData;

namespace Tests.Phases.MoneyCorrection.System
{
    public class SustainableTests
    {
        private GameObject controllerObject;
        private MoneyCorrectionPhaseController controller;

        private GameObject playerDataObject;
        private PlayerData playerData;

        private TextMeshProUGUI text; // TMP_Text is abstract, must use concrete TextMeshProUGUI
        private Button continueButton;

        [SetUp]
        public void SetUp()
        {
            // Suppress coroutine log warnings that fire after the test completes
            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;

            // --- PlayerData Singleton Setup ---
            playerDataObject = new GameObject("PlayerData");
            playerData = playerDataObject.AddComponent<PlayerData>();
            playerData.CurrentMoneySystem = MoneySystems.Sustainable;

            TestHelpers.SetPrivateStaticField(typeof(PlayerData), "_instance", playerData);

            // --- Controller Setup ---
            controllerObject = new GameObject("MoneyCorrectionController");
            controllerObject.SetActive(false);

            controller = controllerObject.AddComponent<MoneyCorrectionPhaseController>();

            // TextMeshProUGUI must be parented under a Canvas to work correctly in tests
            var canvasGo = new GameObject("Canvas");
            canvasGo.AddComponent<Canvas>();
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(canvasGo.transform);
            text = textGo.AddComponent<TextMeshProUGUI>();

            continueButton = TestHelpers.CreateButton("ContinueButton");

            TestHelpers.SetPrivateField(controller, "text", text);
            TestHelpers.SetPrivateField(controller, "continueButton", continueButton);

            controllerObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            // Stop all coroutines before destroying to prevent post-destroy null refs
            controller.StopAllCoroutines();

            Object.DestroyImmediate(controllerObject);
            Object.DestroyImmediate(playerDataObject);
            Object.DestroyImmediate(text.transform.parent.gameObject); // destroys Canvas + Text
            Object.DestroyImmediate(continueButton.gameObject);

            // Clear singleton
            TestHelpers.SetPrivateStaticField(typeof(PlayerData), "_instance", null);
        }

        private void InvokeSustainableSystem()
        {
            TestHelpers.InvokePrivateMethod(controller, "HandleSustainableSystem");
        }

        #region Above 6000 - Penalty Tests

        [Test]
        public void Balance_7000_AppliesPenaltyOf1000_DueToRounding()
        {
            playerData.Balance = 7000;
            InvokeSustainableSystem();
            // (7000 - 6000) / 2 = 500 → RoundToThousand(500) = 1000 (AwayFromZero)
            Assert.AreEqual(6000, playerData.Balance);
        }

        [Test]
        public void Balance_8000_AppliesPenaltyOf1000()
        {
            playerData.Balance = 8000;
            InvokeSustainableSystem();
            // (8000 - 6000) / 2 = 1000 → RoundToThousand(1000) = 1000
            Assert.AreEqual(7000, playerData.Balance);
        }

        [Test]
        public void Balance_10000_AppliesPenaltyOf2000()
        {
            playerData.Balance = 10000;
            InvokeSustainableSystem();
            // (10000 - 6000) / 2 = 2000 → RoundToThousand(2000) = 2000
            Assert.AreEqual(8000, playerData.Balance);
        }

        [Test]
        public void Balance_6500_NoPenalty_DueToRounding()
        {
            playerData.Balance = 6500;
            InvokeSustainableSystem();
            // (6500 - 6000) / 2 = 250 → RoundToThousand(250) = 0
            Assert.AreEqual(6500, playerData.Balance);
        }

        [Test]
        public void Balance_7500_AppliesPenaltyOf1000_DueToRounding()
        {
            playerData.Balance = 7500;
            InvokeSustainableSystem();
            // (7500 - 6000) / 2 = 750 → RoundToThousand(750) = 1000
            Assert.AreEqual(6500, playerData.Balance);
        }

        [Test]
        public void Balance_20000_AppliesPenaltyOf7000()
        {
            playerData.Balance = 20000;
            InvokeSustainableSystem();
            // (20000 - 6000) / 2 = 7000 → RoundToThousand(7000) = 7000
            Assert.AreEqual(13000, playerData.Balance);
        }

        [Test]
        public void Balance_6001_PenaltyRoundsDownToZero()
        {
            playerData.Balance = 6001;
            InvokeSustainableSystem();
            // (6001 - 6000) / 2 = 0.5 → RoundToThousand(0.5) = 0
            Assert.AreEqual(6001, playerData.Balance);
        }

        [Test]
        public void Balance_1000000_AppliesCorrectPenalty()
        {
            playerData.Balance = 1000000;
            InvokeSustainableSystem();
            // (1000000 - 6000) / 2 = 497000 → RoundToThousand(497000) = 497000
            Assert.AreEqual(503000, playerData.Balance);
        }

        [Test]
        public void Balance_IntMaxValue_DoesNotOverflow()
        {
            playerData.Balance = int.MaxValue; // 2,147,483,647
            InvokeSustainableSystem();
            // (2147483647 - 6000) / 2f = 1,073,738,823.5 → RoundToThousand = 1,073,739,000
            Assert.AreEqual(1073744647, playerData.Balance);
        }

        #endregion

        #region Below 4000 - Bonus Tests

        [Test]
        public void Balance_3000_AddsBonus2000()
        {
            playerData.Balance = 3000;
            InvokeSustainableSystem();
            Assert.AreEqual(5000, playerData.Balance);
        }

        [Test]
        public void Balance_0_AddsBonus2000()
        {
            playerData.Balance = 0;
            InvokeSustainableSystem();
            Assert.AreEqual(2000, playerData.Balance);
        }

        [Test]
        public void Balance_3999_AddsBonus2000()
        {
            playerData.Balance = 3999;
            InvokeSustainableSystem();
            Assert.AreEqual(5999, playerData.Balance);
        }

        [Test]
        public void Balance_Negative_AddsBonus2000()
        {
            playerData.Balance = -500;
            InvokeSustainableSystem();
            Assert.AreEqual(1500, playerData.Balance);
        }

        #endregion

        #region Boundary / No Adjustment Tests

        [Test]
        public void Balance_Exactly4000_NoAdjustment()
        {
            playerData.Balance = 4000;
            InvokeSustainableSystem();
            Assert.AreEqual(4000, playerData.Balance);
        }

        [Test]
        public void Balance_Exactly6000_NoAdjustment()
        {
            playerData.Balance = 6000;
            InvokeSustainableSystem();
            Assert.AreEqual(6000, playerData.Balance);
        }

        [Test]
        public void Balance_5000_NoAdjustment()
        {
            playerData.Balance = 5000;
            InvokeSustainableSystem();
            Assert.AreEqual(5000, playerData.Balance);
        }

        [Test]
        public void Balance_4001_NoAdjustment()
        {
            playerData.Balance = 4001;
            InvokeSustainableSystem();
            Assert.AreEqual(4001, playerData.Balance);
        }

        [Test]
        public void Balance_5999_NoAdjustment()
        {
            playerData.Balance = 5999;
            InvokeSustainableSystem();
            Assert.AreEqual(5999, playerData.Balance);
        }

        #endregion

        #region Idempotency - Running Twice

        [Test]
        public void Balance_5000_RunTwice_StillNoAdjustment()
        {
            playerData.Balance = 5000;
            InvokeSustainableSystem();
            InvokeSustainableSystem();
            Assert.AreEqual(5000, playerData.Balance);
        }

        [Test]
        public void Balance_3000_RunTwice_LandsInRangeAfterFirstRun()
        {
            // First run: 3000 → 5000 (bonus applied, now in range)
            // Second run: 5000 → no change
            playerData.Balance = 3000;
            InvokeSustainableSystem();
            Assert.AreEqual(5000, playerData.Balance, "After first run");
            InvokeSustainableSystem();
            Assert.AreEqual(5000, playerData.Balance, "After second run");
        }

        #endregion
    }
}
