﻿using System;
using System.Collections.Generic;
using Hexen.HexenSystem;
using Hexen.ReplaySystem;
using UnityEngine;

namespace Hexen.DeckSystem
{
    public class CardEventArgs<TCard> : EventArgs
    {
        public TCard Card { get; }

        public CardEventArgs(TCard card)
        {
            Card = card;
        }
    }

    public class DeckManager<TCard, TPosition>
        where TCard : ICard<TPosition>
    {
        private List<TCard> _cards = new List<TCard>();
        private int _handSize = 5;
        public void Register(TCard card)
        {
            card.SetActive(false);
            _cards.Add(card);
        }
        public void FillHand(out TCard lastDrawnCard)
        {
            lastDrawnCard = default;
            if (_cards.Count <= _handSize) return;

            for (int i = 0; i < _handSize; i++)
            {
                if (_cards.Count > 0)
                {
                    _cards[i].SetActive(true);
                    lastDrawnCard = _cards[i];
                }
            }
        }
        public void Play(TCard card, TPosition position)
        {
            if (!_cards.Remove(card))
                return;

            if (!card.CanExecute(position))
                return;

            card.Execute(position);
            card.ResetCard();

            FillHand(out var newCard);

            
            card.SetActive(false);
            
            if (_cards.Contains(card))
                _cards.Remove(card);
            
            if (_cards.Contains(newCard))
                newCard.SetActive(true);
                
        }
        

    }
}