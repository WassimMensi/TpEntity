using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Tests.Helpers;

public static class DbContextFactory
{
    // Crée un DbContext en mémoire isolé pour chaque test
    public static HospitalDbContext CreateInMemory(string dbName)
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new HospitalDbContext(options);
    }
}