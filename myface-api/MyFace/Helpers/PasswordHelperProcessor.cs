namespace MyFace.Helpers
{
    public class PasswordHelperProcessor
    {
        public string HashedPassword {get; set;}
        
        public string Salt {get; set;}

        public PasswordHelperProcessor(string hashedPassword, string salt)
        {
            HashedPassword = hashedPassword;
            Salt = salt;
        }
    }
}