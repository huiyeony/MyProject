using System;
namespace Server.Game
{
    public class GameLogic : JobSerializer
    {
        //유일한 객체 
        public static GameLogic Instance { get; } = new GameLogic();
    }
}

