using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Stratego.Core;
using Stratego.Net.Sql;

namespace Stratego.Net.WCF
{
    public class StrategoService : IStrategoService
    {
        StrategoEntities entities;

        public StrategoService() {
            entities = new StrategoEntities();
        }

        public int RegisterPlayer(string name, string password)
        {
            if (entities.Players.SingleOrDefault(x => x.Name == name) != null)
            {
                return -1;
            }

            var player = entities.Players.Create();
            player.Name = name;
            player.Password = password;
            entities.Players.Add(player);
            entities.SaveChanges();
            return player.PlayerId;
        }

        public int Login(string name, string password)
        {
            var player = entities.Players.SingleOrDefault(x => x.Name == name && x.Password == password);
            if (player == null)
            {
                return -1;
            }

            return player.PlayerId;
        }

        public bool CreateInvite(int senderId, string recipientName) {
            if (entities.Players.SingleOrDefault(x => x.PlayerId == senderId) == null)
            {
                return false;
            }

            var recipient = entities.Players.SingleOrDefault(x => x.PlayerId != senderId && x.Name == recipientName);
            if (recipient == null) { return false; }

            var invite = entities.Invites.Create();
            invite.SenderId = senderId;
            invite.RecipientId = recipient.PlayerId;

            entities.Invites.Add(invite);
            entities.SaveChanges();

            return true;
        }

        public bool AcceptInvite(int recipientId, int inviteId)
        {
            var invite = entities.Invites.SingleOrDefault(x => x.InviteId == inviteId);
            if (invite == null || invite.RecipientId != recipientId)
            {
                return false;
            }

            Random rand = new Random();
            var i = rand.Next(2);
            var game = entities.Games.Create();
            StrategoGame s = new StrategoGame();
            game.BluePlayerId = (i == 0) ? invite.SenderId : invite.RecipientId;
            game.RedPlayerId = (i == 0) ? invite.RecipientId : invite.SenderId;
            //game.GameState = s.SaveBinary(

            return true;
        }

        public List<SessionInfo> GetActiveSessions(Guid PlayerID)
        {
            List<SessionInfo> sessions = new List<SessionInfo>();
            //foreach(KeyValuePair<Guid, GameSession> kvp in Games[PlayerID]) {
            //    sessions.Add(new SessionInfo() { SessionID = kvp.Value.SessionID, RedPlayerName = Players[kvp.Value.RedPlayer].Name, BluePlayerName = Players[kvp.Value.BluePlayer].Name });
            //}

            return sessions;
        }
    }
}
