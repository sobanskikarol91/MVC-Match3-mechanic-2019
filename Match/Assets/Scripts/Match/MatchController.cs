﻿using System;


public class MatchController : ControllerMVC<IMatchModel, IMatchView>, IMatchController
{
    public IBoardController[] Board { get; }


    public MatchController(IMatchModel model, IMatchView view) : base(model, view)
    {
        SubscribingSlotsEvents(model);
        model.Interaction.Swap += HandleSwap;
        model.FoundMatchesSuccessful += HandleFoundMatches;
        model.ErasingMatches += HandleEraseMatches;
        model.Shifting += view.ShiftingAnimation;
        model.FillingEmptySlots += view.FillingEmptySlotes;
        model.FoundMatchFailed += HandleNoMatchesFound;

        view.HighlightedMatchesEnd += model.OnErasingMatches;
        view.ErasedMatchesEnd += model.OnShiftTiles;
        view.ShiftingEnd += model.OnFillEmptySlots;
        view.FillingEmptySlotesEnd += HandleFillingEnd;
    }

    private void HandleNoMatchesFound()
    {
        view.Board.EnableInteraction();
    }

    private void HandleFillingEnd()
    {
        model.FindMatch();
    }

    private void HandleEraseMatches(object sender, EraseContentEventArgs e)
    {
        view.EraseMatches(e.ToErase);
    }

    private void HandleFoundMatches(object sender, FoundMatchesEventArgs e)
    {
        view.Board.DisableInteraction();
        view.HighlightMatches(e.Positions);
    }

    private void HandleSwap(object sender, SwapEventArgs e)
    {
        model.FindMatch();
    }

    private void SubscribingSlotsEvents(IMatchModel model)
    {
        ISlotModel[,] slots = model.Board.Slots;

        for (int r = 0; r < model.Board.Rows; r++)
        {
            for (int c = 0; c < model.Board.Columns; c++)
            {
                slots[r, c].Clicked += HandleClickedSlot;
            }
        }
    }

    private void HandleClickedSlot(object sender, EventArgs e)
    {
        ISlotModel slot = sender as ISlotModel;
        model.Interaction.SelectedSlot(slot);
    }
}

public interface IMatchController
{
    IBoardController[] Board { get; }
}