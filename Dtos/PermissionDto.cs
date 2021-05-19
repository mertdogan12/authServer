namespace authServer.Dtos
{
    public record PermissionDto
    {
        public string permission { get; init; }
        public string username { get; init; }
    }
}
