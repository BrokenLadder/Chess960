using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
namespace ChessTests
{
    [TestClass]
    public class UnitTest1
    {
        ChessBoard myChessBoard = new ChessBoard();
        [TestMethod]
        public void getAvailablePlacementReturnsInt()
        {
            var myInt = myChessBoard.getAvailablePlacement();
            if(myInt.GetType() != typeof(int))
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void getAvailablePlacementNotNull()
        {
            Assert.IsNotNull(myChessBoard.getAvailablePlacement());
        }
        [TestMethod]
        public void placeKnightsAndQueensRemovesThreePlacesFromAvailablePlacesList()
        {
            int startingPlacesCount = myChessBoard.AvailablePlaces.Count;
            myChessBoard.placeKnightsAndQueens();
            int endingPlacesCount = myChessBoard.AvailablePlaces.Count;
            if((startingPlacesCount-3) != endingPlacesCount)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void placeRooksRemovesThreePlacesFromAvailablePlacesList()
        {
            int startingPlacesCount = myChessBoard.AvailablePlaces.Count;
            myChessBoard.placeRooks(3);
            int endingPlacesCount = myChessBoard.AvailablePlaces.Count;
            if ((startingPlacesCount - 2) != endingPlacesCount)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void placeBishopRemovesThreePlacesFromAvailablePlacesList()
        {
            int startingPlacesCount = myChessBoard.AvailablePlaces.Count;
            myChessBoard.placeBishops();
            int endingPlacesCount = myChessBoard.AvailablePlaces.Count;
            if ((startingPlacesCount - 2) != endingPlacesCount)
            {
                Assert.Fail();
            }
        }
    }
}
