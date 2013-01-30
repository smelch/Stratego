using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stratego.Core
{
    public class PlayerFactory
    {
        Dictionary<string, ConstructorInfo> playerMap;
        
        public PlayerFactory()
        {
            playerMap = LoadPlayers();
        }

        private Dictionary<string, ConstructorInfo> LoadPlayers()
        {
            var players = new Dictionary<string, ConstructorInfo>();
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            Type t = typeof(BasePlayer);

            foreach (var asm in asms)
            {
                foreach (var type in asm.GetExportedTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(t))
                    {
                        var ctor = type.GetConstructor(new Type[] { typeof(StrategoGame), typeof(PlayerTurn) });
                        players.Add(type.Name, ctor);
                    }
                }
            }

            return players;
        }

        public string[] GetPlayerNames()
        {
            return playerMap.Keys.OrderBy(x => x).ToArray();
        }

        public BasePlayer Create(string typeName, StrategoGame game, PlayerTurn playerColor)
        {
            var player = (BasePlayer)playerMap[typeName].Invoke(new object[] { game, playerColor });
            return player;
        }
    }
}
