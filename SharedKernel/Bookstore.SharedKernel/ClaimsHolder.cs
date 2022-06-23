using System.Security.Claims;

namespace Bookstore.SharedKernel;

public class ClaimsHolder
{
    private Dictionary<string, Dictionary<string, object>> _claims = new();

    public void AddClaim(string userid, string name, object value)
    {
        if (!_claims.ContainsKey(userid))
            _claims.Add(userid, new Dictionary<string, object>());
        if (!_claims[userid].ContainsKey(name))
            _claims[userid].Add(name, value);
        else
            _claims[userid][name] = value;
    }

    public IList<Claim> this[string userid]
    {
        get
        {
            if (!_claims.ContainsKey(userid))
                _claims.Add(userid, new Dictionary<string, object>());
            return _claims[userid]
                .Select(c => new Claim(c.Key, c.Value.ToString() ?? throw new Exception("null claim??")))
                .ToList();
        }
        set
        {
            if (!_claims.ContainsKey(userid))
                _claims.Add(userid, new Dictionary<string, object>());
            foreach (var claim in value)
            {
                if (!_claims[userid].ContainsKey(claim.Type))
                    _claims[userid].Add(claim.Type, claim.Value);
                else
                    _claims[userid][claim.Type] = claim.Value;
            }
        }
    }
}