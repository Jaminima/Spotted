using Spotted_API.Services.Spotify;
using Scrypt;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Spotted_API.Services
{
    public class Session
    {
        public string displayName;
        public string key;
    }

    public class SessionGrant
    {
        [JsonInclude]
        public string displayName;
        [JsonInclude]
        public string key;
        [JsonInclude]
        public int id;
    }

    public class SessionManager
    {
        ScryptEncoder scryptEncoder = new ScryptEncoder();
        ConcurrentDictionary<int, Session> _sessions = new ConcurrentDictionary<int, Session>();
        int lastId = 0;
        Random rnd = new Random();

        public bool CreateSession(Client cli, out SessionGrant? sessionGrant)
        {
            sessionGrant = null;
            string key = RandomStr();

            var existing = _sessions.Where(x => x.Value.displayName == cli.currentUser.DisplayName);
            if (existing.Any())
            {
                foreach (var entry in existing)
                {
                    _sessions.TryRemove(entry);
                }
            }

            lastId+=rnd.Next(1,30);
            
            if (_sessions.TryAdd(lastId, new Session() { key = scryptEncoder.Encode(key), displayName = cli.currentUser.DisplayName })){
                sessionGrant = new SessionGrant() { id = lastId, key = key, displayName = cli.currentUser.DisplayName };
                return true;
            }
            return false;
        }

        public bool FindSession(int idx, string key, out string? displayName)
        {
            displayName = null;
            if (_sessions.TryGetValue(idx, out var session))
            {
                if (scryptEncoder.Compare(key, session.key))
                {
                    displayName = session.displayName;
                    return true;
                }
            }
            return false;
        }

        private string RandomStr(int length = 32)
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += (char) (rnd.Next(0, 2) == 0 ? rnd.Next(65, 91) : rnd.Next(97, 123));
            }
            return s;
        }
    }
}
