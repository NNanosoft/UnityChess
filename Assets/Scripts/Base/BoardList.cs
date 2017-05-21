﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
{
    public class BoardList
    {
        private LinkedList<LinkedListNode<Board>> BList;

        public BoardList(int turn)
        {
            this.BList = new LinkedList<LinkedListNode<Board>>();
        }

        public void AddLastBoard(Board board) { this.BList.AddLast(new LinkedListNode<Board>(new Board(board))); }
        public void AddLastBoard(LinkedListNode<Board> BoardListNode) { this.BList.AddLast(BoardListNode); }
    }
}
