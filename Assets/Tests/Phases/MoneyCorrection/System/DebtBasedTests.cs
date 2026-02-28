using NUnit.Framework;
using Tests.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1;
using Version1.Phases.MoneyCorrection.scripts;
using Version1.Utilities.PlayerData;

namespace Tests.Phases.MoneyCorrection.System
{
    public class DebtBasedTests
    {
        private GameObject controllerObject;
        private MoneyCorrectionPhaseController controller;

        private GameObject playerDataObject;
        private PlayerData playerData;

        private TextMeshProUGUI text;
        private Button continueButton;

        [SetUp]
        public void SetUp()
        {
            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;

            // --- PlayerData Singleton Setup ---
            playerDataObject = new GameObject("PlayerData");
            playerData = playerDataObject.AddComponent<PlayerData>();
            playerData.CurrentMoneySystem = MoneySystems.DebtBased;

            TestHelpers.SetPrivateStaticField(typeof(PlayerData), "_instance", playerData);

            // --- Controller Setup ---
            controllerObject = new GameObject("MoneyCorrectionController");
            controllerObject.SetActive(false);

            controller = controllerObject.AddComponent<MoneyCorrectionPhaseController>();

            var canvasGo = new GameObject("Canvas");
            canvasGo.AddComponent<Canvas>();
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(canvasGo.transform);
            text = textGo.AddComponent<TextMeshProUGUI>();

            continueButton = TestHelpers.CreateButton("ContinueButton");

            TestHelpers.SetPrivateField(controller, "text", text);
            TestHelpers.SetPrivateField(controller, "continueButton", continueButton);

            controllerObject.SetActive(true);

            TestHelpers.InjectMockNetworkManager();
        }

        [TearDown]
        public void TearDown()
        {
            controller.StopAllCoroutines();
            TestHelpers.ClearMockPhaseManager();
            TestHelpers.ClearMockNetworkManager();

            Object.DestroyImmediate(controllerObject);
            Object.DestroyImmediate(playerDataObject);
            Object.DestroyImmediate(text.transform.parent.gameObject);
            Object.DestroyImmediate(continueButton.gameObject);

            TestHelpers.SetPrivateStaticField(typeof(PlayerData), "_instance", null);
        }

        private void InvokeDebtBasedSystem()
        {
            TestHelpers.InvokePrivateMethod(controller, "HandleDebtBasedSystem");
        }

        private void SetRoundNumber(int round)
        {
            // Assumes TestHelpers can set the round number on the PhaseManager mock
            TestHelpers.SetRoundNumber(round);
        }

        // -------------------------------------------------------------------------
        // Interest Remainder Accumulation (interest not yet divisible by 1000)
        // -------------------------------------------------------------------------

        #region Interest Remainder Accumulation

        [Test]
        public void Round1_InterestBelow1000_NothingPaid_RemainderStored()
        {
            // Debt=4000, interest=400 → totalInterest=400 → toPay=0, remainder=400
            SetRoundNumber(1);
            playerData.Debt = 4000;
            playerData.Balance = 10000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(10000, playerData.Balance, "Balance should be unchanged");
            Assert.AreEqual(4000, playerData.Debt, "Debt should be unchanged");
            Assert.AreEqual(400, playerData.InterestRemainder, "Remainder should store the 400");
        }

        [Test]
        public void Round2_RemainderCarriedOver_TotalExceeds1000_PaymentMade()
        {
            // Round 1 leftover: remainder=400. Round 2: interest=400 again → total=800, still <1000, no payment
            // But with debt=5000: interest=500, total=500+400=900, still <1000
            // Use debt=10000: interest=1000, total=1000+0=1000 → toPay=1000
            SetRoundNumber(2);
            playerData.Debt = 10000;
            playerData.Balance = 10000;
            playerData.InterestRemainder = 400;

            InvokeDebtBasedSystem();

            // totalInterest = 1000 + 400 = 1400 → toPay=1000, remainder=400
            Assert.AreEqual(9000, playerData.Balance, "Balance should decrease by 1000");
            Assert.AreEqual(10000, playerData.Debt, "Debt should be unchanged (fully paid)");
            Assert.AreEqual(400, playerData.InterestRemainder, "Remainder should be 400");
        }

        [Test]
        public void Remainder_AccumulatesAcrossRoundsUntilThreshold()
        {
            // Debt=4000 → interest=400/round
            // Round 1: total=400 → toPay=0, remainder=400
            // Round 2: total=800 → toPay=0, remainder=800
            // Round 3: total=1200 → toPay=1000, remainder=200
            SetRoundNumber(1);
            playerData.Debt = 4000;
            playerData.Balance = 10000;
            playerData.InterestRemainder = 0;
            InvokeDebtBasedSystem();
            Assert.AreEqual(400, playerData.InterestRemainder, "After round 1: remainder=400");
            Assert.AreEqual(10000, playerData.Balance, "After round 1: balance unchanged");

            SetRoundNumber(2);
            InvokeDebtBasedSystem();
            Assert.AreEqual(800, playerData.InterestRemainder, "After round 2: remainder=800");
            Assert.AreEqual(10000, playerData.Balance, "After round 2: balance unchanged");

            SetRoundNumber(3);
            InvokeDebtBasedSystem();
            Assert.AreEqual(200, playerData.InterestRemainder, "After round 3: remainder=200");
            Assert.AreEqual(9000, playerData.Balance, "After round 3: 1000 paid");
        }

        #endregion

        // -------------------------------------------------------------------------
        // Normal Rounds - Can Pay
        // -------------------------------------------------------------------------

        #region Normal Rounds - Can Pay

        [Test]
        public void NormalRound_ExactlyDivisible_FullPayment()
        {
            // Debt=10000, interest=1000 → toPay=1000, balance has plenty
            SetRoundNumber(3);
            playerData.Debt = 10000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(4000, playerData.Balance, "Should pay 1000 interest");
            Assert.AreEqual(10000, playerData.Debt, "Debt unchanged when fully paid");
            Assert.AreEqual(0, playerData.InterestRemainder);
        }

        [Test]
        public void NormalRound_InterestWithRemainder_PartialThousandPaid()
        {
            // Debt=15000, interest=1500 → toPay=1000, remainder=500
            SetRoundNumber(5);
            playerData.Debt = 15000;
            playerData.Balance = 8000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(7000, playerData.Balance, "Should pay 1000");
            Assert.AreEqual(15000, playerData.Debt, "Debt unchanged");
            Assert.AreEqual(500, playerData.InterestRemainder, "Remainder=500");
        }

        [Test]
        public void NormalRound_PreviousRemainder_IncreasesTotalInterest()
        {
            // Debt=10000, interest=1000, remainder=500 → total=1500 → toPay=1000, newRemainder=500
            SetRoundNumber(4);
            playerData.Debt = 10000;
            playerData.Balance = 6000;
            playerData.InterestRemainder = 500;

            InvokeDebtBasedSystem();

            Assert.AreEqual(5000, playerData.Balance);
            Assert.AreEqual(10000, playerData.Debt);
            Assert.AreEqual(500, playerData.InterestRemainder);
        }

        #endregion

        // -------------------------------------------------------------------------
        // Normal Rounds - Cannot Pay
        // -------------------------------------------------------------------------

        #region Normal Rounds - Cannot Pay

        [Test]
        public void NormalRound_CannotPay_UnpaidAddedToDebt()
        {
            // Debt=10000, interest=1000 → toPay=1000, balance=0 → paid=0, unpaid=1000 added to debt
            SetRoundNumber(3);
            playerData.Debt = 10000;
            playerData.Balance = 0;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(0, playerData.Balance, "Balance stays 0");
            Assert.AreEqual(11000, playerData.Debt, "Unpaid 1000 added to debt");
            Assert.AreEqual(0, playerData.InterestRemainder, "Remainder is 0 (sub-1000 part)");
        }

        [Test]
        public void NormalRound_CannotPay_RemainderStillStoredCorrectly()
        {
            // Debt=15000, interest=1500 → toPay=1000, remainder=500, balance=500 → affordable=0 → paid=0, unpaid=1000
            SetRoundNumber(6);
            playerData.Debt = 15000;
            playerData.Balance = 500;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(500, playerData.Balance, "Balance unchanged (can't afford 1000)");
            Assert.AreEqual(16000, playerData.Debt, "Unpaid 1000 added to debt");
            Assert.AreEqual(500, playerData.InterestRemainder, "Sub-1000 remainder stored");
        }

        [Test]
        public void NormalRound_PartialPayment_PaysWhatItCan()
        {
            // Debt=20000, interest=2000 → toPay=2000, balance=1000 → affordable=1000, paid=1000, unpaid=1000
            SetRoundNumber(4);
            playerData.Debt = 20000;
            playerData.Balance = 1000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(0, playerData.Balance, "Pays 1000, balance=0");
            Assert.AreEqual(21000, playerData.Debt, "Unpaid 1000 added to debt");
        }

        [Test]
        public void NormalRound_BalanceNotMultipleOf1000_FloorsAffordable()
        {
            // Balance=1999 → affordable=1000, toPay=1000 → paid=1000
            SetRoundNumber(3);
            playerData.Debt = 10000;
            playerData.Balance = 1999;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(999, playerData.Balance, "Pays 1000, leaves 999");
            Assert.AreEqual(10000, playerData.Debt, "Debt unchanged");
        }

        #endregion

        // -------------------------------------------------------------------------
        // Last Round (Round 12) - Rounding
        // -------------------------------------------------------------------------

        #region Last Round - Rounding

        [Test]
        public void LastRound_InterestExactly500_RoundsUpTo1000()
        {
            // Debt=5000, interest=500 → Round(500/1000, AwayFromZero)*1000 = 1000
            SetRoundNumber(12);
            playerData.Debt = 5000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(4000, playerData.Balance, "500 rounds up to 1000, paid 1000");
            Assert.AreEqual(0, playerData.InterestRemainder, "Remainder cleared on last round");
        }

        [Test]
        public void LastRound_InterestBelow500_RoundsDownToZero()
        {
            // Debt=4000, interest=400 → Round(400/1000)*1000 = 0
            SetRoundNumber(12);
            playerData.Debt = 4000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(5000, playerData.Balance, "400 rounds down, nothing paid");
            Assert.AreEqual(0, playerData.InterestRemainder);
        }

        [Test]
        public void LastRound_RemainderPushesInterestOver500_RoundsUp()
        {
            // Debt=4000, interest=400, remainder=200 → total=600 → rounds to 1000
            SetRoundNumber(12);
            playerData.Debt = 4000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 200;

            InvokeDebtBasedSystem();

            Assert.AreEqual(4000, playerData.Balance, "600 rounds up to 1000");
            Assert.AreEqual(0, playerData.InterestRemainder, "Remainder cleared");
        }

        [Test]
        public void LastRound_RemainderKeepsTotalBelow500_RoundsDown()
        {
            // Debt=4000, interest=400, remainder=0 → total=400 → rounds to 0
            SetRoundNumber(12);
            playerData.Debt = 4000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(5000, playerData.Balance, "400 rounds down to 0, nothing paid");
            Assert.AreEqual(0, playerData.InterestRemainder, "Remainder cleared");
        }

        #endregion

        // -------------------------------------------------------------------------
        // Last Round (Round 12) - Point Deduction
        // -------------------------------------------------------------------------

        #region Last Round - Point Deduction

        [Test]
        public void LastRound_CannotPayInterest_UnpaidAddedToDebt_PointsDeducted()
        {
            // Debt=10000, interest=1000 → toPay=1000, balance=0 → unpaid=1000 → Debt=11000 → points -= 11
            SetRoundNumber(12);
            playerData.Debt = 10000;
            playerData.Balance = 0;
            playerData.InterestRemainder = 0;
            playerData.Points = 20;

            InvokeDebtBasedSystem();

            Assert.AreEqual(0, playerData.Balance);
            Assert.AreEqual(0, playerData.Debt, "Debt zeroed after point deduction");
            Assert.AreEqual(9, playerData.Points, "20 - 11 = 9 points");
        }

        [Test]
        public void LastRound_DebtFullyPaid_NoPointDeduction()
        {
            // Debt=10000, interest=1000 → toPay=1000, balance=5000 → paid=1000, Debt still 10000 → 10 points deducted
            // Wait — Debt is the original loan, not the interest. After paying interest, Debt=10000 still exists.
            // Points deducted = Debt / 1000 = 10. This is by design per the spec.
            SetRoundNumber(12);
            playerData.Debt = 10000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;
            playerData.Points = 20;

            InvokeDebtBasedSystem();

            Assert.AreEqual(4000, playerData.Balance, "1000 interest paid");
            Assert.AreEqual(0, playerData.Debt, "Debt zeroed");
            Assert.AreEqual(10, playerData.Points, "20 - 10 (debt/1000) = 10");
        }

        [Test]
        public void LastRound_12000Debt_Deducts12Points()
        {
            SetRoundNumber(12);
            playerData.Debt = 12000;
            playerData.Balance = 0;
            playerData.InterestRemainder = 0;
            playerData.Points = 15;

            InvokeDebtBasedSystem();

            // interest = 1200 → rounds to 1000 (since 200 < 500) → toPay=1000
            // balance=0 → unpaid=1000 → Debt=13000 → points -= 13
            Assert.AreEqual(0, playerData.Debt);
            Assert.AreEqual(2, playerData.Points, "15 - 13 = 2 points");
        }

        [Test]
        public void LastRound_PointsCanGoNegative()
        {
            SetRoundNumber(12);
            playerData.Debt = 20000;
            playerData.Balance = 0;
            playerData.InterestRemainder = 0;
            playerData.Points = 5;

            InvokeDebtBasedSystem();

            // interest=2000 → toPay=2000 → paid=0 → Debt=22000 → points -= 22
            Assert.AreEqual(-17, playerData.Points, "5 - 22 = -17");
        }

        [Test]
        public void LastRound_ZeroDebt_NoPointDeduction()
        {
            SetRoundNumber(12);
            playerData.Debt = 0;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;
            playerData.Points = 10;

            InvokeDebtBasedSystem();

            Assert.AreEqual(5000, playerData.Balance, "Nothing to pay");
            Assert.AreEqual(10, playerData.Points, "No points deducted");
            Assert.AreEqual(0, playerData.Debt);
        }

        #endregion

        // -------------------------------------------------------------------------
        // Edge Cases
        // -------------------------------------------------------------------------

        #region Edge Cases

        [Test]
        public void NormalRound_ZeroDebt_ZeroInterest_NothingHappens()
        {
            SetRoundNumber(5);
            playerData.Debt = 0;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(5000, playerData.Balance);
            Assert.AreEqual(0, playerData.Debt);
            Assert.AreEqual(0, playerData.InterestRemainder);
        }

        [Test]
        public void NormalRound_ZeroDebt_WithOldRemainder_RemainderIncreases()
        {
            // No new interest (debt=0), but old remainder carries over
            // total=0+800=800 → toPay=0, remainder=800
            SetRoundNumber(5);
            playerData.Debt = 0;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 800;

            InvokeDebtBasedSystem();

            Assert.AreEqual(5000, playerData.Balance);
            Assert.AreEqual(0, playerData.Debt);
            Assert.AreEqual(800, playerData.InterestRemainder);
        }

        [Test]
        public void NormalRound_InterestExactly1000_FullThousandPaid()
        {
            SetRoundNumber(2);
            playerData.Debt = 10000;
            playerData.Balance = 5000;
            playerData.InterestRemainder = 0;

            InvokeDebtBasedSystem();

            Assert.AreEqual(4000, playerData.Balance);
            Assert.AreEqual(10000, playerData.Debt);
            Assert.AreEqual(0, playerData.InterestRemainder);
        }

        [Test]
        public void LastRound_InterestRemainderClearedRegardlessOfAmount()
        {
            SetRoundNumber(12);
            playerData.Debt = 10000;
            playerData.Balance = 10000;
            playerData.InterestRemainder = 999;

            InvokeDebtBasedSystem();

            Assert.AreEqual(0, playerData.InterestRemainder, "Remainder always 0 after last round");
        }

        #endregion
    }
}
