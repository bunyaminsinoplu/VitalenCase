using VitalenCase.Models;

namespace VitalenCase.Services
{
    public class LogServices
    {
        private readonly VitalenTestContext _db;

        public LogServices(VitalenTestContext db)
        {
            _db = db;
        }

        public async Task Log(Logging apiLogItem)
        {
            try
            {
                _db.Loggings.Add(apiLogItem);
                await _db.SaveChangesAsync();

            }
            catch (Exception)
            {

            }

        }
    }
}
