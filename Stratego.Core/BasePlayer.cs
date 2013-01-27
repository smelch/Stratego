﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public abstract class BasePlayer
    {
        protected StrategoGame _game;
        protected PlayerTurn _playerColor;
        public bool IsRed { get; private set; }

        public BasePlayer(StrategoGame game, PlayerTurn playerColor)
        {
            _game = game;
            _playerColor = playerColor;
            IsRed = playerColor == PlayerTurn.Red;
        }

        public abstract void BeginTurn();
        //public abstract void EndTurn();

        public abstract void PlacePieces();

        
    }
}
