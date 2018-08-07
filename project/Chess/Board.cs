using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public class ChessBoard
    {
        private static int[] pieceWeights = { 1, 3, 4, 5, 7, 20 };

        public piece_t[][] Grid { get; private set; }
        public Dictionary<Player, position_t> Kings { get; private set; }
        public Dictionary<Player, List<position_t>> Pieces { get; private set; }
        public Dictionary<Player, position_t> LastMove { get; private set; }
        List<int> availablePlaces = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };

        Random randNum = new Random();
        public ChessBoard()
        {
            // init blank board grid
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                    Grid[i][j] = new piece_t(Piece.NONE, Player.WHITE);
            }

            // init last moves
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t();
            LastMove[Player.WHITE] = new position_t();

            // init king positions
            Kings = new Dictionary<Player, position_t>();

            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());
        }

        public ChessBoard(ChessBoard copy)
        {
            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());

            // init board grid to copy locations
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                {
                    Grid[i][j] = new piece_t(copy.Grid[i][j]);

                    // add piece location to list
                    if (Grid[i][j].piece != Piece.NONE)
                        Pieces[Grid[i][j].player].Add(new position_t(j, i));
                }
            }

            // copy last known move
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t(copy.LastMove[Player.BLACK]);
            LastMove[Player.WHITE] = new position_t(copy.LastMove[Player.WHITE]);

            // copy king locations
            Kings = new Dictionary<Player, position_t>();
            Kings[Player.BLACK] = new position_t(copy.Kings[Player.BLACK]);
            Kings[Player.WHITE] = new position_t(copy.Kings[Player.WHITE]);
        }

        /// <summary>
        /// Calculate and return the boards fitness value.
        /// </summary>
        /// <param name="max">Who's side are we viewing from.</param>
        /// <returns>The board fitness value, what else?</returns>
        public int fitness(Player max)
        {
            int fitness = 0;
            int[] blackPieces = { 0, 0, 0, 0, 0, 0 };
            int[] whitePieces = { 0, 0, 0, 0, 0, 0 };
            int blackMoves = 0;
            int whiteMoves = 0;

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.BLACK])
            {
                blackMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                blackPieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.WHITE])
            {
                whiteMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                whitePieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // if viewing from black side
            if (max == Player.BLACK)
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (blackPieces[i] - whitePieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (blackMoves - whiteMoves));
            }
            else
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (whitePieces[i] - blackPieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (whiteMoves - blackMoves));
            }

            return fitness;
        }
        public void SetInitialPlacement960()
        {
            int kPos = randNum.Next(1, 7);
            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }
            Kings[Player.WHITE] = new position_t(kPos, 0);
            Kings[Player.BLACK] = new position_t(kPos, 7);
            SetPiece(Piece.KING, Player.WHITE, kPos, 0);
            SetPiece(Piece.KING, Player.BLACK, kPos, 7);
            availablePlaces.Remove(kPos);
            placeRooks(kPos);
            placeBishops();
            placeKnightsAndQueens();
        }

        private void placeKnightsAndQueens()
        {
            int knightPos = getAvailablePlacement();
            availablePlaces.Remove(knightPos);
            int knight2Pos = getAvailablePlacement();
            availablePlaces.Remove(knight2Pos);
            int queenPos = getAvailablePlacement();
            availablePlaces.Remove(queenPos);
            SetPiece(Piece.KNIGHT, Player.WHITE, knightPos, 0);
            SetPiece(Piece.KNIGHT, Player.WHITE, knight2Pos, 0);
            SetPiece(Piece.KNIGHT, Player.BLACK, knightPos, 7);
            SetPiece(Piece.KNIGHT, Player.BLACK, knight2Pos, 7);
            SetPiece(Piece.QUEEN, Player.WHITE, queenPos, 0);
            SetPiece(Piece.QUEEN, Player.BLACK, queenPos, 7);

        }

        private void placeBishops()
        {
            int bishopPos = 0;
            int bishop2Pos = 0;
            bool correctPlacement = false;
            while(correctPlacement == false)
            {
                bishopPos = getAvailablePlacement();
                bishop2Pos = getAvailablePlacement();
                if ((bishopPos % 2) != (bishop2Pos % 2))
                {
                    SetPiece(Piece.BISHOP, Player.WHITE, bishopPos, 0);
                    SetPiece(Piece.BISHOP, Player.WHITE, bishop2Pos, 0);
                    SetPiece(Piece.BISHOP, Player.BLACK, bishopPos, 7);
                    SetPiece(Piece.BISHOP, Player.BLACK, bishop2Pos, 7);
                    availablePlaces.Remove(bishopPos);
                    availablePlaces.Remove(bishop2Pos);
                    correctPlacement = true;
                }

            }
        }

        public int getAvailablePlacement()
        {
            return availablePlaces[randNum.Next(availablePlaces.Count)];
        }

        public void placeRooks(int KPos)
        {
            int rookPos = 0;
            int rook2Pos = 0;
            bool correctPlacement = false;
            while (correctPlacement == false)
            {
                rookPos = getAvailablePlacement();
                rook2Pos = getAvailablePlacement();
                if ((rookPos < KPos  && rook2Pos > KPos) || (rookPos > KPos && rook2Pos < KPos)){
                    SetPiece(Piece.ROOK, Player.WHITE, rookPos, 0);
                    SetPiece(Piece.ROOK, Player.BLACK, rookPos, 7);
                    SetPiece(Piece.ROOK, Player.WHITE, rook2Pos, 0);
                    SetPiece(Piece.ROOK, Player.BLACK, rook2Pos, 7);
                    availablePlaces.Remove(rookPos);
                    availablePlaces.Remove(rook2Pos);
                    correctPlacement = true;         
                }
            }
           

        }
        public void SetInitialPlacement()
        {
            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }

            SetPiece(Piece.ROOK, Player.WHITE, 0, 0);
            SetPiece(Piece.ROOK, Player.WHITE, 7, 0);
            SetPiece(Piece.ROOK, Player.BLACK, 0, 7);
            SetPiece(Piece.ROOK, Player.BLACK, 7, 7);

            SetPiece(Piece.KNIGHT, Player.WHITE, 1, 0);
            SetPiece(Piece.KNIGHT, Player.WHITE, 6, 0);
            SetPiece(Piece.KNIGHT, Player.BLACK, 1, 7);
            SetPiece(Piece.KNIGHT, Player.BLACK, 6, 7);

            SetPiece(Piece.BISHOP, Player.WHITE, 2, 0);
            SetPiece(Piece.BISHOP, Player.WHITE, 5, 0);
            SetPiece(Piece.BISHOP, Player.BLACK, 2, 7);
            SetPiece(Piece.BISHOP, Player.BLACK, 5, 7);

            SetPiece(Piece.KING, Player.WHITE, 4, 0);
            SetPiece(Piece.KING, Player.BLACK, 4, 7);
            Kings[Player.WHITE] = new position_t(4, 0);
            Kings[Player.BLACK] = new position_t(4, 7);
            SetPiece(Piece.QUEEN, Player.WHITE, 3, 0);
            SetPiece(Piece.QUEEN, Player.BLACK, 3, 7);
        }

        public void SetPiece(Piece piece, Player player, int letter, int number)
        {
            // set grid values
            Grid[number][letter].piece = piece;
            Grid[number][letter].player = player;

            // add piece to list
            Pieces[player].Add(new position_t(letter, number));

            // update king position
            if (piece == Piece.KING)
            {
                Kings[player] = new position_t(letter, number);
            }
        }
    }
}
