namespace Wallet.Application.Dtos.Responses
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? DeactivatedAt { get; set; }
    }
}
