﻿using System.Collections.Generic;

namespace UnityChess {
	public class Pawn : Piece {
		private static int instanceCounter;

		public Pawn(Square startingPosition, Side side) : base(startingPosition, side) {
			ID = ++instanceCounter;
		}

		private Pawn(Pawn pawnCopy) : base(pawnCopy) {
			ID = pawnCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckForwardMovingSquares(board, turn);
			CheckAttackingSquares(board, turn);
			CheckEnPassantCaptures(board, previousMoves, turn);
		}

		private void CheckForwardMovingSquares(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(testSquare, this);

			testSquare.AddVector(0, Side == Side.White ? 1 : -1);
			if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, turn)) {
				if (Position.Rank == (Side == Side.White ? 7 : 2)) {
					// PSEUDO call to gui method which gets user promotion piece choice
					// ElectedPiece userElection = GUI.getElectionChoice();

					//for now will default to Queen election
					ElectedPiece userElection = ElectedPiece.Queen;
					ValidMoves.Add(new PromotionMove(new Square(testSquare), this, userElection));
				} else {
					ValidMoves.Add(new Movement(testMove));

					if (!HasMoved) {
						testSquare.AddVector(0, Side == Side.White ? 1 : -1);
						if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, turn)) {
							ValidMoves.Add(new Movement(testMove));
						}
					}
				}
			}
		}

		private void CheckAttackingSquares(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(testSquare, this);

			foreach (int i in new[] {-1, 1}) {
				testSquare.CopyPosition(Position);
				testSquare.AddVector(i, Side == Side.White ? 1 : -1);

				if (testSquare.IsValid() && testSquare.IsOccupiedBySide(board, Side.Complement()) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position)) {
					if (Position.Rank == (Side == Side.White ? 7 : 2)) {
						// PSEUDO call to gui method which gets user promotion piece choice
						// ElectedPiece userElection = GUI.getElectionChoice();

						//for now will default to Queen election
						ElectedPiece userElection = ElectedPiece.Queen;
						ValidMoves.Add(new PromotionMove(new Square(testSquare), this, userElection));
					} else {
						ValidMoves.Add(new Movement(testMove));
					}
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, LinkedList<Movement> previousMoves, Side turn) {
			if (Side == Side.White ? Position.Rank == 5 : Position.Rank == 4) {
				Square testSquare = new Square(Position);

				foreach (int i in new[] {-1, 1}) {
					testSquare.CopyPosition(Position);
					testSquare.AddVector(i, 0);

					// ReSharper disable once PossibleNullReferenceException
					if (testSquare.IsValid() && board.GetPiece(testSquare) is Pawn && (board.GetPiece(testSquare) as Pawn).Side != Side) {
						Pawn enemyLateralPawn = board.GetPiece(testSquare) as Pawn;
						Piece pieceLastMoved = previousMoves.Last.Value.Piece;

						// ReSharper disable once PossibleUnintendedReferenceComparison
						if (pieceLastMoved is Pawn && pieceLastMoved as Pawn == enemyLateralPawn && pieceLastMoved.Position.Rank == (pieceLastMoved.Side == Side.White ? 2 : 7)) {
							EnPassantMove testMove = new EnPassantMove(new Square(testSquare.Rank + (Side == Side.White ? 1 : -1)), this, enemyLateralPawn);

							if (Rules.MoveObeysRules(board, testMove, turn)) {
								ValidMoves.Add(new EnPassantMove(new Square(testSquare.Rank + (Side == Side.White ? 1 : -1)), this, enemyLateralPawn));
							}
						}
					}
				}
			}
		}

		public override Piece Clone() {
			return new Pawn(this);
		}
	}
}