namespace users_api_dotnet.Models
{
    public class Token
    {
        public Token(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
  
}