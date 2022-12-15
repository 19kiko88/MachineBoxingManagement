namespace MachineBoxingManagement.Web.Services.Interfaces
{
    public interface IJwtService
    {
        public string CreateToken(string userName, int expireMinutes = 1440);
    }
}
