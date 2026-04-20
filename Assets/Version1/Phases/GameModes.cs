namespace Version1.Phases
{
    public static class GameModes
    {
        public static readonly IPhase[] Sustainable =
        {
            PhaseLibrary.MarketPhase,
            PhaseLibrary.MoneyCorrectionPhase,
            PhaseLibrary.LoadingPhase,
            PhaseLibrary.MarketPhase,
            PhaseLibrary.MoneyCorrectionPhase,
            PhaseLibrary.LoadingPhase,
            PhaseLibrary.MarketPhase,
            PhaseLibrary.MoneyCorrectionPhase,
            PhaseLibrary.LoadingPhase,
            PhaseLibrary.MoneyToPointPhase,
            PhaseLibrary.DonatePointsPhase,
            PhaseLibrary.EndPhase
        };

        public static readonly IPhase[] DebtBased =
        {
            PhaseLibrary.BankExplanation,
            PhaseLibrary.MarketPhase,
            PhaseLibrary.MoneyCorrectionPhase,
            PhaseLibrary.PayDebtPhase,
            PhaseLibrary.TakeALoanPhase,
            PhaseLibrary.LoadingPhase,
            PhaseLibrary.MarketPhase,
            PhaseLibrary.MoneyCorrectionPhase,
            PhaseLibrary.PayDebtPhase,
            PhaseLibrary.TakeALoanPhase,
            PhaseLibrary.LoadingPhase,
            PhaseLibrary.MarketPhase,
            PhaseLibrary.MoneyCorrectionPhase,
            PhaseLibrary.MoneyToPointPhase,
            PhaseLibrary.DonatePointsPhase,
            PhaseLibrary.EndPhase
        };
    }
}
