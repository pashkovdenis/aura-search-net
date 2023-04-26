using AuraSearch.Abstractions;

namespace AuraSearch.Domain
{
    public sealed class Client : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  

        public string Name { get; set; }    

        public string Email { get; set; }

        public string Channel { get; set; }
    }
}
